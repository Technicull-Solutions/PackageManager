using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace UniGetUI.Avalonia.Views.Controls;

public partial class BrandWordmark : UserControl
{
    public BrandWordmark()
    {
        InitializeComponent();
        ActualThemeVariantChanged += (_, _) => UpdateThemeVariant();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateThemeVariant();
    }

    private void UpdateThemeVariant()
    {
        bool useDarkWordmark = ActualThemeVariant == ThemeVariant.Dark;
        DarkWordmark.IsVisible = useDarkWordmark;
        LightWordmark.IsVisible = !useDarkWordmark;
    }
}
