# Instructor notes — 03.2 Component map and portability

**Teaching objective.** Students can place any AI-layer component on three axes (load timing, advise vs enforce, scope), choose the right component for a need, and set up one canonical rules source that several agent tools read.

**Likely confusion.** Skills vs sub-agents vs path-scoped rules. Anchor on the axes: a path-scoped rule is a *fact* loaded by file location; a skill is a *procedure* loaded by task; a sub-agent is a *separate context window*. Module 6 goes deep on skills and sub-agents; here only placement matters.

**Common misconception.** "If I write it strongly enough in CLAUDE.md, it is enforced." Rules are advisory. Anything that must never happen needs a hook, permission rule, test or CI gate.

**Key analogy.** Rules and skills are the onboarding handbook and the runbooks; hooks and permissions are the badge readers on the doors; CI is the audit. You need all three in a company, and you do not write "please don't enter the server room" in the handbook and call it security.

**Common failure in the exercise.** Students leave an old `CLAUDE.md` in place after creating `AGENTS.md` and then report "Claude ignores AGENTS.md". Have them check `/memory` before debugging anything else. Tool-specific file names also change; if a format in the table no longer works, that is a teaching moment about the implementation tag, not a lesson failure.

**Expected exercise outcome.** An inventory with axes filled in, `AGENTS.md` + importing `CLAUDE.md`, the migration rule in three formats, and a portability matrix; probe P1 answers agree across two tools.

**Extension exercise.** Write a 30-line script (C# or PowerShell) that extracts the bullet list from `AGENTS.md`'s "Database migrations" section and regenerates the three path-scoped files, then add it as a CI check that fails on drift.

**Discussion question.** Your client standardizes on one agent tool next quarter. Is the portability work wasted? What part of the layer survived the tool change unchanged?
