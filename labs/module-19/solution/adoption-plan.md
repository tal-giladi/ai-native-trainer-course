# Fabrikam Group — adoption plan v1

*Illustrative reference for Module 19, built from the [adoption-plan template](../../../templates/adoption-plan.md). Hypothetical company; people and numbers are made up. Compare structure, not numbers.*

## 0. Facts

- Organisation: Fabrikam Group (hypothetical, Module 12), coding agents for all developers
- Developers: 400
- Countries: DE, NL, IL
- Sponsor: R. Vogel, CTO
- Programme owner after handover: Dana Weiss, enablement lead (platform team)
- Consultant: Noa Levin (external)
- Handover date: W12
- Default tool: Tool A
- Feedback arrivals per week: 14
- Feedback resolved per week: 18
- Feedback target weeks: 2

**Goal.** By W26, agents are a weekly habit for at least 60% of developers in every team, new hires included, with no regression in lead time, change failure rate or review load, and the programme runs without the consultant from W12.

## 1. Stakeholders and objections

| Stakeholder | Group | Influence | Stance | Objection | Answer | Evidence | Changes for them | Owner |
|---|---|---|---|---|---|---|---|---|
| Pilot developers (billing, payments, platform) | developer | medium | supporter | "The rollout will force one workflow on us." | One default tool and a shared AGENTS.md core; each team keeps and owns its own rules on top. | 03.2, ADR-0009 | champion roles, team rules ownership | Dana Weiss |
| Later-wave developers | developer | high | skeptic | "It is almost right and I spend longer fixing it." | That happens, and surveys agree. We start on task types where the pilot measured a gain, practise on your code, and measure your team, not a vendor's. | 13.2, EXP-01 | two protected practice hours a week for four weeks | team champions |
| Senior engineers | developer | high | skeptic | "Juniors will merge code nobody understands." | Review and CI gates do not change; agent PRs pass the same review. Review load is a tracked metric with a stop rule. | 11.2, 11.5 | review-load metric in the monthly review | M. Brandt, principal engineer |
| All developers | developer | high | neutral | "First the tool, then the headcount review." | The sponsor states in writing what this programme is for and what its data will not be used for; no individual metrics exist. | sponsor-memo.md, 19.4 | memo read at every wave kickoff | R. Vogel |
| Team leads | manager | high | skeptic | "My sprints are full; show me it pays." | Four weeks of two practice hours, then your team's own numbers at W12; if outcomes do not move, we change the plan, not your capacity. | EXP-01, 13.6 | practice blocks in sprint planning | R. Vogel |
| Director of engineering | executive | high | supporter | "The board wants 100% adoption; make it mandatory." | Mandated use buys logins, not habit. The target is weekly use with outcomes, reported per team. | 19.1, 13.1 | quarterly sponsor review | R. Vogel |
| CISO office | security | high | skeptic | "An agent with repo and Jira access is an exfiltration path." | Agreed, which is why the platform has per-team keys, a gateway, hooks and an attack suite that passed review. Class C changes need security sign-off. | ADR-0003, ADR-0007, 09.6, security-review.md | security path in training; sign-off on class C | Eitan Shaked |
| Legal counsel | legal | high | neutral | "Does the provider train on our code? Who owns the output?" | No-training terms are a requirement (R2) and in the contract; IP guidance is in the acceptable-use policy legal co-writes. | R2, ADR-0005, acceptable-use-policy.md | co-author of the policy | Dana Weiss |
| Data protection officer | privacy | medium | neutral | "Telemetry has user e-mail on every metric." | Individual attributes are dropped at the collector; only team counts with n of 5 or more are stored and shown. | R4, ADR-0004, 12.4 | DPIA update before W1 | Eitan Shaked |
| Works council (DE) | works council | high | neutral | "A system that shows who uses it how much can monitor performance." | Correct, so the works agreement comes before go-live in Germany: team-level data only, stated purpose, no performance use, council sees the dashboard. | BetrVG §87(1) no. 6, works-agreement-draft.md | co-signs section 4 | HR business partner (K. Maier) |
| Finance business partner | finance | medium | neutral | "400 seats from day one; what about unused seats?" | Seats follow the waves; a seat unused for 30 days is reclaimed; spend per team comes from the gateway. | ADR-0006, R7 | monthly per-team report | Eitan Shaked |
| Procurement | finance | medium | neutral | "Tool B comes free in the mobile bundle." | Any second tool goes through an ADR: gateway path, telemetry, security review and training cost first. | ADR-0011, 19.2 | tool requests go through the platform owner | Eitan Shaked |

## 2. Champions and tools

| Team | Developers | Champion | Hours/week | Agreed with | Tool | Rules file |
|---|---|---|---|---|---|---|
| billing | 48 | Priya Nair, Karl Ott | 3 each | billing lead | Tool A | AGENTS.md + CLAUDE.md import |
| payments | 64 | Tomás Ruiz, Ines Wolf, Ben Adam | 3 each | payments lead | Tool A | AGENTS.md + CLAUDE.md import |
| identity | 36 | Lea König, Rafi Dor | 3 each | identity lead | Tool A | AGENTS.md + CLAUDE.md import |
| lending | 72 | Omar Haddad, Sofie Jansen, Nir Tal | 3 each | lending lead | Tool A | AGENTS.md + CLAUDE.md import |
| mobile | 56 | Yuki Tanaka, Mark de Vries, Hila Ron | 3 each | mobile lead | Tool B (ADR-0011) | AGENTS.md (read natively) |
| data | 40 | Jonas Berg, Ana Moreno | 3 each | data lead | Tool A | AGENTS.md + CLAUDE.md import |
| platform | 34 | Eitan Shaked, Lior Paz | 2 each | platform lead | Tool A | AGENTS.md + CLAUDE.md import |
| web | 50 | Chloe Dubois, Avi Katz | 3 each | web lead | Tool A | AGENTS.md + CLAUDE.md import |

**Tool strategy.** Tool A is the supported default. Mobile uses Tool B for native UI work under ADR-0011, which requires Tool B to route through the gateway (telemetry and quotas) and to read the shared AGENTS.md; seats of the other tool are revoked, not kept "just in case". Any further tool follows the same path. Portable core rules live in AGENTS.md ([03.2](../../../lessons/module-03/lesson-02.md)).

## 3. Enablement

### Training paths

| Audience | Format | Minutes | When | Measured by | Owner |
|---|---|---|---|---|---|
| Developers | 2-hour hands-on workshop on the team's own repository, then four weekly practice blocks with the champion | 120 + 4 x 60 | wave start, weeks 1-4 | pre/post items and two-week follow-up with a PR link (16.5) | Dana Weiss |
| Team leads / managers | 45-minute session: the metrics, the practice budget, review load, what not to do with the data | 45 | one week before the wave | follow-up: practice blocks held in weeks 1-4 | Dana Weiss |
| Security champions | 90-minute threat model and attack-suite walkthrough (09.1, 09.6) | 90 | before wave 1 | exercise: triage five injected tickets | CISO delegate |
| New hires (onboarding) | onboarding path in the joiner checklist + one pairing session with the champion | 60 + 60 | week 1 of employment | first agent-assisted PR within three weeks | team champion |
| Champions | monthly champions' session: new patterns, top questions, AI-layer changes | 60 | monthly | questions resolved in their team, observed at office hours; FAQ entries added | Dana Weiss |

### Rhythms

| Mechanism | Cadence | Owner | Input | Output |
|---|---|---|---|---|
| Office hours | weekly, Tuesday 30 min per wave; fortnightly from W16 | Dana Weiss + rotating champion | stuck tasks, questions | answers, FAQ entries, feedback items |
| Feedback triage | weekly, Thursday | Eitan Shaked | feedback form, channel threads, office-hours items | each item fixed in the AI layer or docs, answered, or declined with a reason, within 2 weeks |
| AI-layer review | monthly | Eitan Shaked | incidents, overrides, changelog (11.5) | rules retired or promoted to checks |
| Metrics review | monthly; quarterly with the sponsor | Dana Weiss | usage.csv, outcome metrics, survey | an action for every team below its control limit |
| Champions' sync | fortnightly | Dana Weiss | notes from each team | cross-team fixes and shared examples |
| #agents-help channel | continuous | champions' rota | questions | threads copied to feedback triage |

## 4. Metrics

| Metric | Level | Definition | Target | Source | Aggregation |
|---|---|---|---|---|---|
| Activated | reach | seats with at least one session since the seat was assigned | 90% within 4 weeks of the wave | gateway | team |
| Weekly active | habit | seats with at least one session in the week | 60% or more; alert below the team's lower control limit | gateway | team (n of 5 or more) |
| Engaged | habit | seats with sessions on at least 3 days in the week | 30% or more | gateway | team |
| New-hire activation | habit | new hires active in weeks 2-4 after joining | 50% or more | gateway + joiner list | cohort (n of 5 or more) |
| Lead time for changes | outcome | commit to production, median (13.1) | no regression vs the pre-rollout baseline | CI/CD | team |
| Change failure rate | outcome | deployments causing a failure in production (13.1) | no regression vs baseline | incident log | team |
| Review load | outcome | reviewer hours per merged PR, from review timestamps | no increase above 15% | GitHub | team |
| Usefulness and trust | experience | five-question quarterly survey | trend, not target | survey | team |

## 5. Anti-regression and handover

| Mechanism | Owner | Trigger | Action |
|---|---|---|---|
| Programme ownership handover | Dana Weiss, with R. Vogel as sponsor | W12 (rehearsed in W10) | handover checklist signed; owners recorded in governance.md |
| Champion succession | team leads | a champion leaves or changes team | co-champion steps up the same week; new co-champion named within two weeks |
| Onboarding | HR business partner + team champion | a new hire starts | joiner checklist item 7: seat, setup, onboarding path, pairing session |
| Usage alert | Dana Weiss | a team's weekly active share below its lower control limit | meet the champion and lead within a week; read that week's events; one action, dated |
| Tool and licence change control | Eitan Shaked | tool, licence, bundle or quota change proposed | ADR with gateway path and telemetry before the change (class D, 11.5) |
| Follow-ups | Noa Levin with Dana Weiss | W16, W20, W26 | rerun AdoptCheck usage and review with the sponsor |

## 6. Cadence summary

Weekly: office hours, feedback triage. Fortnightly: champions' sync. Monthly: metrics review, AI-layer review, champions' session. Quarterly: sponsor review, survey. Dated: handover W12, follow-ups W16, W20, W26.
