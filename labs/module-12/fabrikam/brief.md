# Fabrikam Group — brief for the coding-agent platform

*Hypothetical enterprise used throughout Module 12. Any resemblance to a real company is accidental. Numbers are illustrative.*

## The company

- 400 developers in eight teams: billing 48, payments 64, identity 36, lending 72, mobile 56, data 40, platform 34, web 50.
- Mostly C#/.NET 8 services on SQL Server, plus a React web front end and two mobile apps. Contoso Billing (the app from Modules 3–11) is the billing team's main service.
- Offices in Germany, the Netherlands and Israel; all production systems in EU cloud regions.
- GitHub Enterprise Cloud with EU data residency, GitHub Actions for CI, Jira and Confluence Cloud, Microsoft Entra ID as the identity provider, one primary cloud (hyperscaler A) and a small footprint on a second (hyperscaler B).

## Where they are now

A three-team pilot (30 developers, March to June 2026) used coding agents with keys from each team's own provider account (ADR-0002). It worked; the CTO wants all 400 developers on agents by the end of Q4 2026. The platform team drafted an architecture (`break/architecture.json`) and sent it to security review, which returned it.

## Requirements

| Id | Requirement | From |
|---|---|---|
| R1 | Source code, prompts and tool results are processed and stored only in the EU, including fallback paths. | Group data-protection policy §4.2 |
| R2 | No training on Fabrikam inputs or outputs by any provider. | Legal |
| R3 | Every model call and tool call is attributable to a person or a workload, and to a team. | Security, Finance |
| R4 | Content logs (prompts, responses) at most 30 days; audit metadata kept 400 days and not modifiable by the platform team. | Security, DPO |
| R5 | No long-lived credentials in CI; no provider keys on developer machines. | Security |
| R6 | One team's runaway job must not degrade other teams. | Engineering leadership |
| R7 | Spend reported monthly per team; at least 95% of spend attributable. | Finance |
| R8 | Agents must work during a single provider's outage, within R1. | Engineering leadership |
| R9 | A provider or model switch must not require changes on 400 machines. | Platform |
| R10 | Agents read Jira, Confluence and the SQL Server schema; writes to Jira are limited to comments and labels. | Team leads, Security |

## Volumes (planning numbers)

- About 120,000 agent attempts per month at full rollout; a failed attempt costs about 15 minutes of review (≈ $15).
- Peak hour: about 35% of developers active, three requests a minute each, about 12,000 uncached input tokens per request.
- Organisation rate limit requested from the provider: 7,000,000 input tokens per minute (17,500 per developer, inside the vendor's 15–20k guidance for 100–500 users).

## Deliverables (the Module 12 project)

1. A reference architecture diagram with trust boundaries (`diagram.md`).
2. At least five ADRs, including one that supersedes another.
3. Answers to the 30-question security review (`security-review-questionnaire.json`), each with evidence.
4. A machine-readable architecture file that passes `ArchCheck lint`.
