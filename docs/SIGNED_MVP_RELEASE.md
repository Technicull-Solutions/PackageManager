# Signed MVP release path

The release objective is a trusted Windows employee installer and separate portable
administrator client on GitHub, followed by the employee installer's WinGet listing.
The current maintained `test-release.yml` produces unsigned x64 test packages. The
legacy upstream release workflow is disabled for this repository and must not be used.

## P0: publisher identity and signing

Recommended path: Microsoft Artifact Signing Public Trust, subject to publisher eligibility
and successful identity validation. Set up an Azure subscription, signing account,
verified Technicull publisher identity, and Public Trust certificate profile. Use a
repository-scoped GitHub Actions OIDC identity with the certificate-profile signer role.
Keep credentials and private keys out of the repository and chat. An existing publicly
trusted certificate in Key Vault is an alternative supported by `scripts/sign.ps1`;
that script is not an Artifact Signing integration.

Publisher setup is external work and remains blocked until the account/profile exists.
Do not call unsigned test artifacts a production release or promise that signing eliminates
all SmartScreen or organizational policy prompts. A malware detection needs separate investigation.

## P0: production pipeline

Add a maintained signed release workflow once the provider is selected. A protected
release environment should bind the source commit, version and build, signer identity,
and release channel. Build and test NativeAOT for each advertised architecture.

Required order: stage final employee binaries and helper; sign binaries and portable
administrator; verify trusted Authenticode status and expected publisher; regenerate
employee integrity metadata; archive final signed bytes; build the installer with a
signed uninstaller; sign and verify the installer; generate SHA-256 checksums; verify
extracted ZIP signatures and integrity; exercise fresh install, upgrade, repair/reinstall,
portable startup, and uninstall on disposable Windows. Verify helper-to-host signer binding.
Never modify the bytes of signed executables after signing.

`scripts/build.ps1` currently explicitly disables installer signing and packages the
employee archive before any signing hook. Integrating a signing action after it finishes
is insufficient: it would leave unsigned payloads and stale hashes inside packages.
Update its staging/installer boundaries and `scripts/package-admin.ps1` together.

Publish a GitHub release only after these gates pass. Include employee Setup EXE,
employee portable ZIP, admin portable ZIP, policies, checksums, supported OS/architecture,
publisher, known limitations and concise release notes. Keep asset URLs immutable.
Do not advertise ARM64 runtime coverage from an x64 smoke test or change the broader
NativeAOT support matrix to hide missing coverage.

## P0: version and update contract

Use SemVer public versions and a globally increasing build number. Because 0.2.0 test
assets already exist, use `0.2.1-rc.1` for the next candidate, then `0.2.1` when signed
MVP acceptance passes. Do not reissue different bytes under the same release identity.
Windows file versions include the build (`0.2.1.5`, for example). See [versioning](VERSIONING.md).

Prereleases belong in Beta metadata; stable releases belong in Current. Stamping clears
old asset entries and does not publish an installer. Populate final owned HTTPS URLs and
hashes only after signed assets exist. Test update checks, cancellation/failure/retry,
the intended channel and architecture, and restart into the installed target directory.

## P0: usable employee and administrator MVP

Verify Apps is enabled by default, library deployment from the portable admin, source
and policy enforcement, package install/update/uninstall, actionable failures, activity
history, and safe import/export. Complete the [accessibility review](ACCESSIBILITY.md)
and [trust checks](WINDOWS_RELEASE_TRUST.md). Preserve licenses and provenance while
removing stale customer-facing upstream links. No remote administration is implied.

## P1: WinGet

After the signed GitHub installer is public, generate manifests for
`TechnicullSolutions.EvergreenEndpointPackageManager` with the exact publisher/product,
version, architecture, installer URL, SHA-256, Inno installer type, scopes, silent switches,
and Add/Remove Programs identity. Validate with WinGet and test install/upgrade/uninstall
in a clean Windows sandbox. Submit a manifest PR to `microsoft/winget-pkgs`; catalog
availability depends on their validation and review. Keep the first listing focused on
the employee installer; decide separately whether the authoring-only admin needs a listing.
Never invent installer hashes or submit a manifest pointing at an unsigned/stale candidate.

Sources reviewed 2026-10-18:
[Artifact Signing setup](https://learn.microsoft.com/en-us/azure/artifact-signing/quickstart),
[trust models](https://learn.microsoft.com/en-us/azure/artifact-signing/concept-trust-models),
[WinGet manifests](https://learn.microsoft.com/en-us/windows/package-manager/package/manifest),
[submission](https://learn.microsoft.com/en-us/windows/package-manager/package/repository).
