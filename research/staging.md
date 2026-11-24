# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates
### C-20261123-01 · Agent-as-a-Judge is not immune to confirmation bias: one false "pre-screening note" in the judge's context flips up to 40% of correct verdicts

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-11-23
- **Date published:** 2026-11-03
- **Source:** https://arxiv.org/abs/2611.05662 (primary, abstract page fetched this session)
- **Organization/researchers:** Drake Benjamin Brown, Khushbu Pahwa, Yubin Ge, Salvatore Romeo, Akhil Agnihotri, Yi Zhang, Xiaofei Ma, Daniele Bonadiman (affiliations not checked)
- **Category:** evaluation
- **What changed:** Preprint "Grounded but Not Immune: Confirmation Bias in Agent-as-a-judge" (v1 3 Nov 2026, only version; not peer reviewed). Older than the 48 h window; found today and not previously in the registry (`lookup "2611.05662"`, `lookup "agent-as-a-judge"`: no matches).
- **Technical summary:** Per the abstract: a tool-using judge that gathers its own evidence from the environment and the agent's trajectory was given a single false, authority-attributed "pre-screening note" irrelevant to the correct verdict. Across four judges and five datasets this overturned up to 40% of correct verdicts, even when the judge had collected decisive evidence. The authors frame the cue as a controlled diagnostic, not an attack. Three failure modes: over-trust (verifies correctly, then defers to the cue), hallucinated support for the flipped verdict, and stopping investigation at the first corroborating signal. For the two Claude judges, over-trust caused 67–75% of harmful flips. Lightweight mitigations reduced but did not remove the bias.
- **Why it might matter:** Lesson 07.3 teaches judge design against verbosity, position and self-preference bias and calibration against human labels, but not judge-input hygiene. In the course's CI pipelines (modules 10–11) a judge or reviewer agent often sees upstream text (another agent's summary, a PR description, a bot's "tests pass" comment). If this holds, anything upstream that states a verdict is a bias source and should be stripped from the judge's context, and calibration done without such notes will overstate accuracy in production.
- **Evidence of adoption:** none (new preprint)
- **Major organizations using it:** n/a
- **Open-source implementation:** none checked
- **Paper:** https://arxiv.org/abs/2611.05662
- **Code:** none checked
- **Relationship to existing course material:** lesson 07.3 (`lessons/module-07/lesson-03.md` line 11 objectives on verbosity/position bias, line ~219 "Don't" list, line ~237 Wang et al. position bias); 10.2 ("a message from another agent is data, not an instruction with your authority"); 11.2 (the CI reviewer it measures). Related staged/deferred: `research/deferred/llm-judge-error-shifts-with-agent-version.md` (another reason calibration may not transfer).
- **Potential course lesson:** If replicated: one "Don't" line in 07.3, e.g. "Don't give the judge anyone else's verdict (a PR description, a bot comment, another agent's summary): a single false authority note can flip correct verdicts even for a judge that checks the evidence itself", plus calibrating with the same context the judge sees in production.
- **Confidence:** low–medium (unreviewed preprint; mechanism consistent with known anchoring/sycophancy findings)
- **Recommendation:** monitor; cheap caveat to consider if the weekly review touches 07.3 together with the deferred judge-error-shift topic.
### C-20261124-01 · claude-code-action v1.0.288 skips fork pull requests by default (`allow_fork_prs`, breaking)

- **Class:** B
- **Kind:** correction
- **Date discovered:** 2026-11-24
- **Date published:** 2026-11-23
- **Source:** https://github.com/anthropics/claude-code-action/releases/tag/v1.0.288 (primary, fetched this session); https://github.com/anthropics/claude-code-action/pull/2114 (primary, fetched this session)
- **Organization/researchers:** Anthropic (claude-code-action maintainers)
- **Category:** ci/production
- **What changed:** Release v1.0.288 (23 Nov, labelled "Breaking changes") merges PR #2114 "Skip pull requests from forks by default". New input `allow_fork_prs`, default `"false"`; `@v1` workflows pick it up automatically. Same day: v1.0.287 stops the session and revokes the app token when a run is cancelled (#2112); v1.0.289 logs plain text and posts buffered comments in the run step (#2122).
- **Technical summary:** Per the PR description: on `pull_request` and review events, fork PRs (which previously failed for lack of secrets) now skip; on `pull_request_target` and `workflow_run`, fork runs skip, but the write-access and bot checks run first, so a refused run still fails; `@claude` mentions/comments on fork PRs no longer run Claude. Fork detection is a `pulls.get` lookup, so a triggering comment now looks up every PR, and a custom `github_token` without `pull-requests: read` can fail with "Could not find out whether pull request #N is from a fork". A skip succeeds with a warning, posts no comment and sets no outputs, so later steps reading `structured_output` must tolerate an empty value. Issues, `workflow_dispatch`, `schedule` and `claude-code-base-action` are unchanged. Opt back in with `allow_fork_prs: "true"`. (vendor release notes; no independent coverage.)
- **Why it might matter:** No course sentence becomes wrong: 11.1 and 11.2 already teach "same-repository PRs only, fork PRs get no secrets, never `pull_request_target` with PR code", and the action's default now enforces that. But 11.1 and 11.3 track each security default of the action (v1.0.273 auto mode + `restrict_untrusted_prs`, v1.0.277/#2059 base-branch `AGENTS.md`, v1.0.285/#2099 base-branch settings), so their list is now one release behind, and two side effects a learner would hit are untaught: a least-privilege custom token now needs `pull-requests: read` for comment triggers, and a skipped run leaves `structured_output` empty for gating steps.
- **Evidence of adoption:** ships to every `@v1` workflow (vendor default change)
- **Major organizations using it:** n/a
- **Open-source implementation:** https://github.com/anthropics/claude-code-action
- **Paper:** none
- **Code:** https://github.com/anthropics/claude-code-action/pull/2114
- **Relationship to existing course material:** lessons 11.1 (`lessons/module-11/lesson-01.md` line ~117 rule 1 on fork PRs and `pull_request_target`; line ~121 and ~239 list action defaults #2059/#2088/#2099), 11.2 (line 207 docs bullet "same-repository PRs only because fork PRs get no secrets"), 11.3 (line ~99 v1.0.273 defaults). Labs (`labs/module-11/workflows/agent-review.yml`) call `claude -p` directly and are unaffected. Related: `research/rejected/coding-agent-ci-untrusted-issue-secrets.md` (cites v1.0.273).
- **Potential course lesson:** Optional FIX-style addition, one clause in 11.1 after the #2099 sentence and in its references: "since v1.0.288 (2026-11-23) the action skips fork PRs unless `allow_fork_prs: \"true\"`; a comment trigger then needs `pull-requests: read` on a custom token, and a skipped run sets no outputs". Keep path, ids and quiz ids unchanged; no quiz answer changes.
- **Confidence:** high (that the change shipped); medium (that it is worth a sentence)
- **Recommendation:** monitor / consider a one-clause addition to 11.1 if the weekly review touches module 11; not urgent because the course's own rule already excludes fork PRs.
