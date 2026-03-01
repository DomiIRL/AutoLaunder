using MelonLoader;

namespace AutoLaunder;

public static class Config
{
    private static MelonPreferences_Category _category = null!;
    private static MelonPreferences_Category _loginCategory = null!;

    public static MelonPreferences_Entry<bool> WaitForLastOperation = null!;
    public static MelonPreferences_Entry<bool> OnlyStartAtMaxCapacity = null!;
    public static MelonPreferences_Entry<bool> SendLoginSummary = null!;
    public static MelonPreferences_Entry<bool> ReportSmoothOperations = null!;

    public static void Initialize()
    {
        _category = MelonPreferences.CreateCategory("AutoLaunder_01_General", "General");

        WaitForLastOperation = _category.CreateEntry(
            "WaitForLastOperation",
            true,
            "Wait for last op before restarting",
            "When enabled, AutoLaunder will wait until all currently running laundering operations have finished before starting a new one at full capacity. This prevents sub-capacity runs."
        );

        OnlyStartAtMaxCapacity = _category.CreateEntry(
            "OnlyStartAtMaxCapacity",
            false,
            "Skip runs under max capacity",
            "When enabled, AutoLaunder will refuse to start a laundering operation unless it can fill to full capacity. If there is not enough cash, it treats the business as dry and notifies via R."
        );

        _loginCategory = MelonPreferences.CreateCategory("AutoLaunder_02_LoginSummary", "Login Summary");

        SendLoginSummary = _loginCategory.CreateEntry(
            "SendLoginSummary",
            true,
            "Message on login with business status",
            "When enabled, R will send a message on login summarising any businesses with idle or under-capacity laundering operations."
        );

        ReportSmoothOperations = _loginCategory.CreateEntry(
            "ReportSmoothOperations",
            false,
            "Also message when everything runs fine",
            "When enabled, the login summary will also send a message when all laundering operations are running at full capacity with no issues."
        );
    }

    public static void Reload()
    {
    }
}

