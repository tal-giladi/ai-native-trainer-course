"""Merge curriculum/glossary-inbox/*.md into glossary.md and curriculum/research-inbox/*.md
into references/research-log.md. Idempotent: rerun after any module changes its inbox."""
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

# ---- glossary ----
entries = {}
pat = re.compile(r"^- \*\*(.+?)\*\*\s*[—-]+\s*(.+?)\s*\(((?:\.\./)*lessons/module-\d\d/lesson-\d\d\.md)[^)]*\)\s*$")
for f in sorted((ROOT / "curriculum/glossary-inbox").glob("module-*.md")):
    for line in f.read_text(encoding="utf-8").splitlines():
        m = pat.match(line.strip())
        if not m:
            continue
        term, definition, link = m.group(1).strip(), m.group(2).strip(), m.group(3).lstrip("./")
        key = re.sub(r"[^a-z0-9]+", " ", term.lower()).strip()
        if key not in entries:  # earliest module wins: that's where the term is introduced
            entries[key] = (term, definition, link)

groups = {}
for key in sorted(entries):
    term, d, link = entries[key]
    letter = key[0].upper() if key[0].isalpha() else "#"
    groups.setdefault(letter, []).append(f"- **{term}** — {d} ([lesson {link[15:17].lstrip('0')}.{int(link[-5:-3])}]({link}))")

out = ["# Glossary", "",
       f"{len(entries)} terms. Short, precise definitions; each links to the lesson that introduces it.", "",
       " · ".join(f"[{g}](#{g.lower()})" for g in groups if g != "#"), ""]
for g, items in groups.items():
    out += [f"## {g}", ""] + items + [""]
(ROOT / "glossary.md").write_text("\n".join(out), encoding="utf-8", newline="\n")
print(f"glossary: {len(entries)} terms")

# ---- research log ----
rows, seen = [], set()
for f in sorted((ROOT / "curriculum/research-inbox").glob("module-*.md")):
    for line in f.read_text(encoding="utf-8").splitlines():
        if not line.startswith("|") or re.match(r"^\|\s*-", line) or re.match(r"^\|\s*source\s*\|", line, re.I):
            continue
        cells = [c.strip() for c in line.strip().strip("|").split("|")]
        if len(cells) < 7:
            continue
        key = (cells[1], cells[3][:60])
        if key in seen:
            continue
        seen.add(key)
        rows.append("| " + " | ".join(cells[:7]) + " |")
log = ["# Research log", "",
       "Every factual claim in the course with its source. Re-verify per [COURSE-MAINTENANCE.md](../COURSE-MAINTENANCE.md) by volatility.", "",
       "| Source | URL | Accessed | Claim supported | Lesson(s) | Primary | Volatility |",
       "|---|---|---|---|---|---|---|"] + rows + [""]
(ROOT / "references/research-log.md").write_text("\n".join(log), encoding="utf-8", newline="\n")
print(f"research log: {len(rows)} rows")

import subprocess, sys
subprocess.run([sys.executable, str(ROOT / "scripts/escape_placeholders.py"), str(ROOT / "references/research-log.md"), str(ROOT / "glossary.md")], check=True)
