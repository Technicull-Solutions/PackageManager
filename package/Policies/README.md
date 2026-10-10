# Evergreen Endpoint | Apps administrative configuration

Copy `EvergreenEndpointApps.admx` to `%SystemRoot%\PolicyDefinitions` and copy the `en-US`
resource file into `%SystemRoot%\PolicyDefinitions\en-US`. For a domain, copy the same files to the
domain Central Store under `SYSVOL\<domain>\Policies\PolicyDefinitions`.

Policies appear under:

`Computer Configuration > Administrative Templates > Technicull Solutions > Evergreen Endpoint | Apps > Package managers`

Package-manager configuration is machine-owned and resolves in this order:

1. Group Policy values
2. The JSON file selected by policy
3. `%ProgramData%\Technicull Solutions\Evergreen Endpoint\Apps\package-managers.json`
4. Built-in defaults

Deploy the JSON file with permissions that allow standard users to read it and only administrators
or the deployment system to modify it. Use `package-managers.example.json` as the starting point.
Allowed-source entries match source/repository names case-insensitively. Register or remove the
corresponding WinGet and PowerShell sources as part of device deployment; the application filters
the configured source inventory against the allowlist. The desktop UI, terminal UI, IPC API, and
command-line API cannot add or remove sources for managed package managers.

Configured executable paths fail closed if the file does not exist. Leave an executable path
unconfigured to use the application's normal trusted executable discovery.
