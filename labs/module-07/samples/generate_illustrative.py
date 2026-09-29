"""Generates the ILLUSTRATIVE result sets in labs/module-07/samples/.

These are simulated trials from invented per-task success probabilities, written so you can
practise `EvalHarness stats`, `compare` and `gate` without an API key. They are not measurements
of any model or agent. You do not need to run this script; it is kept so the data's origin is
transparent. Re-running it reproduces the same files (fixed seeds).  Usage: py generate_illustrative.py
"""
import json, random
from pathlib import Path

HERE = Path(__file__).resolve().parent
TASKS = json.loads((HERE.parent / "tasks-v1" / "tasks.json").read_text(encoding="utf-8"))["tasks"]
META = {t["id"]: (t["split"], ";".join(t["tags"])) for t in TASKS}
HEADER = "config,task,split,tags,trial,pass,reason,cost_usd,input_tokens,duration_ms,agent_version,model,layer_sha,tasks_sha"
TASKS_SHA = "illustrative"


def rows(config, probs, trials, rng, agent="illustrative-agent 1.0", layer="layer-a", cost=0.05, meta=None, only=None):
    out = []
    for k in range(1, trials + 1):
        for tid, p in probs.items():
            if only is not None and tid not in only:
                continue
            split, tags = (meta or META).get(tid, ("dev", "representative"))
            ok = rng.random() < p
            c = round(cost * rng.uniform(0.8, 1.2), 4)
            out.append(f"{config},{tid},{split},{tags},{k},{1 if ok else 0},{'' if ok else 'illustrative failure'},"
                       f"{c:.4f},{int(c * 700000)},{int(rng.uniform(15000, 60000))},{agent},(agent default),{layer},{TASKS_SHA}")
    return out


def write(path, lines):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("\n".join([HEADER] + lines) + "\n", encoding="utf-8", newline="\n")


# A. Skill v1 vs v2 on 20 test tickets. Same true success probability per ticket in both versions:
#    there is NO real difference. Seed chosen so that trial 1 alone reads 12/20 vs 14/20.
skill_p = {f"S{i:02d}": p for i, p in enumerate(
    [0.95, 0.9, 0.9, 0.85, 0.8, 0.8, 0.75, 0.7, 0.7, 0.65, 0.6, 0.6, 0.55, 0.5, 0.45, 0.4, 0.35, 0.3, 0.2, 0.1], 1)}
skill_meta = {t: ("dev", "representative") for t in skill_p}
for seed in range(1, 5000):
    rng = random.Random(seed)
    v1 = rows("skill-v1", skill_p, 5, rng, layer="skill-v1", meta=skill_meta)
    v2 = rows("skill-v2", skill_p, 5, rng, layer="skill-v2", meta=skill_meta)
    once1 = [r for r in v1 if r.split(",")[4] == "1"]
    once2 = [r for r in v2 if r.split(",")[4] == "1"]
    s1 = sum(r.split(",")[5] == "1" for r in once1)
    s2 = sum(r.split(",")[5] == "1" for r in once2)
    t1 = sum(r.split(",")[5] == "1" for r in v1)
    t2 = sum(r.split(",")[5] == "1" for r in v2)
    if s1 == 12 and s2 == 14 and abs(t1 - t2) <= 3:
        once = [r.replace("skill-v1,", "skill-v1-once,", 1) for r in once1] + [r.replace("skill-v2,", "skill-v2-once,", 1) for r in once2]
        write(HERE / "skill-compare" / "results.csv", once + v1 + v2)
        print(f"skill-compare: seed {seed}, once {s1}/20 vs {s2}/20, five trials {t1}/100 vs {t2}/100")
        break

# B. Context reduction (Module 4) re-scored: bloated vs reduced layer, 24 tasks x 5 trials.
bloated = dict(T01=0.6, T02=0.5, T03=0.8, T04=0.3, T05=0.9, T06=0.4, T07=0.9, T08=0.8, T09=0.7, T10=0.6, T11=0.7, T12=0.8,
               T13=0.5, T14=0.9, T15=0.95, T16=0.7, T17=0.6, T18=0.6, T19=0.5, T20=0.5, T21=0.5, T22=0.7, T23=0.9, T24=0.6)
reduced = dict(T01=0.95, T02=0.8, T03=0.9, T04=0.9, T05=0.9, T06=0.9, T07=0.9, T08=0.8, T09=0.6, T10=0.9, T11=0.9, T12=0.8,
               T13=0.9, T14=0.9, T15=0.95, T16=0.8, T17=0.9, T18=0.7, T19=0.6, T20=0.55, T21=0.7, T22=0.75, T23=0.9, T24=0.7)
rng = random.Random(2026)
b = rows("bloated", bloated, 5, rng, layer="9f1c0a77e2b4", cost=0.063)
r = rows("reduced", reduced, 5, rng, layer="41d7be0c93aa", cost=0.051)
aa = rows("bloated-aa", bloated, 5, rng, layer="9f1c0a77e2b4", cost=0.063)
# The drift arm: run a week later after the agent CLI auto-updated, and the three code tasks
# (which time out more often) were dropped "to save time".
drift_p = {k: min(0.97, v + 0.05) for k, v in reduced.items()}
d = rows("reduced-later", drift_p, 5, rng, agent="illustrative-agent 1.1", layer="41d7be0c93aa", cost=0.047,
         only=[k for k in reduced if k not in ("T18", "T19", "T20")])
write(HERE / "context-reduction" / "results.csv", b + r + aa + d)
print("context-reduction written")

# C. Gate: layer v1.4 adds a topology map (helps many tasks) but its author deleted the
#    "never edit a merged V###" line. Aggregate rises; golden T06 collapses.
v13 = dict(reduced)
v14 = {k: min(0.98, v + 0.2) for k, v in reduced.items()}
v14["T06"] = 0.0
for seed in range(1, 5000):
    rng = random.Random(seed)
    a = rows("layer-v1.3", v13, 5, rng, layer="41d7be0c93aa", cost=0.051)
    c = rows("layer-v1.4", v14, 5, rng, layer="c07e55a1b9d2", cost=0.053)
    t06a = sum(x.split(",")[5] == "1" for x in a if x.split(",")[1] == "T06")
    if t06a == 5:
        write(HERE / "gate" / "results.csv", a + c)
        print(f"gate: seed {seed}")
        break

# D. Pseudo-replication: 100 trials that are really 4 tasks x 25 trials. Fixed counts, no randomness.
counts = {"T01": 25, "T03": 25, "T07": 24, "T12": 18}
lines = []
for tid, c in counts.items():
    split, tags = META[tid]
    for k in range(1, 26):
        ok = k <= c
        lines.append(f"reduced-4x25,{tid},{split},{tags},{k},{1 if ok else 0},{'' if ok else 'illustrative failure'},"
                     f"0.0500,35000,30000,illustrative-agent 1.0,(agent default),41d7be0c93aa,{TASKS_SHA}")
write(HERE / "pseudo-replication" / "results.csv", lines)
print("pseudo-replication written")
