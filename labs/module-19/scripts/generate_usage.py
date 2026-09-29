"""Generates 26 weeks of weekly agent usage per team for the Fabrikam rollout (illustrative, seeded).
Run from labs/module-19:
    py scripts/generate_usage.py

break/19.4-regression/usage.csv  v0 rollout: the consultant leaves after W12, office hours stop, mobile and
                                 lending are moved to a second tool outside the gateway in W16 (a step, and
                                 partly a measurement gap), new hires get no onboarding. W26: about 10% active.
solution/usage.csv               v1 plan (illustrative): same waves, handover to named owners, one alert on
                                 the data team in W18 that is acted on, new hires onboarded in week one.

Columns: week, team, cohort, seats, active, engaged
  seats   = licensed developers in that team and cohort that week
  active  = developers with at least one agent session that week
  engaged = developers with agent sessions on at least 3 different days that week
Only team-level counts are published: no per-person data (works council, privacy; see 19.4)."""
import csv, math, random
from pathlib import Path

TEAMS = {"billing": 48, "payments": 64, "identity": 36, "lending": 72, "mobile": 56, "data": 40, "platform": 34, "web": 50}
WAVE = {"billing": 1, "payments": 1, "platform": 1, "identity": 3, "data": 3, "web": 3, "lending": 5, "mobile": 5}
PILOT = {"billing", "payments", "platform"}
ROOT = Path(__file__).resolve().parent.parent


def clamp(x):
    return max(0.0, min(1.0, x))


def ramp(team, w):
    """Share active while enablement is running: fast for pilot teams, slower for later waves."""
    start = WAVE[team]
    peak = 0.72 if team in PILOT else 0.63
    k = w - start
    return peak * (1 - math.exp(-(k + 1) / 2.6))


def generate(after, seed):
    rng = random.Random(seed)
    rows = []
    for w in range(1, 27):
        for team, devs in TEAMS.items():
            if w < WAVE[team]:
                continue
            share = ramp(team, w)
            eng = 0.52
            if not after:
                if w > 12:
                    share = ramp(team, 12) * math.exp(-(w - 12) / 9.0)
                    eng = 0.52 - 0.012 * (w - 12)
                if team in ("mobile", "lending") and w >= 16:
                    share = 0.03
                    eng = 0.30
            else:
                if w > 12:
                    share = ramp(team, 12) * (0.95 if w in (13, 14) else 0.98)
                    eng = 0.55
                if team == "data" and w in (18, 19, 20):
                    share = {18: 0.32, 19: 0.30, 20: 0.43}[w]
                if team == "data" and w == 21:
                    share = 0.55
            share = clamp(share + rng.gauss(0, 0.02))
            active = round(devs * share)
            engaged = min(active, round(active * clamp(eng + rng.gauss(0, 0.03))))
            rows.append([f"W{w:02d}", team, "rollout", devs, active, engaged])
        # new hires: two a week from W13, as one pseudo-team so no team row has fewer than 5 people
        if w >= 13:
            seats = 2 * (w - 12)
            if after:
                share = clamp(0.55 + rng.gauss(0, 0.04))
            else:
                share = clamp(0.04 + rng.gauss(0, 0.01))
            active = round(seats * share)
            engaged = round(active * (0.45 if after else 0.2))
            rows.append([f"W{w:02d}", "new-hires", "newhire", seats, active, engaged])
    return rows


def write(path, rows):
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", newline="\n", encoding="utf-8") as f:
        wr = csv.writer(f, lineterminator="\n")
        wr.writerow(["week", "team", "cohort", "seats", "active", "engaged"])
        wr.writerows(rows)


if __name__ == "__main__":
    write(ROOT / "break/19.4-regression/usage.csv", generate(False, 1904))
    write(ROOT / "solution/usage.csv", generate(True, 1905))
    print("wrote break/19.4-regression/usage.csv and solution/usage.csv")
