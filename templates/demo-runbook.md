# Demo run sheet template (`brownfield-demo/demo/runsheet.md`)

One page you can read at a glance while presenting. Introduced in [15.4](../lessons/module-15/lesson-04.md); the design choices behind it come from [15.1](../lessons/module-15/lesson-01.md) (which ticket) and [15.2](../lessons/module-15/lesson-02.md) (which before/after). The table format is what `DemoCheck runsheet` and `DemoCheck drill` (in `labs/module-15/tools/DemoCheck`) read, so keep the column names.

Rules:

- **Every live segment has a fallback** you have opened at least once: a recording with a timestamp (`rec:recordings/<file>@mm:ss`), a pre-baked branch or tag (`branch:<name>`, `tag:<name>`), or a file on that branch.
- **Every live segment has a line to say** when it breaks, written in advance, in your own words, one or two sentences. It names what happened and what the room will see next.
- **Rehearsals are counted**, not remembered: `k/n` clean runs of that segment. Three is the minimum; ten gives an interval worth reading.
- **Budgets add up** to at most 90% of the slot. The last 10% is for the failure you did not plan for.
- A segment that fails in more than one rehearsal in five is either pre-recorded, pre-baked or cut. It is not "more exciting live".

## Header

```markdown
# Run sheet — <ticket> live demo

Slot: <NN> min
Ticket: <id> · Repo: <demo repo> at <after tag> · Fallback branch: <branch>
```

## Before you start (T−30 min)

- [ ] Worktrees for before and after are clean (`git status`), at the right tags.
- [ ] Everything that is built is built (hooks, MCP servers); `dotnet test` passes with the expected count.
- [ ] Time-dependent pieces checked: snapshots, tokens, certificates, anything with an expiry.
- [ ] Fallback recordings open and paused at their timestamps; fallback branch fetched.
- [ ] Notifications off; font size readable from the back row; secrets not in any visible terminal or tab.
- [ ] The reset command is in a second terminal.

## Segments

| # | Segment | Mode | Budget | Rehearsals | Fallback | Recover | If it breaks, say |
|---|---|---|---|---|---|---|---|
| 1 | what the room sees | talk / recorded / live | minutes | k/n | `rec:…@mm:ss` / `branch:…` / `tag:…` | minutes to switch | "your sentence" |

`Mode`: `talk` (you and slides), `recorded` (a clip you play), `live` (the agent runs now).

## Reset between rehearsals

```bash
git -C <after worktree> reset --hard <after tag> && git -C <after worktree> clean -fdx -e <built folders>
```

## Rehearsal log

| Date | Clean? | What broke | Recovered in | Change made |
|---|---|---|---|---|

## Recovery pattern (read it once before every run)

1. **Name it** — say what happened in one sentence, without apologizing twice.
2. **Decide** — wait (up to the segment's recover budget), retry once, or switch to the fallback. Decide by the clock, not by hope.
3. **Teach it** — say what this failure shows about agents in real work; it is often the most useful minute of the talk.
4. **Log it** — after the talk, add the failure to the rehearsal log and change the run sheet.
