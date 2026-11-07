# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261103-01 · Claude Code adds `permissions.blockWritesOutsideWorkingDirectories`

- **Class:** A
- **Kind:** new-topic
- **Date discovered:** 2026-11-03
- **Date published:** 2026-10-30 (shipped in v2.1.314)
- **Source:** https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.314 entry); version/date cross-checked against https://github.com/anthropics/claude-code/releases/tag/v2.1.314 (published 30 Oct 2026)
- **Organization/researchers:** Anthropic (Claude Code team)
- **Category:** agent-config/context (cross-cutting with agent-security)
- **What changed:** A new permission setting, `permissions.blockWritesOutsideWorkingDirectories`, makes the Write, Edit and NotebookEdit tools refuse any path outside the session's working directories, in **every** permission mode (including `acceptEdits`/`bypassPermissions`-style modes where file writes were previously not path-checked against the workspace boundary).
- **Technical summary:** Opt-in boolean in `.claude/settings.json` (or managed settings) under `permissions`. When set, the three file-writing tools do a workspace-boundary check unconditionally, independent of which permission mode is active, closing the class of bug where a permissive mode (or a mode that doesn't prompt) let a write tool touch a path outside the project.
- **Why it might matter:** Module 9 lesson 5 teaches exactly this gap today: "The sandbox only confines the commands it runs. The agent's own file tools are a separate path." It then documents two real incidents in this same gap — a write-time symlink TOCTOU (GHSA-5j29-h97v-84ch, fixed 2.1.129) and an Edit-tool symlink TOCTOU from inside the sandbox (GHSA-5xjx-9qx6-jcrv, fixed 2.1.251) — as things to defend against with layered controls, without a direct "block all writes outside the workspace, in every mode" switch. This setting is close to exactly that control and would strengthen the "Limitations" paragraph's defense-in-depth stack and the Module 8 permission-configuration material (08.2, 08.4) with one more concrete, official layer.
- **Evidence of adoption:** Official changelog entry, not a vendor marketing claim; shipped (not just announced).
- **Major organizations using it:** n/a (CLI feature, opt-in setting)
- **Open-source implementation:** n/a (closed-source CLI; setting documented in changelog only — code.claude.com/docs/en/permissions could not be reached from this session's network allowlist to cross-check the permissions reference page itself)
- **Paper:** none
- **Code:** none
- **Relationship to existing course material:** lessons/module-09/lesson-05.md ("Limitations" paragraph, the three cited CVE rows); lessons/module-08/lesson-02.md and lesson-04.md (permission configuration). `lookup` for "block writes outside working", "write outside the project", "sandbox" and "working directory" found no existing mention of this setting — not a duplicate.
- **Potential course lesson:** not a new lesson. Candidate FIX/addition for weekly review: one sentence in 09.5's "Limitations" paragraph (and/or a settings-table row in 08.2/08.4) naming `permissions.blockWritesOutsideWorkingDirectories` as the official mode-independent mitigation for the write-outside-workspace gap, with a note that it is opt-in (off by default) so still needs to be turned on.
- **Confidence:** medium — the changelog is primary and the version/date is confirmed via GitHub Releases, but the permissions reference page itself (code.claude.com) was unreachable this session, so exact config syntax/defaults are from the changelog bullet only, not the full doc page.
- **Recommendation:** review for FIX/addition to lessons/module-09/lesson-05.md and lessons/module-08/lesson-02.md or lesson-04.md; re-verify config syntax against https://code.claude.com/docs/en/permissions once reachable, before writing the lesson sentence.

### C-20261105-01 · MCP TypeScript SDK: server bearer auth accepted tokens issued for another service (audience not checked)

- **Class:** A
- **Kind:** new-topic
- **Date discovered:** 2026-11-05
- **Date published:** 2026-10-05
- **Source:** https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-rvq5-wwqv-78pq (primary GitHub advisory, fetched this session)
- **Organization/researchers:** `modelcontextprotocol/typescript-sdk` maintainers; reported by gdegrange, published by maxisbey
- **Category:** mcp/tool-protocols (cross-cutting with agent-security)
- **What changed:** The MCP TypeScript SDK's server-side bearer-auth helpers (`requireBearerAuth()` / `verifyBearerToken()`) had no built-in way to check that an incoming access token's audience (`aud`) was the MCP server itself. If one authorization server issued tokens for several services, a token valid for a *different* service was accepted by the MCP server too. CVE-2026-111771, CWE-863 (Incorrect Authorization), CVSS 3.1 6.8 (Moderate, AV:N/AC:H/PR:L/UI:N/S:U/C:H/I:H/A:N).
- **Technical summary:** Affected `@modelcontextprotocol/sdk` 1.6.0–1.31.0, `@modelcontextprotocol/server` 2.0.0–2.2.0, `@modelcontextprotocol/express` 2.0.0–2.0.1, `@modelcontextprotocol/server-legacy` 2.0.0–2.3.0. Fixed in sdk 1.32.0 / server 2.3.0 / express 2.0.2 / server-legacy 2.3.1, via a new **opt-in** `expectedResource` option on `requireBearerAuth()`, paired with `verifyAccessToken()` returning the token's audience as `AuthInfo.resource`. Upgrading alone does not turn the check on — setting only one side makes every request fail with 401. Unpatched servers can mitigate by checking `aud` themselves inside `verifyAccessToken()`.
- **Why it might matter:** lessons/module-08/lesson-01.md (~line 100) already states this as a spec MUST: "the server **MUST** check that a token was issued for *it* (audience)... A server **MUST NOT** accept or pass through tokens issued for anything else." This CVE is a concrete instance of the *reference* MCP SDK itself not enforcing that MUST by default until patched in October 2026 — audience validation existed only as an opt-in flag, so a server built from the SDK's own quick-start bearer-auth helper was non-compliant with the spec's own authorization rule until the app developer wired up both ends by hand. It is the server-side mirror of the client-side issuer-binding CVE already accepted into the course on 2026-10-25 (`research/accepted/mcp-client-oauth-issuer-binding.md`), and gives the lesson's existing MUST statement a second concrete, patched, primary-sourced case: "the spec says MUST; reference-implementation defaults didn't enforce it until this patch."
- **Evidence of adoption:** Official GitHub Security Advisory on the reference SDK repository; not a vendor marketing claim. Shipped fix, not just an announcement.
- **Major organizations using it:** n/a (open-source SDK; used by any custom MCP HTTP server built on the TypeScript SDK)
- **Open-source implementation:** https://github.com/modelcontextprotocol/typescript-sdk
- **Paper:** none
- **Code:** https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-rvq5-wwqv-78pq
- **Relationship to existing course material:** lessons/module-08/lesson-01.md (audience-validation MUST paragraph); references/research-log.md row for the client-side mirror CVE GHSA-6qxp-vccf-f47h; `research/accepted/mcp-client-oauth-issuer-binding.md`. `lookup "audience"` / `"RFC 8707"` / `"token passthrough"` found only the existing spec-level claim, no mention of this CVE — not a duplicate.
- **Potential course lesson:** not a new lesson. Candidate addition for weekly review: one sentence/footnote in lessons/module-08/lesson-01.md's audience-validation paragraph citing this CVE as the server-side case study, paired with the already-accepted client-side one.
- **Confidence:** high — primary GitHub advisory read directly; CVE id, CVSS vector, affected/fixed version ranges and the opt-in mechanism are all stated in the advisory text itself.
- **Recommendation:** review for a one-sentence addition to lessons/module-08/lesson-01.md pairing this with the existing accepted client-side CVE; low risk, high fit — a second primary-sourced concrete instance of a rule the lesson already states as a spec MUST.

### C-20261105-02 · MCP TypeScript SDK: shared server/transport instance delivered one HTTP client's messages to another

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-11-05
- **Date published:** 2026-10-15
- **Source:** https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-jm63-cq47-j5jp (primary GitHub advisory, fetched this session)
- **Organization/researchers:** `modelcontextprotocol/typescript-sdk` maintainers; reported by DavidCarliez and legitu1337, published by maxisbey
- **Category:** mcp/tool-protocols
- **What changed:** When an app built on the MCP TypeScript SDK created one stateless HTTP transport or one `McpServer`/`Server` instance at startup and let multiple HTTP clients share it (a pattern the SDK's own earlier README examples for Hono/Express/Fastify/Node showed), the SDK matched responses and notifications to connections using only the JSON-RPC request `id`. Every client's own id counter starts at 0, so ids collide across clients and one client could receive another client's responses/notifications. CVE-2026-111770, CWE-362 (shared-resource race), CVSS 3.1 7.7 (High).
- **Technical summary:** Affected `@modelcontextprotocol/server` 2.0.0–2.2.0; not affected: `createMcpHandler()` used with a per-call factory, stdio servers, or `@modelcontextprotocol/sdk` >= 1.26.0. Fixed in server 2.3.0, which now refuses to be shared across clients (so an unmigrated app starts failing requests after upgrading, by design); the advisory recommends a per-request or per-session (`sessionIdGenerator`) server/transport instead.
- **Why it might matter:** a concrete, patched cross-client data-leak bug in the reference MCP server SDK, caused by a shared-mutable-state antipattern the SDK's own older documentation examples encouraged. Adjacent to but distinct from the confused-deputy/token-passthrough material already in lessons/module-08/lesson-01.md — this is message routing, not authorization. The course's labs use the MCP **C# SDK**, not this TypeScript SDK, so the fix itself doesn't correct a lab claim, but "don't share one server/transport instance across untrusted clients — the SDK's own examples got this wrong" is a reusable point for building an MCP server in any language.
- **Evidence of adoption:** Official GitHub Security Advisory; shipped fix.
- **Major organizations using it:** n/a (open-source SDK)
- **Open-source implementation:** https://github.com/modelcontextprotocol/typescript-sdk
- **Paper:** none
- **Code:** https://github.com/modelcontextprotocol/typescript-sdk/security/advisories/GHSA-jm63-cq47-j5jp
- **Relationship to existing course material:** lessons/module-08/lesson-03.md (the MCP C# SDK server lab) teaches building an MCP server but not this failure mode. `lookup "shared transport"` / `"session id"` found nothing — not a duplicate, but the course's SDK of record is the C# SDK, so lower priority than C-20261105-01.
- **Potential course lesson:** monitor; worth a mention only if module-08's lab material adds a "common mistakes building an MCP server" subsection, or if the MCP C# SDK is found to have (or had) an analogous per-client-sharing footgun.
- **Confidence:** high — primary advisory read directly.
- **Recommendation:** monitor; re-check whether `modelcontextprotocol/csharp-sdk` (pinned 2.2.0 in labs/module-08) has an analogous shared-instance footgun, since that is the SDK the labs actually use.

### C-20261106-01 · Google adk-python: agent-to-agent trust/identity boundary failure in CI (triage agent spoofed into triggering a maintainer-only agent)

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-11-06
- **Date published:** 2026-08-03 (Pillar Security blog publication; underlying workflows removed/fixed by Google 2026-07-02, commit confirmed 2026-07-21)
- **Source:** https://www.pillar.security/blog/ill-just-call-you-agent-to-agent-privilege-boundary-failures-in-ci-cd-on-googles-adk-repository (primary, fetched this session)
- **Organization/researchers:** Pillar Security (research team); target: Google `google/adk-python` repository
- **Category:** multi-agent (cross-cutting with agent-security)
- **What changed:** A public-facing, low-privilege triage agent (GitHub account `adk-bot`, with Collaborator-level privileges) in `google/adk-python`'s PR/issue automation could be manipulated by an attacker-crafted PR/issue comment (a prompt injection disguised as a "contribution guideline" step) into posting a comment starting with `@gemini-cli`. The repo's `gemini-dispatch.yml` routed that comment to `gemini-invoke.yml`, a workflow meant to be gated to maintainers/collaborators — but because the triage agent itself posted as a Collaborator account, its comment satisfied the trust gate meant for humans. The invoked workflow had no tool scoping, so the privileged agent could run shell commands and read a `GITHUB_TOKEN` with `issues:write`/`pull-requests:write`, enough to edit comments, post/approve reviews and dismiss reviews as `github-actions[bot]` — faking an approval trail on a malicious PR (a maintainer still had to merge). A second, related issue in `issue-analyze.yml`/`issue-fix.yml`: the `run_command` denylist in `scripts/run_antigravity.py` only blocked `gh`/`git` by name, but `git` itself can run arbitrary code via `core.hooksPath` or `!`-prefixed aliases, and the allowed binaries ran with a long-lived `adk-bot` PAT and a GCP service-account key reachable from the same workflow.
- **Technical summary:** The core bug is an identity/trust-boundary failure between two automated agents: a workflow-gating check ("is this commenter a maintainer or collaborator?") designed to authenticate a *human* decision was satisfied by *another bot's* comment, because the bot's GitHub account itself held Collaborator rank. This is a confused-deputy pattern one level up from the usual injected-tool-call case: the attacker never talks to the privileged agent directly, only to the low-privilege one, which then acts as an unwitting, trusted-looking introducer. No CVE was assigned. Google's security team called the first report's impact below its reward bar (a maintainer merge was still required, and the path relied on social engineering) and gave an Honorable Mention; it confirmed the second report's workflows were removed/fixed by commit 66730e9 (2026-07-21), citing an added internal Copybara import/review step as a mitigating factor. VRP verdict was still pending at the time Pillar published.
- **Why it might matter:** lessons/module-09/lesson-04.md already teaches excessive agency and cites OWASP's 2026 "Top 10 for Agentic Applications" ASI02 (Tool Misuse) and ASI03 (Identity & Privilege Abuse) as sources, and lessons/module-10 (handoffs between agents) teaches that "a handoff is everything the next agent will ever know about the work so far." This case is a concrete, real-world instance of exactly ASI03 playing out *between two automated agents in CI*, not between a human and one agent: a privilege/trust boundary that checks "is the sender a collaborator" without asking "is the sender a human or another bot acting on attacker-supplied text" is the handoff-trust gap the course already names in the abstract, now with a named repo, named workflows and a named mitigation (remove tool scoping gaps; don't let a bot account's organizational rank substitute for a human-trust check).
- **Evidence of adoption:** Independent security-research blog post (not a vendor marketing claim) describing a direct engagement with Google's VRP; corroborated by secondary coverage (SC World, The Register, Cloud Security Alliance research note) describing the same mechanism and Google's confirmed remediation. Not a shipped CVE/advisory, and Google treated it as non-rewardable, which tempers how "confirmed" the severity framing is — logged as class B, not A, for that reason.
- **Major organizations using it:** Google (the affected repository is Google's own open-source Agent Development Kit for Python)
- **Open-source implementation:** https://github.com/google/adk-python
- **Paper:** none
- **Code:** none (workflow YAML/script details only; no PoC repo published)
- **Relationship to existing course material:** lessons/module-09/lesson-04.md (OWASP ASI02/ASI03 citations, excessive-agency framing); lessons/module-10/lesson-02.md (handoff trust). `lookup "privilege escalation"`, `"sub-agent trust"`, `"CI agent"`, `"adk-python"`, `"agent-to-agent"` found the existing excessive-agency/handoff material but no mention of this case or of bot-account-as-trust-anchor as a failure mode — not a duplicate. Distinct from `research/deferred/coding-agent-ci-untrusted-issue-secrets.md` (WAIT), which is about the lethal-trifecta/secrets-exfiltration path, not inter-agent identity spoofing.
- **Potential course lesson:** not a new lesson. Candidate addition for weekly review: a short case-study callout in 09.4 (next to the existing ASI03 citation) and/or 10.2 (handoff trust), illustrating "a bot account's repo permissions are not proof a human approved anything" — relevant to any team wiring a second, more-privileged agent to act on a first agent's comment/label/handoff.
- **Confidence:** medium-high — primary blog post read directly and quoted Google's own confirmation text; mechanism is clear and specific (named workflows, named commit), but no CVE/advisory exists to cross-check severity, and the finding is three months old, discovered today only because this week's rotation reached the multi-agent/CI category.
- **Recommendation:** monitor/review for a short 09.4 or 10.2 case-study addition at the next weekly review; low urgency since Google already removed/fixed the specific workflows and no equivalent gap is currently claimed to be present in any tool the course's labs use.

### C-20261107-01 · Claude Code adds `permissions.denyInsteadOfPrompting` (deny on would-block prompts, in terminal sessions, subagents and teammates)

- **Class:** A
- **Kind:** new-topic
- **Date discovered:** 2026-11-07
- **Date published:** 2026-11-06 (shipped in v2.1.319)
- **Source:** https://raw.githubusercontent.com/anthropics/claude-code/main/CHANGELOG.md (v2.1.319 entry, fetched this session); version/date cross-checked against https://github.com/anthropics/claude-code/releases/tag/v2.1.319 (shown as published "06 Nov 17:04", same release cycle as the changelog entry)
- **Organization/researchers:** Anthropic (Claude Code team)
- **Category:** agent-config/context (cross-cutting with multi-agent)
- **What changed:** A new permission setting, `permissions.denyInsteadOfPrompting`, makes a tool call that would otherwise block on an interactive permission prompt get denied immediately instead, and the turn continues — in terminal sessions, subagents and teammates (agent teams) alike. Changelog text: "Added `permissions.denyInsteadOfPrompting` setting: a tool call that would wait on a permission prompt is denied and the turn continues, in terminal sessions, subagents and teammates."
- **Technical summary:** Opt-in boolean, set in `.claude/settings.json` (or managed settings) under `permissions`, same family as `blockWritesOutsideWorkingDirectories` (staged `C-20261103-01`) and `allow`/`ask`/`deny` rules. Before this setting existed, a tool call needing a prompt in a context that cannot show one (a subagent, a teammate in an agent team) presumably either blocked/stalled or depended on whatever the ambient permission mode resolved to; this setting makes "deny and keep going" an explicit, named, mode-independent behavior for exactly those non-interactive-but-not-headless contexts, not only for `-p`/`dontAsk`.
- **Why it might matter:** lessons/module-11/lesson-01.md already teaches this exact mechanism for headless mode by name: "`--permission-mode dontAsk` + `--allowedTools` | Prompts. Every call that would have asked is denied; the tool surface is exactly the list," and its instructor notes warn "`dontAsk` is safe for your credentials, not for your conclusions: it converts every missing permission into a silent denial the model can narrate around." Modules 10.1/10.2 teach subagents and agent teams (teammates) as topologies but say nothing about what happens when one of them hits a permission prompt it cannot show — a gap this setting now has an official, named answer to, and a direct way to extend 11.1's "watch `permission_denials`, don't trust a success summary" lesson from headless `-p` runs to interactive sessions that spawn subagents/teammates. `lookup "denyInsteadOfPrompting"`, `"permission mode"`, `"subagent" + "permission"` confirm no lesson currently names this setting or states what a subagent/teammate does today by default on a would-block prompt — not a duplicate.
- **Evidence of adoption:** Official changelog entry, not a vendor marketing claim; shipped (not just announced).
- **Major organizations using it:** n/a (CLI feature, opt-in setting)
- **Open-source implementation:** n/a (closed-source CLI; setting documented in changelog only — the permissions reference page was not independently re-fetched this session to confirm exact default behavior for subagents/teammates *without* the setting, which the lesson addition would need)
- **Paper:** none
- **Code:** none
- **Relationship to existing course material:** lessons/module-11/lesson-01.md and lesson-01.instructor.md (dontAsk / permission_denials); lessons/module-10/lesson-01.md and lesson-02.md (subagents, agent teams). `research/staging.md` C-20261103-01 (same settings family, same release wave, previous week).
- **Potential course lesson:** not a new lesson. Candidate addition for weekly review: one sentence in 10.1 or 10.2 naming `permissions.denyInsteadOfPrompting` as the official control for "what happens when a subagent/teammate hits a prompt it can't show," cross-referenced to 11.1's existing `dontAsk`/`permission_denials` material, which already makes the same point for headless runs.
- **Confidence:** medium — changelog is primary and the release page cross-checks the version, but the exact pre-setting default behavior for subagents/teammates (block? silently deny already? bubble to the parent session?) is not stated in the changelog bullet itself and needs the permissions reference page before writing the lesson sentence.
- **Recommendation:** review for a small addition to lessons/module-10/lesson-01.md or lesson-02.md; verify the pre-setting default behavior against https://code.claude.com/docs/en/permissions before writing the sentence, same caveat as the still-open `C-20261103-01`.
