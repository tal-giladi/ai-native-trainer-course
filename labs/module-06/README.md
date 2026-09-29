# Module 6 labs — Skills, Sub-Agents and Workflow Automation

Everything the Module 6 labs need. Lessons: [06.1](../../lessons/module-06/lesson-01.md) · [06.2](../../lessons/module-06/lesson-02.md) · [06.3](../../lessons/module-06/lesson-03.md) · [06.4](../../lessons/module-06/lesson-04.md).

The labs run on **Contoso Billing** from [`labs/module-03/brownfield`](../module-03/README.md), in your `ai-layer-lab` repository, with the Module 4 layer (`AGENTS.md` + `CLAUDE.md` importing it) and the Module 5 loop material (tickets BILL-150–152, `tools/LoopGate`, `gates/architecture.rules`, the research-brief and plan templates) already in place. See [`labs/module-05`](../module-05/README.md).

> [!WARNING]
> Skills and sub-agents are executable configuration: a skill can pre-approve tools (`allowed-tools`) and run shell commands when it loads. Read every skill before you install it, including these. Run the breaks on throwaway branches, and keep transcripts of runs on your employer's code in a **private** repository.

## Requirements

- .NET SDK 8 or newer (the tool sets `RollForward=Major`). Verified with SDK 10.0.400.
- Git and `bash` (Git Bash on Windows) for the `new-migration` script and `run-triggers.sh`; or PowerShell 7 for `run-triggers.ps1`.
- Claude Code for the lab as written (skills in `.claude/skills/`, agents in `.claude/agents/`). The `SKILL.md` files follow the open Agent Skills format; Cursor and GitHub Copilot also read `.claude/skills/` (as of 2026-09), but `${CLAUDE_SKILL_DIR}`, `context: fork` and the `.claude/agents/` format are Claude Code specific.
- Trigger tests cost real tokens: 10 queries × 3 runs = 30 short headless sessions per skill.

## Contents

| Path | What it is |
|---|---|
| `tickets/` | BILL-153 (NOT NULL column on an existing table: the edge case), BILL-154 (statement: reuse), BILL-155 (void: first status write). |
| `prd/PRD-collections-q4.md` | A small PRD for the `spec` skill: two requirements already ticketed, one vague, one "later", one non-goal. |
| `layer/.claude/skills/` | Six skills: `new-migration` (06.1) and the five core skills `prime`, `spec`, `plan-feature`, `validate`, `pr-review` (06.2), each with `CHANGELOG.md` and, where it writes a file, `contract.rules`. |
| `layer/.claude/agents/` | `researcher` and `reviewer`, both read-only (06.3). |
| `tools/SkillCheck/` | Dependency-free C# tool: `lint`, `inventory`, `contract`, `triggers`. Read-only. |
| `scripts/run-triggers.{sh,ps1}` | Runs a trigger test: each query K times in fresh headless sessions (plan mode), records whether the skill loaded. `--setting-sources project,local` keeps your `~/.claude/settings.json` and personal skills out, but not your personal `~/.claude/CLAUDE.md` (checked 2026-09-29), so the script also passes `--settings` with a small temporary file that sets `claudeMdExcludes` to that file and `~/.claude/rules/**` and turns auto memory off: personal instructions change results, and runs must be reproducible on anyone's machine. |
| `tests/` | `triggers/*.tsv` (should-load and should-not-load queries), `tickets.md` (the ticket test set), `golden/*.rules` (facts a correct brief or plan must contain). Copy to `skill-tests/` in `ai-layer-lab`. |
| `samples/` | Illustrative trigger results (format demo, not measurements). |
| `solution/` | Reference `research/` and `plans/` for BILL-153 and BILL-154. |
| `break/06.1-vague-skill/` | `db-helper`: a vague skill with numbering as a model step; overlay with the V005 it produced (no U005). |
| `break/06.2-chat-handoff/` | `prime` 0.9.0 and `plan-feature` 0.9.0 that hand off through the chat; the plan produced after `/clear`. |
| `break/06.3-planner-agent/` | A `planner` sub-agent told to "resolve every open question", and the plan it wrote for BILL-153. |
| `break/06.4-migration-edge/` | `prime` and `plan-feature` 1.0.0 (the module lab break) and their BILL-153 brief and plan. |

## Setup (once)

In the root of `ai-layer-lab` (paths assume this folder is available as `<course>/labs/module-06`):

```bash
cp <course>/labs/module-06/tickets/*.md tickets/
mkdir -p prd skill-tests && cp <course>/labs/module-06/prd/*.md prd/
cp -r <course>/labs/module-06/tests/. skill-tests/
cp -r <course>/labs/module-06/tools/SkillCheck tools/
cp <course>/templates/skill-design.md <course>/templates/sub-agent-design.md templates/
dotnet build tools/SkillCheck
git add -A && git commit -m "Module 6: tickets, PRD, SkillCheck, skill tests"
```

Install the skills lesson by lesson (the lessons say when), so each break happens against the version it is about:

```bash
mkdir -p .claude/skills .claude/agents
cp -r <course>/labs/module-06/layer/.claude/skills/new-migration .claude/skills/     # 06.1
```

Every AI-layer change gets a line in `AI-LAYER-CHANGELOG.md` and owner review (Module 3).

## Verified results

Run from `labs/module-06` (or the equivalent paths in `ai-layer-lab`) with `dotnet run --project tools/SkillCheck -- …`:

| Command | Result |
|---|---|
| `lint layer/.claude` | PASS: 6 skills, 2 agents |
| `inventory layer` | 6 skills, 2 agents; descriptions always in context 1,648 chars (~412 tokens); 2 overlaps, both a skill and the agent it delegates to |
| `lint break/06.1-vague-skill/db-helper` | FAIL: 6 problems (no "when", no Inputs / Output contract / Steps, no version, no changelog) |
| `lint break/06.2-chat-handoff`, `lint break/06.3-planner-agent/planner.md`, `lint break/06.4-migration-edge/skills-v1.0.0` | PASS (the breaks are semantic, not syntactic) |
| `LoopGate arch` on Contoso + `break/06.1-vague-skill/overlay` | FAIL: `V005__invoice_po_number.sql has no undo script` |
| `new-migration.sh invoice_po_number BILL-153` on clean Contoso | creates V005 + U005; `LoopGate arch` PASS; second run refuses the duplicate name |
| `triggers samples/triggers-db-helper-v0.tsv` | recall 0.20, false-trigger rate 0.13, precision 0.60 → FAIL |
| `triggers samples/triggers-new-migration-v1.tsv` | recall 0.93, false-trigger rate 0.07, precision 0.93 → PASS |
| `contract break/06.4-…/research-BILL-153.md` with prime 1.0.0 rules / 1.1.0 rules | PASS / FAIL (no `Schema change:` line) |
| `contract break/06.4-…/plan-BILL-153.md` with plan-feature 1.0.0 rules / 1.1.0 rules | PASS / FAIL (3: no U005 in Touch, NOT NULL without DEFAULT or backfill, merged migrations not protected) |
| `LoopGate plan-lint` on that broken plan | PASS (the plan is well-formed; it is wrong) |
| `contract solution/research/BILL-153.md` and `solution/plans/BILL-153.md` with 1.1.0 rules and `tests/golden/BILL-153.*.rules` | PASS |
| `contract break/06.3-planner-agent/plan-BILL-153.md` with plan-feature 1.1.0 rules | FAIL: the backfill value is not at a HUMAN checkpoint |
| `contract break/06.2-chat-handoff/plan-BILL-154.md` with `tests/golden/BILL-154.plan.rules` | FAIL: 4 (no brief cited, `OutstandingAsync` not reused, `Status != InvoiceStatus.Paid`, `Statements/` folder) |
| `contract solution/plans/BILL-154.md` with 1.1.0 rules and golden rules | PASS |

## Lab sequence

1. **06.1 — Skill anatomy.** Build `new-migration`: script for numbering, model for SQL, `LoopGate arch` as the check. `SkillCheck lint`, trigger test. *Break:* `db-helper` fires on 3 of 15 should-load runs and writes V005 without U005.
2. **06.2 — The five core skills.** Install `prime`, `spec`, `plan-feature`, `validate`, `pr-review` and both agents; run BILL-154 end to end; spec the PRD. *Break:* the brief lives only in the chat; after `/clear` the plan duplicates "issued".
3. **06.3 — Sub-agents.** Parallel research on BILL-154 and BILL-155; reviewer on a diff; measure context saved. *Break:* a `planner` sub-agent cannot ask, so it decides the BILL-153 backfill value on its own.
4. **06.4 — Testing, versioning, failure analysis.** Ticket test set, golden checks, trigger runs, version bump, colleague cold-use test. *Break (module lab):* `prime`/`plan-feature` 1.0.0 pass five tickets and fail BILL-153; diagnose trigger vs context vs contract.

## Artifacts to commit to `ai-layer-lab`

- `.claude/skills/` (6 skills) and `.claude/agents/` (2 agents), each skill with `CHANGELOG.md`
- `skill-tests/` with your own trigger results and a ticket test log
- `templates/skill-design.md` filled in for one skill, `templates/sub-agent-design.md` for one agent
- `NOTES.md`: skill test log, failure analysis for BILL-153, cold-use test notes
- `AI-LAYER-CHANGELOG.md` entries for every skill and agent added or changed

See the portfolio scaffold: [ai-layer-lab](../../projects/ai-layer-lab/README.md).
