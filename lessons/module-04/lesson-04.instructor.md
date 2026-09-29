# Instructor notes — 04.4 Compression and compaction

**Teaching objective.** Students choose between compression, prioritization, summarization, compaction and reset; verify what survives compaction with a survival test; and keep task constraints in durable, re-injected files (handoff note, plan, compact instructions, `SessionStart` hook).

**Likely confusion.** Compaction vs `/clear`. Compaction keeps a model-written summary and continues; `/clear` keeps nothing but files. Students think compaction is always the safer choice because "nothing is lost" — it is lossy by design.

**Common misconception.** "If I put it in CLAUDE.md it survives, so I'll put task constraints there." Root rules survive, but a task fact in the root misleads every other task and turns into stale context the day the task ends. Task facts belong to task artifacts.

**Key analogy.** A shift handover in a hospital. The night nurse does not hand over the whole night's conversation; she writes a structured note (patient, constraints like allergies — verbatim — what was done, what is next). The standing ward rules are on the wall and do not need repeating. An allergy mentioned only in a hallway conversation is the one that gets lost.

**Common failure in the exercise.** The constraint survives the break in their run and students conclude "compaction is fine". That is one sample; have them repeat with a steered `/compact` that does not mention constraints, and remind them of $p^k$ from 02.2. Second failure: a `HANDOFF.md` of 200 lines — it becomes a second, stale rules file. Cap at ~40.

**Expected exercise outcome.** A survival table (4 probes × 2 compaction styles) consistent with the docs — root facts survive; the path-rule fact and the chat-only constraint are unreliable; the steered compaction loses the constraint more often. A handoff note that lets a fresh session answer the next step and the migration number. Compact instructions of ≤ 5 lines in `CLAUDE.md`, and optionally the `SessionStart` hook.

**Extension exercise.** Use the API's compaction beta (or your own summarization call from the 02.4 agent loop) on a recorded transcript: write a custom summarization prompt that must keep "constraints verbatim" and measure over 10 runs how often the V010 sentence survives with and without that instruction.

**Discussion question.** Agents increasingly write their own memory files. Who reviews what an agent decided to remember, and how is that different from reviewing a rules file?
