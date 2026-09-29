---
id: "12.4"
module: 12
minutes: 17
practice_minutes: 60
prerequisites: ["12.3", "12.1"]
objectives:
  - Build a data inventory for a coding-agent platform that lists every copy of prompt data, where it is processed and stored, for how long, and who can read it.
  - Distinguish inference geography from storage geography and find the paths that a primary-route-only residency design misses (fallback, tools, telemetry, feedback, client caches).
  - Explain what commercial training-use and retention terms do and do not cover, and why consumer accounts are a data-handling risk.
  - Orient a security-review conversation in GDPR transfers, the EU AI Act timeline and NIST AI RMF without presenting it as legal advice, and state the cost of an EU-only constraint in model choice and price.
volatility: implementation
sources:
  - title: "Claude Code docs — Data usage (training policy, retention, telemetry, default behaviours by provider)"
    url: https://code.claude.com/docs/en/data-usage
  - title: "Claude Platform docs — Data residency (inference geo vs workspace geo)"
    url: https://platform.claude.com/docs/en/manage-claude/data-residency
  - title: "Amazon Bedrock User Guide — Geographic cross-Region inference"
    url: https://docs.aws.amazon.com/bedrock/latest/userguide/geographic-cross-region-inference.html
  - title: "Google Cloud Blog — Multi-region endpoints for Claude on Vertex AI"
    url: https://cloud.google.com/blog/products/ai-machine-learning/multi-region-endpoints-for-claude-available-on-vertex-ai
  - title: "Regulation (EU) 2016/679 (GDPR), official text on EUR-Lex"
    url: https://eur-lex.europa.eu/eli/reg/2016/679/oj
  - title: "European Commission — AI Act: regulatory framework and timeline"
    url: https://digital-strategy.ec.europa.eu/en/policies/regulatory-framework-ai
  - title: "NIST — AI Risk Management Framework"
    url: https://www.nist.gov/itl/ai-risk-management-framework
last_verified: "2026-09-28"
---

# 12.4 · Data, residency and compliance

## Why it matters

Fabrikam's legal team wrote one sentence into the requirements: *no source code may leave the EU*. The draft architecture met it the way most drafts do: the primary model route ran in an EU region, and everyone moved on. Then the data-protection officer asked for a list of every place a prompt goes. The list had nine rows. Three of them were outside the EU.

This lesson is about that list. Residency, retention and training terms are where architecture meets law, and an engineer's job is not to interpret the law (that is legal's job) but to give legal an accurate map: what data, where, for how long, readable by whom, under which terms. A reviewer cannot approve what you cannot describe.

> [!IMPORTANT]
> This lesson is orientation, not legal advice. Regulations and provider terms are jurisdiction-specific and change; every compliance conclusion for a real organisation belongs to its legal and data-protection functions. Your deliverable is an accurate data inventory and architecture they can reason about.

> [!NOTE]
> Content tags. **Concept** (stable): the data inventory, inference versus storage geography, the "every path" rule, training and retention terms as contract facts, the orientation frameworks. **Implementation** (as of 2026-09, re-verify quarterly): which providers offer which geographies, retention periods, client defaults and flags, the AI Act dates.

## How it works

### The data inventory

Start from one prompt and follow every copy. For a coding agent the copies are more numerous than people expect:

| # | Copy | Contains | Where it lives | How long (default) | Readers |
|---|---|---|---|---|---|
| 1 | Client transcript | full session, code, tool output | developer machine, `~/.claude/projects/` | 30 days, `cleanupPeriodDays` | the developer, anything on the machine |
| 2 | Gateway content log | prompts, responses | gateway's log store | your policy (12.3) | platform on-call |
| 3 | Gateway audit log | metadata, hashes | write-once store | your policy | security |
| 4 | Provider processing | full request | inference region(s) | transient | provider systems |
| 5 | Provider retention | full request | provider storage | per contract (e.g. 30 days standard) | provider trust and safety |
| 6 | MCP servers / tools | tool arguments and results | wherever the server runs | the server's terms | the server's operator |
| 7 | Client telemetry | metrics, events | your collector, or the vendor's | your collector's policy | observability team |
| 8 | Feedback reports | transcript, if a developer sends one | vendor | vendor's feedback retention | vendor |
| 9 | Auxiliary checks | e.g. a hostname for a safety check | vendor endpoint | vendor's terms | vendor |

Rows 8 and 9 are the ones nobody lists. For Claude Code, the documentation is specific ([data usage](https://code.claude.com/docs/en/data-usage)): commercial accounts have a 30-day standard retention, with zero data retention available to qualified Enterprise organisations; transcripts sent with `/feedback` are retained for five years; on Bedrock, Google Cloud and Foundry, telemetry, error reporting and feedback upload are off by default and `/feedback` writes a local file instead; and the WebFetch tool sends the requested *hostname* (not the URL or content) to a vendor endpoint for a safety check regardless of provider, unless you disable it with a setting. None of these is a problem in itself. Each is a row that legal must see.

### Inference geography versus storage geography

"Where does it run?" has two answers. The vendor API separates them explicitly: `inference_geo` decides where the model runs per request (`global` or `us` as of 2026-09), and a workspace's geo decides where data is stored at rest (currently `us` only) ([data residency](https://platform.claude.com/docs/en/manage-claude/data-residency)). On Bedrock, a geographic cross-Region profile keeps inference inside the geography (US, EU, APAC) and data at rest in the source region, but where Bedrock stores data for abuse detection it is stored in the *destination* region, still inside the geography ([geographic cross-Region inference](https://docs.aws.amazon.com/bedrock/latest/userguide/geographic-cross-region-inference.html)). Google Cloud offers `us` and `eu` multi-region endpoints that keep processing in the geography while routing between regions for availability ([Google Cloud](https://cloud.google.com/blog/products/ai-machine-learning/multi-region-endpoints-for-claude-available-on-vertex-ai)).

The rule that follows: **residency is a property of every path, not of the primary route.** The fallback route is a path. Each MCP server is a path, because tool arguments and results are prompt data. The telemetry collector is a path. A developer's personal account is a path.

### Training-use and retention terms

Whether a provider may train on your data is a contract fact, not an architecture fact, but the architecture decides *which contract applies*. Claude Code's documentation states that under commercial terms (Team, Enterprise, API, cloud platforms) Anthropic does not train on code or prompts unless the customer opts in, while consumer plans (Free, Pro, Max) let the user choose, with a five-year retention when training is allowed ([data usage](https://code.claude.com/docs/en/data-usage)). So the enterprise risk is not the enterprise contract: it is a developer using a personal consumer login on company code. The control is identity (12.3): managed settings that force login to the organisation, and a gateway that is the only route to a model.

### Compliance frameworks, as orientation

You will hear these names in a security review. Know what each is *for*:

- **GDPR** ([Regulation (EU) 2016/679](https://eur-lex.europa.eu/eli/reg/2016/679/oj)) governs personal data. Source code is rarely personal data; tickets, logs, commit metadata and customer records pasted into prompts often are. Transfers of personal data outside the EU/EEA are regulated in Chapter V (adequacy decisions, appropriate safeguards). This is why legal cares about rows 5, 6 and 8 of the inventory.
- **The EU AI Act** entered into force on 1 August 2024; obligations for general-purpose AI model providers applied from 2 August 2025; most rules applied from 2 August 2026, with the high-risk system deadlines moved to December 2027 and August 2028 by a 2025 amendment package ([European Commission](https://digital-strategy.ec.europa.eu/en/policies/regulatory-framework-ai)). Model-provider obligations fall mainly on the providers; whether an internal coding-agent deployment falls into any regulated category is a question for legal, not an assumption.
- **NIST AI RMF 1.0** (2023) organises AI risk work into four functions, Govern, Map, Measure and Manage, with a generative-AI profile published in 2024 ([NIST](https://www.nist.gov/itl/ai-risk-management-framework)). It is voluntary and useful as a checklist for *your* governance: Module 11's governance policy is the Govern function in practice.
- **ISO/IEC 42001** (AI management systems) and **SOC 2** reports are what procurement asks vendors for; your job is to know which of the vendor's compliance artifacts cover the service you actually use.

### What "no code leaves the EU" costs

*Intuition.* A residency constraint removes options (12.1) and adds price premiums and feature lag.

*Equation.* With monthly spend $S$ at global pricing, a residency premium $\rho$ on the share $s$ of traffic that needs it:

$$\Delta = \rho \, s \, S$$

*Tiny example.* Fabrikam's primary route costs about \$74,400 a month on an EU geographic profile at standard price; a global profile on the same platform is about 10% cheaper, so the EU constraint costs roughly $0.10 \times 1.0 \times 74{,}400 \approx \$7{,}400$ a month on that route, plus the 10% multi-region premium on the fallback's small share. The larger cost is not money: the vendor's own API (newest models, most features) is eliminated outright, and newly released models reach EU endpoints later.

*Interpretation.* Put both numbers in the ADR: the price and the capability lag. Legal decides whether the constraint is worth it; engineering makes sure they decide with the real figures.

## Show me

Fabrikam's draft against the residency and retention rules:

```text
$ ArchCheck lint break/architecture.json --rules RES,RET
FAIL RES-ROUTE      route 'fallback' (fallback, vendor-api api, default routing) runs inference in 'global', outside eu. A fallback is still a route: on the first outage, prompts go there.
FAIL RET-MAX        gateway-log: keeps raw content 365 days, policy max is 30.
WARN RES-STORE      otel-saas: metadata stored in 'us'. Allowed only if the policy covers metadata; list it in the data inventory.
FAIL RES-MCP        docs-search-saas: MCP server hosted in 'us'. Tool results and arguments are prompt data too.
FAIL fabrikam-v0: 3 failure(s), 1 warning(s)
```

Three paths out of the EU (a fallback that only fires during an outage, a documentation-search tool, a telemetry SaaS), and a content log kept twelve times longer than policy. The primary route was never the problem.

## Try it

Budget: 60 minutes. From `labs/module-12`:

1. Run the command above, then on `solution/architecture.json`. List what changed in the solution for each finding (look at `routes`, `stores`, `mcpServers`).
2. Build the data inventory for Fabrikam's v1 using the nine-row table above: fill in location, retention and readers for each row, citing the ADR or document that establishes it. Rows you cannot fill are findings.
3. Do the same for your own team's current agent setup. Check your client's defaults for rows 1, 7, 8 and 9 against the current [data usage](https://code.claude.com/docs/en/data-usage) page (or your tool's equivalent) rather than from memory.
4. Compute $\Delta$ for your organisation: estimate monthly spend and the premium your residency option carries on your platform.

<details>
<summary>Hint for step 2</summary>

Fabrikam v1: client transcripts 14 days via managed `cleanupPeriodDays`; gateway content log redacted, 30 days, EU; audit metadata write-once, 400 days, EU; inference on the EU geographic profile, fallback on the EU multi-region endpoint; provider retention per the cloud contract; MCP servers all EU, per-user OAuth; telemetry to the EU collector without prompt text; feedback upload off by default on cloud providers (and disabled in managed settings to be explicit); the WebFetch hostname check is a documented exception that legal accepted in writing, or disabled with its setting. Every row cites an ADR (0001, 0004, 0005) or the managed-settings file.
</details>

## Break it

The draft is how residency usually fails: the primary route is compliant, so the design is labelled compliant. Before running `lint`, write down every path by which a prompt or a tool result could leave the EU in `break/architecture.json`. Then run it. Did you find the fallback? Did you count the docs-search MCP server as prompt data?

## Fix it

**Diagnose.** Residency was checked for one route instead of every path. The fallback uses the vendor API's default global routing, so the first provider outage moves prompts out of the EU. The docs-search MCP server is a US SaaS receiving search terms taken from code and tickets. The collector holds metadata in the US, which may be acceptable, but only if legal says so in writing and it appears in the inventory.

**Modify.** Replace the fallback with an EU route (Fabrikam uses hyperscaler B's EU multi-region endpoint, ADR-0001). Remove the US docs-search server; serve documentation search from the EU-hosted Confluence integration. Move the collector to the EU tenant (or record the metadata exception). Reduce content-log retention to 30 days. Set `cleanupPeriodDays` in managed settings and restrict MCP servers to an allowlist, so a team cannot reopen a path by adding a server.

**Rerun.** `$A lint solution/architecture.json --rules RES,RET` passes. The data inventory has no empty cell, and every row cites evidence. Write the result as ADR-0005 (12.5), including the price and capability cost you computed.

## How do I know it works?

- [ ] Your data inventory lists every copy of prompt data, including fallback routes, tools, telemetry, feedback and auxiliary checks, with location, retention and readers.
- [ ] Every route, store and MCP server holding content satisfies the residency constraint; metadata exceptions are written and approved.
- [ ] You can name the contract that governs training use and retention for each provider path, and personal consumer accounts cannot reach company code.
- [ ] The cost of the residency constraint (price and capability) is written down where the decision is recorded.
- [ ] Every implementation claim in the inventory carries a source and an "as of" date.

## Use / don't use

**Use** the data inventory as the first artifact of any enterprise engagement: it is cheap, it finds real problems, and it gives legal something concrete. **Use** geography-scoped routes (geographic profiles, multi-region endpoints) when residency is required and single-region capacity or model availability is too thin.

**Don't** call a design compliant because its primary route is. **Don't** answer legal questions yourself; bring them an accurate map and the options with their costs. **Don't** forget the developer's own machine: local transcripts are copies too.

**Limitations.** Provider geographies, retention periods and client defaults change; this lesson's facts are as of 2026-09 and must be re-verified before any review. Regulation summaries here are orientation only. The lint rules check what the architecture file declares; a component nobody put in the file is invisible to them, which is why the inventory walks the data rather than the diagram.

## Reflect

1. Which row of the inventory would your organisation have left out?
2. Who in your organisation owns the answer to "may this data leave the EU?", and have you ever asked them directly?
3. What would you give up if a residency constraint arrived tomorrow?

## Sources

- [Claude Code docs — Data usage](https://code.claude.com/docs/en/data-usage) — commercial versus consumer training policy; 30-day commercial retention; zero data retention for qualified Enterprise organisations; local transcripts 30 days (`cleanupPeriodDays`); `/feedback` transcripts retained five years; telemetry and feedback defaults by provider; WebFetch hostname safety check.
- [Claude Platform docs — Data residency](https://platform.claude.com/docs/en/manage-claude/data-residency) — inference geo (`global`, `us`) versus workspace geo (`us`); workspace restrictions; 1.1× for US-only inference.
- [Amazon Bedrock — Geographic cross-Region inference](https://docs.aws.amazon.com/bedrock/latest/userguide/geographic-cross-region-inference.html) — requests stay within the geography (US, EU, APAC); data at rest in the source region; abuse-detection storage in the destination region.
- [Google Cloud Blog — Multi-region endpoints for Claude on Vertex AI](https://cloud.google.com/blog/products/ai-machine-learning/multi-region-endpoints-for-claude-available-on-vertex-ai) — `us` and `eu` multi-region endpoints keep processing in the geography with cross-region availability.
- [Regulation (EU) 2016/679 (GDPR)](https://eur-lex.europa.eu/eli/reg/2016/679/oj) — official text; Chapter V on transfers of personal data to third countries.
- [European Commission — AI Act](https://digital-strategy.ec.europa.eu/en/policies/regulatory-framework-ai) — entry into force 1 August 2024; GPAI obligations from 2 August 2025; general application 2 August 2026; revised high-risk deadlines.
- [NIST — AI Risk Management Framework](https://www.nist.gov/itl/ai-risk-management-framework) — AI RMF 1.0 (January 2023), Govern/Map/Measure/Manage; generative AI profile NIST-AI-600-1 (July 2024).
