# Module 10 breaks

One deliberate failure per lesson. All four run offline (scripted agent or illustrative data); the golden tests that decide the code outcomes are the real Module 7 ones.

| Lesson | Break | How to run it | What you should see |
|---|---|---|---|
| 10.1 | Parallel specialists on a coupled task: the tester and the coder each decide a name the plan left open | `run … --topology specialists --only T19 --agent fake:scenarios/specialists-t19.json` | `build failed: error CS0246: … 'CollectionSummary' could not be found` |
| 10.2 | Three parallel workers edit `InvoiceService.cs`; last writer wins | `parallel … --only T18,T19,T20 --merge naive --agent fake:scenarios/clobber.json` | 1/3 golden tasks correct while the merged branch's own suite is GREEN |
| 10.3 | Reviewer rule R7 contradicts the task; planner and reviewer never agree | `run … --topology pwr --only T18 --max-rounds 0 --budget-usd 1.00 --on-stall continue --agent fake:scenarios/t18-deadlock.json` (reviewer text: `10.3-deadlock/reviewer.md`) | budget stop after 6 rounds and 19 calls, $1.01; a correct fix graded as a failure |
| 10.4 | A vendor slide: pipeline vs a single agent without verification, 3 tasks, per-trial interval | read `10.4-vendor-claim.md`, then `EvalHarness compare samples/multi-vs-single/code-only.csv …` | the "significant" +33 points vanishes against a compute-matched single agent |

With a real agent (costs money), the 10.1 and 10.3 breaks reproduce by copying the role file into a throwaway roles folder and passing `--roles <folder> --agent claude`. Whether a real model repeats the exact failure varies from run to run; the scripted runs are deterministic so the diagnosis can be taught.
