# Evergreen Endpoint | APPS — identity test v0.2

This folder contains the front-end identity for **Evergreen Endpoint | APPS**. The underlying
project, installer, policies, and management utility retain the **Evergreen Endpoint Package
Manager** name.

## Concept

The mark combines three ideas:

- an evergreen sprout for the Evergreen Endpoint product family;
- a package for the employee-facing app catalog;
- a gold check for organization approval and managed trust.

The faceted construction, forest and mint colors, and restrained gold detail connect it to the
existing Technicull Solutions Evergreen identity. The geometry is simpler than the exploration
images so it survives Windows taskbar and Start menu sizes.

## Core identity

| Token | Value | Intended use |
|---|---:|---|
| Deep forest | `#0D3F30` | Primary brand surface, dark navigation |
| Forest | `#176B4D` | Primary actions and active states |
| Evergreen | `#075844` | Icon depth and dark accents |
| Mint | `#74C7A4` | Secondary brand plane and positive state |
| Pale mint | `#BDEED8` | Highlights and dark-theme text |
| Gold | `#D69B2D` | Approval, focus, and small identity accents |
| Ink | `#17211D` | Primary text |
| Muted | `#5F6C66` | Secondary text |
| Paper | `#F7FAF8` | Light application background |
| Line | `#DCE5DF` | Borders and separators |

Use Lato for marketing and product wordmarks. Use Segoe UI Variable/Segoe UI for the Windows
interface so controls retain native Windows metrics and accessibility behavior.

## Asset roles

- `evergreen-endpoint-apps-wordmark-light.svg`: supplied APPS wordmark for light surfaces.
- `evergreen-endpoint-apps-wordmark-dark.svg`: supplied APPS wordmark for dark surfaces.
- `evergreen-endpoint-package-manager-mark.svg`: full app mark on the branded rounded-square surface.
- `evergreen-endpoint-package-manager-glyph.svg`: transparent sprout/package/check glyph.
- `evergreen-endpoint-package-manager-wordmark.svg`: earlier exploratory package-manager lockup, retained for design history.
- `evergreen-endpoint-package-manager-tray-light.svg`: one-color glyph for dark taskbars.
- `evergreen-endpoint-package-manager-tray-dark.svg`: one-color glyph for light taskbars.
- `evergreen-endpoint-package-manager.ico`: multi-resolution Windows icon candidate.
- `png/`: explicit Windows-size app and tray renders.
- `identity-preview-v0.1.png`: review board showing palette and small-size behavior.
- `app-shell-preview-v0.2.png`: product-owned titlebar and revised shell application test.
- `icon-directions-v0.2.png`: three Penpot-aligned icon directions with small-size comparisons.

## First review criteria

1. The mark reads as “trusted apps” rather than cybersecurity or shipping logistics.
2. The sprout remains recognizable at 20–32 px.
3. The package/check remains recognizable at 16 px.
4. The tray silhouette is distinct on light and dark taskbars.
5. The mark feels related to Evergreen Endpoint without duplicating the shield product icon.
6. Gold remains an accent and never carries meaning by itself.

Regenerate with:

```powershell
python scripts/brand/generate-evergreen-endpoint-package-manager-assets.py
```

The generator requires Pillow and does not affect application runtime dependencies.
