using HarmonyLib;
using MelonLoader;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Variables;
using Il2CppScheduleOne.DevUtilities;
using Il2CppFishNet;
using AutoLaunder.Services;
using System.Collections.Generic;

namespace AutoLaunder.Patches;

[HarmonyPatch(typeof(Business), "CompleteOperation")]
public static class CompleteOperationPatch
{
    // Tracks businesses Ray has already sent a waiting message for this cycle.
    // Cleared once the last operation finishes and we proceed normally.
    private static readonly HashSet<string> _waitingNotified = new();

    [HarmonyPostfix]
    public static void Postfix(ref Business __instance, ref LaunderingOperation op)
    {
        // Multiplayer safety: this flow mutates storage + starts operations,
        // so only the server may execute it.
        if (!InstanceFinder.IsServer)
            return;

        // Count operations still running AFTER this one completed
        // CompleteOperation removes `op` before calling postfix, so any remaining
        // entries in LaunderingOperations are truly still active
        int activeOperations = __instance.LaunderingOperations.Count;

        if (Config.WaitForLastOperation.Value && activeOperations > 0)
        {
            // only send Ray's waiting message once per wait cycle, not on every completion
            if (_waitingNotified.Add(__instance.propertyName))
            {
                RayMessengerService.SendWaitingMessage(__instance.propertyName, activeOperations);
            }
            else
            {
                MelonLogger.Msg($"[AutoLaunder] {__instance.propertyName} still waiting, already notified.");
            }
            return;
        }

        // clear the waiting notification so future waits can trigger ray again
        _waitingNotified.Remove(__instance.propertyName);

        float capacity = __instance.LaunderCapacity;

        // subtract capacity already committed to any remaining operations
        foreach (var operation in __instance.LaunderingOperations)
            capacity -= operation.amount;

        // if other running operations have already consumed all available capacity,
        // there is nothing to start and nothing meaningful to report
        if (capacity <= 0f)
            return;

        // if only max capacity is on, check upfront before touching the storage.
        if (Config.OnlyStartAtMaxCapacity.Value && CashStorageService.GetTotalCash(__instance) < capacity)
        {
            RayMessengerService.SendDryMessage(__instance.propertyName);
            return;
        }

        float remaining = CashStorageService.DrainCashFromStorage(__instance, capacity);
        float cashTaken = capacity - remaining;

        if (cashTaken > 0f)
        {
            __instance.StartLaunderingOperation(cashTaken, 0);
            float value = NetworkSingleton<VariableDatabase>.Instance.GetValue<float>("LaunderingOperationsStarted");
            NetworkSingleton<VariableDatabase>.Instance.SetVariableValue("LaunderingOperationsStarted", (value + 1f).ToString(), true);
        }

        float totalCashLeft = CashStorageService.GetTotalCash(__instance);
        int runsLeft = CashStorageService.GetRunsLeft(totalCashLeft, __instance.LaunderCapacity);

        RayMessengerService.SendStatusMessage(__instance.propertyName, cashTaken, __instance.LaunderCapacity, runsLeft, totalCashLeft);
    }
}
