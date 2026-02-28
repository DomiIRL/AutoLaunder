using MelonLoader;

namespace AutoLaunder;

public static class Config
{
    private static MelonPreferences_Category _category = null!;

    public static MelonPreferences_Entry<bool> WaitForLastOperation = null!;

    public static void Initialize()
    {
        _category = MelonPreferences.CreateCategory("AutoLaunder_01_General", "AutoLaunder - General");

        WaitForLastOperation = _category.CreateEntry(
            "WaitForLastOperation",
            true,
            "Only restart at last operation",
            "When enabled, AutoLaunder will wait until all currently running laundering operations have finished before starting a new one at full capacity. This prevents sub-capacity runs."
        );
    }

    /// <summary>Re-reads entry values after a Mod Manager save event.</summary>
    public static void Reload()
    {
        // MelonPreferences_Entry<T>.Value is always up-to-date after MelonPreferences saves.
        // Nothing extra needed — callers read .Value directly.
        // Method exists so the event handler in Main has a clear call-site to document intent.
    }
}

