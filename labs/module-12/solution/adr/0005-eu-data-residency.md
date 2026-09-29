# ADR-0005: Every copy of prompt data stays in the EU

Status: Accepted
Date: 2026-09-10
Topic: data-residency
Deciders: Data Protection Officer, Head of Platform Engineering
Consulted: Legal, Security, SRE
Answers: Q16, Q17, Q18, Q19, Q20

## Context

"No code may leave the EU" was satisfied by v0's primary route only. The data inventory found three more paths: the fallback route used the vendor API with default (global) routing; a documentation-search MCP server was a US-hosted SaaS; and the observability collector was a US SaaS (metadata only). Tool arguments and results are prompt data too.

## Options considered

### Option A — Primary route in the EU, everything else as is
No work. Fails the policy on the first provider outage (fallback) and on every docs search.

### Option B — Every route, store and MCP server holding content in the EU; metadata-only telemetry allowed outside with legal sign-off
Requires an EU fallback provider, replacing the docs-search server, and an EU collector.

### Option C — Self-host everything in our EU data centre
Strongest control; eliminated by ADR-0001 on pass rate and cost.

## Decision

We will adopt Option B, and go one step further on telemetry: the collector moves to our EU tenant, so no exception is needed. The fallback route is the EU multi-region endpoint of hyperscaler B (ADR-0001). The US docs-search MCP server is removed; docs search runs against our Confluence through the EU-hosted server. Developer machines keep local transcripts 14 days (`cleanupPeriodDays`), set in managed settings.

## Consequences

- Positive: the data inventory has no content path outside the EU.
- Negative: about 10% higher unit price on the fallback route, and fewer API features there.
- Negative: the newest models may reach EU endpoints later than global ones.
- Risk: a new MCP server or SaaS tool added by a team silently reopens a path; MCP servers are restricted to an allowlist in managed settings.

## Confirmation

`ArchCheck lint --rules RES` runs in CI. Every new MCP server or telemetry destination requires an ADR amendment and an entry in the data inventory. The DPO reviews the inventory twice a year.
