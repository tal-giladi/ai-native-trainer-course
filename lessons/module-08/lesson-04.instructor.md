# Instructor notes — 08.4 Hooks: enforcement and audit

**Teaching objective.** Students pick the right event and decision mechanism for a guarantee, write enforcement hooks that fail closed and cover every tool input shape, test hooks against recorded payloads, add a validation and an audit hook, and keep hook latency small enough that nobody disables them.

**Likely confusion.** Exit codes. Students assume "non-zero means blocked", as in CI. In Claude Code only exit 2 (or a JSON deny) blocks; every other failure lets the call through. Put the four-row table on the board and make them predict each row before showing it.

**Common misconception.** "A hook makes it impossible." A hook makes it impossible *through the tools it matches, in this host, while it is installed and working*. The same file can be written through `Bash`, by another agent, or by a person. CI stays the wall. Students who understood 05.3's "the hook is a nudge, CI is the wall" get this quickly; review it if not.

**Key analogy.** A door lock that opens when its battery dies. Some doors should fail open (the fire exit: the audit hook must never trap the work), some must fail closed (the vault: the guard). Deciding which is which is the design; the code is the easy part.

**Common failure in the exercise.** Testing the guard only with an `Edit`, as the v0 authors did. Insist on the payload set, including a `Write`, a `MultiEdit` and a malformed payload. Second: on Windows, the hook command works in the student's PowerShell and fails in the shell Claude Code uses; pipe a payload into the exact command string. Third: the audit log inside the repository without a `.gitignore` entry, committed with the first `git add -A`.

**Expected exercise outcome.** `selftest` 10/10 plus two of the student's own payloads; hooks installed and committed with a changelog entry; three live outcomes recorded (DbContext denied with the ADR reason, V004 edit denied, `unknown column Notes` fed back after the write); an audit report with denies and MCP calls; a latency number and $c \times \ell$ for a real session. In the break, the diagnosis names the `content` versus `new_string` shape, exit 1 as fail-open, the drifted rule list and the missing tests.

**Extension exercise.** Add a `PreToolUse` rule for MCP writes: match `mcp__.*` and deny any tool whose name contains `write`, `merge` or `delete` unless an environment variable marks the session as an approved maintenance session. Where should that decision really live: hook, permission rules, or token scope?

**Discussion question.** Your audit log shows the agent made 212 tool calls in one session, 9 of them MCP calls to GitHub. A security reviewer asks whether this log could prove to an auditor that no write happened. What is your honest answer, and what would you have to add?
