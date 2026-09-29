# Instructor notes — 06.3 Sub-agents, and when not to build one

**Teaching objective.** Students reduce "sub-agent" to its four mechanical differences (fresh context, own tools and model, one message back, cannot ask), apply the four questions to proposed agents, write a delegation contract with a matching tool allowlist, and measure context saved against tokens spent.

**Likely confusion.** "A sub-agent is a smarter specialist." The model is the same unless you change `model`; the only differences are context, tools and what comes back. Ask the room what a "senior architect" persona changes mechanically. The honest answer is: nothing the four differences do not already cover.

**Common misconception.** "More agents means more parallel work means faster tickets." Research parallelizes; implementation of shared code does not, because decisions are implicit in edits (Cognition's point). And tokens add up even when wall-clock time falls (Anthropic's 15× figure).

**Key analogy.** Sending a contractor to the archive with a written request. They come back with a two-page memo, and your desk stays clear. But they cannot phone you from the archive: anything your request did not settle, they settle themselves.

**Common failure in the exercise.** Students skip the non-delegated comparison run in Try it step 3, so the delegation log has one side only and no break-even. Second: the parallel run returns full file contents because the calling prompt asked for "everything relevant". Point them to the agent's `## Output` section.

**Expected exercise outcome.** Design templates for `researcher` and `reviewer` with four answered questions. A delegation log with two runs; typically main-context saving of 30–60k tokens for the two tickets, total tokens 10–30% higher delegated, wall-clock lower. A reviewer run on BILL-154 with 0–3 findings, most of them minor, at least one labelled "noise". The planner break reproduces the silent "N/A".

**Extension exercise.** Give `researcher` `model: haiku` (or your provider's small model) and rerun the two tickets. Compare brief quality with the golden rules for BILL-154, and cost. This is the model-selection question from 02.5, now per agent.

**Discussion question.** A client has 12 agents in `.claude/agents/`, most of them personas. Using the four questions, which would you delete in the first meeting, and how would you show the team it was safe to delete them?
