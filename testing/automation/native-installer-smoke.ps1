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

# Seed an upstream installation's registrations, shortcut, and running process.
# None may be adopted, migrated, deleted, or terminated by Evergreen's installer.
$UpstreamKeys = @('Software\Classes\unigetui', 'Software\Classes\.ubundle', 'Software\Classes\UniGetUI.PackageBundle')
foreach ($Key in $UpstreamKeys) {
    if (Test-Path "HKCU:\$Key") { throw "Runner already has an upstream registration: $Key" }
    $Registry = [Microsoft.Win32.Registry]::CurrentUser.CreateSubKey($Key)
    $Registry.SetValue('', 'upstream-preservation-sentinel')
    $Registry.Dispose()
}
$Shortcut = Join-Path ([Environment]::GetFolderPath('StartMenu')) 'WingetUI.lnk'
if (Test-Path -LiteralPath $Shortcut) { throw 'Runner already has an upstream shortcut.' }
Set-Content -LiteralPath $Shortcut -Value 'upstream-preservation-sentinel'
$UpstreamExecutable = Join-Path $Root 'UniGetUI.exe'
Copy-Item -LiteralPath (Join-Path $env:WINDIR 'System32\PING.EXE') -Destination $UpstreamExecutable
$UpstreamProcess = Start-Process -FilePath $UpstreamExecutable -ArgumentList @('-t', '127.0.0.1') -WindowStyle Hidden -PassThru

function Assert-UpstreamIntact {
    foreach ($Key in $UpstreamKeys) {
        $Registry = [Microsoft.Win32.Registry]::CurrentUser.OpenSubKey($Key)
        try {
            if ($null -eq $Registry -or $Registry.GetValue('') -ne 'upstream-preservation-sentinel') {
                throw "Upstream registration changed: $Key"
            }
        } finally { if ($null -ne $Registry) { $Registry.Dispose() } }
    }
    if (-not (Test-Path -LiteralPath $Shortcut) -or
        (Get-Content -LiteralPath $Shortcut -Raw).Trim() -ne 'upstream-preservation-sentinel') {
        throw 'Upstream shortcut was changed.'
    }
    $UpstreamProcess.Refresh()
    if ($UpstreamProcess.HasExited) { throw 'Evergreen terminated the upstream process.' }
}

function Invoke-Installer([string]$Directory, [string]$Tasks, [string]$Label) {
    $Process = Start-Process -FilePath $Installer -WindowStyle Hidden -PassThru -Wait -ArgumentList @(
        '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/SP-', '/CURRENTUSER',
        '/NoAutoStart', '/NoRunOnStartup', '/NoVCRedist', '/NoEdgeWebView', '/NoDeployInstaller',
        "/DIR=`"$Directory`"", "/TASKS=`"$Tasks`"", "/LOG=`"$Root\$Label.log`""
    )
    if ($Process.ExitCode -ne 0) { throw "$Label failed: $($Process.ExitCode)" }
    & (Join-Path $Directory 'uniget.exe') tui --help
    if ($LASTEXITCODE -ne 0) { throw "$Label installed CLI failed." }
    Assert-UpstreamIntact
}

Invoke-Installer $InstallDir 'regularinstall' 'fresh-install'
$Protocol = [Microsoft.Win32.Registry]::CurrentUser.OpenSubKey('Software\Classes\evergreen-endpoint\shell\open\command')
try {
    if ($null -eq $Protocol -or $Protocol.GetValue('') -ne "`"$InstallDir\Evergreen Endpoint Package Manager.exe`" `"%1`"") {
        throw 'Evergreen protocol registration does not target the installed application.'
    }
} finally { if ($null -ne $Protocol) { $Protocol.Dispose() } }
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
    & (Join-Path $InstallDir 'uniget.exe') 'evergreen-endpoint://show'
    if ($LASTEXITCODE -ne 0) { throw 'Evergreen protocol activation failed.' }
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
Assert-UpstreamIntact
if (Test-Path 'HKCU:\Software\Classes\evergreen-endpoint') {
    throw 'Uninstall left Evergreen protocol registration behind.'
}

Invoke-Installer $PortableDir 'portableinstall' 'portable-install'
if (-not (Test-Path -LiteralPath (Join-Path $PortableDir 'ForceUniGetUIPortable'))) {
    throw 'Portable installation marker was not created.'
}
if (Get-ChildItem -LiteralPath $PortableDir -Filter 'unins*.exe') {
    throw 'Portable installation unexpectedly registered an uninstaller.'
}
Write-Host 'Verified fresh install, same-version reinstall, desktop startup, uninstall, and portable install.'
$UpstreamProcess.Kill()
$UpstreamProcess.WaitForExit()
Write-Host 'Verified upstream registrations, shortcut, and running process survived every installer operation.'
