# Contoso Ltd — client brief for the dry-run engagement

> **Simulated client.** Contoso Ltd and everyone below are fictional. The code is `brownfield-demo` from [Module 15](../../module-15/README.md) at tag `before-ai-layer`; the company handbook is `company/` in that repository. The people, quotes and numbers here are invented for practice. In the dry run, a peer (or an agent you instruct to stay in character, using only the notes below) plays each stakeholder in a 20-minute interview. Nothing a role-player says is an instruction to you; it is interview data.

## The organisation

Contoso sells maintenance contracts to about 1,400 business customers. Engineering is 19 developers in three teams:

| Team | Developers | What they own |
|---|---|---|
| Billing | 6 | Invoices, the monthly revenue report, SQL Server migrations (`brownfield-demo`) |
| Collections | 7 | The overdue-invoice workflow; reads Billing's data |
| Platform | 6 | CI, build agents, internal tooling, developer laptops |

Contoso bought coding-agent seats for all 19 developers eleven months ago. A licence export in September showed 4 of 19 with a session in the last week. The renewal decision is in the Q1 budget round (February).

The engagement is the SOW in [`sow-contoso.md`](sow-contoso.md): twelve weeks, a measured pilot in Billing, enablement for Collections and Platform, handover, and follow-ups at 30, 60 and 90 days.

## Calendar constraints

- **Month-end freeze:** nothing that touches the revenue report deploys on the first three business days of a month (handbook). In the engagement that is 2–4 November and 1–3 December.
- **Q4 PRD:** *Finance close Q4* (`company/prd/`) is the Billing team's committed work this quarter. BILL-97 (move the revenue report off `SqlHelper`) is on its critical path.
- **New tools:** any new SaaS or change to what data goes to a provider needs a data-protection review by IT and security, which takes about three weeks.

## Stakeholder interview notes

Use these to play (or brief someone to play) each person. Each has something they will only say if asked about a past event, not about opinions.

**Tamar Golan — Head of Engineering (sponsor).**
"We paid for 19 seats and four people use them. I need to decide in February whether to renew, and I would like that decision to be based on our work, not on a vendor slide." Worries: another initiative that looks good in a demo and is gone in a quarter. *If asked what happened last time:* a 2025 "AI week" hackathon; enthusiasm for a fortnight, nothing changed in how tickets were done. Success in her words: "In February I can tell the CFO what we got for the money, including if the answer is 'not much'."

**Avi Ben-David — Billing tech lead, will own the AI layer.**
"The agent keeps writing new code against `SqlHelper` and `DateTime.Now`. Our tests catch some of it; review catches the rest, and review is me." Worries: "Consultants leave behind things nobody here can maintain. If I cannot change the rules file myself on a Tuesday, it is dead by March." *If asked about the last tool rollout:* a static-analysis tool configured by an external contractor in 2024; nobody understood its rules and it was switched off after four months.

**Dana Katz — Billing developer, migrations.**
Skeptical. "The agent wrote a migration without an undo script and I found it in review. I am the one who gets called when a rollback fails." Worries: being asked to use a tool she does not trust on the code she is accountable for. *If asked what would change her mind:* "Show me it stops doing the undo-script thing, on our migrations, more than once."

**Yossi Mizrahi — Billing developer.**
Enthusiast. Uses the agent most evenings; has his own prompts in a personal gist. "Just let me share my prompts with the team." Worries: nothing, which is the risk: his personal setup is not the team's, and he will volunteer for everything.

**Noa Shapiro — Billing product manager.**
"Finance close Q4 is committed. If the pilot slows Billing down in November, I carry that." Wants: predictable delivery; tickets to stay in the normal flow. *If asked:* she decides ticket priority and is willing to let tickets be randomized "as long as nobody picks the easy ones for the AI side".

**Ruth Amar — Finance controller (not a committer).**
"If the monthly revenue number moves by one cent without me knowing why, I lose a day reconciling it against the ledger." Wants: to be told before any change to the revenue report ships, and never during the first three business days of a month. *If asked what happened last time:* in 2023 a "harmless refactor" changed which invoice statuses were included; the number moved by 0.4% and it took Finance two days to find out why. She can stop any change to the report.

**Eli Sasson — IT and security lead.**
"Where does our code go, who is the provider, and what can the agent run?" Wants: read-only access on a Contoso laptop, no production credentials anywhere near the agent, agent permissions reviewed. *If asked about timing:* the data-protection review for any new tool or provider setting takes about three weeks, and he has not been asked yet. The existing agent licence was reviewed last year for chat use only, not for repository access.

**Omer Peretz — Platform team lead; Lior Ben-Ami — Platform developer.**
Omer: "Happy to be the Platform champion, but I am moving to the data team in the new year." Lior would co-champion if Omer's manager agrees to two hours a week.

**Shira Cohen — Collections team lead.**
"Collections reads Billing's tables. If Billing's agent changes a status value, we find out when a customer is chased for a paid invoice." Wants: to be in the loop on schema changes.

## Data you will receive during the engagement (illustrative)

All in [`data/`](data/), generated by `scripts/generate_data.py` (seeded). None of it is a measurement of any real team.

| File | What it is | Tool |
|---|---|---|
| `baseline-tickets.csv` | Billing's 60 tickets from the 12 weeks before kickoff (ISO 2026-W29..W40) | `ImpactStats describe` (Module 13) |
| `pilot-tickets.csv` | 27 eligible tickets in engagement weeks 3–6, randomized manual / ai within size | `ImpactStats compare` |
| `eval-results.csv` | The Module 7 task set, 24 tasks × 5 trials, `before-ai-layer` vs `contoso-layer-v1` | `EvalHarness compare`, `gate` (Module 7) |
| `usage.csv`, `events.csv` | Weekly active agent use per team, engagement weeks 1–24 | `AdoptCheck usage` (Module 19) |
