using UniGetUI.Avalonia.ViewModels.Pages;

namespace UniGetUI.Tests;

public class PackagePresentationFilterTests
{
    [Theory]
    [InlineData("Google Chrome", "Google.Chrome")]
    [InlineData("7-Zip", "7zip.7zip")]
    [InlineData("Node.js", "OpenJS.NodeJS")]
    [InlineData(".NET SDK", "Microsoft.DotNet.SDK.10")]
    [InlineData("Visual Studio Code (User Setup)", "Microsoft.VisualStudioCode")]
    [InlineData("mpv.net", "mpv.net")]
    public void FriendlyAppNamesAreAccepted(string name, string id)
        => Assert.True(PackagePresentationFilter.HasUserFriendlyName(name, id));

    [Theory]
    [InlineData("", "empty")]
    [InlineData("1.2.3", "version")]
    [InlineData("@scope/package", "@scope/package")]
    [InlineData("python_package_name", "python_package_name")]
    [InlineData("https://example.test/app", "url")]
    [InlineData("com.vendor.internal.product", "com.vendor.internal.product")]
    public void TechnicalPackageNamesAreRejected(string name, string id)
        => Assert.False(PackagePresentationFilter.HasUserFriendlyName(name, id));
}
