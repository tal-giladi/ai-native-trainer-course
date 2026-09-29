# Fabrikam reference architecture for coding agents (v1)

Two views, in the spirit of the C4 model: a context view (who and what talks to the agent platform) and a container view with trust boundaries. Every arrow that carries prompt or code content is EU-only. Numbers in brackets are the ADRs that decided the element.

## Context view

```mermaid
flowchart LR
    DEV["400 developers<br/>8 teams"] -->|agent sessions| AP["Agent platform<br/>(clients, gateway, MCP, policy)"]
    CI["CI/CD<br/>GitHub Actions"] -->|headless agent jobs| AP
    AP -->|model calls, EU only| PROV["Model providers<br/>primary + fallback"]
    AP -->|read tickets and docs| ATL["Jira / Confluence"]
    AP -->|PRs, reviews| GH["GitHub Enterprise"]
    AP -->|schema reads| SQL["SQL Server (schema only)"]
    IDP["Identity provider<br/>SSO + MFA"] -.->|tokens| AP
    SEC["Security, audit, finance"] -.->|reports| AP
```

## Container view with trust boundaries

```mermaid
flowchart TB
    subgraph DEVZONE["Developer machines (managed devices)"]
        CC["Coding agent client<br/>managed settings: base URL, apiKeyHelper,<br/>MCP allowlist, telemetry flags [0003, 0005]"]
        TR[("Local transcripts<br/>14 days")]
    end
    subgraph CIZONE["CI runners (EU)"]
        HJ["Headless agent jobs<br/>OIDC, 1 h tokens [0003]"]
    end
    subgraph PLAT["Platform zone, EU cluster, private network"]
        GW["Model gateway<br/>per-team keys, quotas, routing,<br/>redaction [0004, 0006, 0007]"]
        MCP["MCP servers<br/>Jira/Confluence (per-user OAuth),<br/>schema (read-only), jira-writes (team bot)"]
        VAULT[("Vault<br/>team keys, provider keys")]
        OTEL["OTel collector (EU)<br/>metrics and events, no prompt text"]
        AUD[("Audit log<br/>metadata + hashes, WORM, 400 days")]
        CLOG[("Content log<br/>redacted, 30 days")]
    end
    subgraph PROVZONE["Model providers (EU regions only) [0001, 0005]"]
        P1["Primary: hyperscaler A<br/>EU geographic profile"]
        P2["Fallback: hyperscaler B<br/>EU multi-region"]
    end
    IDP["IdP (SSO, MFA)"]
    EGR{{"Egress proxy<br/>provider hosts only from gateway [0007]"}}

    CC -->|"prompts, team key"| GW
    HJ -->|"prompts, OIDC-exchanged key"| GW
    CC -->|tool calls| MCP
    CC -.-> TR
    CC -->|metrics| OTEL
    IDP -.->|SSO| CC
    IDP -.->|SSO| MCP
    VAULT -.->|apiKeyHelper| CC
    GW --> VAULT
    GW --> CLOG
    GW --> AUD
    GW --> EGR
    EGR --> P1
    EGR -.->|on primary failure| P2
```

## What the boundaries mean

| Boundary | Crossing | Control |
|---|---|---|
| Developer machine → platform | prompts, tool calls | team key from the vault via `apiKeyHelper`; SSO user recorded by the gateway; managed settings cannot be overridden locally |
| CI → platform | headless prompts | OIDC federation; one-hour credential scoped to the repository's team; own quota |
| Platform → providers | model calls | only the gateway's subnet may reach provider hosts; both routes EU; private endpoints |
| Platform → stores | logs | redaction before any write; audit metadata in write-once storage |
| Agent → tools | MCP calls | per-user OAuth for reads, per-team bot for the few writes, allowlist of servers |

Checked by: `ArchCheck lint solution/architecture.json` (the machine-readable twin of this diagram).
