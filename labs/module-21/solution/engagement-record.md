# Engagement record — C-01 (dry run)

> **Illustrative reference.** The dry-run engagement against `brownfield-demo`, with Contoso as a simulated client. Format: [engagement playbook](../../../templates/engagement-playbook.md). A real record stays **private**: it names people and holds client data. Check each section with `EngageCheck`.

- Client code: C-01
- Client: Contoso Ltd
- Engagement: AI layer with a measured pilot (implementation rung)
- SOW: ../client/sow-contoso.md, version 1.0
- Sponsor: Tamar Golan, Head of Engineering
- Consultant: Student (S)
- Kickoff date: 2026-10-05
- Build start: 2026-10-19
- Handover date: 2026-12-23

## 1. Discovery and kickoff

### Stakeholders

| Stakeholder | Role | Influence | Interest | Wants | Worries | Success in their words | Interviewed | Plan |
|---|---|---|---|---|---|---|---|---|
| Tamar Golan | Head of Engineering, sponsor | H | H | A renew/don't-renew decision in February based on Contoso's own work | Another initiative gone in a quarter (2025 AI week) | "I can tell the CFO what we got for the money, including if it is not much" | 2026-09-29 | Weekly 20-min check-in; signs D2 and D5; read-out in week 11 |
| Avi Ben-David | Billing tech lead, AI-layer owner | H | H | Agent stops writing new code against SqlHelper and DateTime.Now | Being left a layer he cannot change (2024 static-analysis tool, switched off) | "I change the rules file myself on a Tuesday" | 2026-09-30 | Pairs on the first layer PR; merges v0.1 himself; owns the layer from week 3 |
| Dana Katz | Billing developer (migrations) | M | H | Migrations with undo scripts | Paged when a rollback fails; skeptical | "It stops forgetting undo scripts on our migrations, more than once" | 2026-09-30 | Her incident becomes eval task and regression test; she co-writes the migration rule |
| Yossi Mizrahi | Billing developer | L | H | Share his prompts | None stated (risk: personal setup becomes the team's) | "My prompts are the team's prompts" | 2026-10-01 | Channel his prompts through the skill review, not around it |
| Noa Shapiro | Billing product manager | M | M | Finance close Q4 on time | Pilot slows November | "Tickets flow as usual and nobody picks easy ones for the AI side" | 2026-10-01 | Randomization by size in her refinement meeting; weekly flow note |
| Ruth Amar | Finance controller (business user of the revenue report) | H | M | Told before any revenue-report change ships | A number moving without explanation (2023 status change, 0.4%) | "The revenue number never moves by surprise" | 2026-10-02 | Revenue-report changes announced 5 days ahead, never in freeze days; she reviews the BILL-97 comparison output |
| Eli Sasson | IT and security lead | H | M | Read-only access, no production credentials, agent permissions reviewed | New provider settings without a data-protection review (3 weeks) | "I know where the code goes and what the agent can run" | 2026-09-29 | Review requested 2026-09-29; laptop access 2026-10-05; permissions file reviewed in week 2 |
| Shira Cohen | Collections team lead | M | M | Told about schema and status changes | Customers chased for paid invoices | "No surprise status values" | 2026-10-02 | CODEOWNERS entry on migrations; workshop wave 1 |
| Omer Peretz | Platform team lead, champion | M | M | CI gate that does not slow builds | Moving to data team in January | "The gate is ours, not a black box" | 2026-10-02 | Co-champion Lior named from day one |

### Acceptance

| Deliverable | Acceptance criterion | Accepted by | Due |
|---|---|---|---|
| D1 Audit and baseline | As SOW section 3 | Avi Ben-David, Billing tech lead | Week 2 |
| D2 Pre-registration | As SOW section 3 | Tamar Golan, sponsor | Week 2 |
| D3 AI layer v1 | As SOW section 3 | Avi Ben-David, Billing tech lead | Week 6 |
| D4 Enablement | As SOW section 3 | Tamar Golan, sponsor | Week 10 |
| D5 Results report and read-out | As SOW section 3 | Tamar Golan, sponsor | Week 11 |
| D6 Handover pack | As SOW section 3 | Avi Ben-David, Billing tech lead | Week 12 |

### Working agreement

- Access: read-only repository and Jira export on a Contoso laptop from 2026-10-05, after Eli's review (requested 2026-09-29)
- Data handling: code and tickets stay on the Contoso laptop; exports arrive with developer codes D1-D6; deleted within 30 days of the 90-day follow-up
- Cadence: Monday 20-min check-in with Tamar and Avi; Thursday pairing blocks with Billing; demo every second Friday
- Channel: one shared Slack channel, decisions copied into this record
- Escalation: a missed client responsibility is raised at the next Monday check-in, then with Tamar in writing; schedule moves day for day (SOW 6)
- Success measure: the D2 outcomes (cycle time, escaped defects) estimated with 95% intervals on Billing's randomized tickets, plus eval pass rate before and after the layer; any result, including none, is reported
- Detectable effect: with about 27 tickets and a within-size sd(log) of about 0.44, only a cycle-time change of about 38% or more is likely to be detected; smaller real effects will read as inconclusive

### Premortem

| Risk | Early signal | Owner | Mitigation |
|---|---|---|---|
| Access arrives in week 3 because the data-protection review started late | No laptop by 2026-10-05 | Eli Sasson | Review requested before kickoff; dry-run audit on the public demo while waiting |
| The layer is the consultant's, and dies after handover | Consultant authors most layer PRs after week 4 | Avi Ben-David | Pairing plan; client-authored share tracked weekly (EngageCheck build) |
| The comparison is too small to show anything and the sponsor reads that as failure | Sponsor asks for "the number" in week 4 | Tamar Golan | Detectable effect stated at kickoff and in D2; eval and adoption measured as well |
| A revenue-report change ships during month-end freeze | BILL-97 PR opened after the 25th | Noa Shapiro | BILL-97 scheduled for 10 November, reviewed by Ruth's comparison run |
| Enthusiast's personal prompts become the team's rules | Rules PRs from one person only | Avi Ben-David | Skills go through review and the eval gate like any other change |

## 2. Audit and baseline

### Findings

| Finding | Evidence (rungs) | Confirmed | Severity | Owner |
|---|---|---|---|---|
| Build and test with Contoso.Billing.sln; 6 tests pass | ran dotnet test (1), csproj (2) | yes | low | Avi Ben-David |
| New data access = Dapper repository behind an interface; SqlHelper only in Legacy/ | ConventionTests (1), ADR 0007 (3), BILL-66 history (4) | yes | high | Avi Ben-David |
| MonthlyRevenueReport is SqlHelper's last caller (BILL-97) | AiLayerTool scan (1), MonthlyRevenueReport.cs (2) | yes | high | Avi Ben-David |
| Every V### migration needs a U### undo script; V001 and V002 have none | scan (1), V003 comment (2), Dana's incident (7) | yes | high | Dana Katz |
| Time from IClock, never DateTime.Now | ConventionTests (1), InvoiceService (2) | yes | medium | Avi Ben-David |
| docs/ARCHITECTURE.md is stale (2021; names Billing.sln and SqlHelper) | scan (1), ADR 0007 dated 2024 (3) | yes | medium | Avi Ben-David |
| No AI layer: no AGENTS.md, CLAUDE.md, CODEOWNERS | scan (1) | hypothesis: .claude/ exists and may hold personal settings; ask Yossi | medium | Avi Ben-David |
| Agent seats used by 4 of 19 developers | licence export (2), sponsor interview (7) | yes | medium | Tamar Golan |

### Baseline

| Metric | Definition | Source | Window | n | Value | Spread | Captured |
|---|---|---|---|---|---|---|---|
| Cycle time | Hours from In Progress to Done, per ticket, all sizes; size recorded in refinement | Jira export, ImpactStats describe | 12 weeks (2026-W29..W40) | 60 | median 18.5 h | p25 11.3 h, p75 29.3 h; sd(log) within size about 0.44 | 2026-10-09 |
| Review time | Hours from PR opened to merged, per ticket | GitHub export | 12 weeks (2026-W29..W40) | 60 | median by size S 3.1 h, M 7.1 h, L 13.9 h | sd(log) 0.36-0.50 | 2026-10-09 |
| Escaped defects | Share of tickets with a defect ticket linked within 30 days of release | Jira export | 12 weeks (2026-W29..W40) | 60 | 6 of 60 (10%) | Wilson 95% 5% to 20% | 2026-10-09 |
| Eval pass rate | Module 7 task set on before-ai-layer, 24 tasks x 5 trials | EvalHarness, eval-results.csv | one run | 120 | 53% | task-clustered 95% CI 44% to 63% | 2026-10-14 |
| Weekly active agent use | Developers with at least one session in the week, by team | licence export, AdoptCheck usage | 12 weeks | 228 | 3 to 4 of 19 a week | range 2-5 of 19 | 2026-10-09 |

- Detectable effect: about 38% on cycle time (27 tickets, sd(log) 0.44, 80% power); stated in D2

## 3. Architecture and build

### Decisions

| Decision | Decided by | Record |
|---|---|---|
| AGENTS.md canonical, CLAUDE.md imports it | Avi Ben-David, after S recommended it | ADR 0008 |
| Migration rule: every V### with a U###, enforced by a CI check | Dana Katz and Avi Ben-David | PR 12, ADR 0009 |
| Schema MCP server in snapshot mode only, no live database | Eli Sasson | security review note 2026-10-21 |
| Eval gate blocks merges to the AI layer; smoke on PRs, full nightly | Omer Peretz | PR 19 |
| Yossi's prompts become two skills after review, not a shared gist | Avi Ben-David | PR 16 |

The PR log is [`prs.csv`](prs.csv).

## 4. Enable, measure, hand over

### Owners

| Responsibility | Owner | Backup | Ran without consultant |
|---|---|---|---|
| AI layer: rules, skills, changelog | Avi Ben-David | Dana Katz | 2026-11-30 (v1.1 release) |
| Eval regression gate and nightly suite | Omer Peretz | Lior Ben-Ami | 2026-12-02 (gate blocked PR 27) |
| Office hours and questions channel | Dana Katz | Yossi Mizrahi | 2026-12-03 and 2026-12-10 |
| Metrics and usage review (monthly) | Tamar Golan | Avi Ben-David | 2026-12-16 |
| Onboarding item for new developers | Shira Cohen | Omer Peretz | 2026-12-14 (one new hire) |

### Results

| Outcome | Baseline | Result | Interval | Source | Status |
|---|---|---|---|---|---|
| Cycle time, ai vs manual (ratio of geometric means within size) | median 18.5 h | -8% | [-24%, +12%] | ImpactStats compare, pilot-tickets.csv, 27 tickets | inconclusive |
| Escaped defects, ai vs manual | 10% | 0 of 12 vs 2 of 15 (-13 pts) | [-27, 0] pts | ImpactStats compare --binary | inconclusive |
| Eval pass rate, layer v1 vs before | 53% | 75% (+21.7 pts) | [+10.6, +32.8] pts | EvalHarness compare, 24 tasks x 5 trials, paired | improved |
| Weekly active agent use, all teams | 2-3 of 19 | 79% (W09-W12), 89% at 90 days | Wilson 95% [69%, 97%] at 90 days | AdoptCheck usage | improved |

### What this does not show

- Whether Billing is faster: 27 tickets can only detect changes of about 38% or more; the estimate (-8%) is compatible with anything from 24% faster to 12% slower.
- Whether defects fell: 0 of 12 against 2 of 15 is two tickets.
- That the eval gain transfers to delivery: the tasks are Contoso's conventions, not Contoso's throughput.
- Anything about Collections or Platform delivery: they were enabled, not measured.

### Follow-ups

| Follow-up | Date | Checks | Owner | Result |
|---|---|---|---|---|
| 30 days | 2027-01-22 | Gate, office hours, layer release ran without S; usage by team | Tamar Golan with S | All ran; no team below its limit |
| 60 days | 2027-02-19 | Same, plus renewal input for the Q1 budget | Tamar Golan with S | Platform below its limit since W19 (champion moved); Lior announced |
| 90 days | 2027-03-19 | Same; close the engagement | Avi Ben-David with S | All teams in range; closed |

- Access revoked: 2026-12-23
- Data deleted: 2027-03-26
- Case study: ../solution/case-study.md, approved 2027-04-02
