using MelonLoader;
using AutoLaunder.Models;
using AutoLaunder.Services;

namespace AutoLaunder.Debug;

public static class MessageTest
{
    public static void Run()
    {
        MelonLogger.Msg("[AutoLaunder] Running message test...");

        // Dry
        Send(RayMessages.GetDry("Laundromat"));
        Send(RayMessages.GetDry("Car Wash"));

        // Empty — test slang cash edge cases
        Send(RayMessages.GetEmpty("Laundromat", 0f));        // nothing
        Send(RayMessages.GetEmpty("Laundromat", 480f));      // pocket change
        Send(RayMessages.GetEmpty("Laundromat", 500f));      // exact — five hundred
        Send(RayMessages.GetEmpty("Laundromat", 950f));      // almost a grand
        Send(RayMessages.GetEmpty("Laundromat", 1000f));     // exact — a grand
        Send(RayMessages.GetEmpty("Laundromat", 1100f));     // just over a grand
        Send(RayMessages.GetEmpty("Laundromat", 2400f));     // just under two and a half
        Send(RayMessages.GetEmpty("Laundromat", 2500f));     // exact — two and a half
        Send(RayMessages.GetEmpty("Laundromat", 2750f));     // over two and a half
        Send(RayMessages.GetEmpty("Laundromat", 3700f));     // almost four grand
        Send(RayMessages.GetEmpty("Laundromat", 3900f));     // just under four grand
        Send(RayMessages.GetEmpty("Laundromat", 4000f));     // exact — four grand
        Send(RayMessages.GetEmpty("Laundromat", 4150f));     // just over four grand
        Send(RayMessages.GetEmpty("Laundromat", 9800f));     // just under ten grand
        Send(RayMessages.GetEmpty("Laundromat", 12500f));    // 12k range

        // Almost Empty
        Send(RayMessages.GetAlmostEmpty("Taco Ticklers", 900f));
        Send(RayMessages.GetAlmostEmpty("Taco Ticklers", 1950f));
        Send(RayMessages.GetAlmostEmpty("Taco Ticklers", 3300f));

        // Healthy
        Send(RayMessages.GetHealthy("Car Wash", 2, 7800f));   // almost eight grand, 2 runs
        Send(RayMessages.GetHealthy("Car Wash", 3, 12100f));  // just over twelve grand, 3 runs
        Send(RayMessages.GetHealthy("Car Wash", 5, 20000f));  // exact — twenty grand, 5 runs
        Send(RayMessages.GetHealthy("Car Wash", 7, 28400f));  // about twenty eight grand, 7 runs

        // Waiting
        Send(RayMessages.GetWaiting("Post Office", 1));
        Send(RayMessages.GetWaiting("Post Office", 2));
        Send(RayMessages.GetWaiting("Post Office", 4));

        MelonLogger.Msg("[AutoLaunder] Message test complete.");
    }
    
    private static void Send(string message)
    {
        MelonLogger.Msg($"[AutoLaunder] [TEST] {message}");
        CustomMessengerService.SendMessage(message);
    }
}

