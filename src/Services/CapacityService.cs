using HarmonyLib;
using Il2CppScheduleOne.Property;
using MelonLoader;

namespace AutoLaunder.Services;

public static class CapacityService
{
    [HarmonyPatch(typeof(Business), nameof(Business.Start))]
    public static class BusinessStartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Business __instance)
        {
            ApplyCapacity(__instance);
        }
    }

    public static void ApplyCapacity(Business business)
    {
        if (business == null) return;

        string name = business.propertyName;
        if (string.IsNullOrEmpty(name)) return;

        float? newCapacity = name switch
        {
            "Laundromat" => Config.LaundromatCapacity.Value,
            "Post Office" => Config.PostOfficeCapacity.Value,
            "Car Wash" => Config.CarWashCapacity.Value,
            "Taco Ticklers" => Config.TacoTicklersCapacity.Value,
            _ => null
        };

        if (newCapacity.HasValue)
        {
            business.LaunderCapacity = newCapacity.Value;
            MelonLogger.Msg($"[AutoLaunder] Applied capacity {newCapacity.Value} to {name}");
        }
    }

    public static void ApplyAll()
    {
        var businesses = Business.Businesses;
        if (businesses == null) return;

        for (int i = 0; i < businesses.Count; i++)
        {
            ApplyCapacity(businesses[i]);
        }
    }
}
