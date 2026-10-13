using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace UniGetUI.Avalonia.Views.Controls;

public partial class BrodyRunner : UserControl
{
    private const int FrameDecodeWidth = 480;
    private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(110);
    private static readonly Bitmap[] Frames = LoadFrames();

    private readonly DispatcherTimer _timer;
    private int _frameIndex;

    public BrodyRunner()
    {
        InitializeComponent();
        FrameImage.Source = Frames[0];

        _timer = new DispatcherTimer { Interval = FrameInterval };
        _timer.Tick += AdvanceFrame;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _timer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    private void AdvanceFrame(object? sender, EventArgs e)
    {
        _frameIndex = (_frameIndex + 1) % Frames.Length;
        FrameImage.Source = Frames[_frameIndex];
    }

    private static Bitmap[] LoadFrames()
    {
        var frames = new Bitmap[5];
        for (int index = 0; index < frames.Length; index++)
        {
            var uri = new Uri($"avares://UniGetUI/Assets/Images/brody-run-frame-{index + 1}.png");
            using Stream stream = AssetLoader.Open(uri);
            frames[index] = Bitmap.DecodeToWidth(stream, FrameDecodeWidth, BitmapInterpolationMode.HighQuality);
        }

        return frames;
    }
}
