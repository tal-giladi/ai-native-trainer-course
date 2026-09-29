"""Generates ops/runs.jsonl: 30 days of ILLUSTRATIVE run telemetry (simulated, not measurements) for lesson 11.4.

One JSON line per agent run, the shape the workflows' "Ledger line" step writes:
  ts, job, run_id, subtype, num_turns, cost_usd, duration_ms, outcome, attempt

The story: on 2026-09-14 someone changed dep-fix from nightly to hourly "until the build is green" and wrapped
it in a retry-on-any-failure loop with no --max-budget-usd. A breaking package bump arrived on 2026-09-15.
Run `py generate_runs.py` from this folder; output is deterministic.
"""
import json, random
from datetime import datetime, timedelta, timezone
from pathlib import Path

rng = random.Random(1104)
start = datetime(2026, 8, 24, tzinfo=timezone.utc)
rows, rid = [], 7000


def add(ts, job, subtype, turns, cost, ms, outcome, attempt=1, run_id=None):
    global rid
    if run_id is None:
        rid += 1
        run_id = str(rid)
    rows.append({"ts": ts.strftime("%Y-%m-%dT%H:%M:%SZ"), "job": job, "run_id": run_id, "subtype": subtype,
                 "num_turns": turns, "cost_usd": round(cost, 2), "duration_ms": ms, "outcome": outcome, "attempt": attempt})
    return run_id


for d in range(30):
    day = start + timedelta(days=d)
    weekday = day.weekday() < 5
    # agent-review: PRs on weekdays
    for _ in range(rng.randint(4, 8) if weekday else rng.randint(0, 1)):
        t = day + timedelta(hours=rng.randint(8, 18), minutes=rng.randint(0, 59))
        if rng.random() < 0.04:
            add(t, "agent-review", "error_max_turns", 12, rng.uniform(0.5, 0.75), rng.randint(200_000, 400_000), "failed")
        else:
            posted = rng.random() < 0.55
            add(t, "agent-review", "success", rng.randint(4, 10), rng.uniform(0.18, 0.55), rng.randint(60_000, 240_000), "applied" if posted else "no-op")
    # agent-triage: weekday sweep, one run per issue
    if weekday:
        for _ in range(rng.randint(2, 7)):
            t = day + timedelta(hours=5, minutes=30 + rng.randint(0, 20))
            add(t, "agent-triage", "success", rng.randint(1, 3), rng.uniform(0.02, 0.05), rng.randint(8_000, 25_000),
                "applied" if rng.random() < 0.8 else "needs-human")
    # agent-changelog: Mondays
    if day.weekday() == 0:
        t = day + timedelta(hours=6, minutes=2)
        add(t, "agent-changelog", "success", rng.randint(3, 5), rng.uniform(0.08, 0.2), rng.randint(40_000, 90_000),
            "merged" if rng.random() < 0.8 else "rejected")
    # dep-fix: nightly until 09-14, then hourly with a retry loop
    if day < datetime(2026, 9, 15, tzinfo=timezone.utc):
        t = day + timedelta(hours=2, minutes=10)
        r = rng.random()
        if r < 0.7:
            add(t, "dep-fix", "success", rng.randint(0, 2), rng.uniform(0.01, 0.05), rng.randint(5_000, 20_000), "no-op")
        elif r < 0.92:
            add(t, "dep-fix", "success", rng.randint(8, 16), rng.uniform(0.4, 0.9), rng.randint(150_000, 500_000), "merged")
        else:
            add(t, "dep-fix", "error_max_turns", 40, rng.uniform(1.2, 1.6), rng.randint(600_000, 900_000), "failed")
    elif day.date() == datetime(2026, 9, 15).date():
        for h in range(24):
            run_id = None
            for attempt in (1, 2, 3):
                t = day + timedelta(hours=h, minutes=5 + 12 * (attempt - 1))
                run_id = add(t, "dep-fix", "error_max_turns", 40, rng.uniform(1.75, 1.99), rng.randint(600_000, 700_000), "failed", attempt, run_id)
    else:
        # 09-16 onward: a human pinned the package back, kill switch re-enabled nightly schedule
        t = day + timedelta(hours=2, minutes=10)
        add(t, "dep-fix", "success", rng.randint(0, 2), rng.uniform(0.01, 0.05), rng.randint(5_000, 20_000), "no-op")

rows.sort(key=lambda r: r["ts"])
out = Path(__file__).parent / "runs.jsonl"
out.write_text("".join(json.dumps(r) + "\n" for r in rows), encoding="utf-8", newline="\n")
print(f"wrote {len(rows)} runs to {out.name}")
