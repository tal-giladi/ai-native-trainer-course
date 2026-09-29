# Instructor notes — 05.3 Validation gates and reset decisions

**Teaching objective.** Students build a gate ladder that runs regardless of the agent (Stop hook and CI), protect the oracle (no skipped tests, no falling test count, existing tests in Do not touch), and make reset decisions from signals and a simple expected-cost comparison rather than from frustration.

**Likely confusion.** "If the tests pass, the gate passed." Walk through the break: `dotnet test` exits 0 with a skipped test. The exit code of the test runner is a gate only for failures; it says nothing about tests that did not run or were deleted.

**Common misconception.** "Resetting throws away progress." What it throws away is the transcript. The progress lives in the brief, the plan, the learned note and the last green commit — which is why 05.1 and 05.2 make resets cheap. Students who never reset usually have nothing written down to reset *to*.

**Key analogy.** A smoke detector versus a sign saying "please don't smoke". The rule in the prompt is the sign; the Stop hook is the detector that goes off in the room; CI is the sprinkler system that works even if nobody is home.

**Common failure in the exercise.** The Stop hook runs the whole slow suite and students disable it after the third turn. Keep hooks to fast gates. Second: on Windows the hook fails because it is started from the wrong directory or `bash` is missing; have students run it by hand first with a piped JSON input.

**Expected exercise outcome.** `LoopGate all` green with 10 tests on the student's branch; a Stop hook that blocked at least once in a real session (screenshot or transcript line); a CI job visible on a PR; a reset log row. Typical gate timings: arch under 1 s, build + tests 10–30 s. Students who ran `SET NOEXEC` should report what it caught (syntax errors they planted in V005) and what it could not confirm (anything depending on objects the same script creates, because nothing is executed); only the real run plus `U005` proves the migration.

**Extension exercise.** Write a mutation check: temporarily flip one comparison in `InvoiceService` (for example `<` to `<=` in `IsOverdue`) and confirm at least one test fails. Repeat for 5 mutations. How many survive? Each survivor is a behavior no gate encodes. (The `IsOverdue` mutation survives today: no test sits on the boundary, which is exactly the gap BILL-152 closes.)

**Discussion question.** Should an agent ever be allowed to skip a test? If yes, what process would make a skip visible and time-limited rather than a quiet hole in the oracle?
