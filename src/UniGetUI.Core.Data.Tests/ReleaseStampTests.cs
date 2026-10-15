using System.Text.Json;

namespace UniGetUI.Core.Data.Tests;

public class ReleaseStampTests
{
    [Fact]
    public void OwnedUpdateMetadataUsesTheCompiledVersionAndBuildSeries()
    {
        DirectoryInfo? root = new(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "UPSTREAM.json")))
            root = root.Parent;
        Assert.NotNull(root);

        using JsonDocument document = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(root.FullName, "update", "productinfo.json")));
        JsonElement current = document.RootElement
            .GetProperty("TechnicullSolutions.EvergreenEndpointPackageManager")
            .GetProperty("Current");
        Assert.Equal(CoreData.VersionName, current.GetProperty("Version").GetString());
        Assert.Equal(CoreData.BuildNumber, current.GetProperty("BuildNumber").GetInt32());
        Assert.Equal(CoreData.BuildNumber.ToString(System.Globalization.CultureInfo.InvariantCulture),
            current.GetProperty("BuildVersion").GetString());
    }
}
