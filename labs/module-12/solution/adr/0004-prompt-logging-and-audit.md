# ADR-0004: Redact at the gateway; audit metadata in append-only storage

Status: Accepted
Date: 2026-09-08
Topic: prompt-logging
Deciders: CISO delegate, Data Protection Officer
Consulted: Platform, SRE, Legal
Answers: Q12, Q17, Q20, Q21, Q22, Q23

## Context

The v0 gateway logged every prompt and response in full, kept them 365 days, and the platform team could edit the log. A sample of 12 logged prompts contained 5 live-looking credentials (a SQL password, a bearer token, an AWS key id, a GitHub token, a JWT) and 2 items of customer personal data (`ArchCheck redact break/prompt-log.jsonl`). Developers paste whatever they are debugging; the log inherits it.

We still need to answer "which identity called which model or tool, when, with what outcome" for incidents and audits, and to debug the gateway.

## Options considered

### Option A — Log full content, restrict readers
Simple. The log becomes the most sensitive data store in the company, holding secrets from every system.

### Option B — Log nothing
No sensitive store. No incident reconstruction, no evidence for auditors.

### Option C — Redact content at the gateway before writing; keep a metadata audit trail separately
Content log: redacted prompts, 30 days, EU, for debugging. Audit log: metadata only (time, user, team key, model, route, token counts, tool name, outcome, SHA-256 of the original content) in write-once storage for 400 days.

## Decision

We will adopt Option C. Redaction runs in the gateway, before any sink, using the same pattern set as `ArchCheck redact`, extended by the security team. Client-side telemetry exports metrics and events without prompt text (`OTEL_LOG_USER_PROMPTS` and tool-content logging stay off, enforced by managed settings). The hash lets an investigator confirm that a given prompt was sent without us storing it.

## Consequences

- Positive: the audit trail survives an insider with platform access; secrets pasted into prompts stop accumulating.
- Negative: pattern-based redaction misses secrets in unfamiliar shapes; debugging a bad answer is harder with redacted text.
- Risk: a developer enables verbose client logging locally; managed settings lock the telemetry destination and content flags.
- Follow-up: rotate the five credentials found in the sample; security owns the redaction pattern list.

## Confirmation

`ArchCheck lint --rules LOG,RET` passes. A weekly job runs `ArchCheck redact` over a sample of the redacted content log and must report zero secrets. The audit bucket's retention lock is verified by a quarterly delete attempt that must fail.
