# Troubleshooting — Ground before you generate

> Reference (illustrative). Every entry comes from a rehearsal, a setup check reply or a previous delivery, and every fix was run at least once on a clean machine (the **Tested** column). The co-host holds a printed copy. The IDs match what `check-setup.ps1` and `check-setup.sh` print. Rule for the room: if a fix takes longer than the time left in the exercise, the learner moves to the catch-up tag or the paper path and you fix it at the break.

| ID | Symptom | Cause | Fix | Seconds | Tested |
|---|---|---|---|---|---|
| T-01 | `dotnet --version` prints 6.x or 7.x, or `git --version` below 2.30 | Old SDK or Git on a managed laptop | Pair with a neighbour now; at the break, install .NET 8 SDK user-locally with the `dotnet-install` script (no admin, about 4 minutes) | 20 | 2026-10-02 |
| T-02 | Restore fails with 401, 407 or a timeout | Corporate proxy or a private feed in the user's `NuGet.Config` | `dotnet restore Contoso.Billing.sln --source ./packages` (the local feed in the pack) | 40 | 2026-10-02 |
| T-03 | "not at tag ws-0-start" or a dirty tree | Cloned `main`, or edited files before the day | `git stash push -m pre-workshop && git checkout ws-0-start` | 20 | 2026-10-02 |
| T-04 | `dotnet test` fails in `ConventionTests` on a Windows laptop | Git converted line endings and a file-content check sees CRLF | `git config core.autocrlf false && git rm -r --cached -q . && git reset --hard ws-0-start` | 60 | 2026-10-09 |
| T-05 | Several agents fail at once with 429 or "overloaded" in Hands-on 2 | The whole room shares one organization's rate limit; 14 runs started in the same minute | Stagger: odd tables now, even tables one minute later; failed runs retry once after 60 s | 60 | 2026-10-09 |
| T-06 | First edit fails: `.claude/hooks/bin/AgentHooks.dll` not found | Hooks not built (step 1 of the README skipped) | `dotnet build tools/AgentHooks -c Release -o .claude/hooks/bin` | 45 | 2026-10-02 |
| T-07 | Agent CLI not found, or asks to log in | Not installed, or the company has not enabled it for this user | Pair, or paper path. Never log in on someone else's account in the room | 10 | 2026-10-05 |
| T-08 | `/prime` reply shows no file path; learner waits for the brief in the chat | The brief is written to `research/BILL-180.md`; the chat only summarizes | Open `research/BILL-180.md` (fixed in the layer's changelog after stranger test 1) | 15 | 2026-10-09 |
| T-09 | `LoopGate scope` fails on `research/` and `plans/` in Hands-on 3 | Brief and plan not committed | `git add research plans && git commit -m "BILL-180 brief and plan"`, then `/validate` again | 30 | 2026-10-09 |
| T-10 | Projector shows the terminal at unreadable size | Display scaling reset when the laptop was plugged in | Terminal profile "workshop" (18 pt, 100 columns); `Ctrl` + `+` twice as a fallback | 15 | 2026-10-12 |

## Rate limits for a room (T-05)

A room is one customer to the model provider. Anthropic's documentation, for example, says limits are set at the organization level, measured per model class in requests, input tokens and output tokens per minute, enforced with a token bucket, and that a sharp increase in usage can hit separate acceleration limits ([Anthropic rate limits](https://platform.claude.com/docs/en/api/rate-limits), as of 2026-09; other providers publish the same kind of table). Learners on company accounts usually share one organization, and an administrator may have set a lower limit on the workspace the agent uses.

Rough demand at the worst minute of Hands-on 2: 14 learners start `/plan-feature` together; each first call sends the rules, the brief and the files it reads, about 40,000 input tokens, none of it cached yet. That is $14 \times 40{,}000 = 560{,}000$ uncached input tokens in one minute, from an organization that was idle a minute earlier. If that exceeds the limit (or trips an acceleration limit), the excess gets 429 errors, and every rejected learner retries at the same moment. Staggering by one minute halves the peak to about 280,000, and later calls are cheaper because cached input does not count against most limits. Ask the host for the organization's actual limits before the day and redo this arithmetic with them.
