# Instructor notes — 05.1 Research discipline

**Teaching objective.** Students treat research as a phase with defined outputs (reuse, constraints, blast radius, open questions, each with evidence), run it read-only in a separate context, and can show a defect that exists only because research was skipped.

**Likely confusion.** "The agent already explores on its own, so why a phase?" It does, but it explores until it finds *a* way to satisfy the ticket, not until it has checked for existing definitions and constraints. A research phase changes the stopping rule: every acceptance criterion maps to evidence.

**Common misconception.** "Green tests mean the change is right." In the BILL-151 break the agent wrote the tests from the same misunderstanding as the code, so they agree with each other and with nothing else. Ask: who wrote the oracle?

**Key analogy.** A new senior hire on day three. They are competent, but before touching billing they ask "do we already have something for this, and what did we decide about data access?" Research is making the agent ask those questions every time.

**Common failure in the exercise.** Briefs that narrate ("First I opened InvoiceService…") and run to 200 lines. Enforce the template and the one-screen limit. Second: students let the research session edit files "just to try something"; insist on plan mode or a read-only sub-agent.

**Expected exercise outcome.** Two briefs of 40–80 lines. BILL-150's brief names `IsOverdue`, `IClock`, ADR 0007, the V003 undo convention and the 2-argument constructor in existing tests. BILL-151's brief names `OutstandingAsync` with its Issued-only filter. The paste-and-go branch fails `LoopGate arch` on `DateTimeOffset.UtcNow`; the researched version passes; the void/draft test separates them. Two NOTES.md rows with timed research minutes (typically 8–15 minutes each).

**Extension exercise.** Run the same research prompt with and without the grounded `AGENTS.md` loaded, 3 times each, and count how often the brief cites ADR 0007. What does that say about which facts belong in the always-loaded layer versus research?

**Discussion question.** The METR trial found experienced developers slower with AI while believing they were faster. What in your own workflow would make you believe you are faster when you are not, and what in your notes log would reveal it?
