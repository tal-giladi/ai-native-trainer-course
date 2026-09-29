# Instructor notes — 02.1 From text to next token

**Teaching objective.** Learners leave able to explain agent behaviour in terms of tokens, attention and effective context, and to measure — not guess — token counts and position effects on their own code.

**Likely confusion.** "Token" vs "word". Show the boundary printout (`--show 30`) early; `PROCED|URE` and `.us|p|_Get` do more than any definition. Second confusion: context window vs `max_tokens` (the window is input + output; `max_tokens` caps output only).

**Common misconception.** "If it fits in the window, the model sees it." It *attends over* it, with finite, competing weights, and quality degrades with length and position. The multi-needle break is the antidote; run it live if the budget allows, otherwise show a recorded table.

**Key analogy.** Attention as a meeting where every token gets one vote-budget of 1.0 to spend on earlier tokens. Invite 1,000 irrelevant attendees and the one person who knows the answer gets a smaller share of the room's attention. Say explicitly that real networks are much sharper than this — the analogy explains the direction, not the size.

**Common failure.** Learners run the single-needle test, get 15/15, and conclude long context "works". Point to RULER: single-needle retrieval is the easy case. Insist on the multi variant.

**Expected exercise outcome.** A `tokens.md` with ≥3 tokenizers × 4 samples + their solution total; a position table for single and multi. Expect Hebrew at roughly 1.5–4× English depending on tokenizer, SQL at ~3–3.5 chars/token, C# at ~4.5–5. Multi-needle misses, where they appear, cluster in the middle positions and return a distractor value.

**Extension exercise.** Add a fourth tokenizer: an open-weights model's tokenizer (`LlamaTokenizer.Create` with a downloaded `tokenizer.model`) and compare Hebrew again. Or replace the synthetic haystack with 100k tokens of the learner's own wiki export.

**Discussion question.** "Your client's agent budget was set by counting characters in their repo. Which of their teams is most under-budgeted, and how would you show them without a single slide of theory?"
