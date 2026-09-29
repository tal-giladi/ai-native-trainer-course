# Smoke-run brief — real-model check of the labs

Goal: prove each model-calling lab works end to end against the real `claude` CLI with a tiny number of runs. Tal's usage is limited: **hard budget per agent $5 total cost** (sum `total_cost_usd` from every run's JSON). Stop immediately when the next run would exceed it.

Repo: `C:\Users\TalGiladi\OneDrive\repos\course-creator\ai-native-org`. Python is `py`. Scratch work dirs go in `C:\Users\TALGIL~2\AppData\Local\Temp\claude\C--Users-TalGiladi-OneDrive-repos-course-creator-ai-native-org\9467ec07-4bcc-482f-ac87-c0dff479bd0d\scratchpad\smoke\` — never write run outputs inside the repo's lesson files.

## Model
Always Opus 5.5: set env `ANTHROPIC_MODEL=claude-opus-5-5` for every `claude` invocation (the CLI accepts it via the env var; verify `modelUsage` in the JSON shows `claude-opus-5-5`). Where a lab tool takes a `--model` flag, pass `claude-opus-5-5`.

## Rules
- Each run is a separate headless session (`claude -p ...`) — never reuse a session.
- Read the lab README first and follow its setup (copy Contoso from `labs/module-03/brownfield` into scratch, etc.).
- Run **at most 3 real model calls per lab** (a "run" = one `claude -p` session). Pick the cheapest tasks that exercise the full pipeline (run → grade/report). Prefer read-only tasks.
- Use only local fictional code. Security lab (module 9): local Docker only, `docker compose down` afterwards.
- If a lab breaks against the real CLI (flags changed, output format differs, grader misreads real output), FIX the lab code/scripts/README minimally and re-verify — that's the point of this pass. Note every fix.
- Don't change lesson text except to correct a factual statement the real run contradicts (e.g. a quoted command/flag); never insert run numbers as if they were course measurements.
- Remove `bin/` `obj/` folders you create. Stop Docker containers you start.

## Record
Append one row per lab to `curriculum/smoke-runs.md` (create with header if missing — the other agent may create it first; re-read before writing, only append):
`| module | runs | tokens (in+cache+out) | cost USD | pass/fail | fixes made |`
Also append `module-NN smoke done` to `curriculum/status/smoke.log` after each lab; on start skip labs already there.

Report in under 100 words: per-lab result, total cost, fixes.
