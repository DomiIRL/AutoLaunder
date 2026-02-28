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
        Debug.MessageTest.Run();
        
        
        // Count operations still running AFTER this one completed.
        // CompleteOperation removes `op` before calling postfix, so any remaining
        // entries in LaunderingOperations are truly still active.
        int activeOperations = __instance.LaunderingOperations.Count;

        if (Config.WaitForLastOperation.Value && activeOperations > 0)
        {
            // Only send Ray's waiting message once per wait cycle, not on every completion.
            if (_waitingNotified.Add(__instance.propertyName))
            {
                MelonLogger.Msg($"[AutoLaunder] WaitForLastOperation enabled — {activeOperations} operation(s) still running at {__instance.propertyName}. Notifying once.");
                RayMessengerService.SendWaitingMessage(__instance.propertyName, activeOperations);
            }
            else
            {
                MelonLogger.Msg($"[AutoLaunder] WaitForLastOperation enabled — {activeOperations} operation(s) still running at {__instance.propertyName}. Skipping (already notified).");
            }
            return;
        }

        // Clear the waiting notification so future waits can trigger Ray again.
        _waitingNotified.Remove(__instance.propertyName);

        float capacity = __instance.LaunderCapacity;

        // Subtract capacity already committed to any remaining operations.
        foreach (var operation in __instance.LaunderingOperations)
            capacity -= operation.amount;

        // If other running operations have already consumed all available capacity,
        // there is nothing to start and nothing meaningful to report.
        if (capacity <= 0f)
        {
            MelonLogger.Msg($"[AutoLaunder] No capacity slot available at {__instance.propertyName} (consumed by other running operations). Skipping.");
            return;
        }

        MelonLogger.Msg($"[AutoLaunder] Attempting to fund laundering at capacity: {capacity}");

        float remaining = CashStorageService.DrainCashFromStorage(__instance, capacity);
        float cashTaken = capacity - remaining;

        MelonLogger.Msg($"[AutoLaunder] Cash taken: {cashTaken}");

        if (cashTaken > 0 && InstanceFinder.IsServer)
        {
            __instance.StartLaunderingOperation(cashTaken, 0);
            float value = NetworkSingleton<VariableDatabase>.Instance.GetValue<float>("LaunderingOperationsStarted");
            NetworkSingleton<VariableDatabase>.Instance.SetVariableValue("LaunderingOperationsStarted", (value + 1f).ToString(), true);
        }

        float totalCashLeft = CashStorageService.GetTotalCash(__instance);
        int runsLeft = CashStorageService.GetRunsLeft(totalCashLeft, __instance.LaunderCapacity);

        MelonLogger.Msg($"[AutoLaunder] Cash left in storage: ${totalCashLeft} | Estimated runs left: {runsLeft}");

        RayMessengerService.SendStatusMessage(__instance.propertyName, cashTaken, __instance.LaunderCapacity, runsLeft, totalCashLeft);
    }
}

