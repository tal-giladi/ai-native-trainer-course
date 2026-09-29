# Instructor notes — 02.3 The instruction hierarchy

**Teaching objective.** Learners can say where each instruction channel sits, predict and measure a conflict, and choose between instructing and enforcing.

**Likely confusion.** Learners conflate "the rules file is loaded first" with "the rules file has highest authority". Show the Claude Code doc line that CLAUDE.md arrives as a user message, and contrast with the system prompt. Also: AGENTS.md "closest wins" is a convention agents are trained/instructed to follow, not a runtime override.

**Common misconception.** "There's a precedence table like CSS." There is a trained tendency. The Wallace et al. paper exists precisely because the default was *no* reliable hierarchy.

**Key analogy.** A new contractor on day one. The onboarding pack (rules file) says one thing, a dusty wiki page they're told to read says another. A good contractor follows the onboarding pack and flags the wiki (outcome A). A tired one follows whatever they read last (D). You don't fix that with a louder onboarding pack; you fix the wiki and put a check in CI.

**Common failure.** Learners run one planted run per agent and generalise. Enforce 5 runs per condition per agent and a pre-registered prediction. Another: they fix by shouting ("IMPORTANT") and see improvement at N=5 — ask whether it would survive a model update.

**Expected exercise outcome.** An experiment table with baseline / planted / in-rules contradiction / after-fixes rows, outcome codes A–D, gate results and a quoted transcript line per run. Expect some D outcomes on the planted README in at least one agent, and more scatter on the in-rules contradiction. After step 4 the gate catches every D.

**Extension exercise.** Move the planted instruction from the README into a ticket body fetched via an MCP tool or pasted as "customer comment". Does the outcome distribution change with the channel? This previews Module 9.

**Discussion question.** "A client's rules file is 900 lines and contradicts itself in three places. Their engineers say the agent 'randomly ignores rules'. What do you tell their CTO, and what is the first hour of work?"
