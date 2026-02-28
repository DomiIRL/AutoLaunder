using MelonLoader;
using AutoLaunder.Models;
using AutoLaunder.Services;

namespace AutoLaunder.Debug;

public static class MessageTest
{
    public static void Run()
    {
        MelonLogger.Msg("[AutoLaunder] Running message test...");

        // ── Dry — 3 variants ────────────────────────────────────────────────
        Section("DRY");
        Send(RayMessages.GetDry("Laundromat", 0));
        Send(RayMessages.GetDry("Taco Ticklers", 1));
        Send(RayMessages.GetDry("Car Wash",   2));

        // ── Empty — 3 variants × representative cash amounts ────────────────
        // Cash values chosen to exercise: exact named, rounded, near-round, large
        Section("EMPTY");
        Send(RayMessages.GetEmpty("Laundromat", 100f,   0));      // pocket change
        Send(RayMessages.GetEmpty("Taco Ticklers", 350f,   2));   // pocket change
        Send(RayMessages.GetEmpty("Laundromat", 500f,   0));      // five hundred
        Send(RayMessages.GetEmpty("Laundromat", 1000f,  1));      // a grand
        Send(RayMessages.GetEmpty("Post Office", 2500f,  2));     // two and a half grand
        Send(RayMessages.GetEmpty("Laundromat", 3324f,  0));      // rounds to three and a half grand
        Send(RayMessages.GetEmpty("Laundromat", 4000f,  1));      // four grand
        Send(RayMessages.GetEmpty("Laundromat", 6750f,  2));      // rounds to seven grand
        Send(RayMessages.GetEmpty("Laundromat", 12300f, 0));      // 12k
        Send(RayMessages.GetEmpty("Laundromat", 28400f, 1));      // 28k

        // ── Almost Empty — 3 variants × representative cash amounts ─────────
        Section("ALMOST EMPTY");
        Send(RayMessages.GetAlmostEmpty("Car Wash",   1000f, 0));  // a grand
        Send(RayMessages.GetAlmostEmpty("Car Wash",   2000f, 1));  // two grand
        Send(RayMessages.GetAlmostEmpty("Laundromat", 3750f, 2));  // rounds to four grand

        // ── Healthy — 3 variants × SlangRuns coverage ────────────────────────
        Section("HEALTHY");
        Send(RayMessages.GetHealthy("Car Wash",   2, 8000f,  0));   // good for a couple runs, eight grand
        Send(RayMessages.GetHealthy("Car Wash",   3, 12100f, 1));   // good for a few runs, 12k
        Send(RayMessages.GetHealthy("Car Wash",   5, 20000f, 2));   // good for 5 runs, 20k
        Send(RayMessages.GetHealthy("Laundromat", 1, 4000f,  0));   // good for one more run, four grand
        Send(RayMessages.GetHealthy("Laundromat", 6, 24400f, 1));   // good for 6 runs, ~24k
        Send(RayMessages.GetHealthy("Laundromat", 9, 36000f, 2));   // good for 9 runs, fallback

        // ── Waiting — 3 variants × SlangOps coverage ─────────────────────────
        Section("WAITING");
        Send(RayMessages.GetWaiting("Post Office", 1, 0));  // one op
        Send(RayMessages.GetWaiting("Post Office", 2, 1));  // two ops
        Send(RayMessages.GetWaiting("Post Office", 3, 2));  // three ops
        Send(RayMessages.GetWaiting("Laundromat",  5, 0));  // fallback: 5 ops

        MelonLogger.Msg("[AutoLaunder] Message test complete.");
    }

    private static void Section(string name)
        => MelonLogger.Msg($"[AutoLaunder] ── {name} ──────────────────────────");

    private static void Send(string message)
    {
        MelonLogger.Msg($"[AutoLaunder] [TEST] {message}");
        CustomMessengerService.SendMessage(message);
    }
}