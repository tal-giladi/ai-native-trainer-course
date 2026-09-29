# Module 02 plan — LLM and Agent Fundamentals

Stage B · 5 lessons · ~90 min instruction · ~5 h practice · no prerequisites inside the course.
Goal: enough mechanism that the student can explain any agent behaviour to a room of engineers without hand-waving, and classify a failure by the mechanism that caused it.

| Lesson | Objectives (measurable) | Depends on | Lab (labs/module-02) | Deliberate break | Artifact |
|---|---|---|---|---|---|
| 02.1 From text to next token | Count tokens for C#, SQL, Hebrew under two tokenizers; trace text → tokens → logits; run a position test and state effective vs nominal context | — | `01-tokens` TokenLab (Microsoft.ML.Tokenizers cl100k/o200k + optional Anthropic count endpoint; needle test) | Multi-needle "retry count" with stale distractors; chars/4 budget on a Hebrew ticket | `fundamentals/tokens.md` (token table + position table) |
| 02.2 Sampling and (non-)determinism | Compute softmax with temperature and top-p by hand; compute $p^k$ and $1-(1-p)^k$; explain why T=0 is neither deterministic nor correct | 02.1 | `02-sampling` SamplingLab (softmax, chain, runs vs OpenAI-compatible endpoint / Ollama) | "Set temperature to 0, call it fixed" on a 7/10 ticket | `fundamentals/sampling.md` |
| 02.3 The instruction hierarchy | Order the layers by authority; predict and then measure which instruction wins; replace a persuasion with an enforcement | 02.1, 02.2 | `03-hierarchy` fixture repo + planted README + deterministic gate (sh/ps1) | README contradicts AGENTS.md; run in two agents × 5 | `fundamentals/hierarchy-experiment.md` |
| 02.4 Tool calling and the agent loop | Implement a tool loop in C#; handle malformed args, tool failure, runaway loops and path escape; compute per-run input tokens | 02.2, 02.3 | `04-agent-loop` starter (naive, 4 BREAK-IT spots) + reference solution + scripted fake model | AGENT_BREAK=malformed / toolfail / loop / escape | `fundamentals/agent-loop/` (student's fixed loop + traces) |
| 02.5 Failure taxonomy and model selection | Classify 10 failure types from a transcript; compute cost per successful task; build a selection matrix for 3 tasks × ≥3 models × ≥2 providers | 02.1–02.4 | `05-model-selection` ModelBench (models.json, 3 starter tasks incl. Hebrew triage) | Pick by leaderboard / single run, then rerun N=5 | `fundamentals/failure-taxonomy.md`, `fundamentals/model-selection.md` |

Simulation: agent-loop (linked from 02.4 and 02.5; built later).
Templates created: `templates/model-selection-matrix.md`, `templates/agent-failure-taxonomy.md`.
Module quiz: 10 scenario questions incl. the outline's "7/10, temperature 0" scenario.
Forward links: 02.1 → M4 (context budgets), 02.2 → M7 (intervals, pass^k), 02.3 → M3/M8 (rules files, hooks) and M9 (injection), 02.4 → M8 (MCP), M9 (excessive agency), M10 (multi-agent), 02.5 → M5 (phase diagnosis), M7 (evaluation), M12 (hosting constraints).
