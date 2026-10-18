# Portable Evergreen Endpoint Admin

The administrator test client is a separate Windows x64 NativeAOT application. Extract the `EvergreenEndpointAdmin.*.x64.zip` release into a writable folder and run `Evergreen Endpoint Admin.exe`. A separate .NET installation is not required. Current test releases are unsigned.

## Author and export a library

1. Open **Software library**. The initial draft contains an empty **Apps** library.
2. Enter the library ID/name, app name, exact package ID, manager, and configured source name; select **Add app**. Use `WinGet`, `PowerShell`, or `PowerShell7`. Microsoft Store applications use WinGet with source `msstore`.
3. Edit the JSON to remove entries, change versions, or maintain multiple libraries. **Validate and format** checks document structure and duplicate identities. It does not certify publisher identity, package availability, or authorization on a target device.
4. Select **Export JSON** and save `SoftwareCatalog.json` in your workspace. Exports replace only the explicitly chosen workspace file after the save dialog's overwrite confirmation. Opening another draft or closing the app prompts before discarding edits.

No broker or package-manager process is required for authoring. The utility does not install software, register sources, sign documents, connect to remote devices, or apply machine policy. It starts with in-memory drafts; save the files you want to carry between devices. Private settings and logs stay in `AdminData` beside the administrator executable, separately from the employee client and UniGetUI. An unwritable application folder or export destination reports an error and does not silently select another folder.

## Test the employee client

The updated employee client enables the library by default under **Apps**. **Search apps** searches configured package sources separately. `Ctrl+1` opens Apps; `Ctrl+5` opens Search apps. With no deployed library, Apps explains how to proceed. Administrative catalog editing is unavailable from employee navigation.

An administrator or deployment system must copy the exported library to:

`%ProgramData%\Technicull Solutions\Evergreen Endpoint\Package Manager\SoftwareCatalog.json`

Create that directory with standard-user read access and administrator/deployment-system write access. Reload Apps after deployment. Do not place the deployed library in an employee-writable directory. The old `%ProgramData%\Devolutions\UniGetUI\SoftwareCatalog.json` is not automatically adopted or modified; explicitly review and export any previous library to the new destination.

The library is a discovery document, not an authorization grant. Existing manager/source policy and package-operation authorization still apply. Unavailable sources are filtered by default; users can inspect unavailable entries through the library filters. Signed organization catalogs and remote catalog retrieval remain future capabilities.

For a self-hosted configuration repository, follow [Gitea catalog testing](GITEA_CATALOG_TESTING.md).
The portable ZIP includes that guide and a `SoftwareCatalog.example.json` seed. This pilot
uses reviewed Git revisions and local deployment; it does not require a Gitea token in Apps.

## Manager configuration and diagnostics

**Package manager configuration** opens, validates, and exports the current version 1 machine-JSON format. Only supported manager names are accepted; explicit executable paths must be absolute. Sources must already be provisioned by device deployment. Read [the deployment guide](../package/Policies/README.md) for Group Policy precedence, exact registry paths, and JSON deployment.

**Local diagnostics** reads the local effective manager enablement and executable selection through the same provider as the employee client. It displays the machine configuration and library locations. It does not connect to a broker or prove that a manager executable/source is usable. Configuration-provider errors may fall back to its built-in defaults and are not a successful validation of the target configuration file.

Portable exports are unsigned drafts. The existing elevated policy-write protocol still requires signed binaries and a protected installation; this utility does not weaken that boundary.

## Validation and packaging

Read [Windows release trust and package contents](WINDOWS_RELEASE_TRUST.md) for the unsigned
testing-build limitation and the signing prerequisite. Windows may block an unsigned download
entirely; this portable utility does not require installation, but its executable still needs
to satisfy the device's security policy.

The release workflow tests the shared employee/admin document contract and publishes both clients. `scripts/package-admin.ps1` publishes the administrator executable with NativeAOT, runs its published `--self-test` document/export path, and packages the portable application with this guide, policy templates, and manager-configuration example. The release pipeline also verifies that its desktop window starts and exits on a disposable Windows runner. Check the release report for actual tested architecture and GUI coverage.
