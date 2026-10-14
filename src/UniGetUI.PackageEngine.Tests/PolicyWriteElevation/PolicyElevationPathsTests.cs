using UniGetUI.PackageEngine.AgentBroker.PolicyWriteElevation;

namespace UniGetUI.PackageEngine.Tests.PolicyWriteElevation;

/// <summary>
/// The packaged layout is a security boundary: the helper is only ever accepted at one exact path
/// under an administrator-protected root.
/// </summary>
public class PolicyElevationPathsTests
{
    private const string Root = @"C:\Program Files\UniGetUI";

    [Fact]
    public void HelperPath_IsTheExactPackagedLayout()
        => Assert.Equal(
            Path.Combine(Root, "Assets", "Utilities", "EEPM Policy Elevator.exe"),
            PolicyElevationPaths.GetHelperPath(Root));

    [Fact]
    public void HostPath_IsTheExactPackagedLayout()
        => Assert.Equal(Path.Combine(Root, "Evergreen Endpoint Package Manager.exe"), PolicyElevationPaths.GetHostPath(Root));

    [Fact]
    public void InstallRoot_IsRecoveredFromTheHelperPath()
    {
        Assert.True(PolicyElevationPaths.TryGetInstallRootFromHelperPath(
            PolicyElevationPaths.GetHelperPath(Root),
            out string? recovered));

        Assert.Equal(Root, recovered);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(@"C:\Program Files\UniGetUI\EEPM Policy Elevator.exe")]
    [InlineData(@"C:\Program Files\UniGetUI\Assets\EEPM Policy Elevator.exe")]
    [InlineData(@"C:\Program Files\UniGetUI\Assets\Utilities\Evergreen Endpoint Package Manager.exe")]
    [InlineData(@"C:\Program Files\UniGetUI\Utilities\Assets\EEPM Policy Elevator.exe")]
    [InlineData(@"Assets\Utilities\EEPM Policy Elevator.exe")]
    public void AnyOtherLayout_IsRejected(string? helperPath)
    {
        Assert.False(PolicyElevationPaths.TryGetInstallRootFromHelperPath(helperPath, out string? recovered));
        Assert.Null(recovered);
    }
}
