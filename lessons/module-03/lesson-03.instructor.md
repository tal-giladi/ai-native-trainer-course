# Instructor notes — 03.3 The brownfield audit

**Teaching objective.** Students can audit a brownfield .NET repository in five passes, rank evidence, and decide which confirmed findings actually belong in a rules file by probing a fresh agent.

**Likely confusion.** "Confirmed" vs "belongs in the rules file". A finding can be true and still not belong there (the agent infers it). Draw the flowchart on the board and walk one finding down each branch.

**Common misconception.** "More context is always better, so write down everything the audit found." The AGENTS.md study and Anthropic's own guidance both point the other way: include only what the agent would otherwise get wrong. Module 4 makes the cost side quantitative.

**Key analogy.** Onboarding a senior contractor: you do not tell them C# has classes; you tell them "the wiki is wrong about data access, the ADR is right, and every migration needs an undo script — we learned that the hard way."

**Common failure in the exercise.** Students skip running the build and tests ("the README says `dotnet test`"). Insist on it: pass 2 is only done when the command has been executed. Second common failure: probing with the rules file still present, which measures the rules, not the agent's inference.

**Expected exercise outcome.** A Contoso findings table of 6+ rows matching the lesson's (data access, clock, undo scripts, stale doc as rules candidates; build and money as inferable), with the student's own probe counts; plus a private audit report on their own repository with at least one tribal-knowledge finding from interviews.

**Extension exercise.** Extend `AiLayerTool scan` with one new detector relevant to your repo (e.g. `#pragma warning disable` counts per folder, or stored procedures with `sp_` prefix), and justify what agent failure it predicts.

**Discussion question.** The majority pattern in the codebase is the abandoned one. Should the team migrate the remaining 200 call sites before rolling out agents, or rely on rules plus convention tests? What does each choice cost?
