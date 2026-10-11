namespace UniGetUI.Core.Data.Tests;

public class BuildFreshnessTests
{
    [Theory]
    [InlineData(100, 100, BuildFreshnessLevel.Current, 0)]
    [InlineData(101, 100, BuildFreshnessLevel.Current, 0)]
    [InlineData(100, 101, BuildFreshnessLevel.SlightlyBehind, 1)]
    [InlineData(100, 102, BuildFreshnessLevel.SlightlyBehind, 2)]
    [InlineData(100, 103, BuildFreshnessLevel.Behind, 3)]
    [InlineData(100, 104, BuildFreshnessLevel.Behind, 4)]
    [InlineData(100, 105, BuildFreshnessLevel.CriticallyBehind, 5)]
    [InlineData(100, 120, BuildFreshnessLevel.CriticallyBehind, 20)]
    public void CompareUsesNMinusThresholds(
        int current,
        int latest,
        BuildFreshnessLevel expectedLevel,
        int expectedGap)
    {
        BuildFreshnessStatus result = BuildFreshness.Compare(current, latest);

        Assert.Equal(expectedLevel, result.Level);
        Assert.Equal(expectedGap, result.BuildsBehind);
    }
}
