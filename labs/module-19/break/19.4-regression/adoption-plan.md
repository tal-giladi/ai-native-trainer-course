# Fabrikam Group — rollout plan v0 (metrics and handover sections)

*Deliberately flawed draft for lesson 19.4: the plan the first rollout actually ran on. Its usage data is `usage.csv` and `events.csv` in this folder. Hypothetical company; people and numbers are illustrative.*

## 0. Facts

- Organisation: Fabrikam Group, coding agents for all developers
- Developers: 400
- Countries: DE, NL, IL
- Consultant: Sam Carter (external)

## 4. Metrics

| Metric | Level | Definition | Target | Source | Aggregation |
|---|---|---|---|---|---|
| Seats assigned | reach | licences assigned in the vendor portal | 400 by Q4 | vendor portal | organisation |
| Prompts per developer | habit | prompts sent per developer per week | 50 | gateway | individual, leaderboard in the channel |
| Lines of code accepted | outcome | lines written by the agent and accepted | +20% per quarter | vendor dashboard | team |

## 5. Anti-regression and handover

| Mechanism | Owner | Trigger | Action |
|---|---|---|---|
| Consultant check-in | Sam Carter (consultant) | monthly | e-mail the CTO office a status update |
