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
- **Source:** https://github.com/anthropics/claude-code-action/releases/tag/v1.0.277 (primary, fetched this session); https://github.com/anthropics/claude-code-action/pull/2059 (primary, fetched this session); update 2026-11-18: https://github.com/anthropics/claude-code-action/releases/tag/v1.0.279 and https://github.com/anthropics/claude-code-action/pull/2088 (primary, fetched 2026-11-18)
- **Organization/researchers:** Anthropic (claude-code-action maintainers)
- **Category:** ci/production
- **What changed:** Release v1.0.277 (16 Nov, labelled breaking; `@v1` users get it automatically): "Use the base branch's AGENTS.md on pull requests" (#2059). PR #2059 (merged 2026-11-16): "On a pull request, Claude follows the base branch's `AGENTS.md`. If the base branch has none, the file is removed from the working tree for the run." The PR's version is kept at `.claude-pr/AGENTS.md` and still shows in `git diff origin/<base>...HEAD`. The PR says this is done "as with `CLAUDE.md`", so `CLAUDE.md` was already taken from base. Same release: "Use the base branch's npm and Bun config on outside pull requests" (#2020, #2026). No opt-out is mentioned. **Update 2026-11-18:** v1.0.279 (17 Nov) extends the same rule: "Use the default branch's config on pull requests from authors without write access" (#2088, merged 2026-11-17). Per #2088, when the PR author lacks write access (a failed access lookup counts as no access) and the PR does not target the default branch, the restored files (`.claude/`, `.mcp.json`, `CLAUDE.md` "and the rest of that list", npm/Bun config) come from the `base_branch` input or the default branch, not the PR's base, so a PR into a release branch cannot pick up that branch's weaker config. Part of `restrict_untrusted_prs`; `restrict_untrusted_prs: false` turns it off. The changed files are left out of Claude's own commits.
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
- **Recommendation:** review for FIX in 11.1 and `labs/module-11/workflows/agent-review.yml`; consider together with the CI-trust deferred topics on 2026-11-22. (2026-11-18: two releases in two days moving instruction/config files to a trusted branch strengthens the case; if 11.3's v1.0.273 note is touched, mention that `restrict_untrusted_prs` now also governs which branch the config comes from.)

### C-20261120-01 · LLM-judge error shifts with the agent version: calibration measured on an old agent may not transfer to its upgrade

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-11-20
- **Date published:** 2026-09-28
- **Source:** https://arxiv.org/abs/2609.34198 (primary, abstract page fetched this session)
- **Organization/researchers:** Jiapeng Li (single author; affiliation not checked)
- **Category:** evaluation
- **What changed:** Preprint "Frozen Judges, Moving Agents: Version-Dependent LLM-Judge Error and the Limits of Judge-Assisted Agent Evaluation" (v1 28 Sep 2026, v2 29 Sep 2026 adding an OpenHands validation). Not peer reviewed. Older than the 48 h window; found today and not previously in the registry.
- **Technical summary:** Per the abstract: across 35 coding-agent submissions on 250 SWE-bench Verified issues, two tau-bench customer-service agents (155 tasks) and 1,106 expert-labelled AgentRewardBench trajectories, every judge tested rejected task-conditioned error invariance, i.e. a judge's error rates differ between agent versions even on the same tasks. On SWE-bench, eight judge-only intervals declared upgrades that execution-based intervals could not establish, although judge and execution rankings correlated at 0.71–0.79. The rate at which judges accepted failed patches rose with agent capability (observational; the author's follow-up reports mean Spearman +0.944). Carrying calibration measured on an old version over to the new one raised SWE-bench comparison error from 3.8 to 19.5 points. A paired audit of current outputs narrowed intervals by only 5% at 80 labelled tasks. The author recommends paired audits of current outputs over judge-only release decisions or transported calibration; independent human patch review is still pending.
- **Why it might matter:** Lesson 07.3 teaches correcting a judged pass rate with TPR/FPR measured on a calibration set and says the correction holds only if TPR and FPR are the same on both sets, so "recalibrate when the task or the judge changes". If this result holds, the *agent* changing (the very thing an upgrade comparison measures) is also a reason to recalibrate, and a judge's false-pass rate may grow as the agent gets better, which biases upgrade decisions toward "the new version is better".
- **Evidence of adoption:** none (new preprint)
- **Major organizations using it:** n/a
- **Open-source implementation:** none checked
- **Paper:** https://arxiv.org/abs/2609.34198
- **Code:** none checked
- **Relationship to existing course material:** lesson 07.3 (`lessons/module-07/lesson-03.md` lines ~90–102: Rogan–Gladen correction, Lee et al. ICML 2026, "recalibrate when the task or the judge changes"); 07.5 (eval comparisons); 02.5 (model comparison). Registry: no prior entry (`lookup "Frozen Judges"`).
- **Potential course lesson:** If confirmed by independent work: one sentence in 07.3 extending "recalibrate when the task or the judge changes" to "or the agent version you are comparing", plus a note that judge-only upgrade decisions should be checked with execution-based grading or a fresh human-labelled sample of the new version's outputs.
- **Confidence:** low–medium (single-author, unreviewed preprint; mechanism is plausible and consistent with the lesson's own invariance assumption)
- **Recommendation:** monitor; revisit if peer-reviewed or replicated. Cheap to adopt as a caveat if the weekly review touches 07.3.
