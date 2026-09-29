# Module 03 plan — Anatomy of an AI-Native Codebase: The AI Layer

4 lessons · ~60 min instruction · ~4 h practice · depends on Module 2 (02.3 instruction hierarchy, 02.4 agent loop, 02.5 failure taxonomy).

Shared lab material: `labs/module-03/` — `brownfield/` (Contoso Billing, .NET 8 + SQL Server migrations, deliberately stale docs), `starter/CLAUDE.md` (wiki-copied rules with a stale convention), `solution/` (grounded AGENTS.md, CLAUDE.md import, CODEOWNERS, changelog, probes, portability files), `tools/AiLayerTool` (`scan` and `lint`).

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 03.1 The fourth citizen | Name the five engineering properties of the AI layer; set up ownership/review with CODEOWNERS; write a changelog entry tied to an incident | 02.3, 02.5 | Create `ai-layer-lab` on GitHub with starter rules, CODEOWNERS, branch protection, changelog | CODEOWNERS catch-all placed last → AI layer loses its owners (last match wins) | `ai-layer-lab` repo, CODEOWNERS, `AI-LAYER-CHANGELOG.md` |
| 03.2 Component map and portability | Classify components by load timing, enforcement and scope; pick the right component for a need; express one rule in ≥3 tools | 03.1, 02.4 | Portability matrix; migration rule as Claude path rule, Cursor rule, Copilot instructions; AGENTS.md canonical + `@AGENTS.md` import | CLAUDE.md and AGENTS.md both present with different rules → Claude and Copilot/Cursor diverge | `portability-matrix.md` + three rule files |
| 03.3 The brownfield audit | Run a five-pass audit; rank evidence by strength; filter findings with the "can the agent infer it?" test | 03.2 | `AiLayerTool scan` on Contoso, then on own repo; fill audit template | Audit from docs only → stale findings | `audit/brownfield-audit.md` |
| 03.4 Writing a grounded rules file | Write a <300-line rules file where every line cites evidence; lint it; verify with probes and convention tests | 03.3, 02.3 | Rewrite starter rules; `AiLayerTool lint`; probes P1–P4 ×3 | Stale SqlHelper rule → agent reproduces abandoned pattern on BILL-142 | grounded `AGENTS.md`, probe log, changelog v1.1.0 |

Breaks and diagnosis follow §6 (stale/conflicting context seeds M4). Math: none required by §10; lessons use only light arithmetic (sessions × lines, token cost preview) and point forward to M4 for budgets.

Templates created: `templates/brownfield-audit-checklist.md`, `templates/ai-layer-architecture.md`.
No simulation in this module.
