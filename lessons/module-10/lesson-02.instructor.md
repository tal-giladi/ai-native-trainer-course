# Instructor notes — 10.2 Coordination: handoffs, shared state and conflicts

**Teaching objective.** Students design the seams of a multi-agent system: a validated contract on every handoff, one owner per mutable file, a named merge policy for parallel writers, and a precedence order that settles disagreements.

**Likely confusion.** Isolation vs coordination. Students hear "each agent works in its own worktree" and conclude conflicts are solved. Isolation postpones the conflict to the merge; the merge policy is the coordination.

**Common misconception.** "A clean git merge means the changes are compatible." A textual merge proves that lines did not collide. Two helpers with the same name, a changed default, or a signature both sides assumed differently can merge cleanly and break at build or at runtime.

**Key analogy.** Three developers editing the same shared spreadsheet offline, each saving over the file when done. Everyone's own copy looked right; the file on the share holds only the last save. Leases are "check out the file before you edit it", the way old source-control systems worked.

**Common failure in the exercise.** Students look only at the golden column and miss the line "merged branch's own test suite: GREEN". Ask them to read it aloud; it is the most important line in the output. Second: in step 4 (optional git), a clean merge is taken as success without building.

**Expected exercise outcome.** A four-row table matching the Show me numbers (naive 1/3, detect 1/3 with two conflicts, rerun 3/3 at 381 s and $0.75, serialize 3/3 at 303 s and $0.43), the trace notes quoted, and sections 3–4 of the design document written with a merge policy and a precedence order.

**Extension exercise.** Change the scenario timings (`scenarios/make_scenarios.py`, the `ms` of each worker) so T19 finishes first, regenerate, and rerun `naive`. Which ticket survives now? Then change one task's scope in a copy of `tasks.json` so it no longer overlaps, and watch `serialize` form two waves.

**Discussion question.** Agent teams let teammates message each other directly. Which of this lesson's three rules (contract, ownership, precedence) get harder when agents negotiate among themselves instead of through an orchestrator, and which get easier?
