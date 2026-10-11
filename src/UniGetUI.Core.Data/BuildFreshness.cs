namespace UniGetUI.Core.Data;

public enum BuildFreshnessLevel
{
    Current,
    SlightlyBehind,
    Behind,
    CriticallyBehind,
}

public readonly record struct BuildFreshnessStatus(
    BuildFreshnessLevel Level,
    int BuildsBehind
);

public static class BuildFreshness
{
    public static BuildFreshnessStatus Compare(int currentBuildNumber, int latestMainBuildNumber)
    {
        int buildsBehind = Math.Max(0, latestMainBuildNumber - currentBuildNumber);
        BuildFreshnessLevel level = buildsBehind switch
        {
            0 => BuildFreshnessLevel.Current,
            <= 2 => BuildFreshnessLevel.SlightlyBehind,
            <= 4 => BuildFreshnessLevel.Behind,
            _ => BuildFreshnessLevel.CriticallyBehind,
        };

        return new BuildFreshnessStatus(level, buildsBehind);
    }
}
