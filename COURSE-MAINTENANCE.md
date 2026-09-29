# Course maintenance

The agent-tool and model landscape changes quarterly; the concepts do not. Every lesson's front-matter carries `volatility: concept | implementation` and `last_verified`.

## Cadence

| When | What |
|---|---|
| Quarterly | Implementation lessons (agent config formats, MCP spec changes, CI actions, model lineups, pricing), lab setup scripts, the security attack/defense state, simulation presets that quote real numbers |
| Annual | Productivity evidence base (Module 13), enterprise hosting and compliance landscape (Module 12), glossary, capstone rubric |
| When sources change | Stable concepts: tokens, sampling, context layering, eval statistics, experimental design, instructional design, change management |

## Procedure for a lesson review

1. Open the lesson and its row(s) in [the research log](references/research-log.md).
2. Re-verify every source URL and claim; replace dead or superseded sources with primary ones.
3. Re-run the module's lab from `labs/module-NN/` on pinned versions; update pins and smoke tests.
4. Update `last_verified` in the front-matter.
5. Run the pre-publish checker (`py scripts/check.py`), then record the change below.

## Rules that never change

- Never rename or renumber a published lesson, quiz or module path — progress in Tal's Academy hangs on them. Add lessons at the end of a module.
- Quizzes live only in `*.quiz.yaml`; instructor notes only in `*.instructor.md`.

## Changelog

- 2026-09-28 — revision 2 built: 22 modules, 101 lessons.
