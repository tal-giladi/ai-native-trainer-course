# Instructor notes — 09.3 Tool poisoning and the supply chain

**Teaching objective.** Students explain tool-description poisoning, the rug pull and shadowing as injection through a channel that bypasses user review; quantify package hallucination; pin tool descriptions and detect drift; and constrain dependency resolution with a source allowlist.

**Likely confusion.** That a tool has to be *called* to be dangerous. Stress that descriptions enter the context at connection, so an unused tool can already steer the agent. This connects back to 09.1's Break.

**Common misconception.** "I reviewed the server when I added it, so it's safe." The rug pull kills this: trust was a moment, the description changed later. Approval must be re-verifiable (pinning), not a one-time gate.

**Key analogy.** A signed contract you file away, versus a contract someone can quietly re-word after you signed. Pinning is keeping the original to compare against.

**Common failure in the exercise.** Students re-pin the poisoned description to make the check pass — defeating the control. Call this out: DRIFT is a re-approval event for a human, not noise to silence. Also, some expect source mapping to catch every path; remind them it guards restore, not `dotnet list/add` metadata queries.

**Expected exercise outcome.** A pinned snapshot flagged as DRIFT after the rug pull; a hallucinated package failing `dotnet restore` under the allowlist; A04 and A05 scored blocked on hardened. They should name LLM03/ASI04 for each.

**Extension exercise.** Have them add a *checksum/version pin* concept: not just the description text but a version identifier, and discuss how they would gate a legitimate version bump (review the diff, re-pin deliberately) versus a rug pull.

**Discussion question.** Third-party MCP servers are convenient. Under what conditions would you connect one at all, and what would you refuse to hold in the same session with it?
