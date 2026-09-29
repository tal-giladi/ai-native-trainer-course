# Module 15 labs — The demo repo and live demo craft

Everything the Module 15 labs need. Lessons: [15.1](../../lessons/module-15/lesson-01.md) · [15.2](../../lessons/module-15/lesson-02.md) · [15.3](../../lessons/module-15/lesson-03.md) · [15.4](../../lessons/module-15/lesson-04.md).

The labs turn the work of Modules 3–11 into **`brownfield-demo`**: a public repository you can show anyone. It is Contoso Billing from [Module 3](../module-03/README.md) with a real, dated git history, the company around it (tickets, PRDs, handbook), a tagged "before" with an empty AI layer, and the layer you built — grounded rules ([Module 4](../module-04/README.md)), loop gates ([Module 5](../module-05/README.md)), skills and sub-agents ([Module 6](../module-06/README.md)), evals ([Module 7](../module-07/README.md)), the schema MCP server and hooks ([Module 8](../module-08/README.md)), hardened permissions ([Module 9](../module-09/README.md)) and CI ([Module 11](../module-11/README.md)) — committed on top as its own history. Nothing is copied into this folder: `scripts/assemble-demo.sh` pulls each piece from the module that built it.

> [!WARNING]
> `brownfield-demo` is meant to be **public**. Before you push it anywhere, run `DemoCheck credibility` with a deny list of your employer's and clients' names that lives **outside** the repository, and read every finding. A secret or a name in any commit is published even if a later commit deletes it; if one gets out, revoke the secret first and then rewrite history (lesson 15.1). Never demo on employer code, and never type a real key or token on screen.

## Requirements

- Git 2.30 or newer and `bash` (Git Bash on Windows) for `assemble-demo.sh`.
- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400 and Git 2.52.
- `DemoCheck` has no NuGet packages and works offline. The demo repository's own tools restore the packages pinned in Modules 6–11 when you build them.
- For the live parts: Claude Code (or your agent), a screen recorder that writes one file per take (OBS Studio or your OS recorder), and one developer who has never seen your repository.

## Contents

| Path | What it is |
|---|---|
| `scripts/assemble-demo.sh` | Builds `brownfield-demo`: replays `history.tsv` (2016–2026, five authors), tags `before-ai-layer`, replays `layer.tsv` (seven AI-layer commits), tags `ai-layer-v1`, and creates the pre-baked branch `demo/BILL-97-done`. `--break 15.1` / `--break 15.2` build the broken variants. |
| `history.tsv`, `layer.tsv` | The two manifests: date, author, message and file operations per commit, sourced from Modules 3–11 and this folder. |
| `history/` | Early versions of seven files (2016, 2024, 2026-06), so the history shows the code changing instead of arriving finished. |
| `company/` | New company material: demo ticket BILL-97, stranger-sized BILL-180, PRD *Finance close Q4*, handbook, team, glossary, a stale onboarding page. The other tickets and the Collections PRD come from Modules 3, 5, 6 and 8. |
| `demo-kit/` | `demo.json` (what `DemoCheck before` checks), the demo's `.mcp.json` (schema server only, snapshot mode), `.claude/settings.json` (Module 9 permissions + Module 8 hooks), and the recordings README. |
| `solution/BILL-97/` | The finished ticket (Dapper repository, report moved out of `Legacy/`, three tests, brief and plan) — the pre-baked fallback branch. |
| `solution/runsheet.md` | A reference run sheet for a 50-minute demo block. |
| `tools/DemoCheck/` | Dependency-free C# checker: `credibility`, `before`, `timeline`, `runsheet`, `drill`. Read-only. |
| `samples/` | **Illustrative** ticket-selection runs, an annotated unedited run, a passing stranger test, an example deny list. See `samples/README.md`. |
| `break/` | 15.1 a leaked employer config and notes (deleted at HEAD, alive in history); 15.2 a rehearsal committed before the "before" tag and a stale doc "fixed" in the layer commit; 15.3 an edited run and a helped stranger; 15.4 a run sheet without fallbacks. |

## Quick start

From this folder:

```bash
bash scripts/assemble-demo.sh --out ../../../brownfield-demo       # a sibling of the course folder
D="dotnet run --project tools/DemoCheck --"

# 15.1 — credible and safe to publish
$D credibility ../../../brownfield-demo --deny samples/deny-terms.example.txt   # 0 errors, 0 warnings

# 15.2 — a fair before/after
$D before ../../../brownfield-demo                                              # 0 errors

# 15.3 — an unedited run and a stranger test
$D timeline samples/run-01-timeline.tsv --file-duration 17:40                   # unedited run
$D timeline samples/stranger-02.tsv --mode stranger                             # passed at 24:40

# 15.4 — run sheet and drill
$D runsheet solution/runsheet.md                                                # ready, P(clean) 0.65
$D drill solution/runsheet.md --seed 11                                         # api-down during #5
```

(PowerShell: run `assemble-demo.sh` from Git Bash, and type the full `dotnet run --project tools/DemoCheck -- <command>` instead of `$D`.) Every command exits with 1 when its check fails, so `credibility` and `before` can run in the demo repository's CI.

Then, inside `brownfield-demo`:

```bash
dotnet test Contoso.Billing.sln                                                  # 6 passed at both tags
dotnet build tools/AgentHooks -c Release -o .claude/hooks/bin
dotnet publish tools/ContosoSchema.Mcp -c Release -o .claude/mcp/contoso-schema
git worktree add ../demo-before before-ai-layer
git worktree add ../demo-after ai-layer-v1
```

The schema snapshot in `.claude/mcp/` was captured on 2026-09-28, so by demo day it is older than the server's 24-hour limit: the demo's `.mcp.json` sets `--on-stale warn` and the run sheet has a line for refreshing it. That is a real finding from building this lab, and lesson 15.4 uses it.

## Lab order

1. **15.1 — Designing a credible brownfield demo.** Pick the demo ticket from measured runs; assemble the repository; `credibility` with your own deny list. *Break:* `--break 15.1` — config and notes from a previous employer, deleted at HEAD.
2. **15.2 — Company scaffolding and the before state.** Tickets into GitHub Issues (Module 8's `import-tickets.sh`); run BILL-97 in both worktrees; `before`. *Break:* `--break 15.2` — a rehearsal on `main` and a "helpful" doc fix in the layer commit.
3. **15.3 — The unedited recording and the stranger test.** Record one take, annotate the timeline, watch it back; run the stranger test; turn stuck points into layer changes. *Break:* `break/15.3-edited-run/`.
4. **15.4 — When the demo breaks.** Run sheet, fallbacks, reset command, drill a failure on camera and recover. *Break:* `break/15.4-no-fallback/runsheet.md`.

## Verified results

| Command | Result |
|---|---|
| `assemble-demo.sh --out <dir>` | 26 commits on `main` (2016-03-02 to 2026-09-28, 6 authors), tags `before-ai-layer` and `ai-layer-v1`, branch `demo/BILL-97-done` |
| `dotnet test` at `before-ai-layer`, `ai-layer-v1` / on `demo/BILL-97-done` | 6 passed / 9 passed |
| `credibility <demo> --deny samples/deny-terms.example.txt` | 0 errors, 0 warnings (without `--deny`: 1 warning) |
| `credibility <demo built with --break 15.1> --deny …` | 4 errors: 1 secret, 5 e-mail, 2 hosts/IPs, 10 deny-term findings |
| `before <demo>` / `before <demo built with --break 15.2>` | 0 errors / 4 errors (layer-only, ticket open, still open ×2) |
| `timeline samples/run-01-timeline.tsv --file-duration 17:40` | unedited run, 0 warnings |
| `timeline break/15.3-edited-run/run-01-timeline.tsv --file-duration 14:05` | 2 errors (edited, unrecovered error), 3 warnings (13:10, 7 silent gaps, 2 off-script prompts) |
| `timeline break/15.3-edited-run/stranger-01.tsv --mode stranger` / `samples/stranger-02.tsv` | failed (helped twice) / passed at 24:40 |
| `runsheet solution/runsheet.md` | ready; P(clean) 0.65; with `--repo <demo>`: 1 error until you record the two clips |
| `runsheet break/15.4-no-fallback/runsheet.md` | 3 errors, 2 warnings; P(clean) 0.27 |

## Artifacts for `brownfield-demo`

- The assembled repository, pushed public only after `credibility` passes with your private deny list
- `README.md` answering the five portfolio questions ([scaffold](../../projects/brownfield-demo/README.md))
- `demo/demo.json`, `demo/runsheet.md` ([run sheet template](../../templates/demo-runbook.md))
- `recordings/run-01-unedited.*` (or a link to it), `recordings/run-01-timeline.tsv`, `recordings/before-BILL-97.*`
- `recordings/stranger-test-notes.md` ([stranger test template](../../templates/stranger-test.md)) and the AI-layer changelog entries it caused
