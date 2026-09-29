# Illustrative data (simulated, not measurements)

Every row and trace in `multi-vs-single/` was **simulated** by `generate_illustrative.py` from invented per-task success probabilities, per-call costs and pipeline behaviour (reviewer recall and false-alarm rate, damage done by "fixing" a correct answer). They let you practise `EvalHarness compare`, `AgentTeam efficiency` and `AgentTeam trace` without an API key. They say nothing about any real model, agent or vendor pipeline. **Never quote them as results.** Your own runs are the only numbers that count.

| File | Contents | Built to show |
|---|---|---|
| `multi-vs-single/results.csv` | tasks-v1 × 5 trials for `single`, `single-aa`, `solo-verify`, `pwr`, `route` | a pipeline that doubles cost and latency for no detectable gain; a quiet A/A; verification inside one agent (lesson 10.4) |
| `multi-vs-single/code-only.csv` | the T18–T20 rows of `single`, `solo-verify` and `pwr` | the vendor slide: "+33 points" on three tasks with a per-trial interval (10.4 break) |
| `multi-vs-single/traces-pwr/` | 120 `pwr` traces in AgentTeam's span format | cost and time share per role, rounds, stop reasons, where failures came from (lesson 10.3) |

The single-agent success probabilities are the Module 7 illustrative "reduced" layer, so these numbers continue that story. The generator searches for a seed whose sample matches the lessons (the probabilities themselves are fixed and visible in the script); `py generate_illustrative.py` reproduces the same files. You do not need Python for the labs.
