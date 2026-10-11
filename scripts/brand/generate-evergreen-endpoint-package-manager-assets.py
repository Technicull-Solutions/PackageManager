#!/usr/bin/env python3
"""Generate Evergreen Endpoint | APPS front-end assets for Package Manager."""

from __future__ import annotations

from pathlib import Path
from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "media" / "brand" / "evergreen-endpoint-package-manager"
PNG = OUT / "png"
APP_BRAND = ROOT / "src" / "UniGetUI.Avalonia" / "Assets" / "Brand"
SHARED_ASSETS = ROOT / "src" / "SharedAssets"
INSTALLER_ASSETS = ROOT / "InstallerExtras"

COLORS = {
    "forest": "#176B4D",
    "deep_forest": "#0D3F30",
    "evergreen": "#075844",
    "mint": "#BDEED8",
    "mint_mid": "#74C7A4",
    "gold": "#D69B2D",
    "ink": "#17211D",
    "muted": "#5F6C66",
    "paper": "#F7FAF8",
    "line": "#DCE5DF",
    "white": "#FFFFFF",
}


def scaled(points: list[tuple[int, int]], scale: float) -> list[tuple[int, int]]:
    return [(round(x * scale), round(y * scale)) for x, y in points]


def draw_mark(size: int, *, background: bool = True, monochrome: str | None = None) -> Image.Image:
    factor = 4
    canvas = size * factor
    scale = canvas / 512
    image = Image.new("RGBA", (canvas, canvas), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)

    def polygon(points: list[tuple[int, int]], fill: str) -> None:
        draw.polygon(scaled(points, scale), fill=fill)

    if background:
        inset = round(20 * scale)
        radius = round(108 * scale)
        draw.rounded_rectangle(
            (inset, inset, canvas - inset, canvas - inset),
            radius=radius,
            fill=COLORS["deep_forest"],
            outline=COLORS["gold"],
            width=max(1, round(5 * scale)),
        )

    if monochrome:
        # A deliberately compact silhouette for 16–24 px notification-area rendering.
        polygon([(256, 242), (216, 154), (256, 72), (296, 154)], monochrome)
        polygon([(240, 224), (170, 146), (105, 136), (130, 198), (210, 250)], monochrome)
        polygon([(272, 224), (342, 146), (407, 136), (382, 198), (302, 250)], monochrome)
        polygon([(256, 230), (98, 301), (256, 383), (414, 301), (414, 377), (256, 458), (98, 377), (98, 301)], monochrome)
    else:
        polygon([(256, 242), (216, 154), (256, 72), (296, 154)], "#54AD88")
        polygon([(256, 242), (256, 72), (296, 154)], COLORS["mint"])
        polygon([(240, 224), (170, 146), (105, 136), (130, 198), (210, 250)], COLORS["mint_mid"])
        polygon([(240, 224), (210, 250), (170, 146)], COLORS["forest"])
        polygon([(272, 224), (342, 146), (407, 136), (382, 198), (302, 250)], COLORS["mint"])
        polygon([(272, 224), (342, 146), (302, 250)], COLORS["mint_mid"])

        polygon([(256, 224), (101, 297), (256, 373), (411, 297)], COLORS["mint_mid"])
        polygon([(101, 297), (256, 373), (256, 454), (101, 378)], COLORS["forest"])
        polygon([(256, 373), (411, 297), (411, 378), (256, 454)], COLORS["evergreen"])
        polygon([(256, 224), (256, 373), (101, 297)], "#54AD88")
        polygon([(256, 224), (411, 297), (256, 373)], "#94DDBD")

        check = scaled([(278, 382), (311, 415), (378, 344)], scale)
        draw.line(
            check,
            fill=COLORS["gold"],
            width=max(2, round(24 * scale)),
            joint="curve",
        )

    return image.resize((size, size), Image.Resampling.LANCZOS)


def draw_icon_direction(size: int, direction: int) -> Image.Image:
    """Render one of the exploratory icon directions on the shared app-icon surface."""
    if direction == 1:
        return draw_mark(size)

    factor = 4
    canvas = size * factor
    scale = canvas / 512
    image = Image.new("RGBA", (canvas, canvas), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)

    def polygon(points: list[tuple[int, int]], fill: str) -> None:
        draw.polygon(scaled(points, scale), fill=fill)

    inset = round(20 * scale)
    draw.rounded_rectangle(
        (inset, inset, canvas - inset, canvas - inset),
        radius=round(108 * scale),
        fill=COLORS["deep_forest"],
        outline=COLORS["gold"],
        width=max(1, round(5 * scale)),
    )

    if direction == 2:
        polygon([(256, 70), (206, 160), (232, 160), (166, 248), (346, 248), (280, 160), (306, 160)], COLORS["mint"])
        polygon([(256, 70), (256, 248), (166, 248), (232, 160), (206, 160)], COLORS["mint_mid"])
        polygon([(256, 220), (102, 292), (256, 367), (410, 292)], "#94DDBD")
        polygon([(102, 292), (256, 367), (256, 447), (102, 372)], COLORS["forest"])
        polygon([(256, 367), (410, 292), (410, 372), (256, 447)], COLORS["evergreen"])
        polygon([(244, 202), (268, 202), (268, 350), (244, 362)], COLORS["gold"])
    elif direction == 3:
        polygon([(128, 112), (194, 112), (194, 398), (128, 364)], COLORS["mint_mid"])
        polygon([(194, 112), (392, 112), (350, 178), (194, 178)], COLORS["mint"])
        polygon([(194, 220), (340, 220), (300, 282), (194, 282)], "#94DDBD")
        polygon([(194, 332), (392, 332), (350, 398), (194, 398)], COLORS["mint"])
        polygon([(128, 364), (194, 398), (194, 332), (128, 300)], COLORS["forest"])
        draw.rounded_rectangle(
            (round(348 * scale), round(236 * scale), round(396 * scale), round(284 * scale)),
            radius=round(8 * scale),
            fill=COLORS["gold"],
        )
    else:
        raise ValueError(f"Unknown direction: {direction}")

    return image.resize((size, size), Image.Resampling.LANCZOS)


def build_icon_directions_preview() -> Image.Image:
    width, height = 1600, 920
    board = Image.new("RGB", (width, height), COLORS["paper"])
    draw = ImageDraw.Draw(board)
    title = font("Lato-Black.ttf", 48)
    heading = font("Lato-Bold.ttf", 24)
    body = font("Lato-Regular.ttf", 18)
    small = font("Lato-Regular.ttf", 15)

    draw.text((72, 54), "Evergreen Endpoint | APPS", fill=COLORS["ink"], font=title)
    draw.text((74, 116), "Icon direction study · Penpot-aligned palette and geometry", fill=COLORS["muted"], font=body)

    concepts = [
        (1, "01 · TRUSTED GROWTH", "Sprout + package + approval", "Most expressive; strongest employee meaning."),
        (2, "02 · SPRUCE PARCEL", "Evergreen + package ribbon", "Closer to the product family; calmer silhouette."),
        (3, "03 · ENDPOINT E", "Evergreen facets + E monogram", "Most technical; clearest at very small sizes."),
    ]
    for index, (direction, name, description, note) in enumerate(concepts):
        x = 72 + index * 500
        draw.rounded_rectangle((x, 182, x + 450, 792), radius=24, fill=COLORS["white"], outline=COLORS["line"], width=2)
        icon = draw_icon_direction(330, direction)
        board.paste(icon, (x + 60, 222), icon)
        draw.text((x + 34, 578), name, fill=COLORS["deep_forest"], font=heading)
        draw.text((x + 34, 622), description, fill=COLORS["ink"], font=body)
        draw.text((x + 34, 654), note, fill=COLORS["muted"], font=small)

        sx = x + 34
        for icon_size in (64, 44, 32, 20, 16):
            small_icon = draw_icon_direction(icon_size, direction)
            board.paste(small_icon, (sx, 706), small_icon)
            draw.text((sx, 770), str(icon_size), fill=COLORS["muted"], font=small)
            sx += 78

    draw.text((72, 850), "Shared rules: Lato identity · 8 px UI radius · forest/mint core · gold only as an accent", fill=COLORS["muted"], font=small)
    return board


def svg_mark(*, background: bool = True, monochrome: str | None = None) -> str:
    bg = ""
    if background:
        bg = (
            f'<rect x="20" y="20" width="472" height="472" rx="108" '
            f'fill="{COLORS["deep_forest"]}" stroke="{COLORS["gold"]}" stroke-width="5"/>'
        )

    if monochrome:
        art = f"""
  <path fill="{monochrome}" d="m256 242-40-88 40-82 40 82-40 88Z"/>
  <path fill="{monochrome}" d="m240 224-70-78-65-10 25 62 80 52 30-26Z"/>
  <path fill="{monochrome}" d="m272 224 70-78 65-10-25 62-80 52-30-26Z"/>
  <path fill="{monochrome}" d="m256 230-158 71v76l158 81 158-81v-76l-158-71Z"/>
"""
    else:
        art = f"""
  <path fill="#54AD88" d="m256 242-40-88 40-82 40 82-40 88Z"/>
  <path fill="{COLORS["mint"]}" d="m256 242 0-170 40 82-40 88Z"/>
  <path fill="{COLORS["mint_mid"]}" d="m240 224-70-78-65-10 25 62 80 52 30-26Z"/>
  <path fill="{COLORS["forest"]}" d="m240 224-30 26-40-104 70 78Z"/>
  <path fill="{COLORS["mint"]}" d="m272 224 70-78 65-10-25 62-80 52-30-26Z"/>
  <path fill="{COLORS["mint_mid"]}" d="m272 224 70-78-40 104-30-26Z"/>
  <path fill="#54AD88" d="m256 224-155 73 155 76V224Z"/>
  <path fill="#94DDBD" d="m256 224 155 73-155 76V224Z"/>
  <path fill="{COLORS["forest"]}" d="m101 297 155 76v81l-155-76v-81Z"/>
  <path fill="{COLORS["evergreen"]}" d="m256 373 155-76v81l-155 76v-81Z"/>
  <path fill="none" stroke="{COLORS["gold"]}" stroke-width="24" stroke-linecap="round" stroke-linejoin="round" d="m278 382 33 33 67-71"/>
"""

    return f"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" role="img" aria-label="Evergreen Endpoint Package Manager mark">
{bg}{art}</svg>
"""


def font(name: str, size: int) -> ImageFont.FreeTypeFont:
    path = Path("C:/Windows/Fonts") / name
    return ImageFont.truetype(str(path), size=size)


def build_preview() -> Image.Image:
    width, height = 1600, 1050
    board = Image.new("RGB", (width, height), COLORS["paper"])
    draw = ImageDraw.Draw(board)
    title = font("Lato-Black.ttf", 54)
    heading = font("Lato-Bold.ttf", 24)
    body = font("Lato-Regular.ttf", 20)
    small = font("Lato-Regular.ttf", 16)
    label = font("Lato-Bold.ttf", 16)

    draw.text((74, 58), "Evergreen Endpoint", fill=COLORS["ink"], font=title)
    title_width = draw.textbbox((0, 0), "Evergreen Endpoint", font=title)[2]
    draw.text((78 + title_width, 76), "APPS", fill=COLORS["gold"], font=heading)
    draw.text((78, 126), "First-pass visual identity test · v0.1", fill=COLORS["muted"], font=body)

    # Primary mark and lockup.
    primary = draw_mark(350)
    board.paste(primary, (72, 190), primary)
    draw.text((470, 225), "Software employees can trust.", fill=COLORS["deep_forest"], font=font("Lato-Black.ttf", 38))
    draw.text((472, 282), "A managed Windows app catalog for organizations.", fill=COLORS["muted"], font=body)
    draw.text((472, 340), "MARK", fill=COLORS["gold"], font=label)
    draw.text((472, 370), "Evergreen sprout + managed package + approval check", fill=COLORS["ink"], font=body)
    draw.text((472, 414), "VOICE", fill=COLORS["gold"], font=label)
    draw.text((472, 444), "Clear · dependable · calm · employee-friendly", fill=COLORS["ink"], font=body)

    # Palette.
    draw.text((72, 585), "CORE PALETTE", fill=COLORS["ink"], font=heading)
    swatches = [
        ("Deep forest", "deep_forest"),
        ("Forest", "forest"),
        ("Mint", "mint_mid"),
        ("Gold", "gold"),
        ("Ink", "ink"),
        ("Paper", "paper"),
    ]
    for index, (name, key) in enumerate(swatches):
        x = 72 + index * 235
        y = 630
        color = COLORS[key]
        draw.rounded_rectangle((x, y, x + 200, y + 86), radius=14, fill=color, outline=COLORS["line"])
        text_color = COLORS["white"] if key in {"deep_forest", "forest", "ink"} else COLORS["ink"]
        draw.text((x + 14, y + 16), name, fill=text_color, font=label)
        draw.text((x + 14, y + 48), color, fill=text_color, font=small)

    # Size and taskbar checks.
    draw.text((72, 770), "WINDOWS SIZE CHECK", fill=COLORS["ink"], font=heading)
    sizes = [64, 44, 32, 24, 20, 16]
    x = 72
    for icon_size in sizes:
        icon = draw_mark(icon_size)
        board.paste(icon, (x, 825), icon)
        draw.text((x, 902), f"{icon_size}px", fill=COLORS["muted"], font=small)
        x += max(icon_size + 70, 120)

    draw.rounded_rectangle((910, 785, 1525, 974), radius=24, fill="#101614")
    draw.text((944, 807), "TRAY / HIGH-CONTRAST", fill=COLORS["mint"], font=label)
    tray_light = draw_mark(48, background=False, monochrome=COLORS["paper"])
    tray_dark = draw_mark(48, background=False, monochrome=COLORS["deep_forest"])
    board.paste(tray_light, (950, 858), tray_light)
    draw.rounded_rectangle((1050, 842, 1134, 926), radius=12, fill=COLORS["paper"])
    board.paste(tray_dark, (1068, 860), tray_dark)
    draw.text((1175, 853), "One-color silhouette", fill=COLORS["white"], font=body)
    draw.text((1175, 888), "No gold dependency at 16 px", fill="#A9CABC", font=small)

    return board


def build_shell_preview() -> Image.Image:
    width, height = 1600, 1000
    image = Image.new("RGB", (width, height), COLORS["paper"])
    draw = ImageDraw.Draw(image)
    display = font("Lato-Black.ttf", 42)
    heading = font("Lato-Bold.ttf", 25)
    body = font("Lato-Regular.ttf", 19)
    small = font("Lato-Regular.ttf", 15)
    label = font("Lato-Bold.ttf", 16)

    # Product-owned title bar and navigation.
    draw.rectangle((0, 0, width, 52), fill=COLORS["deep_forest"])
    title_wordmark = Image.open(APP_BRAND / "evergreen-endpoint-apps-wordmark-dark.png").convert("RGB")
    title_wordmark = title_wordmark.resize((145, 48), Image.Resampling.LANCZOS)
    image.paste(title_wordmark, (14, 2))
    draw.line((1468, 29, 1478, 29), fill=COLORS["white"], width=1)
    draw.rectangle((1511, 21, 1521, 31), outline=COLORS["white"], width=1)
    draw.line((1557, 22, 1567, 32), fill=COLORS["white"], width=1)
    draw.line((1567, 22, 1557, 32), fill=COLORS["white"], width=1)
    draw.rectangle((0, 52, 284, height), fill=COLORS["deep_forest"])
    sidebar_wordmark = title_wordmark.resize((218, 72), Image.Resampling.LANCZOS)
    image.paste(sidebar_wordmark, (24, 64))

    nav = [("Home", 170), ("Apps", 228), ("Installed", 286), ("Updates", 344), ("Activity", 402)]
    for name, y in nav:
        if name == "Apps":
            draw.rounded_rectangle((16, y - 10, 268, y + 38), radius=9, fill=COLORS["forest"])
            draw.rectangle((16, y - 10, 21, y + 38), fill=COLORS["gold"])
        draw.text((42, y), name, fill=COLORS["white"] if name == "Apps" else "#BDE0D1", font=body)

    draw.text((34, 900), "Managed by your organization", fill="#A9CABC", font=small)
    draw.ellipse((34, 930, 46, 942), fill="#2E8B57")
    draw.text((54, 925), "Status: Up to date", fill=COLORS["mint"], font=small)
    draw.text((54, 950), "Version 0.1.0 · Build 1", fill="#A9CABC", font=small)

    # Employee-facing app catalog.
    draw.text((342, 90), "Apps", fill=COLORS["ink"], font=display)
    draw.text((344, 146), "Find trusted software approved for this device.", fill=COLORS["muted"], font=body)
    draw.rounded_rectangle((342, 196, 1524, 260), radius=13, fill=COLORS["white"], outline=COLORS["line"], width=2)
    draw.text((370, 217), "Search apps, publishers, or tools", fill="#7A8881", font=body)
    draw.rounded_rectangle((1396, 207, 1508, 249), radius=10, fill=COLORS["forest"])
    draw.text((1424, 217), "Search", fill=COLORS["white"], font=label)

    draw.text((342, 314), "Recommended for your team", fill=COLORS["ink"], font=heading)
    cards = [
        ("Microsoft 365", "Microsoft Corporation", "Microsoft Store", "Install", "m365"),
        ("7-Zip", "Igor Pavlov", "WinGet", "Install", "7zip"),
        ("PowerShell", "Microsoft Corporation", "WinGet", "Update", "powershell"),
    ]
    for index, (name, publisher, source, action, icon_kind) in enumerate(cards):
        y = 366 + index * 166
        draw.rounded_rectangle((342, y, 1524, y + 138), radius=14, fill=COLORS["white"], outline=COLORS["line"], width=2)
        if icon_kind == "m365":
            draw.rounded_rectangle((368, y + 27, 452, y + 111), radius=18, fill="#F4F4F4")
            draw.polygon([(388, y + 46), (409, y + 35), (409, y + 69), (388, y + 69)], fill="#F25022")
            draw.polygon([(413, y + 35), (434, y + 46), (434, y + 69), (413, y + 69)], fill="#7FBA00")
            draw.polygon([(388, y + 73), (409, y + 73), (409, y + 103), (388, y + 92)], fill="#00A4EF")
            draw.polygon([(413, y + 73), (434, y + 73), (434, y + 92), (413, y + 103)], fill="#FFB900")
        elif icon_kind == "7zip":
            draw.rounded_rectangle((368, y + 27, 452, y + 111), radius=18, fill="#FFFFFF", outline="#202020", width=2)
            draw.text((390, y + 49), "7z", fill="#202020", font=font("Lato-Black.ttf", 27))
        else:
            draw.rounded_rectangle((368, y + 27, 452, y + 111), radius=18, fill="#1D5B7A")
            draw.text((385, y + 49), ">_", fill=COLORS["white"], font=font("Lato-Black.ttf", 27))
        draw.text((480, y + 25), name, fill=COLORS["ink"], font=heading)
        draw.text((480, y + 62), publisher, fill=COLORS["muted"], font=small)
        badge_width = 24 + draw.textbbox((0, 0), source, font=small)[2]
        draw.rounded_rectangle((480, y + 91, 480 + badge_width, y + 120), radius=14, fill="#E9F6F0")
        draw.text((492, y + 96), source, fill=COLORS["forest"], font=small)
        button_fill = COLORS["forest"] if action == "Install" else COLORS["deep_forest"]
        draw.rounded_rectangle((1384, y + 46, 1494, y + 94), radius=10, fill=button_fill)
        action_width = draw.textbbox((0, 0), action, font=label)[2]
        draw.text((1439 - action_width / 2, y + 59), action, fill=COLORS["white"], font=label)

    draw.text((342, 900), "Friendly names lead. Package IDs and source URLs stay in Technical details.", fill=COLORS["muted"], font=small)
    return image


def write_wordmark_svg() -> None:
    wordmark = f"""<svg xmlns="http://www.w3.org/2000/svg" width="1040" height="260" viewBox="0 0 1040 260" role="img" aria-label="Evergreen Endpoint APPS">
  <g transform="translate(20 20) scale(.43)">{svg_mark(background=True).split('>', 1)[1].rsplit('</svg>', 1)[0]}</g>
  <text x="270" y="115" fill="{COLORS["ink"]}" font-family="Lato, Segoe UI, sans-serif" font-size="64" font-weight="900">Evergreen Endpoint</text>
  <text x="273" y="176" fill="{COLORS["gold"]}" font-family="Lato, Segoe UI, sans-serif" font-size="34" font-weight="800" letter-spacing="4">APPS</text>
  <text x="275" y="216" fill="{COLORS["muted"]}" font-family="Lato, Segoe UI, sans-serif" font-size="18" font-weight="700" letter-spacing="2">A TECHNICULL SOLUTIONS PRODUCT</text>
</svg>
"""
    (OUT / "evergreen-endpoint-package-manager-wordmark.svg").write_text(wordmark, encoding="utf-8")


def write_windows_packaging_assets() -> None:
    """Keep Windows shell and installer artwork on the selected product mark."""
    assets = SHARED_ASSETS / "Assets"
    images = assets / "Images"

    icon_512 = draw_mark(512)
    icon_512.save(images / "icon.png", optimize=True)
    icon_512.save(images / "icon_unsquare.png", optimize=True)
    icon_512.convert("RGB").save(images / "icon.bmp")

    ico_sizes = [(16, 16), (20, 20), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)]
    icon_512.save(SHARED_ASSETS / "icon.ico", format="ICO", sizes=ico_sizes)
    icon_512.save(images / "icon.ico", format="ICO", sizes=ico_sizes)

    for file_name, size in (
        ("Square44x44Logo.png", 44),
        ("Square150x150Logo.png", 150),
        ("StoreLogo.png", 50),
    ):
        draw_mark(size).save(assets / file_name, optimize=True)

    wide = Image.new("RGBA", (310, 150), COLORS["deep_forest"])
    supplied_wordmark = Image.open(APP_BRAND / "evergreen-endpoint-apps-wordmark-dark.png").convert("RGBA")
    supplied_wordmark.thumbnail((290, 96), Image.Resampling.LANCZOS)
    wide.alpha_composite(supplied_wordmark, ((310 - supplied_wordmark.width) // 2, (150 - supplied_wordmark.height) // 2))
    wide.save(assets / "Wide310x150Logo.png", optimize=True)

    banner = Image.new("RGB", (164, 314), COLORS["deep_forest"])
    banner_mark = draw_mark(124)
    banner.paste(banner_mark, (20, 43), banner_mark)
    banner_draw = ImageDraw.Draw(banner)
    banner_draw.text((18, 205), "EVERGREEN", fill=COLORS["white"], font=font("Lato-Black.ttf", 17))
    banner_draw.text((18, 228), "ENDPOINT", fill=COLORS["white"], font=font("Lato-Black.ttf", 17))
    banner_draw.text((18, 260), "PACKAGE", fill=COLORS["mint"], font=font("Lato-Bold.ttf", 12))
    banner_draw.text((18, 278), "MANAGER", fill=COLORS["mint"], font=font("Lato-Bold.ttf", 12))
    banner.save(INSTALLER_ASSETS / "installer-banner.png", optimize=True)
    draw_mark(256).save(INSTALLER_ASSETS / "unigetui-256.png", optimize=True)


def main() -> None:
    PNG.mkdir(parents=True, exist_ok=True)
    (OUT / "evergreen-endpoint-package-manager-mark.svg").write_text(svg_mark(), encoding="utf-8")
    (OUT / "evergreen-endpoint-package-manager-glyph.svg").write_text(svg_mark(background=False), encoding="utf-8")
    (OUT / "evergreen-endpoint-package-manager-tray-light.svg").write_text(
        svg_mark(background=False, monochrome=COLORS["paper"]), encoding="utf-8"
    )
    (OUT / "evergreen-endpoint-package-manager-tray-dark.svg").write_text(
        svg_mark(background=False, monochrome=COLORS["deep_forest"]), encoding="utf-8"
    )
    write_wordmark_svg()

    sizes = [16, 20, 24, 32, 44, 48, 64, 128, 150, 256, 512]
    images: list[Image.Image] = []
    for icon_size in sizes:
        icon = draw_mark(icon_size)
        icon.save(PNG / f"app-icon-{icon_size}.png", optimize=True)
        images.append(icon)

    images[-1].save(
        OUT / "evergreen-endpoint-package-manager.ico",
        format="ICO",
        sizes=[(16, 16), (20, 20), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)],
    )

    write_windows_packaging_assets()

    for theme, color in (("light", COLORS["paper"]), ("dark", COLORS["deep_forest"])):
        for icon_size in (16, 20, 24, 32):
            draw_mark(icon_size, background=False, monochrome=color).save(
                PNG / f"tray-{theme}-{icon_size}.png", optimize=True
            )

    build_preview().save(OUT / "identity-preview-v0.1.png", optimize=True)
    build_shell_preview().save(OUT / "app-shell-preview-v0.2.png", optimize=True)
    build_icon_directions_preview().save(OUT / "icon-directions-v0.2.png", optimize=True)


if __name__ == "__main__":
    main()
