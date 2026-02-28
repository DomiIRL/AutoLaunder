using HarmonyLib;
using MelonLoader;
using Il2CppSystem.Collections.Generic;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Variables;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Money;
using Il2CppFishNet;
using AutoLaunder.Services;

namespace AutoLaunder.Patches;

[HarmonyPatch(typeof(Business), "CompleteOperation")]
public static class CompleteOperationPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref Business __instance, ref LaunderingOperation op)
    {
        float capacity = __instance.LaunderCapacity;

        foreach (var operation in __instance.LaunderingOperations)
            capacity -= operation.amount;

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

        bool launchFailed = remaining > 0 || cashTaken == 0;
        RayMessengerService.SendStatusMessage(__instance.propertyName, launchFailed, runsLeft, totalCashLeft);
    }
}