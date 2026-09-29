# Module 12 labs — Enterprise AI-agent architecture

Lessons: [12.1](../../lessons/module-12/lesson-01.md) · [12.2](../../lessons/module-12/lesson-02.md) · [12.3](../../lessons/module-12/lesson-03.md) · [12.4](../../lessons/module-12/lesson-04.md) · [12.5](../../lessons/module-12/lesson-05.md).

You design the coding-agent platform for **Fabrikam**, a hypothetical 400-developer .NET company with an EU-only data policy ([brief](fabrikam/brief.md)). The platform team's first draft (`break/`) failed security review. Across five lessons you find out why, fix it, and defend the result with ADRs, a reference diagram and answers to a 30-question security review (`solution/` is one reference answer).

Everything runs **offline**: no Docker, no cloud account, no API key. The data is illustrative: prices, pass rates and loads are made up to show mechanisms, not quotes or measurements. Provider facts cited in the lessons are as of 2026-09.

> [!WARNING]
> `break/prompt-log.jsonl` contains **fake** credentials in the shapes of real ones (the AWS key id is AWS's own documented example). If you point `ArchCheck redact` at real logs, do it inside your organisation's approved environment, never copy real log lines elsewhere, and rotate every credential it finds.

## Requirements

- .NET SDK 8 or later (`RollForward=Major`, no NuGet packages). Verified with SDK 10.
- Python 3 only if you want to regenerate the usage data (`scripts/generate_usage.py`).

## Contents

| Path | What it is |
|---|---|
| `tools/ArchCheck/` | Dependency-free C# checker: `hosting`, `simulate`, `chargeback`, `lint`, `redact`, `adr`, `review`. |
| `fabrikam/brief.md` | The company, its requirements R1–R10, planning volumes, deliverables. |
| `fabrikam/hosting-options.json` | Six hosting options with geographies, features, pass rates (n=50) and illustrative costs. |
| `fabrikam/prices.json`, `fabrikam/usage-2026-08.csv` | Illustrative per-MTok prices and headcount; one month of gateway usage per key per day. |
| `fabrikam/security-review-questionnaire.json` | The 30-question enterprise security review (8 categories). |
| `break/architecture.json` | Draft v0: shared keys, direct provider access, global fallback, raw prompt log kept 365 days, US-hosted MCP server. |
| `break/gateway.json` | One shared key for every team and CI, with a runaway CI loop in the load. |
| `break/prompt-log.jsonl` | Twelve logged prompts; five contain fake secrets, two contain personal data. |
| `break/adr/`, `break/security-review.md` | The draft decision log and review answers, with the problems 12.5 asks you to find. |
| `solution/` | v1: architecture, per-team gateway, September usage, seven ADRs, `diagram.md`, `security-review.md`. |
| `scripts/generate_usage.py` | Regenerates both months of usage data (seeded). |

## Quick start

From this folder:

```bash
dotnet build tools/ArchCheck -c Release
A="dotnet tools/ArchCheck/bin/Release/net8.0/ArchCheck.dll"

# 12.1 hosting: constraints first, then cost per successful task
$A hosting fabrikam/hosting-options.json --by price    # the naive ranking (Break)
$A hosting fabrikam/hosting-options.json               # the real one

# 12.2 gateway: quotas and chargeback
$A simulate break/gateway.json        # one shared key: 8 teams throttled
$A simulate solution/gateway.json     # per-team keys: only the runaway CI key is throttled
$A chargeback fabrikam/usage-2026-08.csv fabrikam/prices.json    # 67% attribution: FAIL
$A chargeback solution/usage-2026-09.csv fabrikam/prices.json    # 100%: PASS

# 12.3 identity, secrets, network, audit
$A lint break/architecture.json --rules IDN,SEC,NET,LOG,RET,MCP
$A redact break/prompt-log.jsonl --out out/prompt-log.redacted.jsonl

# 12.4 residency and retention
$A lint break/architecture.json --rules RES,RET
$A lint solution/architecture.json    # PASS

# 12.5 ADRs and the security review
$A adr break/adr
$A adr solution/adr
$A review fabrikam/security-review-questionnaire.json break/security-review.md
$A review fabrikam/security-review-questionnaire.json solution/security-review.md
```

Every command exits 2 on a usage or input error. `hosting` is a report and exits 0; the others exit 0 on pass and 1 on a finding (for `simulate`, more than one team throttled above 2%), so each can gate a CI job.

## The checks, briefly

| Command | Checks | Lesson |
|---|---|---|
| `hosting` | eliminates options on residency, required features and a pass-rate floor (with a Wilson interval), then ranks survivors by cost per successful task including fixed cost and review of failures | 12.1 |
| `simulate` | token buckets per gateway key inside an organisation bucket, refilled every second, with Poisson arrivals; reports 429s per load and flags noisy neighbours | 12.2 |
| `chargeback` | cost per team from usage and prices; attribution coverage; FAIL below 95% | 12.2 |
| `lint` | `IDN-*` shared or long-lived credentials; `SEC-*` where secrets are stored; `NET-DIRECT` bypassing the gateway; `RES-*` routes, stores and MCP servers outside the residency set; `LOG-*` raw content logs, missing or mutable audit log; `RET-MAX` retention; `MCP-SHARED`; `AVL-FALLBACK` | 12.3, 12.4 |
| `redact` | secret and personal-data patterns in logged prompts; writes a redacted copy with a short SHA-256 per changed field | 12.3 |
| `adr` | ADR structure (`templates/adr.md`), valid status, ≥2 options, negative consequence, confirmation, supersession links both ways, one accepted ADR per topic | 12.5 |
| `review` | every questionnaire item answered, not TBD, with evidence; evidence links resolve | 12.5 |

All of these are heuristics over files you declare. A clean `lint` says the architecture file follows the rules, not that the running system does; that is what the confirmations in the ADRs (egress reports, delete tests, weekly redaction runs) are for.

## Lab sequence

1. **12.1 — Hosting options.** `hosting` with and without `--by price`; sensitivity to review cost and volume. *Break:* price ranking picks a model that fails the pass-rate floor.
2. **12.2 — Gateways and routing.** `simulate` both gateways; `chargeback` both months; availability with the gateway in series. *Break:* one shared key throttles eight teams and hides a third of spend.
3. **12.3 — Identity, secrets, networking, audit.** `lint` identity and logging rules; `redact` the prompt log. *Break (outline):* agent tokens shared across teams and prompts logged with secrets.
4. **12.4 — Data, residency, compliance.** `lint --rules RES,RET`; build the nine-row data inventory. *Break:* primary route in the EU, fallback, docs-search tool and collector outside it.
5. **12.5 — ADRs and the reference architecture.** `adr` and `review` on the draft and the solution; build your own `enterprise-architecture` repository. *Break:* two accepted ADRs on one topic, a one-option ADR, a dangling supersession, answers without evidence that contradict the architecture file.

## Artifacts to commit to `enterprise-architecture`

- `diagram.md` — context and container views with trust boundaries and ADR numbers
- `architecture.json` — the lintable twin of the container view
- `adr/` — at least five ADRs ([template](../../templates/adr.md)), at least one superseding another
- `security-review.md` — 30 answers with evidence
- `data-inventory.md` — every copy of prompt data, where, how long, who reads it
- a CI workflow running `lint`, `adr` and `review`

See the portfolio scaffold: [enterprise-architecture](../../projects/enterprise-architecture/README.md). Reused templates: [security review checklist](../../templates/security-review-checklist.md), [agent threat model](../../templates/agent-threat-model.md), [MCP security checklist](../../templates/mcp-security-checklist.md), [model selection matrix](../../templates/model-selection-matrix.md), [governance policy](../../templates/governance-policy.md).
