# Instructor notes — 12.5 ADRs and the reference architecture

**Teaching objective.** Students write ADRs that a reviewer can trust (real options, a negative consequence, a runnable confirmation), change decisions by supersession, draw context and container views with trust boundaries traceable to ADRs, and answer a 30-question security review with evidence, all checked in CI.

**Likely confusion.** What deserves an ADR. Students either write none or write one per library upgrade. Use the test: would a security reviewer or a new platform lead ask "why?" about it within a year, and is it expensive to reverse? Hosting, gateway, identity, logging and residency pass; a folder name does not.

**Common misconception.** "The ADR log should always show the current state, so update old ADRs." That destroys the reasoning trail. The current state is the set of accepted ADRs; the history is the superseded ones. Show the `Topic:` check catching two accepted records on one question.

**Key analogy.** Court judgments. A later ruling can overturn an earlier one, but nobody edits the earlier judgment; the new one cites it, and lawyers can read both to understand why the law changed.

**Common failure.** Review answers written from memory of the target design. The Break's fourth problem (answers contradicting the architecture file) is the most important one; make students find it before running the tools. Also: confirmations like "we will monitor this", which nobody can run.

**Expected exercise outcome.** A portfolio `enterprise-architecture` repository with at least five ADRs including one supersession, a two-view diagram with boundaries and ADR labels, a lintable architecture file, 30 evidenced answers, and a CI job running `lint`, `adr` and `review`. For the Break: 0002/0003 linked by supersession, a new ADR superseding the "log everything" record, 0005 restored, nine answers rewritten with evidence, and the architecture file fixed so the answers are true.

**Extension exercise.** Pair up. Each student acts as the other's security reviewer: pick five questions, open the cited evidence, and write one finding where the evidence does not support the answer. Swap and fix.

**Discussion question.** Your client's CISO wants the ADRs in a wiki "where security can find them," not in the repository. What do you lose, what do you gain, and what compromise keeps the confirmation checks running?
