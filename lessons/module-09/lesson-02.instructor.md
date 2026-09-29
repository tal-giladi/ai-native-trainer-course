# Instructor notes — 09.2 Prompt injection, direct and indirect

**Teaching objective.** Students explain injection as a missing data/instruction boundary, trace an indirect injection from an untrusted source through a tool to an observable sink, and demonstrate for themselves that a prompt-level "ignore injections" rule reduces but does not eliminate the attack — measured as a success rate, not a yes/no.

**Likely confusion.** Direct vs indirect. Make it concrete: direct = you typed it; indirect = it rode in on a ticket while you asked something innocent. The coding-agent threat is almost entirely indirect.

**Common misconception.** "I told it to ignore injected instructions, so I'm safe." The Break exists to kill this belief experimentally. Have them predict the outcome before running so the result lands.

**Key analogy.** A single mailbox where letters and to-do notes are mixed. The clerk can't tell which is which, so a letter that reads like a to-do gets done. You can't fix that by adding a note that says "ignore the to-do notes" — it's just another note in the same pile.

**Common failure in the exercise.** Reporting one trial ("it wrote the file, so the attack works"). Push them to five trials and an interval. Also, students often can't find the sink; point them at exfil.txt and the catcher's hits.jsonl.

**Expected exercise outcome.** The four-step path written for A01 and A02, baseline success rates with Wilson intervals from the sample evidence, and the (small) drop from the CLAUDE.md rule that does not reach zero there. They should be able to say why the hardened arm holds (the sink is gone) while the prompt-only arm does not.

**Live runs will often not breach.** On 2026-09-29 a live A01 trial with Claude Opus 5.5 ignored the planted BILL-901 instruction on the over-permissive baseline as well as on hardened, so both arms held. Expect this with current frontier models and say so up front, or students conclude the baseline is fine or the lab is broken. A held run proves nothing (0 in 1 has a 95% upper bound of 95%; 09.6's rule of three), success varies by model, phrasing and trial, and the lesson's point is that defenses must hold when the model does not resist. Make the model-agnostic checks the primary evidence: the samples (a stand-in agent that follows the injection), `CanaryCheck trifecta`, and piping the obedient tool call into `guard.sh` (lab README, "Seeing the contrast reliably"). Do not have students write stronger injections to force a breach; the module's attack content stays the benign canary fixtures.

**Extension exercise.** Have them design a *second* prompt-level mitigation (e.g. wrapping untrusted content in delimiters and instructing the model to treat anything inside as data) and measure whether it beats the first. It will help a little and still not zero out — reinforcing that boundaries are architectural.

**Discussion question.** If prompt-level defenses can't be certified, why bother writing them at all? (Answer: defense in depth raises attacker cost and catches lazy attacks; they are layers, not boundaries.)
