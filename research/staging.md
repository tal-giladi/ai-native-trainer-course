# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261111-01 · .NET 8 reached end of support on 2026-11-10; labs target net8.0 and CI templates install SDK 8.0.x

- **Class:** A
- **Kind:** correction
- **Date discovered:** 2026-11-11
- **Date published:** 2026-11-10
- **Source:** https://github.com/dotnet/core/blob/main/releases.md (primary, fetched this session; dotnet.microsoft.com support-policy page does not resolve from this session's network)
- **Organization/researchers:** Microsoft (.NET team)
- **Category:** ci/production
- **What changed:** The official .NET releases table now lists .NET 8.0 (LTS, final patch 8.0.34) and .NET 9.0 (STS, final patch 9.0.23) as end-of-life with end of support 2026-11-10. .NET 11.0 (STS) shipped 2026-11-10; supported releases are now .NET 10.0 (LTS, 10.0.15) and .NET 11.0, both supported to 2028-11-14. Related: `actions/setup-dotnet` is at v6.1.0 (2026-11-03; v5.0.0 moved to Node.js 24), while course workflows use `@v4` — https://github.com/actions/setup-dotnet/releases (primary, fetched this session).
- **Technical summary:** 35 lab `.csproj` files target `net8.0` (32 with `RollForward=Major`, so they still run on a newer runtime; the three without are class libraries). An SDK 10/11 still builds `net8.0` but warns that the target framework is out of support, and no further security patches ship for the 8.0 runtime. The CI workflows learners copy into their own repos install the now-unsupported SDK with `dotnet-version: "8.0.x"`: `labs/module-11/workflows/{agent-fix,agent-triage,ai-layer-governance,budget-guard}.yml`, `labs/module-11/ops/break/dep-fix-hourly.yml`, `labs/module-05/hooks/loop-gates.yml`, `labs/module-07/ci/agent-evals.yml`, and the inline workflow in lesson 12.5 (`actions/setup-dotnet@v4` + `dotnet-version: "8.0.x"`, line ~164). Lessons 08.1, 12.1, 12.5 and labs/module-08, labs/module-12 READMEs hard-code `bin/Release/net8.0/...` paths, which stay correct only while the TFM is unchanged. Lesson 02.1 says "You need the .NET 8+ SDK" (still true as a minimum).
- **Why it might matter:** A course about running agents safely in CI teaches templates that install an end-of-support SDK; a learner who copies them into a production pipeline adopts an unpatched toolchain. The module-03 brownfield repo is a deliberate legacy fixture and can keep net8.0 as story; the CI templates and tool projects are not fixtures.
- **Evidence of adoption:** n/a (platform lifecycle fact).
- **Major organizations using it:** n/a
- **Open-source implementation:** https://github.com/dotnet/core
- **Paper:** none
- **Code:** none
- **Relationship to existing course material:** lessons 02.1, 08.1, 12.1, 12.5; labs/module-02, 05, 07, 08, 11, 12 (workflows and tool csproj files); `labs/module-22/solution/practitioner-kit/CHANGELOG.md` already records a .NET 10-only setup failure (T-02). `lookup "net8.0"` shows no registry topic.
- **Potential course lesson:** FIX, not ADD. Sentences/lines now stale: lesson 12.5 inline workflow `dotnet-version: "8.0.x"`; the seven lab workflow files above. Decide whether to retarget tool/lab projects to `net10.0` (LTS) and update the `bin/Release/net8.0/` paths in 08.1, 12.1, 12.5 and the module-08/12 READMEs together, keeping ids stable. Consider bumping `actions/setup-dotnet@v4` in the same pass.
- **Confidence:** high (dates from the official dotnet/core releases table)
- **Recommendation:** review for FIX: CI templates to `dotnet-version: "10.0.x"`; weigh a single TFM sweep to net10.0 (with path updates) vs keeping net8.0 + RollForward and adding an "as of 2026-11, .NET 8 is out of support" note.

### C-20261115-01 · claude-code-action v1.0.273 makes auto mode the default, so `--allowedTools` is no longer a strict list

- **Class:** A
- **Kind:** correction
- **Date discovered:** 2026-11-15
- **Date published:** 2026-11-12
- **Source:** https://github.com/anthropics/claude-code-action/releases/tag/v1.0.273 (primary, fetched this session)
- **Organization/researchers:** Anthropic (claude-code-action maintainers)
- **Category:** ci/production
- **What changed:** Release v1.0.273 (12 Nov, labelled breaking; workflows pinned to `@v1` pick it up automatically) tightens several defaults and loosens one. Quoted: "A safety review now decides tool calls that no allow rule covers. Calls that used to be denied can run if the review passes, so `--allowedTools` is no longer a strict list. Workflows with a `prompt` can also edit workspace files without an allow rule." Opt-out: `--permission-mode acceptEdits` (mention runs) or `--permission-mode default` (prompt runs) in `claude_args`; old defaults also kept when `allowed_non_write_users`, `allowed_bots`, a custom `ANTHROPIC_BASE_URL` or `path_to_claude_code_executable` is set. Other changes in the same release: code-running commands (e.g. `npm test`) denied on PRs whose author lacks write access (opt out `restrict_untrusted_prs: false`, which "lets code from that pull request run in a job that holds your workflow's tokens"); only the triggering user's text starts a session and edits to others' comments no longer trigger; `GITHUB_ENV`/`GITHUB_PATH`/git config restored at session end (pass values via `GITHUB_OUTPUT` or `--json-schema`); no token left in `.git/config` when `allowed_bots` is set; `@claude` commits reject `--amend`/`--no-verify`/`--author` and force pushes are blocked unless re-allowed in `claude_args` (a settings-file rule "doesn't count"); signed commits from `prompt` runs go to `issue-N`/`pr-N` branches unless `CLAUDE_BRANCH` is set. Follow-ups v1.0.274 (CI results readable on outside PRs) and v1.0.276 (Nov 13) — https://github.com/anthropics/claude-code-action/releases.
- **Technical summary:** The official docs page https://code.claude.com/docs/en/github-actions (fetched this session) has NOT caught up: it still says that for a plain-text `prompt` "Claude has no shell or GitHub API access until you grant the tools the prompt needs, with `--allowedTools` in `claude_args`". The release note is the newer primary source; the docs lag. The course's own lab workflows (`labs/module-11/workflows/*.yml`, `ops/break/dep-fix-hourly.yml`, `automation/break/triage-v0.yml`) call the CLI directly (`claude -p --permission-mode dontAsk --allowedTools ...`), not the action, so they are unaffected: `dontAsk` + explicit list remains a strict surface on the CLI.
- **Why it might matter:** Module 11 teaches "the tool surface is exactly the list" as the CI contract. A learner who follows 11.2/11.3's pointers to the action and writes `claude_args: --allowedTools ...` now gets a model-reviewed (auto mode) surface instead of a strict allowlist, unless they add `--permission-mode default`. That changes the threat model the course teaches (deterministic deny vs classifier decision). Conversely, the new untrusted-PR command denial and env/git-config restoration are hardening a lesson on CI trust boundaries could cite.
- **Evidence of adoption:** n/a (default change in a widely used action; automatic for `@v1` users).
- **Major organizations using it:** n/a
- **Open-source implementation:** https://github.com/anthropics/claude-code-action
- **Paper:** none
- **Code:** none
- **Relationship to existing course material:** lesson 11.2 further-reading line ~207 ("`claude_args` for `--max-turns` and `--allowedTools`"); lesson 11.3 further-reading line ~200 ("tools must be granted via `--allowedTools`") and line ~95 (bot-actor check, still accurate); lesson 11.1 table line ~67 (CLI `dontAsk` + `--allowedTools` = exact surface, still accurate for the CLI); `references/research-log.md` row "Claude Code docs — GitHub Actions" (verified 2026-09-28, quarterly). Related registry topics: `research/accepted/agent-to-agent-ci-trust-boundary.md`, `research/deferred/coding-agent-ci-untrusted-issue-secrets.md`. `lookup "restrict_untrusted_prs"` empty.
- **Potential course lesson:** FIX, not ADD. 11.3 line ~200 "tools must be granted via `--allowedTools`" is now wrong for the action's default; 11.2 line ~207 should note that with the action (v1.0.273+) `--allowedTools` is strict only with `--permission-mode default`/`dontAsk` in `claude_args`. Optionally one sentence in 11.1 contrasting CLI `dontAsk` (strict) with the action's auto-mode default, and cite `restrict_untrusted_prs` as an example of the vendor moving the fork-PR boundary into the tool. Re-verify the docs page during the weekly run (it may update) and bump the research-log row.
- **Confidence:** high for the release content (primary, quoted); medium on how long the docs page will lag.
- **Recommendation:** review for FIX in 11.2 / 11.3 further-reading annotations and research-log row; no lab change needed (labs use the CLI directly).
