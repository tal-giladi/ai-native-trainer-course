# Instructor notes — 11.5 Governance of the AI layer

**Teaching objective.** Students write a one-to-two-page governance policy for their AI layer — scope, owners, change classes, the system-evolution loop, cadence, thresholds, overrides, deactivation — and enforce the mechanical parts in CI so the policy survives busy weeks.

**Likely confusion.** Governance vs the gates. The gates (07.6, 09.6) decide whether one change is acceptable; governance decides who may change the gates, how incidents turn into tasks, and when the whole thing is reviewed. A team can have excellent gates and no governance, and then someone lowers a threshold on a Friday.

**Common misconception.** "Governance means approvals." Most of this policy is about making the cheap path the right one: change classes keep typos cheap, the evolution loop is a checklist, the checker does the policing. Approvals are one row in one table.

**Key analogy.** Aviation's incident loop. A near-miss is reported without blame, turned into a checklist item or a simulator scenario, and the scenario is flown by every crew. A memo that says "be careful with X" is the CLAUDE.md hotfix; the simulator scenario is the regression task.

**Common failure in the exercise.** In step 3 the new regression task passes on the current layer, and students conclude the layer is fine. Usually the task was written in the rule's words, not the incident's. Send them back to the transcript. Second: students paste their whole old changelog into `evolution` and give up; point them to `--since`.

**Expected exercise outcome.** `governance.md` filled from the template; CODEOWNERS extended and code-owner review required; ten recent commits classified; one real incident taken through the full loop with a failing-then-passing task and a complete changelog entry; `ai-layer-governance.yml` failing a PR without a changelog entry; the first monthly review scheduled.

**Extension exercise.** Measure governance latency: median time from PR open to merge for AI-layer PRs vs ordinary PRs over the last month. If layer PRs are more than twice as slow, find which class or check causes it and propose a change backed by the numbers.

**Discussion question.** The course says "don't blame the developer who ran the agent". But the developer approved the agent's migration edit before review caught it. Where does individual responsibility sit in a blameless AI-layer incident process?
