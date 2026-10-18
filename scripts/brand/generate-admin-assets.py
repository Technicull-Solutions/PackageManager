"""Generate the Admin derivative icon while preserving the canonical package symbol.
Design-time requirements: Pillow and resvg-py (same as the APPS asset generator).
"""
from pathlib import Path
from io import BytesIO

from PIL import Image
import resvg_py

ROOT = Path(__file__).resolve().parents[2]
BRAND = ROOT / 'media/brand/evergreen-endpoint-package-manager'
ADMIN = ROOT / 'src/Evergreen.Admin/Assets'


def main():
    canonical = (BRAND / 'evergreen-package-manager-icon-dark.svg').read_text(encoding='utf-8')
    symbol = canonical[canonical.index('<g '):canonical.rindex('</svg>')]
    svg = f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" role="img" aria-label="Evergreen Endpoint Admin: package with management controls">
<rect width="512" height="512" rx="16" fill="#0D3F30"/>
<g transform="translate(8 0) scale(.82)">{symbol}</g>
<rect x="308" y="312" width="188" height="184" rx="16" fill="#D69B2D" stroke="#0D3F30" stroke-width="12"/>
<g stroke="#17211D" stroke-width="14" stroke-linecap="round" fill="none">
<path d="M340 358 H464 M340 404 H464 M340 450 H464"/>
<path d="M374 344 V372 M432 390 V418 M386 436 V464" stroke-width="20"/>
</g></svg>
'''
    (BRAND / 'evergreen-endpoint-admin.svg').write_text(svg, encoding='utf-8')
    ADMIN.mkdir(parents=True, exist_ok=True)
    rendered = Image.open(BytesIO(resvg_py.svg_to_bytes(svg_string=svg, width=512, height=512))).convert('RGBA')
    rendered.save(ADMIN / 'admin.png', optimize=True)
    rendered.save(ADMIN / 'admin.ico', sizes=[(n, n) for n in (16, 20, 24, 32, 40, 48, 64, 128, 256)])


if __name__ == '__main__':
    main()
