# Run the real installer only on a disposable GitHub-hosted Windows runner.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Installer)

$ErrorActionPreference = 'Stop'
if ($env:GITHUB_ACTIONS -ne 'true' -or $env:RUNNER_ENVIRONMENT -ne 'github-hosted') {
    throw 'Installer smoke tests require a disposable GitHub-hosted runner.'
}

$Root = Join-Path $env:RUNNER_TEMP 'eepm-installer-smoke'
$null = New-Item -Path $Root -ItemType Directory -Force
$InstallDir = Join-Path $Root 'Installed'
$PortableDir = Join-Path $Root 'Portable'

function Invoke-Installer([string]$Directory, [string]$Tasks, [string]$Label) {
    $Process = Start-Process -FilePath $Installer -WindowStyle Hidden -PassThru -Wait -ArgumentList @(
        '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/SP-', '/CURRENTUSER',
        '/NoAutoStart', '/NoRunOnStartup', '/NoVCRedist', '/NoEdgeWebView', '/NoDeployInstaller',
        "/DIR=`"$Directory`"", "/TASKS=`"$Tasks`"", "/LOG=`"$Root\$Label.log`""
    )
    if ($Process.ExitCode -ne 0) { throw "$Label failed: $($Process.ExitCode)" }
    & (Join-Path $Directory 'uniget.exe') tui --help
    if ($LASTEXITCODE -ne 0) { throw "$Label installed CLI failed." }
}

Invoke-Installer $InstallDir 'regularinstall' 'fresh-install'
Invoke-Installer $InstallDir 'regularinstall' 'same-version-reinstall'

$Desktop = Start-Process -FilePath (Join-Path $InstallDir 'Evergreen Endpoint Package Manager.exe') -WindowStyle Hidden -PassThru
try {
    $Deadline = [DateTime]::UtcNow.AddSeconds(60)
    do {
        Start-Sleep -Milliseconds 250
        $Desktop.Refresh()
        if ($Desktop.HasExited) { throw "Installed desktop exited during startup: $($Desktop.ExitCode)" }
    } while ($Desktop.MainWindowHandle -eq 0 -and [DateTime]::UtcNow -lt $Deadline)
    if ($Desktop.MainWindowHandle -eq 0) { throw 'Installed desktop did not create a window.' }
    Write-Host 'Verified installed NativeAOT desktop startup.'
} finally {
    if (-not $Desktop.HasExited) {
        $null = $Desktop.CloseMainWindow()
        if (-not $Desktop.WaitForExit(10000)) { $Desktop.Kill(); $Desktop.WaitForExit() }
    }
}

$Uninstallers = @(Get-ChildItem -LiteralPath $InstallDir -Filter 'unins*.exe')
if ($Uninstallers.Count -ne 1) { throw 'Expected exactly one installed uninstaller.' }
$Uninstaller = $Uninstallers[0]
$Process = Start-Process -FilePath $Uninstaller.FullName -WindowStyle Hidden -PassThru -Wait -ArgumentList @(
    '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', "/LOG=`"$Root\uninstall.log`""
)
if ($Process.ExitCode -ne 0) { throw "Uninstall failed: $($Process.ExitCode)" }
if (Test-Path -LiteralPath (Join-Path $InstallDir 'Evergreen Endpoint Package Manager.exe')) {
    throw 'Uninstall left the application executable behind.'
}

Invoke-Installer $PortableDir 'portableinstall' 'portable-install'
if (-not (Test-Path -LiteralPath (Join-Path $PortableDir 'ForceUniGetUIPortable'))) {
    throw 'Portable installation marker was not created.'
}
if (Get-ChildItem -LiteralPath $PortableDir -Filter 'unins*.exe') {
    throw 'Portable installation unexpectedly registered an uninstaller.'
}
Write-Host 'Verified fresh install, same-version reinstall, desktop startup, uninstall, and portable install.'
