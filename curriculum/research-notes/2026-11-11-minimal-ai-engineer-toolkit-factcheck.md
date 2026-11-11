# Fact-check: KDnuggets "The Minimal AI Engineer Toolkit for 2026" (Vinod Chugani, 2026-08-06)

Checked 2026-11-11 at primary sources (PyPI/npm/GitHub APIs, official docs). [S] = snippets only.

- LangGraph 1.2.20 (2026-11-05); HITL `interrupt(value)` + `Command(resume=...)`; new optional `response_schema=` on interrupt.
- Burr renamed: Apache Burr (incubating), `pip install "apache-burr[start]"`, 0.43.0 (2026-08); old `burr` package stops at 0.42.0. Repo github.com/apache/burr.
- n8n 2.47.4 stable (2026-11-11); MCP Client Tool (AI Agent sub-node) and MCP Server Trigger.
- MCP spec 2026-07-28 (verified on changelog page): stateless, no initialize handshake, no Mcp-Session-Id, server/discover required, subscriptions/listen, ping/logging/setLevel removed, tasks moved to extension; roots/sampling/logging deprecated.
- Ollama 0.40.16 (2026-11-09): OpenAI-compatible /v1/chat/completions, /v1/responses, Anthropic-compatible /v1/messages.
- MLX 0.32.4, mlx-lm 0.32.2 (Oct 2026).
- Newest small open models (<10B): Qwen3.5 0.8B/2B/4B/9B (~Mar 2026); Gemma 4 E2B/E4B (Apr 2026); Ministral 3 3B/8B (Dec 2025); IBM Granite 4.2 3B/8B (Aug 2026); SmolLM3-3B; gpt-oss-20b (Aug 2025, 21B). No Phi-5, Qwen4 or Llama 5. Phi newest: Phi-4-mini-flash-reasoning (Jun 2025).
- Small vs 2024 frontier: holds on reasoning/instruction/tool-use benchmarks (Qwen3.5-9B model card: GPQA Diamond 81.7, vendor-reported) but small models lag on knowledge and hallucination (Artificial Analysis, 2026-04-13, https://artificialanalysis.ai/articles/sub-32b-open-weights). Scope the claim.
- Promptfoo 0.125.1 (2026-10-25), MIT; OpenAI announced agreement to acquire it 2026-03-09 (https://openai.com/index/openai-to-acquire-promptfoo/); closing not confirmed; "will remain open source".
- LangSmith SDK 0.14.17; "LangSmith Platform" (Deployment = former LangGraph Platform); Developer $0 / Plus $39 per seat. Braintrust SDK 0.49.3; $80M Series B Feb 2026 [S]. Langfuse acquired by ClickHouse Jan 2026 [S].
- Outlines 1.3.3 (2026-08); Instructor 1.17.0 (2026-09), repo 567-labs/instructor.
- vLLM 0.33.0 (2026-11-02): `guided_*` params removed in 0.12.0; use `structured_outputs` / `StructuredOutputsParams`; backends auto/xgrammar/guidance.
- Hosted structured outputs: OpenAI strict json_schema (Responses `text.format`); Anthropic GA `output_config.format` (old `output_format` deprecated, beta header only); Gemini Interactions API `response_format` (older `responseSchema` in generationConfig is dated).
- Git 2.56.1 (2026-10-23). Built-in worktree support: Claude Code `claude --worktree`, Codex desktop app, Cursor Agents Window (`.cursor/worktrees.json`).
