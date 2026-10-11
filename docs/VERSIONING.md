# Evergreen Endpoint Package Manager versioning

The current application exposes two authoritative values from `CoreData`:

| Value | Current source value | Purpose |
|---|---:|---|
| Version | `0.1.0` | Evergreen development version shown to users and administrators. |
| Build | `1` | Evergreen's independent, monotonically increasing build number. |

The sidebar, tray tooltip, About page, update metadata, and build-freshness calculation must read these centralized values. Display text must never contain copied version literals.

The build number drives the comparison with `main`:

| Difference from main | Indicator | Meaning |
|---:|---|---|
| N | Green | Up to date |
| N-1 to N-2 | Yellow | Slightly behind |
| N-3 to N-4 | Orange | Behind |
| N-5 or older | Red | Critically behind |

Release versions are changed manually through `scripts/set-version.ps1`. A future dated release identifier is assigned only when a release is deliberately triggered; ordinary development builds increment the build number without deriving a date-based release automatically.

The fork began from UniGetUI `3.3.7`. Its exact upstream tag and commit are recorded in `UPSTREAM.json`; upstream provenance does not participate in Evergreen version ordering.
