"""Generates the ILLUSTRATIVE data for the Module 21 dry-run engagement (Contoso, simulated client).

Nothing here is a measurement of any team, model or agent. The files exist so you can practise the
engagement's audit, baseline, measurement and follow-up steps with the tools from Modules 7, 13 and 19.
Re-running reproduces the same files (fixed seeds).  Run from labs/module-21:
    py scripts/generate_data.py

client/data/baseline-tickets.csv  Billing team, the 12 ISO weeks before kickoff (2026-W29..W40), ImpactStats format.
client/data/pilot-tickets.csv     Engagement weeks 3-6 (ISO W43..W46): eligible tickets randomized manual / ai,
                                  blocked by size. True effect in the simulation: cycle time x0.85 on ai tickets;
                                  escaped defects unchanged (8%).
client/data/eval-results.csv      24 eval tasks (Module 7 tasks-v1) x 5 trials, before-ai-layer vs contoso-layer-v1.
client/data/usage.csv, events.csv Weekly agent usage per team, engagement weeks W01..W24 (handover W12,
                                  follow-ups W16, W20, W24), AdoptCheck format. Platform's champion moves in W18;
                                  the 60-day follow-up (W20) catches it and a co-champion takes over.
break/21.4-calendar-handover/usage.csv, events.csv  Same engagement, handed over on the calendar date to the
                                  consultant's own "office hours", no follow-ups: usage decays after W12.
"""
import csv, json, math, random
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DATA = ROOT / "client" / "data"
TICKET_HEADER = ["ticket", "developer", "week", "size", "story_points", "assigned", "used_ai", "agent_version",
                 "hours_to_pr", "review_hours", "cycle_hours", "rework_commits", "escaped_defects", "lines_changed", "notes"]
MEDIAN = {"S": 9.0, "M": 20.0, "L": 42.0}
POINTS = {"S": 2, "M": 5, "L": 8}
SIZE_P = [("S", 0.35), ("M", 0.45), ("L", 0.20)]
DEVS = ["D1", "D2", "D3", "D4", "D5", "D6"]


def size(rng):
    x = rng.random()
    for s, p in SIZE_P:
        if x < p:
            return s
        x -= p
    return "L"


def ticket(rng, tid, week, s, assigned, factor):
    cycle = MEDIAN[s] * math.exp(rng.gauss(0, 0.38)) * factor
    review = cycle * rng.uniform(0.25, 0.45)
    return [tid, rng.choice(DEVS), week, s, POINTS[s], assigned, "yes" if assigned == "ai" else "no", "agent 2.1",
            f"{cycle - review:.1f}", f"{review:.1f}", f"{cycle:.1f}", rng.choice([0, 0, 1, 1, 2]),
            1 if rng.random() < 0.08 else 0, int(cycle * rng.uniform(18, 30)), ""]


def write_csv(path, header, rows):
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as f:
        w = csv.writer(f, lineterminator="\n")
        w.writerow(header)
        w.writerows(rows)


def tickets():
    rng = random.Random(21)
    rows, n = [], 400
    for week in range(29, 41):
        for _ in range(rng.choice([4, 5, 5, 6])):
            n += 1
            rows.append(ticket(rng, f"BILL-{n}", week, size(rng), "baseline", 1.0))
    rows[7][-1] = "blocked 4 days on month-end freeze"
    rows[7][10] = f"{float(rows[7][10]) + 96:.1f}"
    write_csv(DATA / "baseline-tickets.csv", TICKET_HEADER, rows)

    rng = random.Random(4321)
    rows = []
    n = 470
    for week in range(43, 47):
        batch = [size(rng) for _ in range(rng.choice([6, 7]))]
        # blocked randomization by size within the week: alternate arms in a shuffled order
        by = {}
        for s in batch:
            by.setdefault(s, []).append(s)
        for s, items in by.items():
            arms = (["manual", "ai"] * len(items))[: len(items)]
            rng.shuffle(arms)
            for arm in arms:
                n += 1
                rows.append(ticket(rng, f"BILL-{n}", week, s, arm, 0.85 if arm == "ai" else 1.0))
    write_csv(DATA / "pilot-tickets.csv", TICKET_HEADER, rows)
    return rows


def evals():
    tasks = json.loads((ROOT.parent / "module-07" / "tasks-v1" / "tasks.json").read_text(encoding="utf-8"))["tasks"]
    meta = {t["id"]: (t["split"], ";".join(t["tags"])) for t in tasks}
    before = [0.45, 0.35, 0.7, 0.2, 0.75, 0.3, 0.8, 0.7, 0.4, 0.5, 0.6, 0.7, 0.4, 0.8, 0.9, 0.6, 0.35, 0.5, 0.45, 0.4, 0.4, 0.6, 0.8, 0.5]
    after = [0.8, 0.75, 0.85, 0.7, 0.85, 0.75, 0.9, 0.85, 0.75, 0.8, 0.75, 0.85, 0.6, 0.9, 0.95, 0.75, 0.7, 0.8, 0.6, 0.6, 0.65, 0.75, 0.9, 0.65]
    rng = random.Random(7)
    out = []
    for config, probs, layer in (("before-ai-layer", before, "none"), ("contoso-layer-v1", after, "c0ffee21a1b2")):
        for k in range(1, 6):
            for i, p in enumerate(probs, 1):
                tid = f"T{i:02d}"
                split, tags = meta[tid]
                ok = rng.random() < p
                c = round(0.05 * rng.uniform(0.8, 1.2), 4)
                out.append([config, tid, split, tags, k, 1 if ok else 0, "" if ok else "illustrative failure", f"{c:.4f}",
                            int(c * 700000), int(rng.uniform(15000, 60000)), "illustrative-agent 2.1", "(agent default)", layer, "illustrative"])
    write_csv(DATA / "eval-results.csv", ["config", "task", "split", "tags", "trial", "pass", "reason", "cost_usd", "input_tokens",
                                          "duration_ms", "agent_version", "model", "layer_sha", "tasks_sha"], out)


TEAMS = {"billing": (6, 3), "collections": (7, 6), "platform": (6, 7)}  # seats, first live week


def usage(path_usage, path_events, decay, seed=None):
    rng = random.Random(seed if seed is not None else (2 if not decay else 193))
    rows = []
    for w in range(1, 25):
        for team, (seats, live) in TEAMS.items():
            if w < live:
                p = 0.15 if team == "billing" else 0.1  # a few people already tried the tool on their own
            else:
                ramp = min(1.0, (w - live + 1) / 2)
                p = 0.2 + 0.62 * ramp
                if decay and w > 12:
                    p = max(0.2, p - 0.05 * (w - 12))
                if not decay and team == "platform" and 18 <= w <= 20:
                    p = 0.3
            active = sum(rng.random() < p for _ in range(seats))
            engaged = sum(rng.random() < 0.55 for _ in range(active))
            rows.append([f"W{w:02d}", team, "rollout", seats, active, engaged])
    write_csv(path_usage, ["week", "team", "cohort", "seats", "active", "engaged"], rows)
    ev = [("W01", "Kickoff; stakeholder interviews; access granted on Contoso laptop"),
          ("W03", "Billing pilot starts: AI layer v0.1 merged by Avi; tickets randomized manual / ai"),
          ("W06", "Enable: collections live; developer workshop; Dana runs office hours"),
          ("W07", "Enable: platform live; champion Omer (co-champion Lior)")]
    if decay:
        ev += [("W12", "Handover on the contract end date; office hours stay with the consultant 'as needed'"),
               ("W14", "Consultant office hours cancelled twice (other client)"),
               ("W18", "Platform champion moves to another team; no successor")]
    else:
        ev += [("W12", "Handover: Avi owns the AI layer; Dana office hours; Tamar the metrics review"),
               ("W16", "30-day follow-up: gate; office hours and review all ran without the consultant"),
               ("W18", "Platform champion Omer moves to the data team; co-champion Lior not yet announced"),
               ("W20", "60-day follow-up: platform below its usual level; Lior announced as champion"),
               ("W24", "90-day follow-up: all three teams back in range; engagement closed")]
    write_csv(path_events, ["week", "event"], ev)


if __name__ == "__main__":
    tickets()
    evals()
    usage(DATA / "usage.csv", DATA / "events.csv", decay=False)
    usage(ROOT / "break" / "21.4-calendar-handover" / "usage.csv", ROOT / "break" / "21.4-calendar-handover" / "events.csv", decay=True)
    print("wrote client/data/*.csv and break/21.4-calendar-handover/*.csv")
