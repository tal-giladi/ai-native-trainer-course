# Engagement record — C-01 (break 21.4: the calendar handover)

> **Deliberately flawed.** The engagement ended on the contract date. The results were written for the read-out the day before, and the "handover" left the recurring work with the consultant. `usage.csv` and `events.csv` in this folder show the 24 weeks that followed. Run `EngageCheck handover` on this file and `AdoptCheck usage` on the data.

- Client code: C-01
- Consultant: Student (S)
- Handover date: 2026-12-23

## 4. Enable, measure, hand over

### Owners

| Responsibility | Owner | Backup | Ran without consultant |
|---|---|---|---|
| AI layer: rules, skills, changelog | Avi Ben-David | | |
| Office hours and questions channel | S, as needed | | |
| Metrics and usage review | S, monthly e-mail | | |

### Results

| Outcome | Baseline | Result | Interval | Source | Status |
|---|---|---|---|---|---|
| Cycle time, ai vs manual (pooled medians) | median 18.5 h | -23% | | pilot-tickets.csv | improved |
| Escaped defects, ai vs manual | 10% | 0 of 12 vs 2 of 15 (-13 pts) | [-27, 0] pts | ImpactStats compare --binary | improved |
| Eval pass rate, layer v1 vs before | 53% | 75% (+21.7 pts) | [+10.6, +32.8] pts | EvalHarness compare | improved |

### Follow-ups

| Follow-up | Date | Checks | Owner | Result |
|---|---|---|---|---|
| On request | - | Anything | S | |
