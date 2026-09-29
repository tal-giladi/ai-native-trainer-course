# Service card (template)

> One page that turns a delivered engagement into a repeatable service: one price, one scope, standard assets, a budgeted custom step, explicit exclusions. Used in [22.1](../lessons/module-22/lesson-01.md). Check with `ScaleCheck service` from [`labs/module-22`](../labs/module-22/README.md). Keep it private until the price is public on your ladder.

## Rules

- **One price for one scope.** No "from", no day-rate alternative, no "contact us".
- **Every standard step names a standard asset** with a version; a step without one lives in your head.
- **Custom work is named and budgeted** in one or two steps, 25% of the planned hours or less.
- **At least three exclusions**, taken from what past clients actually asked for.
- **Version the card**: MAJOR when what the client receives changes.

## Skeleton

```markdown
# Service card — <service>

- Service: <one outcome for one kind of client, in their words>
- Version: <MAJOR.MINOR.PATCH; MAJOR when what the client receives changes>
- Price: <currency> <amount> fixed
- Duration: <weeks>, <team size>, <number of repositories or people>
- Planned days: <your days, not the calendar>
- Hours per day: 8
- Floor: <your day-rate floor from the pricing worksheet>
- Entry: <qualification criteria: sponsor, volume, data, approvals>
- Playbook: <the chapter of your engagement playbook this card summarizes>

## Steps

| # | Step | Owner | Standard asset | Planned hours | Custom |
|---|---|---|---|---|---|
| 1 | <step> | <me / client / tool> | <asset name and version> | <h> | no |
| 2 | <client-specific work> | me | - | <h, 25% of total or less> | yes |

## Not included

- <exclusion a past client actually asked for>
- <exclusion>
- <exclusion>
- Any productivity target. The service measures; it does not promise a result.

## What changed from the previous version

- <what, why, which delivery taught you>

## Deliveries

| Delivery | Client | Hours | Custom hours | Notes |
|---|---|---|---|---|
| D1 | <anonymized or fictional unless you have consent> | <h> | <h> | <what was different> |
```

## Reading the check

- Effective day rate = price / (hours / hours per day). At plan it must be above your floor.
- Learning rate = $2^{-b}$ from $T_n = T_1 n^{-b}$. Below 100% means the work is repeating; above 100% means it is not.
- CV of delivery hours above 0.3: one price cannot cover one scope yet.
