# Adoption plan template

*Used in Module 19 ([19.1](../lessons/module-19/lesson-01.md) to [19.4](../lessons/module-19/lesson-04.md)). One file, six sections, one or two pages of tables. `AdoptCheck` from [`labs/module-19`](../labs/module-19/README.md) reads the lines and tables below by their column names, so keep the headers; add columns and rows freely.*

An adoption plan answers one question for a sponsor: **what will make people keep using this, correctly, after the people who launched it have moved on?** It is not a licence rollout schedule. Write it for your own team, a client, or the hypothetical Fabrikam Group from [Module 12](../labs/module-12/fabrikam/brief.md).

## 0. Facts

Keep these as `- Key: value` lines; the checker reads them.

- Organisation: \<name, size, what the rollout is>
- Developers: \<number in scope>
- Countries: \<ISO codes where developers are employed, e.g. DE, NL, IL — employee-representation rules depend on it>
- Sponsor: \<name, role — the executive who owns the outcome and can remove obstacles>
- Programme owner after handover: \<name, role — an employee, not you>
- Consultant: \<your name, if you are external; leave out if you are internal>
- Handover date: \<week or date when the programme owner takes over>
- Default tool: \<the one supported agent tool; exceptions need an ADR>
- Feedback arrivals per week: \<expected items>
- Feedback resolved per week: \<capacity of the triage owner>
- Feedback target weeks: \<target time from report to answer>

**Goal in one sentence.** What changes in delivery, for whom, by when — not "400 seats by Q4".

## 1. Stakeholders and objections (19.1)

One row per group whose support, time or signature the rollout needs. Every objection gets an answer you can back with a reference someone can open: a lesson, ADR, experiment (EXP-), incident, requirement id, file or URL. No promises you cannot keep ("guaranteed", "10x", "completely safe", "nobody will be replaced").

| Stakeholder | Group | Influence | Stance | Objection | Answer | Evidence | Changes for them | Owner |
|---|---|---|---|---|---|---|---|---|
| \<who> | developer / manager / security / legal / privacy / works council / finance / executive | high / medium / low | champion / supporter / neutral / skeptic / blocker | \<in their words> | \<concede what is true, then the boundary and the evidence> | \<reference> | \<what is different in their week> | \<who meets them, by name> |

Required groups: developers, managers, security, legal or privacy; employee representation (works council) where developers are employed in countries that have it.

## 2. Champions and tools (19.2)

One row per team. Champions are peers inside the team, with time agreed by their manager; the consultant is never a champion. Planning heuristic: about one champion per 25 developers.

| Team | Developers | Champion | Hours/week | Agreed with | Tool | Rules file |
|---|---|---|---|---|---|---|
| \<team> | \<n> | \<name, co-champion> | \<h each> | \<manager> | \<default tool, or other tool (ADR-nnnn)> | \<AGENTS.md + tool-specific import> |

**Tool strategy.** One default, a named exception process (ADR), a portable rules core ([03.2](../lessons/module-03/lesson-02.md)), and what happens to seats of a tool nobody uses.

## 3. Enablement (19.3)

### Training paths

| Audience | Format | Minutes | When | Measured by | Owner |
|---|---|---|---|---|---|
| Developers | \<hands-on workshop on their own code + practice blocks> | | \<wave start> | \<pre/post items, two-week follow-up> | |
| Team leads / managers | | | | | |
| Security | | | | | |
| New hires (onboarding) | | | \<week 1 of employment> | | |

Measure at least one path beyond attendance or satisfaction ([16.5](../lessons/module-16/lesson-05.md)).

### Rhythms

| Mechanism | Cadence | Owner | Input | Output |
|---|---|---|---|---|
| Office hours | \<weekly, day, length> | \<employee> | stuck tasks, questions | answers, FAQ entries, feedback items |
| Feedback triage | | | feedback form, channel threads | each item fixed, answered or declined within the target |
| AI-layer review | monthly | | incidents, overrides, changelog ([11.5](../lessons/module-11/lesson-05.md)) | rules retired or promoted to checks |
| Metrics review | monthly | | usage and outcome metrics | actions for teams below their control limit |

A chat channel is a place, not a rhythm: list it, but it does not replace any row above.

## 4. Metrics (19.4)

| Metric | Level | Definition | Target | Source | Aggregation |
|---|---|---|---|---|---|
| Activated | reach | | | | team |
| Weekly active | habit | at least one session in the week | | | team (n ≥ 5) |
| Engaged | habit | sessions on at least 3 days in the week | | | team |
| \<delivery metric, 13.1> | outcome | | no regression vs baseline | | team |

Never aggregate per individual. Volume counts (seats, prompts, tokens, lines) are diagnostics, not targets.

## 5. Anti-regression and handover (19.4)

| Mechanism | Owner | Trigger | Action |
|---|---|---|---|
| Programme ownership handover | \<programme owner> | \<handover date> | handover checklist signed; owner in `governance.md` |
| Champion succession | \<team leads> | champion leaves or changes team | co-champion steps up; new co-champion named within two weeks |
| Onboarding | \<HR partner + champion> | new hire starts | agent setup and onboarding path in the joiner checklist |
| Usage alert | \<programme owner> | team weekly active below its lower control limit | meet champion and lead within a week; check that week's events |
| Tool and licence change control | \<platform owner> | tool, licence or quota change proposed | ADR and telemetry path before the change |
| Follow-ups | \<consultant + programme owner> | 30, 60, 90 days after handover | rerun the usage check and review with the sponsor |

## 6. Cadence summary

A one-line calendar: weekly (office hours, triage), monthly (metrics review, AI-layer review, champions' session), quarterly (sponsor review, survey), plus the dated follow-ups.
