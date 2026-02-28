using MelonLoader;

[assembly: MelonInfo(typeof(AutoLaunder.Main), "AutoLaunder", "1.0", "DomiIRL")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace AutoLaunder
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("AutoLaunder initialized.");
        }
    }
}