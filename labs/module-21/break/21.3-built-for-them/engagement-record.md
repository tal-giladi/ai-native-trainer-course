# Engagement record — C-01 (break 21.3: built for them)

> **Deliberately flawed.** The AI layer from `brownfield-demo` (`layer.tsv` in Module 15: seven commits by one author in one week) delivered to a client the same way: fast, complete, and written by the consultant. Run `EngageCheck build` on it with `--log prs.csv`.

- Client code: C-01
- Consultant: Student (S)

## 3. Architecture and build

### Decisions

| Decision | Decided by | Record |
|---|---|---|
| AGENTS.md canonical, CLAUDE.md imports it | S | |
| Schema MCP server in snapshot mode | S | ADR 0008 |
| Eval gate blocks merges to the AI layer | S | |
| Governance: AI-layer changes need two approvals | S | governance.md |
