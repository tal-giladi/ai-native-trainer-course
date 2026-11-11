# Open items (not blocking)

- No lab has been run against a real model/`claude` CLI (cost); all lab tools were tested offline with stand-ins. Sample results in `labs/*/samples/` are labelled illustrative.
- Model ids and prices are "as of 2026-09" (volatility: implementation) — first quarterly review due 2026-12.
- `labs/module-03/.../V004__due_not_null.sql` fails on real SQL Server on purpose-by-accident; Module 8 teaches it as a CI finding with a workaround. Fix V004 only if you drop that teaching point.
- Some sources were confirmed via search because the publisher page returned 403 (SWE-bench Verified page, Loftus & Palmer, Guest et al., BMJ/ACM items, Kingman/Wilson DOIs, GDPR EUR-Lex). Listed in `references/research-log.md`.
- A few lessons run 2,500–3,100 words (19.1, 19.4, 21.1, 21.2), over the 2,000-word guideline.
- `ai-native-trainer-path.html` (the source teardown of another trainer's paid workshop) is kept out of the public repo via .gitignore.

# KDnuggets "AI managers" article — done 2026-11-11 (not committed)

Every figure re-checked at its primary source: `curriculum/research-notes/2026-11-11-ai-managers-factcheck.md`.
Dropped as unverifiable: SecondTalent "+135.8% prompt-engineering roles", the "Monte Carlo 75-90% per step -> 50%" claim, PwC 56% (now 62%, 2026 edition).

- ai-native-org: 01.2, 01.3, 10.1, 10.3, 11.4, 20.3 extended (4 new quiz questions); 12.4 and 19.3 EU AI Act wording fixed to the AI Omnibus. Logged in research/CHANGELOG.md. check.py and Academy check-course: 0 problems.
- llm-evals-course: 10.3 "the bar depends on what the answer does next"; quiz q5 replaced by q6. Academy check: 0 problems.
- ai-transformation-field-course: 04.6 and references/laws-and-guidance.md (Art 4 / Art 50(2) wording), 13.4 (McKinsey data-barrier figures), 14.4 (three oversight levels; quiz q3 replaced by q6). Academy check: 0 problems.
- Skipped: frontier LLM research course (no fit); ai-security-course (already covers it; has someone's uncommitted work in progress); capstone portfolio items (already in the capstone).
- Done: OWASP LLM ids moved to the 2026 edition in ai-native-org and ai-transformation-field-course; McKinsey/Lightcast/BCG figures moved to their 2026 editions.

# Ideas from KDnuggets "The Minimal AI Engineer Toolkit for 2026" (Vinod Chugani, 2026-08-06), read 2026-11-11

All tools and versions re-checked at primary sources: `curriculum/research-notes/2026-11-11-minimal-ai-engineer-toolkit-factcheck.md`.
Best fit is the ai-engineer course (12 modules). Note: ai-engineer is still in the old format (no front-matter, no .quiz.yaml), so it needs converting before Academy import anyway.

ai-engineer course:
1. Local-first dev loop (Module 1 or 10): build and debug against Ollama 0.40 or MLX on Apple Silicon, then swap only the base URL for production (Ollama now speaks OpenAI /v1, Responses and Anthropic /v1/messages). Current small models: Qwen3.5 (0.8-9B), Gemma 4 E2B/E4B, Ministral 3, Granite 4.2. Teach the honest limit: they match 2024 frontier models on reasoning and tool-use benchmarks but lag on knowledge and hallucinate more.
2. Code graph vs visual event orchestrator (Module 7): LangGraph 1.2 or Apache Burr (package renamed to apache-burr) for multi-turn, stateful agents; n8n 2.x (with its MCP Client Tool and MCP Server Trigger) for event-triggered automations. One decision rule plus one exercise.
3. MCP 2026-07-28 (lesson 7.4): the course does not say which spec revision it teaches. The newest revision is stateless (no initialize, no sessions, server/discover, roots and sampling deprecated). Add the version and what changed.
4. Eval tools in CI (lesson 3.5): map the regression suite onto Promptfoo, Braintrust or LangSmith; teach the pass-rate gate (e.g. 95%) as a risk-dependent choice, not a constant.
5. Constrained decoding vs validate-and-retry (lessons 2.2-2.3): Outlines 1.3, vLLM 0.33 structured_outputs (the old guided_json API was removed), Instructor 1.17; and the current native provider APIs (OpenAI strict json_schema, Anthropic output_config.format now GA, Gemini response_format on the Interactions API). Check 2.2 for any older parameter names.
6. Git worktrees for parallel agent and eval runs (Module 10 or 12): Claude Code, Codex app and Cursor now create them for you.

llm-evals-course:
7. Lesson 11.4 maps the suite onto promptfoo: add that OpenAI announced it is acquiring Promptfoo (March 2026; still MIT, says it will stay open source) and pin version 0.125.1.

Already covered elsewhere (no change): ai-native-org teaches MCP 2026-07-28 (08.1) and worktrees (10 lessons).
