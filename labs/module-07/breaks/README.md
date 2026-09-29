# Module 7 breaks

Each lesson's "Break it" and "Fix it" sections walk through the diagnosis. Apply file changes to a **working copy** only, never to a shared repository.

| Lesson | Break | What you use | What gives it away |
|---|---|---|---|
| 07.1 | A one-run demo is taken as proof that skill v2 is better | `samples/skill-compare/results.csv`, configs `skill-v1-once`, `skill-v2-once` | the five-trial configs `skill-v1`, `skill-v2` |
| 07.2 | Task answers leak into the rules file | append `eval-hints-section.md` to `AGENTS.md` in your working copy | dev pass rate jumps, holdout does not; `EvalHarness leak` |
| 07.3 | The LLM judge rewards verbosity | `tasks-v1/graders/judge-v1.md`, `calibration/` | `calibrate`: precision 0.54, kappa 0.08, length bias |
| 07.4 | 100 trials reported as 100 independent observations (4 tasks × 25) | `samples/pseudo-replication/results.csv` | `stats`: task-clustered interval 2.5× wider than the naive one |
| 07.5 | Arms differ in more than the treatment | `samples/context-reduction/results.csv`, config `reduced-later` | `compare` warnings: agent version, missing tasks |
| 07.6 | The gate checks only the aggregate | `samples/gate/results.csv`, `gate --aggregate-only` | a golden task went 5/5 → 0/5 while the mean rose |

Undo the 07.2 break by restoring `AGENTS.md` (`git checkout -- AGENTS.md`, or copy the reference layer again).
