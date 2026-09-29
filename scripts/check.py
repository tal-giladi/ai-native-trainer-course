"""Pre-publish checker for the Tal's Academy import contract (outline §19.9).
Usage: py scripts/check.py            # whole course
       py scripts/check.py 7          # only module 7 (lessons, quizzes, module quiz, links)
Exit code 1 if any error."""
import re, sys
from pathlib import Path
import yaml
sys.path.insert(0, str(Path(__file__).parent))
from manifest import lessons, MODULES

ROOT = Path(__file__).resolve().parent.parent
SECTIONS = ["Why it matters", "How it works", "Show me", "Try it", "Break it", "Fix it",
            "How do I know it works?", "Use / don't use", "Reflect", "Sources"]
FM_KEYS = ["id", "module", "minutes", "practice_minutes", "prerequisites", "objectives",
           "volatility", "sources", "last_verified"]
ALLOWED_TAGS = {"details", "summary", "kbd", "sub", "sup", "br"}
errors, warnings = [], []


def err(p, msg): errors.append(f"{p}: {msg}")
def warn(p, msg): warnings.append(f"{p}: {msg}")


def strip_code(text):
    return re.sub(r"```.*?```|~~~.*?~~~|`[^`\n]*`", "", text, flags=re.S)


def check_html_and_links(path, text, html=True):
    body = strip_code(text)
    for tag in (re.findall(r"(?<!\\)</?([a-zA-Z][a-zA-Z0-9-]*)[^>]*>", body) if html else []):
        if tag.lower() not in ALLOWED_TAGS:
            err(path, f"disallowed HTML <{tag}>")
    for m in re.finditer(r"!?\[[^\]]*\]\(([^)\s]+)(?:\s+\"[^\"]*\")?\)", body):
        url = m.group(1)
        if url.startswith(("http://", "https://", "mailto:", "#")):
            if "github.io" in url:
                err(path, f"absolute link to course pages: {url}")
            continue
        if url == "/":
            continue
        target = (path.parent / url.split("#")[0].split("?")[0]).resolve()
        if not target.exists():
            err(path, f"broken link: {url}")
    if re.search(r"^!\[\]\(", body, re.M):
        err(path, "image without alt text")


def check_quiz(path, lo, hi):
    if not path.exists():
        err(path, "missing quiz file"); return
    try:
        data = yaml.safe_load(path.read_text(encoding="utf-8"))
    except Exception as e:
        err(path, f"YAML parse error: {e}"); return
    if not isinstance(data, list) or not lo <= len(data) <= hi:
        err(path, f"needs {lo}-{hi} questions, has {len(data) if isinstance(data, list) else '??'}"); return
    ids, positions = set(), []
    for q in data:
        qid = q.get("id")
        if qid in ids: err(path, f"duplicate id {qid}")
        ids.add(qid)
        for k in ("id", "question", "options", "correct", "explanation"):
            if k not in q: err(path, f"{qid}: missing {k}")
        opts = q.get("options") or []
        if len(opts) != 4 or len(set(map(str, opts))) != 4:
            err(path, f"{qid}: needs 4 distinct options"); continue
        c = q.get("correct")
        if not isinstance(c, int) or not 0 <= c <= 3:
            err(path, f"{qid}: correct must be 0-3"); continue
        positions.append(c)
        low = " ".join(map(str, opts)).lower()
        if "all of the above" in low or "none of the above" in low:
            err(path, f"{qid}: all/none of the above")
        lens = [len(str(o)) for o in opts]
        others = [x for i, x in enumerate(lens) if i != c]
        if lens[c] > 1.35 * max(others) and lens[c] - max(others) > 25:
            warn(path, f"{qid}: correct option noticeably longest ({lens[c]} vs {max(others)})")
    if len(positions) >= 4 and len(set(positions)) < 3:
        warn(path, f"correct positions poorly spread: {positions}")


def check_lesson(m, l, g, lid, title, rel):
    p = ROOT / rel
    if not p.exists():
        err(rel, "missing lesson"); return
    text = p.read_text(encoding="utf-8")
    if "\r\n" in text: err(rel, "CRLF line endings")
    mm = re.match(r"---\n(.*?)\n---\n", text, re.S)
    if not mm:
        err(rel, "no front-matter"); return
    try:
        fm = yaml.safe_load(mm.group(1))
    except Exception as e:
        err(rel, f"front-matter YAML: {e}"); return
    for k in FM_KEYS:
        if k not in fm: err(rel, f"front-matter missing {k}")
    if str(fm.get("id")) != lid: err(rel, f"id {fm.get('id')!r} != {lid}")
    if fm.get("module") != m: err(rel, "module mismatch")
    if fm.get("volatility") not in ("concept", "implementation"): err(rel, "bad volatility")
    for pr in fm.get("prerequisites") or []:
        if not re.fullmatch(r"\d\d\.\d+", str(pr)): err(rel, f"bad prerequisite {pr!r}")
    for s in fm.get("sources") or []:
        if not isinstance(s, dict) or "title" not in s or "url" not in s: err(rel, f"bad source {s!r}")
    body = text[mm.end():]
    lines = body.lstrip("\n").split("\n")
    h1s = re.findall(r"^# (.+)$", strip_code(body), re.M)
    if len(h1s) != 1: err(rel, f"{len(h1s)} H1s")
    if not lines[0].startswith("# "): err(rel, "H1 is not first line after front-matter")
    elif lines[0] != f"# {lid} · {title}": err(rel, f"H1 {lines[0]!r} != '# {lid} · {title}'")
    h2s = re.findall(r"^## (.+)$", strip_code(body), re.M)
    if h2s != SECTIONS: err(rel, f"sections {h2s} != required order")
    check_html_and_links(p, text)
    check_quiz(p.with_suffix(".quiz.yaml"), 3, 5)
    if not p.with_suffix(".instructor.md").exists(): err(rel, "missing instructor notes")
    words = len(re.findall(r"\w+", body))
    if words < 900: warn(rel, f"only {words} words")


def main():
    only = int(sys.argv[1]) if len(sys.argv) > 1 else None
    for row in lessons():
        if only is None or row[0] == only:
            check_lesson(*row)
    for m in range(1, len(MODULES) + 1):
        if only is not None and m != only: continue
        q = ROOT / f"assessments/module-{m:02d}-quiz.md"
        if not q.exists(): err(q.relative_to(ROOT), "missing module quiz page")
        else: check_html_and_links(q, q.read_text(encoding="utf-8"))
        check_quiz(ROOT / f"assessments/module-{m:02d}-quiz.quiz.yaml", 8, 10)
    if only is None:
        sb = (ROOT / "_sidebar.md").read_text(encoding="utf-8")
        if "instructor" in sb: err("_sidebar.md", "links instructor notes")
        check_html_and_links(ROOT / "_sidebar.md", sb)
        for f in ["README.md", "glossary.md", "COURSE-MAP.md", "PROGRESS.md", "COURSE-MAINTENANCE.md"]:
            p = ROOT / f
            if not p.exists(): err(f, "missing")
            else: check_html_and_links(p, p.read_text(encoding="utf-8"))
        for p in list(ROOT.glob("templates/*.md")) + list(ROOT.glob("labs/**/*.md")) + list(ROOT.glob("projects/**/*.md")) + list(ROOT.glob("assessments/*.md")) + list(ROOT.glob("references/*.md")) + list(ROOT.glob("simulations/*.md")):
            check_html_and_links(p, p.read_text(encoding="utf-8"), html="labs" not in p.parts)
    for w in warnings: print("WARN ", w)
    for e in errors: print("ERROR", e)
    print(f"{len(errors)} errors, {len(warnings)} warnings")
    sys.exit(1 if errors else 0)


if __name__ == "__main__":
    main()
