# Research brief template

Use before planning any change that touches more than one file in a brownfield repository. The brief is written by a read-only research pass (a human, an agent in plan mode, or a read-only sub-agent) and is short: 40–80 lines, around 1,000 tokens. It is the input to the plan, not a narrative of the exploration. Introduced in [05.1 · Research discipline](../lessons/module-05/lesson-01.md).

```markdown
# Research brief — <ticket id> <ticket title>

Researcher: <who / which agent, read-only?> · time spent: <min> · explored: <~tokens> · brief: <~tokens>
Rules version: <AI-layer version> · Date: <yyyy-mm-dd>

## Question

<One or two sentences: what must be true about the codebase before we can plan this ticket?>

## Existing capabilities to reuse

| Need in the ticket | Already exists | Evidence (path) |
|---|---|---|
| <criterion or noun from the ticket> | <symbol, or "nothing (searched: terms)"> | `<path>` |

## Architecture facts that constrain the design

| Fact | Evidence (strongest first) | Rung (test / code / ADR / history / docs / people) |
|---|---|---|
| | `<path>` | |

## Dependencies and blast radius

- Callers / consumers of what will change: <list, with how they were found>
- Schema, config, contracts touched: <list>
- Out-of-scope areas that look related: <path — owner / ticket>

## Open questions (for the plan checkpoint)

1. <Question> — proposed assumption: <…>

## What I did not look at

<Anything not verified: no database, no runtime, no access to X.>
```

## Checklist before handing the brief to planning

- [ ] Every ticket noun was searched for in code (not just in docs), and the search terms are written down.
- [ ] Every "we already have X" row names a symbol and a path.
- [ ] Every architecture fact cites evidence higher on the ladder than a wiki or a stale doc; ADRs in `docs/adr/` were read.
- [ ] Callers of anything that will change were found by search, not guessed.
- [ ] Open questions are listed rather than silently answered.
- [ ] The brief fits on one screen. Exploration output stayed out of it.
