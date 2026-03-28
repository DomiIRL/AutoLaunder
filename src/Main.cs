using System;
using System.Linq;
using AutoLaunder.Services;
using MelonLoader;

[assembly: MelonInfo(typeof(AutoLaunder.Main), "AutoLaunder", "2.1", "DomiIRL")]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonOptionalDependencies("ModManager&PhoneApp")]

namespace AutoLaunder
{
    public class Main : MelonMod
    {
        private bool _modManagerFound;
        private bool _loginSummaryCoroutineStarted;

        public override void OnInitializeMelon()
        {
            Config.Initialize();

            try
            {
                _modManagerFound = MelonBase.RegisteredMelons
                    .Any(m => m?.Info?.Name == "Mod Manager & Phone App");

                if (_modManagerFound)
                {
                    LoggerInstance.Msg("[AutoLaunder] Mod Manager found — subscribing to save events.");
                    SubscribeToModManagerEvents();
                }
                else
                {
                    LoggerInstance.Msg("[AutoLaunder] Mod Manager not found — skipping event subscription.");
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"[AutoLaunder] Error checking RegisteredMelons: {ex}");
                _modManagerFound = false;
            }

            LoggerInstance.Msg("[AutoLaunder] Initialized.");
        }

        public override void OnDeinitializeMelon()
        {
            if (_modManagerFound)
                UnsubscribeFromModManagerEvents();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            CustomMessengerService.Reset();
            _loginSummaryCoroutineStarted = false;
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName?.Contains("Main") != true || _loginSummaryCoroutineStarted) return;
            if (!Config.SendLoginSummary.Value) return;

            _loginSummaryCoroutineStarted = true;
            MelonCoroutines.Start(LoginSummaryService.Run());
        }

        // ── Mod Manager wiring ────────────────────────────────────────────────

        private void SubscribeToModManagerEvents()
        {
            try
            {
                ModManagerPhoneApp.ModSettingsEvents.OnPhonePreferencesSaved += HandleSettingsUpdate;
                ModManagerPhoneApp.ModSettingsEvents.OnMenuPreferencesSaved += HandleSettingsUpdate;
                LoggerInstance.Msg("[AutoLaunder] Subscribed to Mod Manager save events.");
            }
            catch (TypeLoadException)
            {
                LoggerInstance.Error("[AutoLaunder] TypeLoadException while subscribing — Mod Manager may be incompatible.");
                _modManagerFound = false;
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"[AutoLaunder] Unexpected error while subscribing to Mod Manager events: {ex}");
                _modManagerFound = false;
            }
        }

        private void UnsubscribeFromModManagerEvents()
        {
            try { ModManagerPhoneApp.ModSettingsEvents.OnPhonePreferencesSaved -= HandleSettingsUpdate; } catch { /* ignored */ }
            try { ModManagerPhoneApp.ModSettingsEvents.OnMenuPreferencesSaved -= HandleSettingsUpdate; } catch { /* ignored */ }
        }

        private void HandleSettingsUpdate()
        {
            Config.Reload();
        }
    }
}
