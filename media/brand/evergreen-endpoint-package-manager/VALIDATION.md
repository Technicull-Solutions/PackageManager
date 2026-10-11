# Visual identity validation — v0.1

## Automated checks

| Check | Result |
|---|---|
| Windows ICO frames | Pass: 16, 20, 24, 32, 48, 64, 128, and 256 px |
| Explicit app PNGs | Pass: 16 through 512 px, including 44 and 150 px Windows assets |
| Tray PNGs | Pass: light and dark variants at 16, 20, 24, and 32 px |
| SVG parsing | Pass: mark, glyph, wordmark, and both tray variants are well-formed XML |
| Transparent artwork | Pass: glyph and tray assets retain alpha |
| Regeneration | Pass: all generated files are reproducible from the checked-in Python script |

## Contrast checks

Ratios use WCAG relative luminance calculations.

| Foreground / background | Ratio | Use |
|---|---:|---|
| Ink / Paper | 15.71:1 | Pass for body text |
| Muted / Paper | 5.23:1 | Pass for body text |
| White / Forest | 6.47:1 | Pass for buttons |
| White / Deep forest | 11.86:1 | Pass for navigation |
| Gold / Deep forest | 4.85:1 | Pass for normal text and focus accents |
| Forest / pale green status surface | 5.82:1 | Pass for source and status badges |
| Gold / Paper | 2.33:1 | Decorative/large display use only; do not use for body text |

## Visual checks

- The three-point shoot reads more clearly as a plant than the earlier two-leaf exploration.
- The package and check remain distinct at 16 px in the generated Windows-size review row.
- The one-color tray mark does not depend on the gold accent and works on light and dark surfaces.
- The rounded-square mark remains related to the faceted Evergreen shield while giving Apps its own
  product identity.
- The shell preview keeps package IDs, raw repositories, and package-manager controls out of the
  employee-facing result list.

## Review status

The asset construction and technical checks pass for a first visual review. Human brand approval is
still required before these files replace shipping icons, installer artwork, Start menu assets, or
application identity resources.
