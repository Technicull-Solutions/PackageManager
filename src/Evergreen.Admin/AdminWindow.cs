using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Evergreen.Configuration;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;

namespace Evergreen.Admin;

public sealed class AdminWindow : Window
{
    private readonly TextBox _library = Editor(AdminWorkspace.FormatLibrary(AdminWorkspace.EmptyLibrary));
    private readonly TextBox _managers = Editor(AdminWorkspace.FormatManagers(AdminWorkspace.DefaultManagers));
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };
    private readonly TextBlock _diagnostics = new() { TextWrapping = TextWrapping.Wrap };
    private string _savedLibrary = AdminWorkspace.FormatLibrary(AdminWorkspace.EmptyLibrary);
    private string _savedManagers = AdminWorkspace.FormatManagers(AdminWorkspace.DefaultManagers);
    private bool _dirty;
    private bool _confirmedClose;

    public AdminWindow()
    {
        Title = CoreTools.Translate("Evergreen Endpoint | ADMIN");
        Icon = AdminBrandStyles.CreateIcon();
        Width = 1100; Height = 800; MinWidth = 760; MinHeight = 600;
        var header = new StackPanel { Spacing = 10, Margin = new Thickness(24, 20) };
        var identity = new WrapPanel { Orientation = Orientation.Horizontal };
        using (var stream = AssetLoader.Open(new Uri("avares://Evergreen.Admin/Assets/admin.png")))
            identity.Children.Add(new Image { Source = new Bitmap(stream), Width = 56, Height = 56, Margin = new Thickness(0, 0, 16, 8) });
        var brandFont = new FontFamily("avares://Evergreen.Admin/Assets/Fonts#Lato");
        var wordmark = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Margin = new Thickness(0, 0, 0, 8), VerticalAlignment = VerticalAlignment.Center };
        wordmark.Children.Add(new TextBlock
        {
            Text = CoreTools.Translate("EVERGREEN\nENDPOINT"),
            FontFamily = brandFont,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
        });
        var separator = new Border { Width = 3, Height = 44 };
        separator.Bind(Border.BackgroundProperty, separator.GetResourceObservable("AdminGold"));
        wordmark.Children.Add(separator);
        wordmark.Children.Add(new TextBlock
        {
            Text = CoreTools.Translate("ADMIN"),
            FontFamily = brandFont,
            FontSize = 28,
            VerticalAlignment = VerticalAlignment.Center,
        });
        identity.Children.Add(wordmark);
        header.Children.Add(identity);
        header.Children.Add(new TextBlock { Text = CoreTools.Translate("Portable authoring and local diagnostics"), FontSize = 16 });
        header.Children.Add(new TextBlock
        {
            Text = CoreTools.Translate("Edit and export drafts locally. Deployment, signing, and machine policy changes are separate administrator operations."),
            TextWrapping = TextWrapping.Wrap,
        });
        var appearanceRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        var appearanceLabel = new TextBlock { Text = CoreTools.Translate("Appearance"), VerticalAlignment = VerticalAlignment.Center };
        var appearance = new ComboBox
        {
            ItemsSource = new[] { CoreTools.Translate("System"), CoreTools.Translate("Light"), CoreTools.Translate("Dark") },
            SelectedIndex = Program.Appearance switch { "Light" => 1, "Dark" => 2, _ => 0 },
            MinWidth = 140,
        };
        AutomationProperties.SetName(appearance, CoreTools.Translate("Appearance"));
        appearance.SelectionChanged += (_, _) =>
        {
            string value = appearance.SelectedIndex switch { 1 => "Light", 2 => "Dark", _ => "System" };
            if (Application.Current is { } app) app.RequestedThemeVariant = AdminAppearance.ToThemeVariant(value);
            try { AdminAppearance.Save(Program.DataDirectory, value); }
            catch (Exception ex) { _status.Text = CoreTools.Translate("Appearance changed but could not be saved: {0}", ex.Message); }
        };
        appearanceRow.Children.Add(appearanceLabel);
        appearanceRow.Children.Add(appearance);
        var tabs = new TabControl
        {
            ItemsSource = new TabItem[]
            {
                new() { Header = CoreTools.Translate("Software library"), Content = DocumentPage(_library, true) },
                new() { Header = CoreTools.Translate("Package manager configuration"), Content = DocumentPage(_managers, false) },
                new() { Header = CoreTools.Translate("Local diagnostics"), Content = DiagnosticsPage() },
            },
        };
        var layout = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto") };
        var brandHeader = new Border { Child = header };
        brandHeader.Classes.Add("brandHeader");
        layout.Children.Add(brandHeader);
        appearanceRow.Margin = new Thickness(24, 12);
        Grid.SetRow(appearanceRow, 1); layout.Children.Add(appearanceRow);
        tabs.Margin = new Thickness(8, 0);
        Grid.SetRow(tabs, 2); layout.Children.Add(tabs);
        _status.Margin = new Thickness(20, 12);
        AutomationProperties.SetLiveSetting(_status, AutomationLiveSetting.Polite);
        Grid.SetRow(_status, 3); layout.Children.Add(_status);
        Content = layout;
        _library.TextChanged += (_, _) => MarkDirty();
        _managers.TextChanged += (_, _) => MarkDirty();
        Closing += async (_, e) =>
        {
            if (!_dirty || _confirmedClose) return;
            e.Cancel = true;
            if (await ConfirmAsync(CoreTools.Translate("Close without exporting your drafts?")))
            {
                _confirmedClose = true;
                Close();
            }
        };
    }

    private void MarkDirty()
    {
        _dirty = _library.Text != _savedLibrary || _managers.Text != _savedManagers;
        if (!_dirty) return;
        _status.Text = CoreTools.Translate("Draft changed. Export to keep your changes.");
    }

    private static TextBox Editor(string text)
    {
        // Tab moves to the next control rather than trapping keyboard users in the draft.
        var editor = new TextBox { Text = text, AcceptsReturn = true, AcceptsTab = false, TextWrapping = TextWrapping.NoWrap };
        editor.Classes.Add("jsonEditor");
        AutomationProperties.SetName(editor, CoreTools.Translate("JSON draft"));
        return editor;
    }

    private Control DocumentPage(TextBox editor, bool library)
    {
        var tools = new WrapPanel { Orientation = Orientation.Horizontal };
        tools.Children.Add(ActionButton(CoreTools.Translate("New draft"), async () =>
        {
            if (await ConfirmAsync(CoreTools.Translate("Replace this draft? Export any changes you want to keep first.")))
                editor.Text = library ? AdminWorkspace.FormatLibrary(AdminWorkspace.EmptyLibrary) : AdminWorkspace.FormatManagers(AdminWorkspace.DefaultManagers);
        }));
        tools.Children.Add(ActionButton(CoreTools.Translate("Open JSON"), async () =>
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = CoreTools.Translate("Open a draft"),
                AllowMultiple = false,
                FileTypeFilter = [JsonType],
            });
            if (files.Count != 1 || files[0].TryGetLocalPath() is not { } path) return;
            string json = await AdminWorkspace.OpenAsync(path);
            string formatted = library ? AdminWorkspace.FormatLibrary(json) : AdminWorkspace.FormatManagers(json);
            if (await ConfirmAsync(CoreTools.Translate("Replace this draft? Export any changes you want to keep first.")))
                editor.Text = formatted;
        }));
        tools.Children.Add(ActionButton(CoreTools.Translate("Validate and format"), () =>
        {
            editor.Text = library ? AdminWorkspace.FormatLibrary(editor.Text ?? "") : AdminWorkspace.FormatManagers(editor.Text ?? "");
            _status.Text = CoreTools.Translate("Document structure is valid. Publisher trust, source availability, and target policy have not been verified.");
            return Task.CompletedTask;
        }));
        tools.Children.Add(ActionButton(CoreTools.Translate("Export JSON"), async () =>
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = CoreTools.Translate("Export draft to your workspace"),
                DefaultExtension = "json",
                FileTypeChoices = [JsonType],
                SuggestedFileName = library ? "SoftwareCatalog.json" : "package-managers.json",
                ShowOverwritePrompt = true,
            });
            if (file?.TryGetLocalPath() is not { } path) return;
            await AdminWorkspace.ExportAsync(path, editor.Text ?? "", library);
            if (library) _savedLibrary = editor.Text ?? "";
            else _savedManagers = editor.Text ?? "";
            _dirty = _library.Text != _savedLibrary || _managers.Text != _savedManagers;
            _status.Text = CoreTools.Translate("Exported {0}. Deploy this file separately; the endpoint has not been changed.", path);
        }, primary: true));
        var top = new StackPanel { Spacing = 8 };
        top.Children.Add(tools);
        if (library) top.Children.Add(ApplicationForm(editor));
        else top.Children.Add(new TextBlock { Text = CoreTools.Translate("Group Policy overrides machine JSON. Source names must match repositories already provisioned on the device."), TextWrapping = TextWrapping.Wrap });
        var page = new Grid { RowDefinitions = new RowDefinitions("Auto,*"), Margin = new Thickness(16), RowSpacing = 12 };
        var card = new Border { Child = top };
        card.Classes.Add("card");
        page.Children.Add(card); Grid.SetRow(editor, 1); page.Children.Add(editor);
        return page;
    }

    private Control ApplicationForm(TextBox editor)
    {
        var form = new WrapPanel();
        TextBox Field(string label, string value)
        {
            var input = new TextBox { Text = value, MinWidth = 150, Margin = new Thickness(0, 0, 8, 8) };
            AutomationProperties.SetName(input, CoreTools.Translate(label));
            var group = new StackPanel();
            group.Children.Add(new TextBlock { Text = CoreTools.Translate(label) }); group.Children.Add(input); form.Children.Add(group);
            return input;
        }
        var catalogId = Field(CoreTools.Translate("Library ID"), "apps");
        var catalogName = Field(CoreTools.Translate("Library name"), "Apps");
        var name = Field(CoreTools.Translate("App name"), "");
        var id = Field(CoreTools.Translate("Package ID"), "");
        var source = Field(CoreTools.Translate("Source"), "winget");
        var manager = new ComboBox { ItemsSource = new[] { "WinGet", "PowerShell", "PowerShell7" }, SelectedIndex = 0, Margin = new Thickness(0, 18, 8, 8) };
        AutomationProperties.SetName(manager, CoreTools.Translate("Package manager"));
        manager.SelectionChanged += (_, _) => source.Text = manager.SelectedIndex == 0 ? "winget" : "PSGallery";
        form.Children.Add(manager);
        form.Children.Add(ActionButton(CoreTools.Translate("Add app"), () =>
        {
            editor.Text = AdminWorkspace.AddApplication(editor.Text ?? "", catalogId.Text ?? "", catalogName.Text ?? "",
                name.Text ?? "", id.Text ?? "", (string?)manager.SelectedItem ?? "", source.Text ?? "");
            return Task.CompletedTask;
        }));
        return form;
    }

    private Control DiagnosticsPage()
    {
        var panel = new StackPanel { Spacing = 16, Margin = new Thickness(20) };
        panel.Children.Add(ActionButton(CoreTools.Translate("Refresh local configuration"), () =>
        {
            ManagedPackageManagerConfiguration.Reload();
            var rows = new List<string>
            {
                CoreTools.Translate("Read-only local configuration. No package managers or broker connections are started."),
                "", CoreTools.Translate("Software library") + ": " + SoftwareLibrary.FilePath,
                CoreTools.Translate("Library file exists") + ": " + File.Exists(SoftwareLibrary.FilePath),
                CoreTools.Translate("Machine configuration") + ": " + ManagedPackageManagerConfiguration.DefaultConfigurationPath,
                "HKLM\\" + ManagedPackageManagerConfiguration.PolicyRegistryPath,
            };
            foreach (string manager in new[] { "WinGet", "PowerShell", "PowerShell7" })
            {
                rows.Add($"{manager}: {CoreTools.Translate("Enabled")} = {ManagedPackageManagerConfiguration.IsManagerEnabled(manager)}; "
                    + $"{CoreTools.Translate("Executable")} = {ManagedPackageManagerConfiguration.GetExecutablePath(manager) ?? CoreTools.Translate("Automatic discovery")}");
            }
            rows.Add("");
            rows.Add(CoreTools.Translate("Unsigned drafts do not establish approval or publisher trust. Opening a workspace never applies machine configuration."));
            _diagnostics.Text = string.Join(Environment.NewLine, rows);
            return Task.CompletedTask;
        }));
        panel.Children.Add(_diagnostics);
        return new ScrollViewer { Content = panel };
    }

    private Button ActionButton(string label, Func<Task> action, bool primary = false)
    {
        var button = new Button { Content = label, Margin = new Thickness(0, 4, 8, 4) };
        if (primary) button.Classes.Add("primary");
        button.Click += async (_, _) =>
        {
            button.IsEnabled = false;
            try { await action(); }
            catch (Exception ex) { _status.Text = CoreTools.Translate("Action could not be completed: {0}", ex.Message); }
            finally { button.IsEnabled = true; }
        };
        return button;
    }

    private async Task<bool> ConfirmAsync(string message)
    {
        var dialog = new Window { Title = CoreTools.Translate("Review draft changes"), Icon = AdminBrandStyles.CreateIcon(), Width = 440, SizeToContent = SizeToContent.Height, CanResize = false, WindowStartupLocation = WindowStartupLocation.CenterOwner };
        var panel = new StackPanel { Margin = new Thickness(20), Spacing = 16 };
        panel.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap });
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        var cancel = new Button { Content = CoreTools.Translate("Cancel") }; cancel.Click += (_, _) => dialog.Close(false);
        var proceed = new Button { Content = CoreTools.Translate("Continue") }; proceed.Click += (_, _) => dialog.Close(true);
        proceed.Classes.Add("primary");
        dialog.Opened += (_, _) => cancel.Focus();
        buttons.Children.Add(cancel); buttons.Children.Add(proceed); panel.Children.Add(buttons); dialog.Content = panel;
        return await dialog.ShowDialog<bool>(this);
    }

    private static FilePickerFileType JsonType => new("JSON") { Patterns = ["*.json"] };
}
