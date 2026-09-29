# Run sheet — BILL-97 live demo (first draft)

Slot: 45 min

| # | Segment | Mode | Budget | Rehearsals | Fallback | Recover | If it breaks, say |
|---|---|---|---|---|---|---|---|
| 1 | Contoso, BILL-97 and the PRD non-goal | talk | 4 | - | - | - | - |
| 2 | Before: same ticket, no AI layer | live | 3 | 2/2 | - | - | - |
| 3 | `/prime BILL-97` | live | 5 | 9/10 | - | 2 | "Give it a second." |
| 4 | `/plan-feature` | live | 6 | 10/10 | branch:demo/BILL-97-done | 1 | "Here is the plan from rehearsal." |
| 5 | Implement the whole ticket in one prompt | live | 10 | 2/2 | - | - | - |
| 6 | Live schema check against SQL Server in Docker | live | 5 | 1/3 | restart docker | 3 | - |
| 7 | `/validate`, `dotnet test`, `/pr-review` | live | 8 | 9/10 | branch:demo/BILL-97-done | 2 | "The checks are what I trust." |
| 8 | Questions | talk | 8 | - | - | - | - |
