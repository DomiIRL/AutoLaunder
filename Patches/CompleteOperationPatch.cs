using HarmonyLib;
using MelonLoader;
using Il2CppSystem.Collections.Generic;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.Storage;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Variables;
using Il2CppFishNet;
using System;

namespace AutoLaunder.Patches;

[HarmonyPatch(typeof(Business), "CompleteOperation")]
public static class CompleteOperationPatch {

    [HarmonyPostfix]
    public static void Postfix(ref Business __instance, ref LaunderingOperation op) {

        float capacity = __instance.LaunderCapacity;

        foreach (var operation in __instance.LaunderingOperations)
        {
            capacity -= operation.amount;
        }

        MelonLogger.Msg($"Removing Cash from Safes to launder at capacity: {capacity}");

        float remaining = capacity;

        foreach(var placeableStorageEntity in __instance.GetBuildablesOfType<PlaceableStorageEntity>())
        {
            StorageEntity storageEntity = placeableStorageEntity.StorageEntity;

            remaining = RemoveCash(storageEntity, remaining);

            if (remaining <= 0)
            {
                break;
            }
        }


        float cashTaken = capacity - remaining;
        MelonLogger.Msg($"Auto taken: {cashTaken}");
        if (cashTaken > 0 && InstanceFinder.IsServer)
        {
            __instance.StartLaunderingOperation(cashTaken, 0);
            float value = NetworkSingleton<VariableDatabase>.Instance.GetValue<float>("LaunderingOperationsStarted");
            NetworkSingleton<VariableDatabase>.Instance.SetVariableValue("LaunderingOperationsStarted", (value + 1f).ToString(), true);
        }

        if (remaining > 0 || cashTaken == 0)
        {
            Singleton<NotificationsManager>.Instance.SendNotification(
                __instance.propertyName,
                "Safe is empty. Please refill!",
                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon,
                5f,
                true
            );
        } else
        {
            Singleton<NotificationsManager>.Instance.SendNotification(
                __instance.propertyName,
                "Restarted Laundering at capacity..",
                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon,
                5f,
                true
            );
        }

    }

    public static float GetCash(StorageEntity storage)
    {
        float num = 0f;
        foreach(ItemInstance item in storage.GetAllItems())
        {
            if (item.Name == "Cash")
            {
                var cashInstance = new CashInstance(item.Pointer);
                num += cashInstance.Balance;
            }

        }
        return num;
    }

    public static int RemoveCash(StorageEntity storage, float amount)
    {
        amount = UnityEngine.Mathf.Abs(amount);
        float remaining = amount;

        for (int i = 0; i < storage.ItemSlots.Count; i++)
        {
            var item = storage.ItemSlots[i].ItemInstance;
            if (item != null && item.Name == "Cash")
            {
                var cashInstance = new CashInstance(item.Pointer);
                float toRemove = UnityEngine.Mathf.Min(cashInstance.Balance, remaining);
                cashInstance.ChangeBalance(-toRemove);
                remaining -= toRemove;
            }

            if (remaining <= 0f)
                break;
        }

        return UnityEngine.Mathf.CeilToInt(remaining);
    }
}