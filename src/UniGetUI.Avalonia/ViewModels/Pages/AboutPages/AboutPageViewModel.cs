using CommunityToolkit.Mvvm.Input;
using UniGetUI.Core.Data;
using UniGetUI.Core.Tools;

namespace UniGetUI.Avalonia.ViewModels.Pages.AboutPages;

public partial class AboutPageViewModel : ViewModelBase
{
    public string VersionText { get; } =
        $"{CoreTools.Translate("Version")}: {CoreData.VersionName} · {CoreTools.Translate("Build")}: {CoreData.BuildNumber}";

    public string UpstreamBaseText { get; } =
        CoreTools.Translate("Based on UniGetUI {0}", CoreData.UpstreamBaseVersion);

    public string AboutTitle { get; } =
        CoreTools.Translate("About Evergreen Endpoint | APPS");

    public string PublisherText { get; } =
        CoreTools.Translate("Evergreen Endpoint Package Manager by Technicull Solutions");

    public string ProductDescription { get; } = CoreTools.Translate(
        "Evergreen Endpoint | APPS is the user interface for Evergreen Endpoint Package Manager. It provides a simple, organization-managed way to find, install, and update approved Windows applications.");

    public string DisclaimerTitle { get; } = CoreTools.Translate("Disclaimer");

    public string DisclaimerMessage { get; } = CoreTools.Translate(
        "Evergreen Endpoint Package Manager is maintained by Technicull Solutions and is based on UniGetUI.");

    [RelayCommand]
    private static void OpenHomepage() =>
        CoreTools.Launch("https://github.com/Technicull-Solutions/PackageManager");

    [RelayCommand]
    private static void OpenIssues() =>
        CoreTools.Launch("https://github.com/Technicull-Solutions/PackageManager/issues/new/choose");

    [RelayCommand]
    private static void OpenRepository() =>
        CoreTools.Launch("https://github.com/Technicull-Solutions/PackageManager/");
}
