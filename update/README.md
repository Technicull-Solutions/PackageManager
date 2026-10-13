# Evergreen Endpoint update channel

`productinfo.json` is the only release metadata consumed by production builds of
Evergreen Endpoint Package Manager. Production clients use the copy published on
the `main` branch of `Technicull-Solutions/PackageManager` and reject metadata and
installer URLs outside that repository.

For a release, update `Version`, `BuildVersion`, and `BuildNumber`, then add the
matching Windows installer to `Files`:

```json
{
  "Arch": "x64",
  "Type": "exe",
  "Url": "https://github.com/Technicull-Solutions/PackageManager/releases/download/<tag>/EvergreenEndpointPackageManager.<version>-build.<build>.x64.Setup.exe",
  "Hash": "<lowercase SHA-256>"
}
```

Keep `Files` empty until the corresponding artifact is published. A client on the
same release and build treats the metadata as current without requiring an
installer entry.
