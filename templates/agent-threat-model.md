# Agent threat model — [system name]

*Template. Copy into your project and fill in. Introduced in [Module 9, lesson 09.1](../lessons/module-09/lesson-01.md). Companion: [security review checklist](security-review-checklist.md).*

> [!CAUTION]
> If this model covers a security lab, state it: local lab only, never real systems.

## 1. System and scope

- **What the agent does:** [one sentence]
- **Agent harness / model:** [e.g. Claude Code, pinned version, model]
- **In scope:** [the AI layer, tools, MCP servers, data this covers]
- **Out of scope:** [what this model does not cover]

## 2. Identities

Who or what is acting, and with whose authority.

| Identity | What it is | Authority / credentials it holds |
|---|---|---|
| User | The engineer running the agent | Their own repo and cloud access |
| Agent | The model + harness | The union of every tool it can call |
| Tool / MCP server | [name] | [scope: read/write, which system] |

## 3. Trust boundaries and content sources

Mark every source the agent reads. Anything not authored by the user is **untrusted**.

| Source | Trusted? | Reaches the agent via |
|---|---|---|
| User's chat prompt | Trusted | direct |
| Repo files the user wrote | Trusted | Read |
| Ticket / issue text | **Untrusted** | MCP / fetch |
| Docs / web pages | **Untrusted** | fetch |
| Third-party MCP tool output & descriptions | **Untrusted** | MCP |
| Dependency code & package names | **Untrusted** | build |

## 4. The lethal trifecta / Rule of Two

Check each leg that is reachable **in one session**:

- [ ] Untrusted content reaches the agent
- [ ] Private data or powerful (write/exec) tools are in reach
- [ ] An outbound channel exists (network egress, or a write/comment/PR tool)

All three ⇒ an injection can exfiltrate. Cut at least one leg (Meta's *Agents Rule of Two*).

## 5. Threats

Map each to OWASP LLM / Agentic ids and name the concrete path.

| # | Threat | OWASP id | Path in this system | Likelihood | Impact |
|---|---|---|---|---|---|
| T1 | Indirect prompt injection | LLM01 / ASI01 | [ticket → agent → tool] | | |
| T2 | Tool poisoning / rug pull | LLM03 / ASI04 | | | |
| T3 | Excessive agency | LLM06 / ASI02–03 | | | |
| T4 | Exfiltration | LLM02 | | | |
| T5 | Hallucinated package | LLM03 | | | |

## 6. Mitigations (mapped to threats)

| Threat | Control | Layer | Status |
|---|---|---|---|
| T1 | [e.g. cut the outbound leg; approval gate] | config / hook | planned / done |

## 7. Residual-risk register

For each threat, what remains after the controls — and who accepts it.

| Threat | Residual risk after controls | Evidence (attack retest) | Accepted by | Review date |
|---|---|---|---|---|
| T1 | [e.g. 0/25 breaches; 95% upper bound ≈ 12%] | link to results.csv | | |
