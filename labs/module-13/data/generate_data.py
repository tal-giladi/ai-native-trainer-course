"""Generates the ILLUSTRATIVE data sets in labs/module-13/data/.

Every row is simulated from invented parameters so you can practise the Module 13 analyses
without a team and without weeks of waiting. The files are not measurements of any team, tool,
model or agent. Never quote them as results.

The script is deterministic: each data set searches seeds 1, 2, 3, ... and keeps the first seed
whose simulated data shows the property the lesson needs (the property and the accepted seed are
printed). Re-running it reproduces the same files byte for byte.  Usage: py generate_data.py

What is true in the simulation (the "ground truth" the lessons ask you to recover):

  contoso-selfselected.csv  AI has NO effect on cycle time. Developers chose the AI mode mostly
                            for small tickets, so the naive comparison shows a ~40% speed-up.
  contoso-randomized.csv    AI multiplies cycle time by 0.85 (a 15% reduction). Arms were
                            randomized within developer x size blocks (96 tickets). The AI arm opens PRs
                            later in the ticket's life. One manual ticket is a 170-hour outlier.
                            Escaped-defect probability is 1.3x higher with AI.
  prepost-teams.csv         AI has NO effect. The pilot team was picked because it had the worst
                            Q2; its Q2 was a bad-luck quarter.
"""
import math
import random
import statistics
from pathlib import Path

HERE = Path(__file__).resolve().parent
TICKET_HEADER = ("ticket,developer,week,size,story_points,assigned,used_ai,agent_version,"
                 "hours_to_pr,review_hours,cycle_hours,rework_commits,escaped_defects,lines_changed,notes")

SIZES = {"S": dict(base=5.0, points=(1, 2), p_defect=0.04, rework=0.3, lines=60),
         "M": dict(base=16.0, points=(3, 5), p_defect=0.10, rework=0.8, lines=220),
         "L": dict(base=48.0, points=(8, 13), p_defect=0.22, rework=1.8, lines=700)}


def poisson(rng, lam):
    # Knuth's method; small lambdas only.
    limit, k, p = math.exp(-lam), 0, 1.0
    while True:
        p *= rng.random()
        if p <= limit:
            return k
        k += 1


def ticket_row(rng, tid, dev, dev_mult, week, size, arm, used_ai, agent, effect, sigma, pr_frac, defect_mult, notes=""):
    s = SIZES[size]
    cycle = s["base"] * dev_mult * (effect if arm == "ai" else 1.0) * math.exp(rng.gauss(0, sigma))
    frac = min(0.9, max(0.1, pr_frac[arm] + rng.gauss(0, 0.08)))
    to_pr = round(cycle * frac, 1)
    review = round(cycle - to_pr, 1)
    if review < 0.3:
        review = 0.3
    cycle = round(to_pr + review, 1)
    defects = 1 if rng.random() < s["p_defect"] * (defect_mult if arm == "ai" else 1.0) else 0
    rework = poisson(rng, s["rework"])
    lines = int(s["lines"] * math.exp(rng.gauss(0, 0.5)) * (1.35 if used_ai == "yes" else 1.0))
    return dict(ticket=tid, developer=dev, week=week, size=size, story_points=rng.choice(s["points"]),
                assigned=arm, used_ai=used_ai, agent_version=agent, hours_to_pr=to_pr, review_hours=review,
                cycle_hours=cycle, rework_commits=rework, escaped_defects=defects, lines_changed=lines, notes=notes)


def write_tickets(path, rows):
    cols = TICKET_HEADER.split(",")
    lines = [TICKET_HEADER] + [",".join(str(r[c]) for c in cols) for r in rows]
    path.write_text("\n".join(lines) + "\n", encoding="utf-8", newline="\n")


def glog(xs):
    return statistics.fmean(math.log(x) for x in xs)


def strat_log_ratio(rows, metric="cycle_hours"):
    """Stratified (by size) difference of mean log metric, ai minus manual, weighted by stratum n."""
    tot, acc, var = 0, 0.0, 0.0
    for size in SIZES:
        a = [math.log(r[metric]) for r in rows if r["size"] == size and r["assigned"] == "manual"]
        b = [math.log(r[metric]) for r in rows if r["size"] == size and r["assigned"] == "ai"]
        if len(a) < 2 or len(b) < 2:
            return None, None
        w = len(a) + len(b)
        acc += w * (statistics.fmean(b) - statistics.fmean(a))
        var += w * w * (statistics.variance(a) / len(a) + statistics.variance(b) / len(b))
        tot += w
    return acc / tot, math.sqrt(var) / tot


# ---------------------------------------------------------------------------------------------
# A. Self-selected: no true effect; developers pick AI for small tickets.
def selfselected(seed):
    rng = random.Random(seed)
    devs = {"D1": 0.85, "D2": 0.9, "D3": 1.0, "D4": 1.0, "D5": 1.1, "D6": 1.2}
    p_ai = {"S": 0.8, "M": 0.45, "L": 0.25}
    rows = []
    for i in range(120):
        week = i // 10 + 1
        dev = rng.choice(list(devs))
        size = rng.choices(["S", "M", "L"], weights=[45, 35, 20])[0]
        arm = "ai" if rng.random() < p_ai[size] else "manual"
        rows.append(ticket_row(rng, f"BILL-{300 + i}", dev, devs[dev], week, size, arm,
                               "yes" if arm == "ai" else "no", "agent 1.8", effect=1.0, sigma=0.45,
                               pr_frac={"manual": 0.45, "ai": 0.6}, defect_mult=1.0))
    return rows


for seed in range(1, 20000):
    rows = selfselected(seed)
    man = [r["cycle_hours"] for r in rows if r["assigned"] == "manual"]
    ai = [r["cycle_hours"] for r in rows if r["assigned"] == "ai"]
    med_ratio = statistics.median(ai) / statistics.median(man)
    d, se = strat_log_ratio(rows)
    if d is None:
        continue
    cells_ok = all(sum(1 for r in rows if r["size"] == s and r["assigned"] == a) >= 5
                   for s in SIZES for a in ("ai", "manual"))
    per = []
    for s in SIZES:
        a = [r["cycle_hours"] for r in rows if r["size"] == s and r["assigned"] == "manual"]
        b = [r["cycle_hours"] for r in rows if r["size"] == s and r["assigned"] == "ai"]
        per.append(math.exp(glog(b) - glog(a)))
    if (cells_ok and 0.58 <= med_ratio <= 0.62 and abs(d) <= 0.03
            and all(0.85 <= x <= 1.15 for x in per)):
        write_tickets(HERE / "contoso-selfselected.csv", rows)
        print(f"contoso-selfselected: seed {seed}, n={len(rows)}, median ratio ai/manual {med_ratio:.3f}, "
              f"stratified ratio {math.exp(d):.3f}, per-size {[round(x, 3) for x in per]}")
        break


# ---------------------------------------------------------------------------------------------
# B. Randomized within developer x size blocks: true effect 0.85; contamination; outlier; version change.
def randomized(seed):
    rng = random.Random(seed)
    devs = {"D1": 0.9, "D2": 1.0, "D3": 1.05, "D4": 1.15}
    plan = []
    for dev in devs:
        sizes = ["S"] * 10 + ["M"] * 10 + ["L"] * 4
        rng.shuffle(sizes)
        arms = {}
        for size in SIZES:
            n = sizes.count(size)
            block = ["ai", "manual"] * (n // 2)
            rng.shuffle(block)
            arms[size] = block
        for k, size in enumerate(sizes):
            plan.append((dev, k, size, arms[size].pop()))
    plan.sort(key=lambda t: (t[1], t[0]))
    rows = []
    for i, (dev, k, size, arm) in enumerate(plan):
        week = k * 12 // 24 + 1
        agent = "agent 1.8" if week <= 6 else "agent 1.9"
        rows.append(ticket_row(rng, f"BILL-{500 + i}", dev, devs[dev], week, size, arm,
                               "yes" if arm == "ai" else "no", agent, effect=0.85, sigma=0.30,
                               pr_frac={"manual": 0.40, "ai": 0.65}, defect_mult=1.3))
    # Contamination: three manual-assigned tickets where the developer used the agent anyway.
    manual_idx = [i for i, r in enumerate(rows) if r["assigned"] == "manual" and r["size"] == "M"]
    for i in rng.sample(manual_idx, 3):
        rows[i]["used_ai"] = "yes"
        rows[i]["notes"] = "used agent for the tests despite manual assignment"
    # Outlier: one manual L ticket blocked on an external team.
    big = [i for i, r in enumerate(rows) if r["assigned"] == "manual" and r["size"] == "L"][0]
    r = rows[big]
    r["hours_to_pr"], r["review_hours"] = 150.0, 20.0
    r["cycle_hours"] = 170.0
    r["notes"] = "blocked 5 days waiting for DBA sign-off"
    return rows


def welch_mean_reduction(rows):
    a = [r["cycle_hours"] for r in rows if r["assigned"] == "manual"]
    b = [r["cycle_hours"] for r in rows if r["assigned"] == "ai"]
    return 1 - statistics.fmean(b) / statistics.fmean(a)


for seed in range(1, 20000):
    rows = randomized(seed)
    d, se = strat_log_ratio(rows)
    if d is None:
        continue
    ratio, upper = math.exp(d), math.exp(d + 1.96 * se)
    pr_a = [r["review_hours"] for r in rows if r["assigned"] == "manual"]
    pr_b = [r["review_hours"] for r in rows if r["assigned"] == "ai"]
    pr_ratio = math.exp(glog(pr_b) - glog(pr_a))
    mean_red = welch_mean_reduction(rows)
    da = sum(r["escaped_defects"] for r in rows if r["assigned"] == "manual")
    db = sum(r["escaped_defects"] for r in rows if r["assigned"] == "ai")
    if 0.83 <= ratio <= 0.87 and upper <= 0.97 and pr_ratio <= 0.60 and mean_red >= 0.25 and db > da:
        write_tickets(HERE / "contoso-randomized.csv", rows)
        print(f"contoso-randomized: seed {seed}, n={len(rows)}, stratified ratio {ratio:.3f} (normal upper {upper:.3f}), "
              f"PR-clock ratio {pr_ratio:.3f}, mean reduction {mean_red:.3f}, defects manual {da} vs ai {db}")
        break


# ---------------------------------------------------------------------------------------------
# C. Pre/post by team: no effect; the pilot team is the one with the worst Q2 (regression to the mean).
TEAMS = ["Atlas", "Billing", "Catalog", "Checkout", "Data", "Edge", "Forms", "Growth", "Identity", "Ledger", "Mobile", "Search"]
QUARTERS = ["2025-Q4", "2026-Q1", "2026-Q2", "2026-Q3"]
TREND = [1.0, 1.03, 0.99, 1.0]


def prepost(seed):
    rng = random.Random(seed)
    out = {}
    for t in TEAMS:
        level = rng.uniform(18, 34)
        out[t] = [round(level * TREND[q] * math.exp(rng.gauss(0, 0.20)), 1) for q in range(4)]
    return out


for seed in range(1, 20000):
    data = prepost(seed)
    PRE1, PRE2, SEL, PILOT = 0, 1, 2, 3   # 2025-Q4, 2026-Q1, 2026-Q2 (selection quarter), 2026-Q3 (pilot)
    pilot = max(TEAMS, key=lambda t: data[t][SEL])
    others = [t for t in TEAMS if t != pilot]

    def did(pre, post):
        return (math.log(data[pilot][post] / data[pilot][pre])
                - statistics.fmean(math.log(data[t][post] / data[t][pre]) for t in others))

    drop = data[pilot][PILOT] / data[pilot][SEL]
    spike = data[pilot][SEL] / data[pilot][PRE2]
    if drop <= 0.72 and spike >= 1.35 and abs(did(PRE2, PILOT)) <= 0.05 and abs(did(PRE1, PILOT)) <= 0.05:
        lines = ["team,quarter,median_cycle_hours,tickets,ai_pilot"]
        for k, t in enumerate(TEAMS):
            for q in range(4):
                lines.append(f"{t},{QUARTERS[q]},{data[t][q]},{30 + (k * 7 + q * 3) % 19},"
                             f"{'yes' if (t == pilot and q == PILOT) else 'no'}")
        (HERE / "prepost-teams.csv").write_text("\n".join(lines) + "\n", encoding="utf-8", newline="\n")
        print(f"prepost-teams: seed {seed}, pilot {pilot}, Q2->Q3 ratio {drop:.3f}, Q1->Q2 spike {spike:.3f}, "
              f"DiD from Q2 {math.exp(did(SEL, PILOT)):.3f}, DiD from Q1 {math.exp(did(PRE2, PILOT)):.3f}, "
              f"DiD from 2025-Q4 {math.exp(did(PRE1, PILOT)):.3f}")
        break
