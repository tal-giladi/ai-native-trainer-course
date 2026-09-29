# Module 8 quiz — MCP, APIs and Hooks

Nine scenario questions on connecting agents to external systems and constraining what they can do there: MCP roles and credentials, least-privilege integrations with a tracker and a code host, building a data server that is honest about freshness, and hooks that enforce, validate and audit. They test reasoning, not recall: most describe a configuration, a transcript or an incident and ask what is wrong, what to change first, or what would have caught it.

**Covers**

- [08.1 · MCP architecture, authentication and authorization](../lessons/module-08/lesson-01.md) — host, client, server; tools, resources, prompts; stdio versus HTTP credentials; the three permission layers; MCP versus API versus CLI; tool-definition cost.
- [08.2 · A real integration on your wedge stack](../lessons/module-08/lesson-02.md) — action classes, identity choice, read-only token, server mode and host rules, allowlists over denylists, eval tasks that need the integration.
- [08.3 · Building a custom MCP server in C#](../lessons/module-08/lesson-03.md) — narrow read-only tools, stdout discipline, least-privilege database login, provenance and refusing stale data, testing at two levels.
- [08.4 · Hooks: enforcement and audit](../lessons/module-08/lesson-04.md) — events and exit codes, fail closed versus fail open, payload tests, validation and audit hooks, latency.

**Pass mark:** 70% (7 of 9). Unlimited retries; answers and explanations appear after you submit.

Before you start, it helps to have done the labs in [`labs/module-08`](../labs/module-08/README.md) — several questions use Contoso Billing, the schema server and the hooks built there.
