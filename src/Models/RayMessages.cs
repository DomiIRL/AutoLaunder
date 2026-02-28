using UnityEngine;

namespace AutoLaunder.Models;

public static class RayMessages
{
    private static readonly System.Random _rng = new();

    // ── Dry (no cash at all — nothing started) ───────────────────────────────

    private static readonly string[] Dry =
    {
        "{0} is bone dry. Nothing to run — get cash in the safe. - R",
        "Couldn't start {0}. Safe's empty. Fill it up. - R",
        "{0} has nothing. No run started. Needs cash now. - R",
    };

    // ── Empty (storage now empty after this) ────

    private static readonly string[] Empty =
    {
        "Started {0} with ${1:N0} — that's all there was. Safe's empty now. - R",
        "{0} safe's tapped. Only had ${1:N0}. Refill soon. - R",
        "Short run at {0}, ${1:N0} used. Nothing left in storage. - R",
    };

    // ── Almost Empty (full capacity, but ≤1 full run left in storage) ────────

    private static readonly string[] AlmostEmpty =
    {
        "{0} running full. Heads up — only ${1:N0} left, maybe one run. - R",
        "Full run at {0}. But ${1:N0} in safe, won't last long. - R",
        "{0} is good for now. ${1:N0} left though — top it up soon. - R",
    };

    // ── Healthy (full capacity, 2+ full runs left in storage) ────────────────

    private static readonly string[] Healthy =
    {
        "{0} running full. ${1:N0} left, ~{2} runs. We're good. - R",
        "All good at {0}. Full run, ${1:N0} in safe, {2} runs left. - R",
        "{0} sorted. ${1:N0} sitting there, {2} more runs easy. - R",
    };

    // ── Waiting (last operation still running) ────────────────────────────────

    private static readonly string[] Waiting =
    {
        "{0} still has {1} run(s) going. Waiting to restart full. - R",
        "Holding off on {0}. {1} op(s) active,'ll restart at cap. - R",
        "{0} not done yet — {1} run(s) left. Full restart after. - R",
    };

    // ── Public helpers ────────────────────────────────────────────────────────

    public static string GetWaiting(string businessName, int activeOperations)
        => string.Format(Pick(Waiting), businessName, activeOperations);

    public static string GetDry(string businessName)
        => string.Format(Pick(Dry), businessName);

    public static string GetEmpty(string businessName, float cashTaken)
        => string.Format(Pick(Empty), businessName, Mathf.FloorToInt(cashTaken));

    public static string GetAlmostEmpty(string businessName, float cashLeft)
        => string.Format(Pick(AlmostEmpty), businessName, Mathf.FloorToInt(cashLeft));

    public static string GetHealthy(string businessName, int runsLeft, float cashLeft)
        => string.Format(Pick(Healthy), businessName, Mathf.FloorToInt(cashLeft), runsLeft);

    private static string Pick(string[] pool)
        => pool[_rng.Next(pool.Length)];
}