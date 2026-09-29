# security-lab

A local Docker Compose lab with intentionally vulnerable components — never pointed at real systems.

- **Built in:** M9
- **Visibility:** Public (safe by design)

## Expected contents

- `docker-compose.yml` — ticket service with an injected ticket, docs service with a poisoned page, malicious MCP server, canary secrets, egress catcher
- `threat-model.md`
- `attacks/` — ≥5 executed attacks with transcripts
- `mitigations.md` — the diff and retest results
- `residual-risk.md` — the register

## README sections to fill in

Every portfolio README answers the same five questions:

1. **Purpose** — what problem this repo exists to solve.
2. **What was built** — the components, with links.
3. **Why it exists** — the incident, experiment or client need that justified it.
4. **How it was evaluated** — the evidence: tasks, trials, intervals, reviewers.
5. **Lessons learned** — what failed, what you changed, what you would do differently.

> [!IMPORTANT]
> Never commit employer or client code, secrets, or personal data to a public repository. Anonymize before publishing.
