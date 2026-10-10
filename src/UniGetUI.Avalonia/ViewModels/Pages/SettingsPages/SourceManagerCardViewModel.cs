using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniGetUI.Core.Logging;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine.Interfaces;

namespace UniGetUI.Avalonia.ViewModels.Pages.SettingsPages;

public partial class SourceManagerCardViewModel : ViewModelBase
{
    private readonly IPackageManager _manager;

    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private ObservableCollection<IManagerSource> _sources = [];

    public string TitleText => CoreTools.Translate("{0} sources", _manager.DisplayName);
    public string ManagedNotice { get; } = CoreTools.Translate(
        "Sources are configured by your organization through Group Policy or the machine configuration file."
    );

    public SourceManagerCardViewModel(IPackageManager manager)
    {
        _manager = manager;
        _ = DoLoadSources();
    }

    [RelayCommand]
    private Task ReloadSources() => DoLoadSources();

    private async Task DoLoadSources()
    {
        IsLoading = true;
        Sources.Clear();

        if (!_manager.Status.Found)
        {
            IsLoading = false;
            return;
        }

        try
        {
            var loaded = await Task.Run(_manager.SourcesHelper.GetSources);
            foreach (var s in loaded)
                Sources.Add(s);
        }
        catch (Exception e)
        {
            Logger.Warn($"[SourceManagerCard] Failed to load sources for {_manager.DisplayName}: {e.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

}
