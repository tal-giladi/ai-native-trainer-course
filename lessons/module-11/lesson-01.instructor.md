# Instructor notes — 11.1 Headless agents

**Teaching objective.** Students treat a headless agent run as a function call with a written contract (context, tools, caps, output), classify its ending from the JSON result, decide success with deterministic gates, and keep secrets and write tokens away from untrusted triggers.

**Likely confusion.** Exit code vs subtype vs outcome. Exit code says the process ended; `subtype` says how the loop ended; only diff, build and tests say whether the task happened. Draw the three as nested boxes and put `denied.json` in the gap between the second and third.

**Common misconception.** "`dontAsk` is the safe mode, so the job is safe." `dontAsk` is safe for your credentials, not for your conclusions: it converts every missing permission into a silent denial the model can narrate around. Safety of the result needs the gates.

**Key analogy.** A contractor working in your house while you are away. You do not ask them "did you fix the leak?" and leave it at that; you give them keys only to the rooms they need, a spending limit, a deadline, and you check the pipe when you get back. `pull_request_target` with a checkout is giving a stranger's contractor your house keys because they sent you a quote.

**Common failure in the exercise.** Allowlist path syntax. `Edit(src/**)` vs `Edit(./src/**)` and `Bash(dotnet test*)` vs `Bash(dotnet test *)` (the space matters for prefix matching). Students then reach for `bypassPermissions`; stop them and have them read `permission_denials` instead. Second: forgetting that `--bare` drops `CLAUDE.md`, so the agent ignores conventions until `--append-system-prompt-file AGENTS.md` is added.

**Expected exercise outcome.** AgentOps built; the four samples classified with a next action each; `wflint` clean on their own workflows after adding `permissions:` and `timeout-minutes` to Module 5's job; one live T18 run through `run-headless.sh` passing `result`, scope and tests; the spread of three runs (diff, turns, cost) in `NOTES.md`; `agent-fix.yml` dispatched once with environment approval. Typical T18 cost is well under a dollar; students report their own.

**Extension exercise.** Add a `--json-schema` to the fix job that makes the agent return `{files_changed, tests_run, tests_passed}` and have the job compare the claims with `git diff --name-only` and the real `dotnet test` result. Count how often the claims and the facts disagree over ten runs.

**Discussion question.** Where is the line between a job the agent may run unattended and one that must stay interactive? Try to state it as a property of the task (verifiability, reversibility, blast radius), not of the model.
