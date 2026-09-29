# Engagement playbook template

*Placeholders are in `{braces}`. Used in Module 21 ([21.1](../lessons/module-21/lesson-01.md) to [21.5](../lessons/module-21/lesson-05.md)). Two parts: the **playbook** (how you run every engagement, reused and improved each time) and the **engagement record** (one per client, private). `EngageCheck` from [`labs/module-21`](../labs/module-21/README.md) reads the record's `- Key: value` lines and tables by their column names, so keep the headers; add columns and rows freely. Reference record: [`labs/module-21/solution/engagement-record.md`](../labs/module-21/solution/engagement-record.md).*

> [!WARNING]
> An engagement record names people and holds client data. Keep it in a **private** repository, never on a personal cloud drive the SOW does not allow, and delete client exports on the date the SOW says. Only the anonymized, approved case study ([template](case-study.md)) ever leaves.

## Part A — The playbook

Discovery → Audit → Baseline → Architecture → Build → Enable → Measure → Handover → Follow-up. Each phase has an exit gate; do not start the next phase until the gate is met, or record in the record why you did.

| Phase | Weeks (12-week engagement) | You produce | Exit gate | Tooling |
|---|---|---|---|---|
| Discovery and kickoff | before 1, 1 | Stakeholder map from interviews, acceptors per deliverable, working agreement, premortem | Every high-influence stakeholder interviewed; access dated; `EngageCheck kickoff` clean | [discovery-call script](discovery-call-script.md), [SOW](sow.md) |
| Audit | 1–2 | Findings with evidence rungs | Every finding confirmed (two sources, one from rungs 1–3) or labelled a hypothesis | `AiLayerTool scan` (Module 3), [audit checklist](brownfield-audit-checklist.md) |
| Baseline | 1–2 | Delivery metrics with spread, a guardrail, an eval baseline, the detectable effect | Captured before the build starts; `EngageCheck baseline` clean; pre-registration signed | `ImpactStats describe`, `power` (Module 13); `EvalHarness` (Module 7); [experiment design](experiment-design.md) |
| Architecture | 2–3 | Decisions as ADRs, decided by client people | Each decision has a client decider and a record | [ADR](adr.md), [AI-layer architecture](ai-layer-architecture.md) |
| Build | 3–6 | The AI layer, built in pairs; the PR log | Client-authored share of AI-layer changes at least 70% in the last third; client truck factor at least 2; `EngageCheck build` clean | `SkillCheck` (Module 6), `EvalHarness gate` (Module 7) |
| Enable | 6–10 | Workshop, office hours, champions; owners who are employees | Each rhythm run once by its owner without you | [workshop](workshop-template.md), [adoption plan](adoption-plan.md), [pre/post](pre-post-assessment.md) |
| Measure | 6–11 | Results with intervals; what this does not show | Every pre-registered outcome reported, including null and negative | `ImpactStats compare`, `EvalHarness compare`, `AdoptCheck usage` |
| Handover | 12 | Owners table, access revoked, data deletion date | `EngageCheck handover` clean | [experiment report](experiment-report.md) |
| Follow-up | +30, +60, +90 days | Short notes; the case study | Usage and rhythms checked by team; case study approved in writing | `AdoptCheck usage`; [case study](case-study.md), `EngageCheck casestudy` |

**Your playbook changes.** After each engagement, add one line per phase: what slipped, what you now do differently. The playbook is the part of your consulting that becomes a product (Module 22).

| Date | Engagement | Phase | What happened | What the playbook now says |
|---|---|---|---|---|
| YYYY-MM-DD | C-01 | Kickoff | Security review took three weeks and started late | Request the review before signature; access date is a SOW client responsibility |

## Part B — The engagement record

One file per client, `case-studies/{client-code}/engagement-record.md`, filled in phase by phase.

### Facts

- Client code: {C-01; the only name used outside the private repository}
- Client: {legal name; private}
- Engagement: {what, which rung of your offer ladder}
- SOW: {path and version}
- Sponsor: {name, role}
- Consultant: {your name (handle used in the PR log)}
- Kickoff date: YYYY-MM-DD
- Build start: YYYY-MM-DD
- Handover date: YYYY-MM-DD

### 1. Discovery and kickoff (21.1)

**Stakeholders.** Required: a sponsor, the technical owner, developers, security or IT, and at least one business person who lives with the software's output. Interview every high-influence person before kickoff and ask about a past event, not an opinion.

| Stakeholder | Role | Influence | Interest | Wants | Worries | Success in their words | Interviewed | Plan |
|---|---|---|---|---|---|---|---|---|
| {name} | {role} | H / M / L | H / M / L | {in their words} | {in their words; the past event} | {what they would see; never a promise} | YYYY-MM-DD | {how and when you involve them} |

**Acceptance.** One named client role per SOW deliverable; never you, never "the client".

| Deliverable | Acceptance criterion | Accepted by | Due |
|---|---|---|---|
| D1 … | {from the SOW} | {name, role} | Week n |

**Working agreement.**

- Access: {what, where, from YYYY-MM-DD}
- Data handling: {where client code and exports live; codes instead of names; deletion date}
- Cadence: {check-ins, pairing blocks, demos}
- Channel: {one channel; decisions copied into this record}
- Escalation: {what happens when a client responsibility slips}
- Success measure: {the SOW's measurement, restated; no target, no guarantee}
- Detectable effect: {the smallest effect this engagement can see, from 21.2}

**Premortem.** Imagine it is the 90-day follow-up and the engagement failed. Why?

| Risk | Early signal | Owner | Mitigation |
|---|---|---|---|
| {reason it failed} | {what you would see in week 2} | {client name} | {what you do now} |

### 2. Audit and baseline (21.2)

**Findings.** Evidence rungs from [03.3](../lessons/module-03/lesson-03.md): (1) executable, (2) recent code, (3) decision records, (4) history, (5) repo docs, (6) wiki, (7) memory. Confirmed = two sources, one from rungs 1–3.

| Finding | Evidence (rungs) | Confirmed | Severity | Owner |
|---|---|---|---|---|
| {finding} | {source (1), source (3)} | yes / hypothesis: {how to test} | high / medium / low | {client name} |

**Baseline.** Captured before `Build start`. Tickets and teams, never people. At least one guardrail (defects, failures, reverts, rework) and one eval baseline.

| Metric | Definition | Source | Window | n | Value | Spread | Captured |
|---|---|---|---|---|---|---|---|
| {metric} | {start/stop events, unit, inclusion} | {export, tool} | {12 weeks (dates)} | {count} | {median} | {p25/p75, sd(log), or interval} | YYYY-MM-DD |

- Detectable effect: {from ImpactStats power with the baseline sd(log) and the tickets the SOW window allows}

### 3. Architecture and build (21.3)

**Decisions.** You recommend; a client person decides.

| Decision | Decided by | Record |
|---|---|---|
| {decision} | {client name, after you recommended it} | {ADR, PR} |

**PR log** (`prs.csv`, one row per merged change):

```text
pr,week,author,author_side,reviewer,reviewer_side,area,mode,files
1,W03,S,consultant,Avi Ben-David,client,ai-layer,pair,AGENTS.md;CLAUDE.md
```

`author_side` and `reviewer_side` are `client`, `consultant` or `none`; `area` is `ai-layer`, `code`, `ci` or `docs`; `mode` is `solo`, `pair` or `mob`; `files` are separated by `;`.

### 4. Enable, measure, hand over, follow up (21.4)

**Owners.** Employees, each with a backup, each having run the rhythm once without you before the handover date.

| Responsibility | Owner | Backup | Ran without consultant |
|---|---|---|---|
| AI layer: rules, skills, changelog | {name} | {name} | YYYY-MM-DD |
| Eval regression gate | | | |
| Office hours and questions | | | |
| Metrics and usage review | | | |

**Results.** Every pre-registered outcome, with its interval and a status that matches the interval: improved, worse, inconclusive, or no detectable change.

| Outcome | Baseline | Result | Interval | Source | Status |
|---|---|---|---|---|---|
| {outcome} | {baseline value} | {estimate} | {[low, high]} | {command, file} | {status} |

**What this does not show.** At least three bullets.

**Follow-ups.** About 30, 60 and 90 days after the handover date.

| Follow-up | Date | Checks | Owner | Result |
|---|---|---|---|---|
| 30 days | YYYY-MM-DD | {rhythms ran without you; usage by team} | {client owner with you} | |

- Access revoked: YYYY-MM-DD
- Data deleted: YYYY-MM-DD
- Case study: {path}, approved YYYY-MM-DD
