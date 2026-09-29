# Instructor notes — 03.1 The fourth citizen

**Teaching objective.** Students leave treating agent instruction files as code with owners, review, tests and a changelog — and can configure CODEOWNERS so review actually binds.

**Likely confusion.** "Isn't CLAUDE.md just documentation?" Documentation is read by humans who can judge it; the rules file is read by an agent that acts on it in every session. Use the 1,440-sessions-a-month arithmetic to make the blast radius concrete.

**Common misconception.** "The most specific CODEOWNERS pattern wins." It is the *last matching* pattern. Many real repositories have their catch-all at the bottom and do not know their AI layer is unowned.

**Key analogy.** The rules file is a shared configuration file for a very literal new hire who starts over every morning, reads it top to bottom, and follows it without questioning — including the outdated parts.

**Common failure in the exercise.** Students enable CODEOWNERS but not "Require review from Code Owners" (or have no branch protection on their plan), so owners are requested but merges are not blocked. Have them test with a real PR rather than trusting the settings screen.

**Expected exercise outcome.** A private `ai-layer-lab` repo with the Contoso code, the deliberately poor v1.0.0 `CLAUDE.md`, a `CODEOWNERS` with the catch-all first, branch protection, an honest v1.0.0 changelog entry, and a test PR that is blocked pending owner review.

**Extension exercise.** Add a GitHub Actions job that fails a PR when an AI-layer path changes but `AI-LAYER-CHANGELOG.md` does not (e.g. `git diff --name-only origin/main...HEAD`). Discuss whether this is worth the friction.

**Discussion question.** Who should own the AI layer in a 40-engineer organization: a platform team, each service team, or a rotating guild? What goes wrong with each?
