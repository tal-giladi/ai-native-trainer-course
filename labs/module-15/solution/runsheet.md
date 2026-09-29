# Run sheet — BILL-97 live demo

> Reference (illustrative). One student's run sheet for the 50-minute demo block of a lunch-and-learn.
> Rehearsal counts are theirs; yours come from your own rehearsals.

Slot: 50 min
Ticket: BILL-97 · Repo: brownfield-demo at `ai-layer-v1` · Fallback branch: `demo/BILL-97-done`

## Before you start (T−30 min)

- Two worktrees: `../demo-before` at `before-ai-layer`, `../demo-after` at `ai-layer-v1`; both `git status` clean.
- `dotnet build tools/AgentHooks -c Release -o .claude/hooks/bin` and the schema server published; `dotnet test` → 6 passed.
- Refresh the schema snapshot or accept the `--on-stale warn` banner (it is older than 24 hours by demo day).
- Recordings open in a paused player: `recordings/before-BILL-97.mp4`, `recordings/run-01-unedited.mp4`.
- Notifications off, font 18 pt, terminal 100 columns, a second terminal with the reset command ready.

## Segments

| # | Segment | Mode | Budget | Rehearsals | Fallback | Recover | If it breaks, say |
|---|---|---|---|---|---|---|---|
| 1 | Contoso, BILL-97 and the PRD non-goal | talk | 4 | - | - | - | - |
| 2 | Before: same ticket, no AI layer (90 s clip) | recorded | 3 | - | rec:recordings/before-BILL-97.mp4 | - | - |
| 3 | `/prime BILL-97` and the two open questions | live | 5 | 9/10 | rec:recordings/run-01-unedited.mp4@02:00 | 2 | "The model API is having a bad minute. This is exactly why I recorded Tuesday's run: same ticket, same layer. Watch the brief." |
| 4 | `/plan-feature` and the HUMAN checkpoint | live | 6 | 10/10 | `plans/BILL-97.md` from branch:demo/BILL-97-done | 1 | "Let me show you the plan it wrote in rehearsal; the point is the Do-not-touch list, not the typing." |
| 5 | Implement: tests first, then the repository | live | 8 | 8/10 | branch:demo/BILL-97-done | 2 | "This is the step that fails about one run in five, and that is worth seeing. Here is what the finished change looks like." |
| 6 | `/validate` and `dotnet test` | live | 4 | 10/10 | branch:demo/BILL-97-done | 1 | "The checks are the part I trust; let me run them on the finished branch." |
| 7 | `/pr-review` and the draft PR | live | 4 | 9/10 | rec:recordings/run-01-unedited.mp4@14:20 | 2 | "Review is slow today; here is the review from the recorded run." |
| 8 | What the layer did, what it did not; questions | talk | 8 | - | - | - | - |

## Reset between rehearsals (and before the room)

```bash
git -C ../demo-after reset --hard ai-layer-v1 && git -C ../demo-after clean -fdx -e .claude/hooks/bin -e .claude/mcp
```

## Rehearsal log

| Date | Clean? | What broke | Recovered in |
|---|---|---|---|
| 2026-09-29 | yes | — | — |
| 2026-09-30 | no | #5: test file in the wrong namespace, build red | 1:40 (fallback branch) |
| 2026-10-01 | no | #3: overload error, retried | 0:35 (waited, narrated) |
