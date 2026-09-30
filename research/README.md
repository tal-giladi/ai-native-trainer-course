# Curriculum modernization system

The course is a **curriculum, not a news feed**. The agent-tool and model landscape changes every
quarter; the concepts do not ([COURSE-MAINTENANCE.md](../COURSE-MAINTENANCE.md)). This folder is how
the course stays current without churning: many discoveries go in, very few serious candidates come
out, and the course changes at most once a week. **A week with zero course changes is a successful
week.**

Unlike the purely additive research courses, this course has many `volatility: implementation`
lessons (agent config formats, MCP, CI actions, model lineups, pricing). So the weekly review has a
second job besides adding topics: **FIX** facts in existing lessons that went stale, keeping every
path, id and quiz id stable.

```text
DAILY  (research only — never touches the course)
  web research ─▶ classify A/B/C/D/E ─▶ daily/YYYY-MM-DD.md (all items)
                                     └▶ staging.md (A and B only, full candidate record)
     a candidate is either a new-topic or a correction (an existing lesson's fact went stale)

WEEKLY (the only process allowed to change the course)
  staging.md + registry + `research.py stale` ─▶ decisions
     ADD    ─▶ new lesson at the end of its module (+ quiz, sidebar) ─▶ CHANGELOG.md
     FIX    ─▶ corrected sentences in the existing lesson, last_verified, research-log row
     WAIT   ─▶ deferred/<topic>.md   (re-reviewed when new evidence appears)
     REJECT ─▶ rejected/<topic>.md   (not reconsidered without materially new evidence)
  ─▶ weekly/YYYY-Www.md report ─▶ staging.md reset
```

## Layout

| Path | What it holds | Written by |
|---|---|---|
| `PROTOCOL-daily.md` | Instructions for the daily research run | human |
| `PROTOCOL-weekly.md` | Instructions for the weekly curriculum review | human |
| `templates/` | Candidate, topic, daily, staging and weekly-report templates | human |
| `daily/YYYY-MM-DD.md` | Everything found that day, classified A–E | daily run |
| `staging.md` | This week's A/B candidates in full | daily run; reset weekly |
| `weekly/YYYY-Www.md` | Weekly review report + archived staging content | weekly run |
| `accepted/` `deferred/` `rejected/` | One file per decided topic, with its history | weekly run |
| `REGISTRY.md` | Generated index of every topic and its status | `tools/research.py index` |
| `CHANGELOG.md` | Learner-facing log of curriculum changes | weekly run |
| `tools/research.py` | Stdlib helper: scaffold, validate, index, dedup lookup, scope guard, stale list | human |

## Tool

```bash
python research/tools/research.py new-day            # create today's daily file if missing
python research/tools/research.py lookup "MCP"       # has the course or registry seen this?
python research/tools/research.py stale 5            # oldest-verified implementation lessons
python research/tools/research.py check              # validate candidates + topic files
python research/tools/research.py index              # regenerate REGISTRY.md
python research/tools/research.py scope --daily      # fail if staged changes leave research/
```

`research/` is never imported into Tal's Academy (it is not in `_sidebar.md`).

## Automation

Two cloud routines run this for several courses at once ("Courses — daily research" and "Courses —
weekly curriculum review"). Their instructions live in the tals-academy repo,
`docs/course-upkeep/`. Both follow the protocols in this folder; to change this course's
behaviour, edit the protocol here, not the routine.
