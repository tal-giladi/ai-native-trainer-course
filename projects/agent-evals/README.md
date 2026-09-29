# agent-evals

Task dataset, graders, harness, attack regression suite and comparison reports. The instrument that tells you whether any AI-layer change helped.

- **Built in:** M4, M7, M9, M10
- **Visibility:** Private → public harness

## Expected contents

- `tasks-v0/` — first 10 pass/fail tasks (Module 4)
- `tasks/` — task set v1, ≥20 tasks with golden answers (Module 7)
- `graders/` — deterministic checks, rubric, calibrated LLM judge + calibration report (Module 7)
- `harness/` — N-trial runner with Wilson intervals (Module 7)
- `attacks/` — security-lab attacks as permanent regression tests (Module 9)
- `reports/` — comparison reports incl. `multi-vs-single.md` (Modules 7, 10)

## README sections to fill in

Every portfolio README answers the same five questions:

1. **Purpose** — what problem this repo exists to solve.
2. **What was built** — the components, with links.
3. **Why it exists** — the incident, experiment or client need that justified it.
4. **How it was evaluated** — the evidence: tasks, trials, intervals, reviewers.
5. **Lessons learned** — what failed, what you changed, what you would do differently.

> [!IMPORTANT]
> Never commit employer or client code, secrets, or personal data to a public repository. Anonymize before publishing.
