# Module 9 quiz — Agent security

Nine scenario questions on securing your own coding-agent setup: trust boundaries, prompt injection, tool poisoning and the supply chain, excessive agency, layered defenses, and the red-team cycle. They test reasoning, not recall — most describe a configuration, an attack, or a result and ask what it shows, what is wrong, or what to do next.

**Covers**

- [09.1 · Threat modeling agents](../lessons/module-09/lesson-01.md) — identities, trust boundaries, the lethal trifecta / Rule of Two, OWASP LLM & ASI ids.
- [09.2 · Prompt injection, direct and indirect](../lessons/module-09/lesson-02.md) — the data/instruction boundary; why prompt-only defenses are not boundaries; success as a rate.
- [09.3 · Tool poisoning and the supply chain](../lessons/module-09/lesson-03.md) — poisoned descriptions, rug pulls, package hallucination; pinning and source allowlists.
- [09.4 · Excessive agency and exfiltration](../lessons/module-09/lesson-04.md) — functionality/permissions/autonomy; exfil channels; vulnerable generated code; approval gates.
- [09.5 · Layered defenses](../lessons/module-09/lesson-05.md) — least privilege, sandbox egress, secret isolation, fail-closed hooks; why boundaries live in tools/network.
- [09.6 · Red-team your own layer](../lessons/module-09/lesson-06.md) — attack → observe → mitigate → retest → residual risk; the 1.0 golden floor; the rule of three.

**Pass mark:** 70% (7 of 9). Unlimited retries; answers and explanations appear after you submit.

Before you start, do the labs in [`labs/module-09`](../labs/module-09/README.md): several questions use the `security-lab`, `CanaryCheck` and the hardened config.

> [!CAUTION]
> Everything in this module runs only in the local `security-lab`. Never attack a real system.
