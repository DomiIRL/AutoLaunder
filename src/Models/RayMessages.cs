using UnityEngine;

namespace AutoLaunder.Models;

public static class RayMessages
{
    private static readonly string[] Dry =
    {
        "Nothing to run at {0}. Safe's empty, didn't start. Get cash in. - R",
        "Dry over at {0}. No run, nothing. Fill it up. - R",
        "Couldn't start {0} — bone dry. Drop some paper in there. - R",
    };

    private static readonly string[] Empty =
    {
        "Tapped out at {0}. Only had {1}, used it all. Refill soon. - R",
        "Short run at {0} with {1}. Safe's gone now. Top it up. - R",
        "Running on fumes at {0}. Used {1}, nothing left after this. - R",
    };

    private static readonly string[] AlmostEmpty =
    {
        "Getting low at {0}. Full run but only {1} left. Worth a look. - R",
        "Running but thin at {0} — {1} left. Wouldn't wait too long. - R",
        "Won't last at {0}. {1} in the safe. Top it before next cycle. - R",
    };

    private static readonly string[] Healthy =
    {
        "All good at {0}. Full run, {1} in safe, {2}. Don't worry about it. - R",
        "Sorted at {0}. {1} left, {2}. We're sitting fine. - R",
        "{0} running smooth. {1} in there, {2}. Nothing to worry about. - R",
    };

    private static readonly string[] Waiting =
    {
        "Still running at {0}, {1} in progress. Holding off til it's done. - R",
        "Letting {0} finish first — {1} still going. Full restart after. - R",
        "{0} not done yet, {1} in progress. Better to restart full. - R",
    };

    public static string GetWaiting(string businessName, int activeOperations)
        => string.Format(Pick(Waiting), businessName, SlangOps(activeOperations));

    public static string GetDry(string businessName)
        => string.Format(Pick(Dry), businessName);

    public static string GetEmpty(string businessName, float cashTaken)
        => string.Format(Pick(Empty), businessName, SlangCash(Mathf.FloorToInt(cashTaken)));

    public static string GetAlmostEmpty(string businessName, float cashLeft)
        => string.Format(Pick(AlmostEmpty), businessName, SlangCash(Mathf.FloorToInt(cashLeft)));

    public static string GetHealthy(string businessName, int runsLeft, float cashLeft)
        => string.Format(Pick(Healthy), businessName, SlangCash(Mathf.FloorToInt(cashLeft)), SlangRuns(runsLeft));

    // ── Slang formatters ─────────────────────────────────────────────────────

    private static string SlangCash(int amount)
    {
        if (amount == 0) return "nothing";

        int rounded = Mathf.RoundToInt(amount / 500f) * 500;
        int diff = rounded - amount;

        string prefix = diff switch
        {
            0               => "",
            > 0 and <= 50   => "barely under ",
            > 0 and <= 100  => "just under ",
            > 0 and <= 200  => "a bit under ",
            > 0 and <= 350  => "under ",
            > 0             => "almost ",
            >= -50          => "barely over ",
            >= -100         => "just over ",
            >= -200         => "a bit over ",
            >= -350         => "over ",
            _               => "about ",
        };

        string slang = rounded switch
        {
            < 500                => "pocket change",
            500                  => "five hundred",
            1000                 => "a grand",
            1500                 => "fifteen hundred",
            2000                 => "two grand",
            2500                 => "two and a half grand",
            3000                 => "three grand",
            3500                 => "three and a half grand",
            4000                 => "four grand",
            4500                 => "four and a half grand",
            5000                 => "five grand",
            <= 10000 when rounded % 1000 == 0 => $"{rounded / 1000} grand",
            <= 10000             => $"~{rounded / 1000}k",
            _                    => $"{rounded / 1000}k",
        };

        return prefix + slang;
    }

    private static string SlangRuns(int runs)
    {
        return runs switch
        {
            1  => "one run left", // will never be used currently but good to have the singular case covered in case of future changes
            2  => "two runs left",
            3  => "three runs left",
            4  => "four runs left",
            5  => "five runs left",
            _  => $"{runs} runs left",
        };
    }

    private static string SlangOps(int ops)
    {
        return ops switch
        {
            1 => "one op",
            2 => "two ops",
            3 => "three ops",
            _ => $"{ops} ops",
        };
    }

    private static string Pick(string[] pool)
        => pool[System.Random.Shared.Next(pool.Length)];
}