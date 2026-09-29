# Build progress (for resuming after a crash)

Goal: build the course in `curriculum/course-outline.md`. Max 2 writer agents at a time.
Each module agent follows `curriculum/AGENT-BRIEF.md` and checkpoints per lesson in `curriculum/status/module-NN.log`.
Resume: check this file + `curriculum/status/*.log`; respawn unfinished modules with "Write module NN per curriculum/AGENT-BRIEF.md" (the agent skips done lessons).
Validate: `py scripts/check.py [NN]`. Titles/paths: `scripts/manifest.py` (regenerates `_sidebar.md`).

Units total: 22 modules + foundations + 6 simulations + templates + projects + capstone + glossary/research merge + final QA = 33.

## Foundations
- [x] manifest + _sidebar.md
- [x] scripts/check.py
- [x] AGENT-BRIEF.md
- [x] site shell (index.html, assets/site/*)
- [x] README, COURSE-MAP, PROGRESS (learner), COURSE-MAINTENANCE, references/README

## Modules (order: M2–M13 first, then M1, M14–M22)
- [x] M02
- [x] M03
- [x] M04
- [x] M05
- [x] M06
- [x] M07
- [x] M08
- [x] M09
- [x] M10
- [x] M11
- [x] M12
- [x] M13
- [x] M01
- [x] M14
- [x] M15
- [x] M16
- [x] M17
- [x] M18
- [x] M19
- [x] M20
- [x] M21
- [x] M22

## After modules
- [x] simulations: agent-loop, context-budget, evaluation, security, multi-agent, measurement
- [x] templates/README.md + fill missing §13 templates
- [x] projects/ scaffolds (9 READMEs)
- [x] assessments/capstone.md + capstone-rubric.md
- [x] merge glossary-inbox → glossary.md; research-inbox → references/research-log.md
- [x] QA fix: LoopGate (labs/module-05) treats # as comment, truncating "V###" in gates/architecture.rules
- [x] (kept on purpose: Module 8 README teaches it as a real-engine CI finding) QA fix: labs/module-03/brownfield/db/migrations/V004__due_not_null.sql fails on real SQL Server (V003 index includes DueUtc); module-08 has workaround db/init/before-V004.sql
- [x] QA: quiz length bias fixed — correct option longest in 123/624 (19.7%)
- [x] QA: verify Hovland & Weiss 1951 four-week (sleeper effect) claim in module 18 (cited from memory)
- [x] QA: verify Kotter HBR "Leading Change" error list in module 19 (cited from memory)
- [x] final QA: `py scripts/check.py` clean

## Log
- 2026-09-28: foundations done; M02, M03 agents spawned
- 2026-09-28: projects scaffolds + capstone brief/rubric done. Progress: 4 of 33 units done
- M03 done; M04 spawned. Progress: 5 of 33 units
- M02 done; M05 spawned. Progress: 6 of 33 units
- M05 done; M06 spawned. Progress: 7 of 33 units
- M04 done; M07 spawned. Progress: 8 of 33 units
- M06 done; M08 spawned. Progress: 9 of 33 units
- M07 done; M09 spawned. Progress: 10 of 33 units
- M09 first attempt stopped by safety classifier, no files; respawned with defensive scope (mechanism-level attacks, benign canary payloads, own checker writing EvalHarness results.csv)
- M08 done; M10 spawned. Progress: 11 of 33 units
- M09 done; M11 spawned. Progress: 12 of 33 units
- M10 done; M12 spawned. Progress: 13 of 33 units
- M11 done; M13 spawned. Progress: 14 of 33 units
- PAUSED by Tal (session tokens ending): no new agents after M12 + M13 finish. Resume next session from the first unchecked module (M01, then M14–M22), then simulations/templates/glossary merge/QA.
- M12 done. Progress: 15 of 33 units
- M13 done. Progress: 16 of 33 units. Paused per Tal; next: M01.
- 2026-09-29: resumed by Tal; M01 + M14 spawned. Progress: 16 of 33 units
- M01 done; M15 spawned. Progress: 17 of 33 units
- M14 done; M16 spawned. Progress: 18 of 33 units
- M16 done; M17 spawned. Progress: 19 of 33 units
- M15 done; M18 spawned. Progress: 20 of 33 units
- M17 done; M19 spawned. Progress: 21 of 33 units
- M18 done; M20 spawned. Progress: 22 of 33 units
- M19 done; M21 spawned. Progress: 23 of 33 units
- M20 done; M22 spawned. Progress: 24 of 33 units
- M21 done; simulations batch 1 (agent-loop, context-budget, evaluation) spawned. Progress: 25 of 33 units
- templates index done; checker now skips HTML check in labs and accepts \< escapes. Progress: 26 of 33 units
- M22 done; LoopGate fixed; simulations batch 2 (security, multi-agent, measurement) spawned. Progress: 27 of 33 units
- glossary + research log merged (scripts/merge_inboxes.py). Progress: 28 of 33 units
- Hovland/Kotter claims verified; quiz rebalance agent spawned. Progress: 29 of 33 units
- quiz rebalance agent stopped (would exceed 2 agents); respawn after a simulation batch finishes — it resumes from quiz-balance.log
- sims batch 2 (security, multi-agent, measurement) done; quiz rebalance respawned. Progress: 29 of 33 units (sims half done)
- all 6 simulations done. Progress: 30 of 33 units
- site smoke test passed (header fix in course.js: H1 regex needed /m). Full check 0 errors. Remaining: quiz rebalance agent, then final QA. Progress: 31 of 33 units
- 2026-09-29: BUILD COMPLETE. 33 of 33 units. check.py 0 errors/0 warnings; 101 lessons, 101 quizzes, 101 instructor notes, 22 module quizzes, 6 simulations, 45 templates, 394 glossary terms, 392 research rows.
- 2026-09-29: smoke runs vs real claude (Opus 5.5), ≤3 runs/lab, $5 cap per agent. Agent A: M04,M06,M07. Agent B: M09,M10,M11,M02. Brief: curriculum/SMOKE-BRIEF.md, results curriculum/smoke-runs.md. Baseline: M04 T01 x1 = 121k tokens, $0.50.
- smoke agent B: blocked — auto-mode denied headless claude with Bash/Edit (M11 T18); $0 spent. Fixed run-headless.sh --bare (AGENT_BARE=0). Open: M09 live run impossible (.mcp.json points at non-MCP REST services on unresolvable hosts; CanaryCheck run --agent claude throws); guard.sh needs jq (not installed). M09/M10/M11 smoke pending Tal permission.
- smoke agent A: M04, M06, M07 pass vs real claude (Opus 5.5), $1.65. Fixed M06 run-triggers (max-turns 2→5, count Read of SKILL.md). Open: user-global ~/.claude/CLAUDE.md leaks into headless lab runs; M07 judge doesn't save cost JSON. Waiting for Tal to add permission rule for M09–M11.
- smoke M10 + M11 ran ($1.14). Both code fixes correct but graded fail: labs/module-04/solution/docs/ai/topology.md line 13 quotes buggy 'DueUtc < clock.UtcNow' so agents edit the doc out of scope. Agent's edit to it was blocked by auto mode — left for Tal. Smoke total ≈ $2.80 + $0.50.
- 2026-09-29: Tal: fix everything broken. Agent: M09 live-run rebuild. Agent: headless isolation from ~/.claude (all labs) + M07 judge JSON saving.
- headless isolation (--setting-sources project,local) in all labs + M07 judge JSON/cost: done, verified ($0.19).
- M09 lab rebuilt: MCP via 127.0.0.1:8809, CanaryCheck run works, guard.sh jq-free ($0.25). Live: Opus 5.5 resisted A01 on baseline too (no contrast). Conflict: M09 agent says --setting-sources does NOT exclude ~/.claude/CLAUDE.md. Reconcile agent spawned.
- isolation fixed for real (--settings with claudeMdExcludes + autoMemoryEnabled off, all labs); M09 lessons/README made honest about model resisting injection ($0.28). Remaining open: topology.md stale line (Tal).
- topology.md line 13 fixed by me (Tal authorized). Verifying M11 with one run.
- M11 re-run passes ($0.20); run-headless.sh jq dependency removed. All known issues fixed.
- 2026-09-29: published https://github.com/tal-giladi/ai-native-trainer-course → https://tal-giladi.github.io/ai-native-trainer-course/ (ai-native-trainer-path.html gitignored: third-party workshop teardown).
