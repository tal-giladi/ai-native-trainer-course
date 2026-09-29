# Fabrikam coding-agent platform — security review answers (v0 draft, as first submitted)

Answers to the 30 questions in [`fabrikam/security-review-questionnaire.json`](../fabrikam/security-review-questionnaire.json). Each answer names evidence a reviewer can open and check. Draft. Checked by `ArchCheck review`. Several answers were pasted from the target design and do not describe v0 as it is in [`architecture.json`](architecture.json); cross-check them with `ArchCheck lint`.

## Hosting & vendor

### Q01 Where does inference run, including fallback?
Answer: Primary: hyperscaler A, EU geographic inference profile. Fallback: hyperscaler B, EU multi-region endpoint. No other route exists; clients cannot reach providers directly.
Evidence: [ADR-0001](adr/0001-eu-hosted-cloud-models.md), [ADR-0003](adr/0003-central-model-gateway.md), [architecture.json](architecture.json) routes, `ArchCheck lint --rules RES,NET` output.

### Q02 Contract terms and training on our data?
Answer: Yes, the vendor does not train on our data.
Evidence: 

### Q03 How do we switch provider or model?
Answer: Provider and model are gateway configuration. A switch requires passing the Module 7 regression gate on our task set, then a config change; no developer machine changes.
Evidence: [ADR-0003](adr/0003-central-model-gateway.md), quota spreadsheet (finance) decision.

### Q04 Allowed model versions and pinning?
Answer: The platform team pins model ids per tier in managed settings and in gateway routes; a new version needs an eval report and a platform approval.
Evidence: [ADR-0001](adr/0001-eu-hosted-cloud-models.md) follow-up; managed settings file in the platform repository.

## Identity & access

### Q05 Developer authentication?
Answer: SSO with MFA through the corporate IdP; the gateway records the SSO user next to the team key.
Evidence: identity design (in progress), architecture whiteboard photo.

### Q06 Unattended agent identity and credential lifetime?
Answer: A CI secret with a service key.
Evidence: TBD

### Q07 Can one team spend another team's budget or reach its data?
Answer: No. Keys, quotas and budgets are per team; MCP reads use each user's own grants; writes use per-team bots.
Evidence: identity design (in progress), quota spreadsheet (finance), `ArchCheck lint --rules IDN,MCP`.

### Q08 Revocation on leave or team change?
Answer: Yes.
Evidence: 

### Q09 Who can change gateway, managed settings and MCP configuration?
Answer: The platform team only, through reviewed pull requests to the platform repository under CODEOWNERS; changes follow the AI-layer governance policy.
Evidence: [ADR-0003](adr/0003-central-model-gateway.md); [governance policy template](../../../templates/governance-policy.md).

## Secrets

### Q10 Where are credentials stored; can developers read the provider key?
Answer: Provider keys live only in the vault and the gateway. Developers receive a team gateway key at run time, never the provider key.
Evidence: identity design (in progress), architecture whiteboard photo.

### Q11 Rotation?
Answer: TBD
Evidence: 

### Q12 Secrets in repositories or pasted text?
Answer: Agent sessions deny reads of `.env` and secret paths (Module 9 hardened settings); the gateway redacts secret patterns before any log is written. Providers still receive what developers paste; we train developers not to paste credentials and rotate any found.
Evidence: [ADR-0004](adr/0004-prompt-logging.md); [security review checklist](../../../templates/security-review-checklist.md).

## Network

### Q13 Direct provider access bypassing the gateway?
Answer: Blocked. Provider hosts are reachable only from the gateway subnet through the egress proxy.
Evidence: [ADR-0003](adr/0003-central-model-gateway.md), `ArchCheck lint --rules NET`.

### Q14 Private connectivity to providers?
Answer: Yes, secure connection.
Evidence: 

### Q15 Agent's own outbound access when running tools?
Answer: The agent's shell runs in the client sandbox with an allowlist of package and source hosts; web fetch is limited to allowed domains.
Evidence: Module 9 hardened configuration; [security review checklist](../../../templates/security-review-checklist.md).

## Data & residency

### Q16 What leaves the developer machine, and where to?
Answer: Prompts, file excerpts and tool results to the gateway (EU); metrics and events without prompt text to the EU collector; tool calls to EU MCP servers. Nothing else, per the data inventory.
Evidence: [ADR-0005](adr/0005-eu-data-residency.md), architecture whiteboard photo.

### Q17 Every stored copy, retention, readers?
Answer: All data is stored securely.
Evidence: see data inventory

### Q18 No source code leaves the EU, including fallback, MCP, telemetry?
Answer: Yes: both routes EU, all content stores EU, US docs-search MCP server removed, collector moved to the EU tenant.
Evidence: [ADR-0005](adr/0005-eu-data-residency.md), `ArchCheck lint --rules RES`.

### Q19 Personal data reaching the model?
Answer: N/A
Evidence: 

### Q20 Deletion requests and retention limits?
Answer: Content logs expire at 30 days automatically; audit logs hold no content, only hashes; provider retention follows contract terms.
Evidence: [ADR-0004](adr/0004-prompt-logging.md).

## Logging & audit

### Q21 What is logged per call; is prompt content logged?
Answer: Metadata for every model and tool call; prompt content only in redacted form, 30 days.
Evidence: [ADR-0004](adr/0004-prompt-logging.md), `ArchCheck redact` weekly job.

### Q22 Can the audited modify the audit log?
Answer: Only admins can access the logs.
Evidence: 

### Q23 Reconstructing a session?
Answer: Audit entries carry user, team key, session id and prompt id; client events correlate by prompt id; content hash confirms what was sent.
Evidence: [ADR-0004](adr/0004-prompt-logging.md).

### Q24 Spend attribution and unattributed share?
Answer: Every key belongs to a team; September 2026 attribution coverage was 100%.
Evidence: quota spreadsheet (finance), `ArchCheck chargeback solution/usage-2026-09.csv`.

## Agent behaviour & tools

### Q25 Tools and MCP servers; production writes?
Answer: An allowlist of four MCP servers; only `jira-writes` can write, through a per-team bot, for comments and labels. No agent has production database or deployment rights.
Evidence: [architecture.json](architecture.json) `mcpServers`; [MCP security checklist](../../../templates/mcp-security-checklist.md).

### Q26 Prompt injection and untrusted content?
Answer: Threat model per the Module 9 method: no session holds untrusted input, secrets and an outbound channel together; attacks run as a regression suite.
Evidence: [agent threat model template](../../../templates/agent-threat-model.md), Module 9 residual-risk register.

### Q27 Review and testing of agent-generated code?
Answer: Developers review agent code.
Evidence: 

### Q28 Ownership and change approval of the AI layer?
Answer: Each repository's AI layer has a named owner; changes follow the governance policy with an eval gate.
Evidence: [governance policy template](../../../templates/governance-policy.md).

## Operations & cost

### Q29 Quotas, rate limits, budgets; behaviour when hit?
Answer: Per-team TPM quotas (17,500 per developer), separate CI keys, monthly budgets with 80% alerts; CI keys stop at 100%, interactive keys alert only.
Evidence: quota spreadsheet (finance), `ArchCheck simulate solution/gateway.json`.
