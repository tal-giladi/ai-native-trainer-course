# Module 4 breaks

Apply each break to your **working copy** (never to a shared repository), run the named tasks, diagnose,
then undo it. Each lesson's "Break it" and "Fix it" sections walk through the diagnosis.

| Lesson | Break | How to apply | Tasks that should move |
|---|---|---|---|
| 04.1 | History eats the budget: a verbose test log enters the session | In an interactive session ask the agent to run `dotnet test Contoso.Billing.sln -v diag`, then run `/context` | cost and latency per turn (not pass rate) |
| 04.2 | A critical fact is moved into a path-scoped rule and deleted from the root | Move the `U###` line from `AGENTS.md` into `.claude/rules/migrations.md` | T04 |
| 04.3 | Contradictory directory rule | Copy `Invoices-CLAUDE.md` to `src/Contoso.Billing/Invoices/CLAUDE.md` | T02 (intermittently) |
| 04.4 | A constraint stated only in conversation is lost at compaction | Follow the script in lesson 04.4 | the agent edits `V004` after `/compact` |
| 04.5 | The cut removes one critical fact | Delete the "never edit a merged `V###`" line | T06 |

Undo everything with a fresh copy of `solution/` over your working copy, or `git checkout -- .` if it is a repository.
