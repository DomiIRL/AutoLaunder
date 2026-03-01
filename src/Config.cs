using MelonLoader;

namespace AutoLaunder;

public static class Config
{
    private static MelonPreferences_Category _category = null!;

    public static MelonPreferences_Entry<bool> WaitForLastOperation = null!;
    public static MelonPreferences_Entry<bool> OnlyStartAtMaxCapacity = null!;

    public static void Initialize()
    {
        _category = MelonPreferences.CreateCategory("AutoLaunder_01_General", "AutoLaunder - General");

        WaitForLastOperation = _category.CreateEntry(
            "WaitForLastOperation",
            true,
            "Only restart at last operation",
            "When enabled, AutoLaunder will wait until all currently running laundering operations have finished before starting a new one at full capacity. This prevents sub-capacity runs."
        );

        OnlyStartAtMaxCapacity = _category.CreateEntry(
            "OnlyStartAtMaxCapacity",
            false,
            "Only start at max capacity",
            "When enabled, AutoLaunder will refuse to start a laundering operation unless it can fill to full capacity. If there is not enough cash, it treats the business as dry and notifies via R."
        );
    }

    public static void Reload()
    {
    }
}

