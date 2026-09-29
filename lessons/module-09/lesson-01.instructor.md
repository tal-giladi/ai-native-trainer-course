# Instructor notes — 09.1 Threat modeling agents

**Teaching objective.** Students draw the identities and trust boundaries of an agent system, label every content source (including MCP tool descriptions) as trusted or untrusted, and use the lethal-trifecta / Rule-of-Two test to decide whether an injection could exfiltrate — then map threats to OWASP LLM and ASI ids.

**Likely confusion.** Ownership vs authorship. Students assume "my Jira / my repo" is trusted. Hammer that the boundary is *who wrote the text*, not who owns the server. A ticket in your own tracker is untrusted.

**Common misconception.** "A good system prompt keeps the model on my instructions." It is a trained preference, not an enforced boundary. This is the hinge for the whole module; if they believe prompts are boundaries, 09.2's break will not land.

**Key analogy.** A confused deputy: the agent is a clerk with keys to every room (the union of its tools). An attacker who slips a note into the inbox can get the clerk to open a room and mail out what is inside — the clerk is not malicious, just authorized and gullible.

**Common failure in the exercise.** Students model data flows but forget tool *descriptions* enter the context at connection time (the Break). Also, they list threats without a concrete path; push them to write "source → tool → sink".

**Expected exercise outcome.** A one-page map for their own `ai-layer-lab` with identities, a trusted/untrusted table, the trifecta count for baseline and hardened configs, and four threats with OWASP ids and paths. The top threat names a *leg to cut*, not just a filter.

**Extension exercise.** Have them find a real published incident (e.g. the GitHub MCP or a documented indirect-injection case) and re-express it in this template: identities, boundary crossed, trifecta legs, OWASP id, and the leg that would have blocked it.

**Discussion question.** For their real work setup, which leg of the trifecta is cheapest to cut, and what capability do they lose by cutting it? Is that trade worth it?
