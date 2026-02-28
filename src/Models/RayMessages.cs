using UnityEngine;

namespace AutoLaunder.Models;

/// <summary>
/// All text messages sent by Ray regarding laundering status.
/// </summary>
public static class RayMessages
{
    private static readonly System.Random _rng = new();

    // ── Empty ────────────────────────────────────────────────────────────────

    private static readonly string[] Empty =
    {
        "Heads up — {0} just ran dry. Not enough cash in storage to restart laundering. Get money in there. — Ray",
        "Your {0} ground to a halt. Safe's empty, nothing left to launder. Sort it out. — Ray",
        "Just so you know, {0} stopped. Pulled everything and it still wasn't enough. Refill the safe. — Ray",
    };

    // ── Almost Empty (≤1 run left) ────────────────────────────────────────────

    private static readonly string[] AlmostEmpty =
    {
        "Restarted {0}, but you've only got ${1:N0} left in storage. That's maybe one more run. Top it up. — Ray",
        "{0} is running, but barely. ${1:N0} left — one more cycle at best. Don't let it run out. — Ray",
        "Got {0} going again. Fair warning: ${1:N0} in storage isn't much. One run left, maybe. — Ray",
    };

    // ── Healthy (2+ runs left) ────────────────────────────────────────────────

    private static readonly string[] Healthy =
    {
        "Laundering restarted at {0}. ${1:N0} still sitting in storage — roughly {2} more runs. We're good. — Ray",
        "{0} back up at full capacity. You've got ${1:N0} left, about {2} runs worth. Keep it coming. — Ray",
        "All running at {0}. ${1:N0} in storage, {2} runs left before you need to refill. Nice. — Ray",
    };

    // ── Public helpers ────────────────────────────────────────────────────────

    public static string GetEmpty(string businessName)
        => string.Format(Pick(Empty), businessName);

    public static string GetAlmostEmpty(string businessName, float cashLeft)
        => string.Format(Pick(AlmostEmpty), businessName, Mathf.FloorToInt(cashLeft));

    public static string GetHealthy(string businessName, int runsLeft, float cashLeft)
        => string.Format(Pick(Healthy), businessName, Mathf.FloorToInt(cashLeft), runsLeft);

    private static string Pick(string[] pool)
        => pool[_rng.Next(pool.Length)];
}