"""Generates two months of gateway usage per key per day (illustrative, seeded). Run from labs/module-12:
    py scripts/generate_usage.py
fabrikam/usage-2026-08.csv  BEFORE: two keys have no owning team: 'shared-ci' (every team's CI jobs) and
                            'hackathon-2026' (created for an event, never revoked).
solution/usage-2026-09.csv  AFTER: one CI key per team (workload identity), hackathon key revoked."""
import csv, datetime, random

random.seed(20260801)
teams = {"billing": 48, "payments": 64, "identity": 36, "lending": 72, "mobile": 56, "data": 40, "platform": 34, "web": 50}
# per active developer-day on the large model (tokens): uncached input, cache writes, cache reads, output
DEV_DAY = (300_000, 400_000, 12_000_000, 150_000)

def month(year, mon, days, after):
    rows = []
    for d in range(1, days + 1):
        day = datetime.date(year, mon, d)
        weekday = day.weekday() < 5
        for team, devs in teams.items():
            active = devs * (0.62 if weekday else 0.04) * random.uniform(0.85, 1.15)
            for model, share in (("large", 0.8), ("small", 0.2)):
                f = active * share * random.uniform(0.9, 1.1)
                rows.append([day.isoformat(), f"gw-{team}", team, model] + [int(x * f) for x in DEV_DAY])
        # CI jobs: one shared key before; one key per team after (split by headcount)
        f = random.uniform(55, 75) * (1.0 if weekday else 0.6)
        if after:
            for team, devs in teams.items():
                g = f * devs / 400
                rows.append([day.isoformat(), f"ci-{team}", team, "large"] + [int(x * g) for x in DEV_DAY])
        else:
            rows.append([day.isoformat(), "shared-ci", "", "large"] + [int(x * f) for x in DEV_DAY])
            # a hackathon key that was never revoked; someone's scheduled agent kept using it
            if d >= 10:
                h = random.uniform(18, 26)
                rows.append([day.isoformat(), "hackathon-2026", "", "large"] + [int(x * h) for x in DEV_DAY])
    return rows


for path, rows in (("fabrikam/usage-2026-08.csv", month(2026, 8, 31, False)),
                   ("solution/usage-2026-09.csv", month(2026, 9, 30, True))):
    with open(path, "w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh, lineterminator=chr(10))
        w.writerow(["date", "key", "team", "model", "input", "cache_write", "cache_read", "output"])
        w.writerows(rows)
    print(f"{path}: {len(rows)} rows")
