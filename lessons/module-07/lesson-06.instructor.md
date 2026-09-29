# Instructor notes — 07.6 Evals in the loop

**Teaching objective.** Students run evals automatically on AI-layer changes, gate merges with a policy that combines comparability, per-task golden floors, a paired aggregate bound and a cost ceiling, turn incidents into regression tasks in the right order, and run the pipeline without exposing secrets to untrusted code.

**Likely confusion.** Baseline vs previous run. The baseline is a committed, pinned, full-suite result; the gate compares the candidate against it, not against "whatever ran last". When the agent version changes, the baseline must be regenerated in the same PR.

**Common misconception.** "If the average went up, nothing got worse." Show the gate sample: +7.5 points and T06 gone. Averages are votes; a golden task is a veto.

**Key analogy.** A building inspection. The inspector does not average the building: great plumbing does not compensate for a missing fire exit. Golden tasks are the fire exits.

**Common failure in the exercise.** The first CI run fails "not comparable" because the baseline came from a laptop with another Claude Code version. Let it happen; it teaches the manifest better than any slide. Second: students run the eval workflow on `pull_request_target` to get secrets for forks — stop this explicitly; it hands untrusted code your API key.

**Expected exercise outcome.** A committed baseline, a passing local gate on a harmless change, a CI job with stats and gate in the job summary, one incident turned into a regression task with fail-before and pass-after evidence and a changelog entry, and the gate policy in `CHARTER.md`. Typical smoke cost on Contoso: a few dollars per PR at the time of writing; students should report their own figure.

**Extension exercise.** Estimate the gate's false-alarm rate: run the same configuration as candidate and baseline ten times (3 trials each on the smoke set) and count gate failures. Then compute how many trials per task would bring it under 5%.

**Discussion question.** Should an engineer ever be allowed to override the eval gate to merge? If yes, what must the override record, and who reviews it afterwards?
