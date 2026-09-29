# Module 12 quiz — Enterprise AI-agent architecture

Nine scenario questions about designing and defending a coding-agent platform for a large organisation: choosing where inference runs, putting a gateway with quotas and fallback in the middle, giving every caller a narrow short-lived identity, keeping every copy of prompt data where the policy says, and recording decisions so a security review can check them. They test reasoning, not recall: most describe a design, an incident or a review finding and ask what is wrong or what to do next.

**Covers**

- [12.1 · Hosting options](../lessons/module-12/lesson-01.md) — the four hosting models; hard constraints before cost; cost per successful task with fixed cost.
- [12.2 · Model gateways and routing](../lessons/module-12/lesson-02.md) — what a gateway centralises; token-bucket quotas and noisy neighbours; fallback availability; chargeback coverage.
- [12.3 · Identity, secrets, networking and audit](../lessons/module-12/lesson-03.md) — four identities; reach and exposure of a leaked credential; separation of duties; redact-before-write and append-only audit.
- [12.4 · Data, residency and compliance](../lessons/module-12/lesson-04.md) — the data inventory; residency on every path; training and retention terms; compliance orientation; the cost of "EU only".
- [12.5 · ADRs and the reference architecture](../lessons/module-12/lesson-05.md) — ADR structure and supersession; context and container views with trust boundaries; evidence-backed review answers.

**Pass mark:** 70% (7 of 9). Unlimited retries; answers and explanations appear after you submit.

Before you start, work through the labs in [`labs/module-12`](../labs/module-12/README.md): several questions use Fabrikam's numbers and the `ArchCheck` outputs.
