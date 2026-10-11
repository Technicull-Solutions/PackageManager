using System.Globalization;
using Avalonia.Threading;
using UniGetUI.Avalonia.ViewModels;
using UniGetUI.Core.Data;
using UniGetUI.Core.Logging;
using UniGetUI.Core.Tools;

namespace UniGetUI.Avalonia.Infrastructure;

internal static class MainBuildStatusService
{
    private static readonly HttpClient Client = new(CoreTools.GenericHttpClientParameters)
    {
        Timeout = TimeSpan.FromSeconds(8),
    };

    public static async Task RefreshAsync(SidebarViewModel sidebar)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, CoreData.MainBuildNumberUrl);
            request.Headers.UserAgent.ParseAdd(CoreData.UserAgentString);
            using HttpResponseMessage response = await Client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead
            ).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength > 32)
                throw new FormatException("The main branch build number response was too large.");
            await response.Content.LoadIntoBufferAsync(32).ConfigureAwait(false);

            string value = (await response.Content.ReadAsStringAsync().ConfigureAwait(false)).Trim();
            if (value.Length > 10
                || !int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int latest)
                || latest <= 0)
            {
                throw new FormatException("The main branch build number was invalid.");
            }

            await Dispatcher.UIThread.InvokeAsync(() => sidebar.UpdateBuildFreshness(latest));
        }
        catch (Exception ex)
        {
            Logger.Warn($"Could not check build freshness against main: {ex.Message}");
        }
    }
}
