# ai-layer-lab

The AI layer for a real brownfield .NET/SQL Server repository: rules, skills, sub-agents, MCP config, hooks, CI agent workflow, governance — plus `NOTES.md`, the dated log of every agent incident and experiment that later feeds your method.

- **Built in:** M3–M6, M8, M11
- **Visibility:** Private

## Expected contents

- `audit/` — brownfield audit report (Module 3)
- `rules/` or `AGENTS.md` / `CLAUDE.md` — grounded rules file and portability matrix (Module 3)
- `context/` — context audit and reduced layer (Module 4)
- `loop/` — research/plan/implement/validate templates (Module 5)
- `.claude/skills/`, `.claude/agents/` — five core skills and ≥1 sub-agent with test tickets and `CHANGELOG.md` (Module 6)
- `mcp/`, `hooks/` — integration config, custom server, enforcement and audit hooks (Module 8)
- `.github/workflows/agent-review.yml`, `governance.md` (Module 11)
- `fundamentals/` — agent loop, tokenizer notebook, failure taxonomy, model-selection matrix (Module 2)
- `NOTES.md` — every entry dated

## README sections to fill in

Every portfolio README answers the same five questions:

1. **Purpose** — what problem this repo exists to solve.
2. **What was built** — the components, with links.
3. **Why it exists** — the incident, experiment or client need that justified it.
4. **How it was evaluated** — the evidence: tasks, trials, intervals, reviewers.
5. **Lessons learned** — what failed, what you changed, what you would do differently.

> [!IMPORTANT]
> Never commit employer or client code, secrets, or personal data to a public repository. Anonymize before publishing.
