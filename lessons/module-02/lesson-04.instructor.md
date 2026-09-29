# Instructor notes — 02.4 Tool calling and the agent loop

**Teaching objective.** Learners build and harden a tool loop, and internalise that the harness — not the model — executes, authorizes and bounds everything.

**Likely confusion.** "The model runs the tool." Stop and trace one `tool_use` → C# method → `tool_result` by `tool_use_id` in `trace.json`. Second confusion: statelessness. Learners expect the API to remember; show the `messages` array growing and the `in=` counter climbing.

**Common misconception.** "Strict mode / JSON Schema makes tool calls safe." It makes them well-formed. The `escape` break is schema-valid and is the one that leaks data.

**Key analogy.** The model is a very capable colleague on the phone who cannot touch your keyboard. They say "open src/OrderService.cs and read it to me". You decide whether to do it. If they say "now read me the production config from the other folder", saying no is your job, not theirs — and a good colleague on the phone does not make that request safe.

**Common failure.** Learners fix `malformed` by catching the exception and returning a generic "error". The model then loops on the same bad call. Make them write instructive error text and watch the corrected call appear. Second failure: repeat detection keyed on tool name only, which stops legitimate multi-file reads — key on name + arguments.

**Expected exercise outcome.** A fixed loop where all five modes end cleanly; no canary in `trace.json`; a real-model trace on the learner's own workspace with a token-growth check. Reference: `AgentLoop.Solution` (about 145 lines).

**Extension exercise.** Port the loop to an OpenAI-compatible endpoint (a local Ollama model with tool support): `tools` with `type: function`, `tool_calls` on the assistant message, `role: tool` results. Note every place the shapes differ — this is the portability tax Module 3 talks about. Or add a third tool, `search_code(pattern)`, and measure whether turns drop.

**Discussion question.** "Claude Code asks permission before running some tools and not others. Using what you built today, argue which of your two tools should require approval, and what would change if you added `write_file`."
