# Illustrative result sets (simulated, not measurements)

Every row in these files was **simulated** by `generate_illustrative.py` from invented per-task success probabilities, so you can practise `stats`, `compare` and `gate` without an API key. They say nothing about any real model or agent. **Never quote them as results.** Your own runs are the only numbers that count.

| File | Configs | Built to show |
|---|---|---|
| `skill-compare/results.csv` | `skill-v1-once`, `skill-v2-once` (trial 1 only), `skill-v1`, `skill-v2` (5 trials) on 20 test tickets S01–S20 | the true success probabilities are **identical** in both versions; one run reads 12/20 vs 14/20 (lessons 07.1, 07.4) |
| `context-reduction/results.csv` | `bloated`, `reduced`, `bloated-aa` (a second run of `bloated`), `reduced-later` (newer agent, code tasks dropped) on tasks-v1 × 5 | a real improvement, an A/A check, and a confounded arm (lesson 07.5) |
| `pseudo-replication/results.csv` | `reduced-4x25`: 4 tasks × 25 trials, fixed counts | 100 trials that are not 100 independent observations (lesson 07.4) |
| `gate/results.csv` | `layer-v1.3`, `layer-v1.4` on tasks-v1 × 5 | the mean rises while golden T06 collapses (lesson 07.6) |

The generator uses fixed seeds; `py generate_illustrative.py` reproduces the same files. You do not need Python for the labs.
