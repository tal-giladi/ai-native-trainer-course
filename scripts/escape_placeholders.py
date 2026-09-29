"""Escape <placeholder> text outside code so the importer does not strip it as HTML.
Usage: py scripts/escape_placeholders.py FILE..."""
import re, sys
from pathlib import Path

ALLOWED = {"details", "summary", "kbd", "sub", "sup", "br"}
BS = chr(92)
TAG = re.compile(r"(?<!\\)<(/?)([a-zA-Z][a-zA-Z0-9-]*)([^>]*)>")


def fix(m):
    if m.group(2).lower() in ALLOWED:
        return m.group(0)
    return BS + "<" + m.group(1) + m.group(2) + m.group(3) + ">"


for f in sys.argv[1:]:
    p = Path(f)
    t = p.read_text(encoding="utf-8")
    parts = re.split(r"(```.*?```|~~~.*?~~~|`[^`\n]*`)", t, flags=re.S)
    for i in range(0, len(parts), 2):
        parts[i] = TAG.sub(fix, parts[i])
    p.write_text("".join(parts), encoding="utf-8", newline="\n")
    print("fixed", f)
