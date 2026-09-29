# Module 19 labs — AI adoption and change management

Everything the Module 19 labs need. Lessons: [19.1](../../lessons/module-19/lesson-01.md) · [19.2](../../lessons/module-19/lesson-02.md) · [19.3](../../lessons/module-19/lesson-03.md) · [19.4](../../lessons/module-19/lesson-04.md).

The labs turn the Fabrikam platform from [Module 12](../module-12/README.md) (or your own team's AI layer) into an **adoption plan**: stakeholder map with evidence-backed answers, a champion network and tool strategy, enablement with owners who stay, adoption metrics, and the mechanisms that stop usage from sliding back once the consultant leaves. The plan follows the [adoption-plan template](../../templates/adoption-plan.md).

> [!WARNING]
> Adoption telemetry is data about people. Use only team-level counts with at least five people per group, never per-person tables, leaderboards or e-mail-keyed exports. In countries with employee representation (for example Germany's works councils), a system able to monitor behaviour or performance needs the council's agreement before it is introduced. If you build the plan for your real employer or client, anonymise it before it leaves the organisation and get the sponsor's permission to use it in your portfolio.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`). Verified with SDK 10.0.400.
- No NuGet packages. `AdoptCheck` is read-only and deterministic; it works offline.
- Python 3 only if you want to regenerate the illustrative usage data (`py scripts/generate_usage.py`).

## Contents

| Path | What it is |
|---|---|
| `tools/AdoptCheck/` | Dependency-free C# checker: `stakeholders` (19.1), `champions` (19.2), `enablement` (19.3), `plan` (all sections plus metrics and anti-regression, 19.4), `usage` (weekly adoption by team and cohort with p-chart control limits and events, 19.4). |
| `fabrikam/brief.md` | The rollout brief: teams, waves, countries, and eleven stakeholder quotes to build the map from. |
| `solution/` | **Illustrative** reference: `adoption-plan.md` (v1, passes `plan`), `usage.csv` and `events.csv` for 26 weeks under that plan. |
| `break/` | One deliberately flawed piece per lesson: a stakeholder map full of promises (19.1), a champion network of three enthusiasts and four tools (19.2), enablement as one webinar and a chat channel (19.3), the first rollout's metrics, handover and 26 weeks of usage that fell to about 10% (19.4). |
| `scripts/generate_usage.py` | Regenerates both `usage.csv` files (seeded). |

## Quick start

From this folder:

```bash
A="dotnet run --project tools/AdoptCheck --"

# 19.1 — stakeholders and objections
$A stakeholders break/19.1-stakeholders/adoption-plan.md     # 11 errors, 8 warnings
# 19.2 — champions and tools
$A champions break/19.2-champions/adoption-plan.md           # 20 errors, 16 warnings; 3.5 effective tools
# 19.3 — enablement
$A enablement break/19.3-enablement/adoption-plan.md         # 9 errors, 7 warnings; backlog +9 a week
# 19.4 — metrics, anti-regression and the usage data
$A plan  break/19.4-regression/adoption-plan.md              # errors in every section
$A usage break/19.4-regression/usage.csv --events break/19.4-regression/events.csv    # 10 errors: every team below its limit
# the reference
$A plan  solution/adoption-plan.md                           # clean
$A usage solution/usage.csv --events solution/events.csv     # clean; data dipped in W18 and recovered
```

(PowerShell: type the full `dotnet run --project tools/AdoptCheck -- <command>` instead of `$A`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem.

## File formats

- **Plan**: markdown with `- Key: value` fact lines and tables found by their column names (see the template). Add columns and rows freely; keep the header names.
- **usage.csv**: `week,team,cohort,seats,active,engaged` — weeks as `W01`; active = at least one session in the week; engaged = sessions on at least 3 days in the week. Team or cohort level only.
- **events.csv**: `week,event` — one line per week that something changed (a wave, a handover, a tool or quota change, a champion leaving).

Your own telemetry: export weekly counts per team from your gateway (Module 12), your agent tool's OpenTelemetry metrics, or your vendor's usage API, and drop anything that identifies a person before it lands in the CSV.

## Your own adoption plan

Build it in lesson order, for your team or for Fabrikam, in `enterprise-architecture/adoption-plan.md` (Fabrikam) or next to your `governance.md` in `ai-layer-lab` (your team):

1. **19.1** — facts and section 1 from at least five stakeholder conversations. `stakeholders` clean.
2. **19.2** — section 2: champions named with their managers' agreement, tool strategy. `champions` clean.
3. **19.3** — section 3: training paths and rhythms with owners who are not you. `enablement` clean.
4. **19.4** — sections 4–6 and a usage export (real or the Fabrikam data). `plan` clean; `usage` read and its errors explained in writing.

## Expected results

| Check | Break | Reference |
|---|---|---|
| `stakeholders` (19.1) | 11 errors: no legal/privacy, no works council, promises ("guaranteed", "10x", "completely safe"), usage tied to performance reviews, answers without evidence, an unowned high-influence blocker and skeptic | 0 |
| `champions` (19.2) | 20 errors: no default tool, consultant as champion for three teams, one champion for two teams, two teams without one, no sanctioned hours; 3.5 effective tools, 3 rules formats | 0; 1.3 effective tools |
| `enablement` (19.3) | 9 errors: no paths for leads, security, new hires; nothing measured beyond attendance; office hours owned by the consultant; no triage, AI-layer or metrics review; feedback 14 in, 5 out per week | 0; Little's law 28 open items |
| `plan` (19.4) | 11 errors: sections 1–3 missing; a per-person prompt leaderboard, volume targets (prompts, lines); no handover date, ownership, succession, onboarding or alert — only a consultant check-in | 0 |
| `usage` (19.4) | 10 errors: organisation 63% → 9%; six teams decay from W15–W17, mobile and lending step down in W16 (Tool B outside the gateway), new hires at 4% | 0; data below its limit W18, recovered |
