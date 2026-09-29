# Hands-on 3 — Prove your BILL-180 change (7 minutes)

> Reference (illustrative). Objective O4. Handout page 7.

- Start: your change from Hands-on 2, or `ws-3-built` (which has the test run saved in `runs/BILL-180-test.txt`).
- Task: run `/validate`, then `dotnet test`. On the handout's table, mark every check as **evidence** or **not evidence** for each acceptance criterion, and write why in five words.
- Done when: every row of the table is marked, and you can name the one check that would catch a second void rule next month.
- Time: 7 minutes; one-minute warning at 6.
- Hints:
  1. Who wrote each test, and in which run?
  2. A test the agent wrote to describe its own code agrees with the code. What would it take to disagree?
  3. `git log --format="%h %an %s" -- tests/` tells you which tests existed before today.
- Extension: change `CanVoid` to allow paid invoices, run the tests again, and see which checks notice.
- Catch-up: `git checkout ws-3-built`, open `runs/BILL-180-test.txt`, do the marking only.
- Paper path: the saved test run and the diff are on handout page 8.

## Reference marking (instructor only)

| Check | Evidence? | Why |
|---|---|---|
| Four new `CanVoid` tests, written by the agent this run | Weak | Agrees with itself; you reviewed them, so they count for AC 2 only |
| Six existing tests unchanged, still green | Yes | Written before today; nothing edited |
| Convention test, no `Void` comparison outside `InvoiceService` | Yes | Agent cannot edit it; proves AC 3 |
| `gates/architecture.rules` pass | Yes | Written by the team; enforced by the hook |
| "All 10 tests pass" in the agent's summary | No | A claim about the run, not a check |
