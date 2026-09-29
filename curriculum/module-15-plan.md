# Module 15 plan — The Demo Repo and Live Demo Craft

4 lessons · ~60 min instruction · ~8 h practice · depends on Modules 3–11 (the AI layer on Contoso Billing: rules, gates, skills, evals, MCP, hooks, hardened settings, CI), Module 7 (Wilson intervals, headless runs as trials), Module 13 (claims ladder: a demo is not evidence) and Module 14 (the method and its vocabulary the demo illustrates; employer IP in 14.4).

The student turns their `ai-layer-lab` work into `brownfield-demo`: a public, confidentiality-safe repository with a real dated history, a company around it, a tagged "before" with an empty AI layer and the layer committed on top, one demo ticket chosen on evidence, an unedited recording, a stranger test, and a run sheet with fallbacks. Module 17 builds the workshop around this demo; Module 18 publishes pieces cut from it.

Shared lab material: `labs/module-15/` — `scripts/assemble-demo.sh` (replays `history.tsv` + `layer.tsv`, sourcing every file from Modules 3–11 plus a few early versions in `history/`; `--break 15.1|15.2`), `company/` (demo ticket BILL-97, BILL-180, PRD *Finance close Q4*, handbook, team, glossary, stale onboarding page), `demo-kit/` (demo.json, demo `.mcp.json` in snapshot mode, composed `settings.json`), `solution/BILL-97/` (pre-baked fallback branch) and `solution/runsheet.md`, `tools/DemoCheck` (dependency-free C#: `credibility`, `before`, `timeline`, `runsheet`, `drill`), illustrative samples, four breaks.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 15.1 Designing a credible brownfield demo | Six properties of a credible demo; choose the demo ticket from ≥10 headless runs with Wilson intervals, before/after gap, duration and recognizability; assemble and pass `credibility` with a private deny list | 03.3, 07.4, 11.1, 14.4 | Ticket selection from `samples/ticket-selection.csv` then own runs; `assemble-demo.sh`; `credibility` | `--break 15.1`: employer config + notes, deleted at HEAD, alive in history; work e-mail as author | `brownfield-demo` history + `demo/demo.json` |
| 15.2 Company scaffolding and the before state | Company scaffold (PRDs, tickets, handbook, glossary, stale page) into a tracker; before/after as a controlled comparison (only the layer differs, ticket still open); run both worktrees and log the difference; what n=1 can show | 15.1, 13.6, 08.2 | Import tickets with `gh`; worktrees at both tags; BILL-97 once each side; `before` | `--break 15.2`: rehearsal committed before the tag; stale doc "fixed" in the layer commit | `before-ai-layer`/`ai-layer-v1` tags, company scaffold, before/after notes |
| 15.3 The unedited recording and the stranger test | Record one take 15–20 min; annotate a timeline; watch-back rubric (silent gaps, off-script prompts, jargon); stranger-test protocol (think aloud, zero help, 30 min); $1-(1-L)^n$ for how many strangers; stuck points → layer changes | 15.2, 06.4 | `timeline` on own run; stranger test; changelog entries | Edited run (paused, trimmed, 2 off-script prompts) and "hints" that were help | `recordings/run-01-unedited`, timeline, `stranger-test-notes.md` |
| 15.4 When the demo breaks | Failure classes of live agent demos; P(clean) as a product of per-segment rehearsal rates, Wilson lower bounds; live / recorded / pre-baked per segment; run sheet with fallbacks, say lines, reset; recover on camera: name → decide → teach; narrating failure as teaching | 15.3, 02.2, 07.4 | Run sheet from template; `runsheet --repo`; `drill` a failure during a recorded rehearsal; recover on camera | Run sheet with 3 live segments and no fallback, over budget, a 1/3 segment | `demo/runsheet.md`, rehearsal log, recovery clip |

Math (§10): none new. 15.1 and 15.4 reuse the Wilson interval (07.4); 15.4 reuses $p^k$ (02.2) as $\prod p_i$ across demo segments; 15.3 uses $1-(1-L)^n$ (Nielsen–Landauer) to size the stranger test.

Simulation: none for this module.

Templates created: `templates/demo-runbook.md` (run sheet + recovery pattern), `templates/stranger-test.md` (protocol, timeline format, notes).

Exit test (outline): a stranger gets a working PR from your skills in 30 minutes, unaided — protocol in the stranger-test template, checked with `DemoCheck timeline --mode stranger`.

Links to Module 14 (written in parallel) use manifest paths `lessons/module-14/lesson-0N.md`.
