# One rule, three tools

The migration convention from `../AGENTS.md`, expressed as a path-scoped rule in three agent tools.
All three load only when the agent works on files under `db/migrations/`.

| Tool | File | Scoping key |
|---|---|---|
| Claude Code | `.claude/rules/migrations.md` | `paths:` list in front-matter |
| Cursor | `.cursor/rules/migrations.mdc` | `globs:` with `alwaysApply: false` |
| GitHub Copilot | `.github/instructions/migrations.instructions.md` | `applyTo:` glob |

The repo-wide rules stay in one `AGENTS.md` that Cursor and Copilot read natively and that
`CLAUDE.md` imports with `@AGENTS.md`. Formats as of 2026-09; check each tool's docs before copying.

Three copies of the same three bullets are a drift risk. Keep them identical by generating
them from one source, or by running the lint in `../../tools/AiLayerTool` in CI.
