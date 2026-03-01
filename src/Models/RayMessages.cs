using UnityEngine;

namespace AutoLaunder.Models;

public static class RayMessages
{
    private static readonly string[] Dry =
    {
        "Nothing to run at {0}. Safe's empty, didn't start. Get cash in.",
        "Dry over at {0}. No run, nothing. Fill it up.",
        "Couldn't start {0} — bone dry. Drop some paper in there.",
    };

    private static readonly string[] Empty =
    {
        "Tapped out at {0}. Only had {1}, used it all. Refill soon.",
        "Short run at {0} with {1}. Safe's gone now. Top it up.",
        "Running on fumes at {0}. Used {1}, nothing left after this.",
    };

    private static readonly string[] AlmostEmpty =
    {
        "Getting low at {0}. Full run but only {1} left. Worth a look.",
        "Running but thin at {0} — {1} left. Wouldn't wait too long.",
        "Won't last at {0}. {1} in the safe. Top it before next cycle.",
    };

    private static readonly string[] Healthy =
    {
        "All good at {0}. Full run, {1} in safe, {2}. Don't worry about it.",
        "Sorted at {0}. {1} left, {2}. We're sitting fine.",
        "{0} running smooth. {1} in there, {2}. Nothing to worry about.",
    };

    private static readonly string[] Waiting =
    {
        "Still running at {0}, {1} in progress. Holding off til it's done.",
        "Letting {0} finish first — {1} still going. Full restart after.",
        "{0} not done yet, {1} in progress. Better to restart full.",
    };

    private static readonly string[] LoginUnderCapacity =
    {
        "Running under capacity at {0} — could be doing more.",
        "Not hitting full loads at {0}. Worth topping up.",
        "{0} is running short. Should fix it next cycle.",
    };

    private static readonly string[] LoginIdle =
    {
        "Nothing running at {0}. Check the safes.",
        "Ops stopped at {0}. Might be dry, might not. Worth checking.",
        "{0} is just sitting there. Ran out of paper?",
    };

    private static readonly string[] LoginMixed =
    {
        "Running under load at {0}. Also nothing going on at {1} — check those.",
        "Short on capacity at {0}. Plus {1} has stopped entirely.",
        "{0} isn't running full. And {1} isn't even running at all.",
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

    public static string GetLoginUnderCapacity(string underCapacityList)
        => string.Format(Pick(LoginUnderCapacity), underCapacityList);

    public static string GetLoginIdle(string idleList)
        => string.Format(Pick(LoginIdle), idleList);

    public static string GetLoginMixed(string underCapacityList, string idleList)
        => string.Format(Pick(LoginMixed), underCapacityList, idleList);

    internal static string GetDry(string businessName, int variant)
        => string.Format(Pick(Dry, variant), businessName);

    internal static string GetEmpty(string businessName, float cashTaken, int variant)
        => string.Format(Pick(Empty, variant), businessName, SlangCash(Mathf.FloorToInt(cashTaken)));

    internal static string GetAlmostEmpty(string businessName, float cashLeft, int variant)
        => string.Format(Pick(AlmostEmpty, variant), businessName, SlangCash(Mathf.FloorToInt(cashLeft)));

    internal static string GetHealthy(string businessName, int runsLeft, float cashLeft, int variant)
        => string.Format(Pick(Healthy, variant), businessName, SlangCash(Mathf.FloorToInt(cashLeft)), SlangRuns(runsLeft));

    internal static string GetWaiting(string businessName, int activeOperations, int variant)
        => string.Format(Pick(Waiting, variant), businessName, SlangOps(activeOperations));

    // Slang formatters

    private static string SlangCash(int amount)
    {
        if (amount == 0)  return "nothing";
        if (amount < 250) return "pocket change";

        int step    = amount < 1000 ? 500 : 1000;
        int rounded = (int)System.Math.Round(amount / (float)step, System.MidpointRounding.AwayFromZero) * step;
        int diff    = rounded - amount;

        // normalise diff to a 0-1 scale relative to the step size
        float norm = diff / (float)step;

        string prefix = norm switch
        {
            0f                     => "",
            > 0f and <= 0.05f      => "barely under ",
            > 0f and <= 0.10f      => "just under ",
            > 0f and <= 0.20f      => "a bit under ",
            > 0f and <= 0.45f      => "under ",
            > 0f                   => "almost ",
            >= -0.05f              => "barely over ",
            >= -0.10f              => "just over ",
            >= -0.20f              => "a bit over ",
            >= -0.45f              => "over ",
            _                      => "about ",
        };

        string slang = rounded switch
        {
            500   => "five hundred",
            1000  => "a grand",
            2000  => "two grand",
            3000  => "three grand",
            4000  => "four grand",
            5000  => "five grand",
            6000  => "six grand",
            7000  => "seven grand",
            8000  => "eight grand",
            9000  => "nine grand",
            10000 => "ten grand",
            _     => $"{rounded / 1000} grand",
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

    internal static string Pick(string[] pool, int index)
        => pool[index % pool.Length];
}