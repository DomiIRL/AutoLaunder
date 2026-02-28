using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Storage;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;
using UnityEngine;

namespace AutoLaunder.Services;

public static class CashStorageService
{

    public static float DrainCashFromStorage(Business business, float amount)
    {
        float remaining = amount;

        foreach (var placeable in business.GetBuildablesOfType<PlaceableStorageEntity>())
        {
            remaining = DrainFromStorage(placeable.StorageEntity, remaining);
            if (remaining <= 0f) break;
        }

        return remaining;
    }

    public static float GetTotalCash(Business business)
    {
        float total = 0f;

        foreach (var placeable in business.GetBuildablesOfType<PlaceableStorageEntity>())
            total += GetCashInStorage(placeable.StorageEntity);

        return total;
    }

    public static int GetRunsLeft(float totalCash, float launderCapacity)
    {
        if (launderCapacity <= 0f) return 0;
        return Mathf.FloorToInt(totalCash / launderCapacity);
    }

    private static float GetCashInStorage(StorageEntity storage)
    {
        float total = 0f;
        foreach (ItemInstance item in storage.GetAllItems())
        {
            if (item.Name == "Cash")
            {
                var cashInstance = new CashInstance(item.Pointer);
                total += cashInstance.Balance;
            }
        }
        return total;
    }

    private static float DrainFromStorage(StorageEntity storage, float amount)
    {
        amount = Mathf.Abs(amount);
        float remaining = amount;

        for (int i = 0; i < storage.ItemSlots.Count; i++)
        {
            var item = storage.ItemSlots[i].ItemInstance;
            if (item != null && item.Name == "Cash")
            {
                var cashInstance = new CashInstance(item.Pointer);
                float toRemove = Mathf.Min(cashInstance.Balance, remaining);
                cashInstance.ChangeBalance(-toRemove);
                remaining -= toRemove;
            }
            if (remaining <= 0f) break;
        }

        return remaining;
    }
}