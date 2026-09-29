"""Generates the ILLUSTRATIVE multi-vs-single data in labs/module-10/samples/.

Simulated trials from invented per-task probabilities and per-call costs, so you can practise
`EvalHarness compare`, `AgentTeam efficiency` and `AgentTeam trace` without an API key. They are not
measurements of any model, agent or pipeline. Never quote them as results. Re-running reproduces the
same files (fixed seed).  Usage: py generate_illustrative.py

Configurations (tasks-v1, 24 tasks x 5 trials each):
  single       one headless agent, the Module 7 reduced layer (EvalHarness run)
  single-aa    the same configuration run again (A/A check)
  solo-verify  one agent told to plan, test and re-check its own work (AgentTeam --topology single, roles/solo.md)
  pwr          planner -> worker -> reviewer, max 2 rounds (AgentTeam --topology pwr); traces written too
  route        code tasks through pwr, question tasks through solo-verify (AgentTeam --topology route)
"""
import json, random
from pathlib import Path

HERE = Path(__file__).resolve().parent
OUT = HERE / "multi-vs-single"
TASKS = json.loads((HERE.parent.parent / "module-07" / "tasks-v1" / "tasks.json").read_text(encoding="utf-8"))["tasks"]
META = {t["id"]: (t["split"], ";".join(t["tags"]), t["kind"]) for t in TASKS}
HEADER = "config,task,split,tags,trial,pass,reason,cost_usd,input_tokens,duration_ms,agent_version,model,layer_sha,tasks_sha"
AGENT, LAYER, TSHA = "illustrative-agent 1.0", "41d7be0c93aa", "illustrative"

# Single-agent success per task: the Module 7 illustrative "reduced" layer.
SINGLE = dict(T01=0.95, T02=0.8, T03=0.9, T04=0.9, T05=0.9, T06=0.9, T07=0.9, T08=0.8, T09=0.6, T10=0.9, T11=0.9, T12=0.8,
              T13=0.9, T14=0.9, T15=0.95, T16=0.8, T17=0.9, T18=0.7, T19=0.6, T20=0.55, T21=0.7, T22=0.75, T23=0.9, T24=0.7)
# Verification inside one agent mostly helps the code tasks (it runs the tests it wrote).
SOLO = dict(SINGLE, T18=0.9, T19=0.8, T20=0.75, T21=0.72, T22=0.77)

# Per-call cost (USD) and seconds by kind; each draw gets +-20% noise.
COST = {"qa": dict(single=0.050, solo=0.056, planner=0.030, worker=0.040, reviewer=0.030, replan=0.028, rework=0.030),
        "code": dict(single=0.250, solo=0.330, planner=0.055, worker=0.230, reviewer=0.065, replan=0.050, rework=0.120)}
SECS = {"qa": dict(single=24, solo=30, planner=20, worker=19, reviewer=15, replan=16, rework=15),
        "code": dict(single=170, solo=230, planner=42, worker=150, reviewer=38, replan=30, rework=75)}
# Pipeline behaviour. On question tasks the reviewer rarely catches a wrong fact it cannot check any better than the
# worker (low recall, same model), and "fixing" a terse correct answer after a false alarm often breaks it.
PIPE = {"qa": dict(plan_ok=0.97, recall=0.35, false_alarm=0.12, p_fix=0.45, damage=0.35),
        "code": dict(plan_ok=0.95, recall=0.70, false_alarm=0.15, p_fix=0.65, damage=0.05)}
MAX_ROUNDS = 2


def write(path, lines):
    path.write_text("\n".join(lines) + "\n", encoding="utf-8", newline="\n")


def noisy(rng, x):
    return x * rng.uniform(0.8, 1.2)


def row(config, tid, k, ok, cost, secs, reason="illustrative failure"):
    split, tags, _ = META[tid]
    return (f"{config},{tid},{split},{tags},{k},{1 if ok else 0},{'' if ok else reason},{cost:.4f},{int(cost * 700000)},"
            f"{int(secs * 1000)},{AGENT},(agent default),{LAYER},{TSHA}")


def one_agent(config, probs, key, rng):
    out = []
    for k in range(1, 6):
        for tid, p in probs.items():
            kind = META[tid][2]
            out.append(row(config, tid, k, rng.random() < p, noisy(rng, COST[kind][key]), noisy(rng, SECS[kind][key])))
    return out


def pipeline_trial(tid, k, rng, spans):
    """Simulates planner -> worker -> reviewer (max MAX_ROUNDS); appends spans; returns (ok, cost, secs, stop)."""
    kind = META[tid][2]
    c, s, pp = COST[kind], SECS[kind], PIPE[kind]
    t, cost = 0.0, 0.0

    def call(role, rnd, key, **extra):
        nonlocal t, cost
        cc, ss = noisy(rng, c[key]), noisy(rng, s[key])
        spans.append(dict(trace_id=f"{tid}.r{k}", span_id=f"s{len(spans) + 1}", parent_id="s0", name=f"invoke_agent {role}", role=role,
                          round=rnd, start_ms=int(t * 1000), duration_ms=int(ss * 1000), cost_usd=round(cc, 4),
                          input_tokens=int(cc * 700000), output_tokens=int(cc * 20000), status="ok", **extra))
        t += ss
        cost += cc

    plan_ok = rng.random() < pp["plan_ok"]
    call("planner", 1, "planner")
    p_worker = SINGLE[tid] + (0.05 if kind == "code" else 0.0)
    correct = rng.random() < (p_worker if plan_ok else p_worker * 0.4)
    call("worker", 1, "worker")
    stop = "approved"
    for rnd in range(1, MAX_ROUNDS + 1):
        flag = rng.random() < (pp["false_alarm"] if correct else pp["recall"])
        call("reviewer", rnd, "reviewer", verdict="REQUEST_CHANGES" if flag else "APPROVE",
             **({"finding": "- illustrative finding"} if flag else {}))
        if not flag:
            break
        if rnd == MAX_ROUNDS:
            stop = "max_rounds"
            break
        call("planner", rnd + 1, "replan", verdict="REVISED")
        call("worker", rnd + 1, "rework")
        correct = (rng.random() >= pp["damage"]) if correct else (rng.random() < pp["p_fix"])
    rounds = max(sp["round"] for sp in spans if sp["role"] == "reviewer")
    root = dict(trace_id=f"{tid}.r{k}", span_id="s0", parent_id=None, name="invoke_workflow pwr", role="orchestrator", round=rounds,
                start_ms=0, duration_ms=int(t * 1000), cost_usd=round(cost, 4), input_tokens=int(cost * 700000), calls=len(spans),
                stop_reason=stop, status="ok")
    spans.insert(0, root)
    return correct, cost, t


def build(seed):
    rng = random.Random(seed)
    rows = one_agent("single", SINGLE, "single", rng)
    rows += one_agent("single-aa", SINGLE, "single", rng)
    rows += one_agent("solo-verify", SOLO, "solo", rng)
    traces = {}
    for k in range(1, 6):
        for tid in SINGLE:
            spans = []
            ok, cost, secs = pipeline_trial(tid, k, rng, spans)
            rows.append(row("pwr", tid, k, ok, cost, secs))
            traces[f"{tid}.r{k}"] = spans
    for k in range(1, 6):
        for tid in SINGLE:
            if META[tid][2] == "code":
                ok, cost, secs = pipeline_trial(tid, k, rng, [])
                rows.append(row("route", tid, k, ok, cost, secs))
            else:
                rows.append(row("route", tid, k, rng.random() < SOLO[tid], noisy(rng, COST["qa"]["solo"]), noisy(rng, SECS["qa"]["solo"])))
    return rows, traces


def passes(rows, cfg, code_only=False):
    return sum(r.split(",")[5] == "1" for r in rows if r.startswith(cfg + ",") and (not code_only or r.split(",")[1] in ("T18", "T19", "T20")))


def main():
    # Search for a seed whose sampled data shows what each lesson needs (the underlying probabilities never change):
    # the pipeline does not beat one agent overall, the A/A check is quiet, and on the three code tasks the pipeline
    # beats a plain agent but not an agent that verifies its own work.
    for seed in range(1, 20000):
        rows, traces = build(seed)
        s, aa, sv, pwr, rt = (passes(rows, c) for c in ("single", "single-aa", "solo-verify", "pwr", "route"))
        cs, csv_, cp = passes(rows, "single", True), passes(rows, "solo-verify", True), passes(rows, "pwr", True)
        if -6 <= pwr - s <= 0 and abs(aa - s) <= 3 and 2 <= rt - s <= 6 and abs(rt - sv) <= 4 and cs <= 10 and cp >= 13 and abs(cp - csv_) <= 1 and passes(rows, "route", True) >= 12:
            break
    OUT.mkdir(parents=True, exist_ok=True)
    tdir = OUT / "traces-pwr"
    tdir.mkdir(exist_ok=True)
    for f in tdir.glob("*.trace.jsonl"):
        f.unlink()
    for tid, spans in traces.items():
        write(tdir / f"{tid}.trace.jsonl", [json.dumps(x) for x in spans])
    write(OUT / "results.csv", [HEADER] + rows)
    code = [r for r in rows if r.split(",")[1] in ("T18", "T19", "T20") and r.split(",")[0] in ("single", "pwr", "solo-verify")]
    write(OUT / "code-only.csv", [HEADER] + code)
    print(f"seed {seed}")
    for cfg in ("single", "single-aa", "solo-verify", "pwr", "route"):
        rs = [r.split(",") for r in rows if r.startswith(cfg + ",")]
        print(f"{cfg:12} {passes(rows, cfg)}/{len(rs)}  code {passes(rows, cfg, True)}/15  ${sum(float(r[7]) for r in rs) / len(rs):.4f}/trial")


if __name__ == "__main__":
    main()
