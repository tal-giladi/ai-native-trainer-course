# Instructor notes — 02.2 Sampling and (non-)determinism

**Teaching objective.** Learners replace "it's random" and "set temperature to 0" with a measured pass rate, a view of where variation comes from, and the compounding arithmetic ($p^k$, $1-(1-p)^k$).

**Likely confusion.** Temperature vs top-p: temperature reshapes the whole distribution, top-p cuts the tail. Work the 2/1/0 table on the board at three temperatures before mentioning top-p. Second: "deterministic" vs "correct" — greedy decoding is perfectly repeatable and can be repeatably wrong.

**Common misconception.** "Temperature 0 means deterministic." Two counters: the Thinking Machines result (80 distinct completions in 1,000 at T=0, first divergence at token 103, caused by batch-size-dependent numerics) and the API docs that say so outright. Then the operational one: several current models no longer accept a temperature setting at all.

**Key analogy.** A choose-your-own-adventure book with twenty forks. Each fork is 97% obvious; the reader still ends up at the wrong ending almost half the time. Setting temperature to 0 is gluing the book so it always opens on the same page — you haven't fixed the forks, you've hidden them.

**Common failure.** Learners compute $1-(1-p)^k$ and conclude retries solve everything. Ask: "What detects the failure?" and "Were your failures alike?" Correlated failures are the norm when the cause is context.

**Expected exercise outcome.** Correct hand calculations (3/1/0 at T=0.5 → 0.980/0.018/0.002); a 10-run table for one real task with a criterion written in advance; a distinct-outputs count at T=0 and T=1 on a local or API model; a recorded 400 from any model that rejects temperature.

**Extension exercise.** Use Ollama's `seed` with T=1: same seed, same machine, 10 runs — identical? Then run the same seed on a colleague's machine or a different quantization. Discuss what "reproducible" can and cannot mean for a client's audit requirement.

**Discussion question.** "A regulated client demands deterministic AI output for audit. What can you honestly promise, and what architecture (logging, verifiers, human approval) gives them the auditability they actually need?"
