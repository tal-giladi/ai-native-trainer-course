# Agent-to-agent trust boundary failure in CI (Google adk-python)

- **Topic ID:** agent-to-agent-ci-trust-boundary
- **Status:** WAIT
- **Next review:** 2026-11-22
- **Course change:** none
- **Candidates:** C-20261106-01

## Summary

Pillar Security showed that a public, low-privilege triage bot in `google/adk-python` could be prompt-injected into posting an `@gemini-cli` comment; because the bot's account had Collaborator rank, the dispatch workflow treated it as a trusted maintainer trigger and ran a privileged, unscoped agent holding a write-scoped `GITHUB_TOKEN`. Google removed the workflows (commit 66730e9, 2026-07-21). The pattern: one agent's output authorizing another, with a bot account's repository rank standing in for a human decision.

## Evidence

- https://www.pillar.security/blog/ill-just-call-you-agent-to-agent-privilege-boundary-failures-in-ci-cd-on-googles-adk-repository — primary research write-up (2026-08-03), read by the daily run.
- Independent coverage found 2026-11-08: SecurityWeek (https://www.securityweek.com/gemini-agent-to-agent-attack-exposed-secrets-enabled-pull-request-tampering/), Cloud Security Alliance research note (https://labs.cloudsecurityalliance.org/research/csa-research-note-google-adk-trustissues-agent-injection-202/). No CVE; Google rated it below its reward bar (a maintainer merge was still needed).

## What would change the decision

Mostly covered already: 10.2 says a message from another agent is data, not an instruction with your authority, and that trust boundaries run between agents; 11.3 covers the Claude Code action's bot-actor check. Review on 2026-11-22 together with `coding-agent-ci-untrusted-issue-secrets` and `cross-vendor-coding-agent-injection-cves`: if one short case-study sentence in 09.4 (ASI03) or 10.2 is warranted, this is the strongest-documented example for "a bot account's rank is not a human approval".

## History

- 2026-11-08 — WAIT — well-documented real case of a pattern 10.2 already states; decide as part of the CI/case-study pool review on 2026-11-22 rather than add piecemeal — weekly/2026-W45.md — candidates C-20261106-01
