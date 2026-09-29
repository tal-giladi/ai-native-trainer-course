# Instructor notes — 12.3 Identity, secrets, networking and audit

**Teaching objective.** Students map each identity in an agent platform to a per-team, short-lived, run-time credential; quantify a leak by reach and exposure window; separate the powers to use, to change policy and to read audit; treat the network as a second layer; and design an audit trail that holds no secrets.

**Likely confusion.** "Agent session" as an identity. Students think the agent is either the developer or a service account. Emphasise delegation: the session acts with the developer's authority plus whatever credentials it is handed, so each credential given to a session is authority the model can exercise (09.1).

**Common misconception.** "Our network is private, so a shared key is fine." Cite the zero-trust premise and run the reach calculation: the private network does nothing for attribution, revocation or the runaway-job problem from 12.2.

**Key analogy.** Building access badges. One master key taped under the reception desk (shared, long-lived, stored in a known place) versus badges per department that expire nightly and log every door. And the visitor log must not record the visitor's PIN.

**Common failure.** Students redact the log and forget to rotate the credentials that were in it. Also: they design the audit log but give the platform team delete rights "for GDPR". Separate retention (automatic expiry of content) from deletion rights (nobody deletes audit metadata ad hoc).

**Expected exercise outcome.** Each FAIL mapped to an identity and to reach or lifetime; reach and exposure computed for four draft identities (all 8/8; exposures of about 182 days) and their replacements (1/8; 30 minutes to 15 days); a redacted log with zero secrets and hashes kept; a four-row roles table ending with "delete audit: no one".

**Extension exercise.** Write the managed-settings fragment that delivers `ANTHROPIC_BASE_URL`, an `apiKeyHelper`, telemetry to an EU collector with prompt logging off, an MCP allowlist, and disables bypass mode. Check each key against the current docs and note which ones a developer could still override.

**Discussion question.** Per-user attribution means the gateway knows which developer sent each prompt. What does that change for trust within teams, and how do you communicate it before rollout?
