# Illustrative data sets (simulated, not measurements)

Every row in these files was **simulated** by `generate_data.py` from invented parameters, so you can practise the Module 13 analyses without a team and without waiting weeks. They say nothing about any real team, tool, model or agent. **Never quote them as results.** Your own experiment is the only number that counts.

| File | Rows | What is true in the simulation | Built to show |
|---|---|---|---|
| `contoso-selfselected.csv` | 120 tickets, 6 developers, 12 weeks | The agent has **no effect** on cycle time. Developers chose the agent mostly for small tickets (S 74%, M 46%, L 33%). | A naive 42% "speed-up" that disappears when you compare within ticket size (lessons 13.1, 13.3, 13.6) |
| `contoso-randomized.csv` | 96 tickets, 4 developers, 12 weeks | Arms randomized within developer × size. The agent multiplies cycle time by **0.85**. With the agent, PRs are opened later in the ticket's life. Escaped-defect probability is 1.3× higher with the agent. One manual ticket is a 170-hour outlier; three manual tickets used the agent anyway; the agent version changes in week 7. | Clock definitions (13.1), outliers and the log scale, bootstrap, permutation, effect sizes, blocking (13.5), contamination and version checks (13.4), a full report (13.6) |
| `prepost-teams.csv` | 12 teams × 4 quarters | The agent has **no effect**. The pilot team (Forms) was chosen in 2026-Q3 because it had the worst 2026-Q2; that quarter was bad luck. | Regression to the mean, difference-in-differences and placebo checks (13.4) |
| `backlog-example.csv` | 24 refined tickets, 2 developers | — (no outcomes; hand-written) | Blocked random assignment with `ImpactStats assign` (lesson 13.3) |
| `ticket-log-template.csv` | header only | — | The column layout `ImpactStats` expects; copy it for your own experiment |

## Columns (ticket files)

| Column | Meaning | Set when |
|---|---|---|
| `ticket`, `developer`, `week` | identifiers | before work |
| `size`, `story_points` | refinement estimate (S/M/L) | **before** assignment — a valid variable to stratify on |
| `assigned` | arm (`ai` or `manual`); in the self-selected file, what the developer chose | at start |
| `used_ai` | whether the agent was actually used | after work |
| `agent_version` | agent + model version in use | during work |
| `hours_to_pr` | In Progress → PR opened (working hours) | after work |
| `review_hours` | PR opened → merged | after work |
| `cycle_hours` | In Progress → merged (= the two above) | after work |
| `rework_commits` | fix commits on the same files within 21 days of merge | 21 days later |
| `escaped_defects` | bugs traced to the ticket within 30 days of deploy | 30 days later |
| `lines_changed` | additions + deletions | after work — **post-treatment**: never stratify or "control" on it |
| `notes` | free text | any time |

## Regenerating

```bash
py generate_data.py
```

The script tries seeds 1, 2, 3, … for each file and keeps the first one whose data shows the property the lesson needs; it prints the seed and the key numbers. Re-running it reproduces the same files byte for byte. You do not need Python for the labs.
