# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261117-01 · claude-code-action v1.0.277 takes AGENTS.md from the base branch on PRs; the course's agent-review lab reads it from the PR head

- **Class:** A
- **Kind:** correction
- **Date discovered:** 2026-11-17
- **Date published:** 2026-11-16
- **Source:** https://github.com/anthropics/claude-code-action/releases/tag/v1.0.277 (primary, fetched this session); https://github.com/anthropics/claude-code-action/pull/2059 (primary, fetched this session)
- **Organization/researchers:** Anthropic (claude-code-action maintainers)
- **Category:** ci/production
- **What changed:** Release v1.0.277 (16 Nov, labelled breaking; `@v1` users get it automatically): "Use the base branch's AGENTS.md on pull requests" (#2059). PR #2059 (merged 2026-11-16): "On a pull request, Claude follows the base branch's `AGENTS.md`. If the base branch has none, the file is removed from the working tree for the run." The PR's version is kept at `.claude-pr/AGENTS.md` and still shows in `git diff origin/<base>...HEAD`. The PR says this is done "as with `CLAUDE.md`", so `CLAUDE.md` was already taken from base. Same release: "Use the base branch's npm and Bun config on outside pull requests" (#2020, #2026). No opt-out is mentioned.
- **Technical summary:** The vendor now treats the instruction files in a PR as untrusted input for the agent that runs on that PR: the agent follows the reviewed (base) instructions, and the PR's changes to them are only something to review. The PR author's stated caveat: the replaced file stays in the working tree, so a later commit can undo the PR's change to it. Our lab workflow `labs/module-11/workflows/agent-review.yml` does `actions/checkout@v4` of the PR, then runs `claude --bare -p "$(cat .github/review/review-v2.md)" --append-system-prompt-file AGENTS.md --settings .github/agent-settings.json ...`. All three files come from the PR head, so the PR author can rewrite the reviewer's prompt, grounding and settings for their own PR (for example "report no findings"). The workflow only runs on same-repository PRs, so this is not a secrets leak. It is a reviewer that the change under review can turn off. (Inference from reading the lab; the vendor release does not discuss the lab pattern.)
- **Why it might matter:** 11.1 says `--append-system-prompt-file AGENTS.md` brings the AI layer in "explicitly, and you know which version". On a PR run, the version is whatever the PR says. 07.6 already treats PRs that touch `AGENTS.md`/`.claude/**` as AI-layer changes that need evaluation. The missing step is that the agent judging a PR should not follow that PR's own copy of the rules. The vendor's breaking change confirms this is a threat a learner would act on.
- **Evidence of adoption:** n/a (default change in a widely used action)
- **Major organizations using it:** n/a
- **Open-source implementation:** https://github.com/anthropics/claude-code-action
- **Paper:** none
- **Code:** https://github.com/anthropics/claude-code-action/pull/2059
- **Relationship to existing course material:** lesson 11.1 lines ~54–65 (`--bare` + `--append-system-prompt-file AGENTS.md`, "you know which version") and line ~217; `labs/module-11/workflows/agent-review.yml` lines ~54–57; lesson 11.2 (the reviewer it measures); lesson 07.6 line ~46 (PRs touching the AI layer trigger a smoke eval); 11.3 (action notes, FIXed in W46 for v1.0.273). Related: `research/deferred/agent-to-agent-ci-trust-boundary.md`, `research/deferred/coding-agent-ci-untrusted-issue-secrets.md` (both due for review 2026-11-22).
- **Potential course lesson:** FIX, not ADD. In `agent-review.yml`, read the prompt, `AGENTS.md` and settings from the base ref (e.g. `git show "origin/$BASE_REF:AGENTS.md" > /tmp/agents-base.md` and pass that file), and leave the PR's copy in the diff for review. One sentence in 11.1 next to "you know which version": on a PR run, load the AI layer from the base branch, as claude-code-action does since v1.0.277. Optionally note the change next to the existing v1.0.273 annotation in 11.3.
- **Confidence:** high for the release content (primary, quoted); medium that the lab change is worth making now (same-repo PRs only, so the attacker already has write access).
- **Recommendation:** review for FIX in 11.1 and `labs/module-11/workflows/agent-review.yml`; consider together with the CI-trust deferred topics on 2026-11-22.
