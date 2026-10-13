# Visual identity validation — v0.3

## Canonical-source checks

| Check | Result |
|---|---|
| App icon source | Supplied `evergreen-package-manager-icon-dark.svg` |
| Light logo source | Supplied `evergreen-package-manager-logo-light.svg` |
| Alternate icon concepts | Removed |
| Windows ICO frames | 16, 20, 24, 32, 48, 64, 128, and 256 px |
| Explicit app PNGs | 16 through 512 px, including 44 and 150 px Windows assets |
| Background-free mark PNGs | Supplied light/dark artwork at 16, 32, 64, 128, 256, 512, and 1024 px |
| Tray PNGs | Derived from the supplied background-free marks at 16, 20, 24, and 32 px |
| UI wordmarks | Supplied EEA PNG families at 0.5x, 1x, and 2x |
| Installer and tile assets | Generated from the same canonical SVG pair |

## Visual checks

- The arrow-and-package symbol remains recognizable at taskbar, notification-area, and Start menu
  sizes.
- The dark SVG supplies the stable rounded-square surface needed by Windows shell icons.
- The dark background-free PNG is used on light taskbars and product surfaces.
- The light background-free PNG is used on dark taskbars and product surfaces.
- Light mode uses `EEA-Logo-Text_Light` on evergreen chrome.
- Dark mode uses `EEA-Logo-Text_Light-Alternate` on forest chrome.
- Package IDs, raw repositories, and manager controls remain outside the employee-facing result
  list in the shell preview.

The checked-in icon SVGs, supplied background-free logo PNGs, and supplied EEA wordmark PNGs are
the source of truth. Generated raster, ICO, Windows tile, and installer files are reproducible outputs of
`scripts/brand/generate-evergreen-endpoint-package-manager-assets.py`.
