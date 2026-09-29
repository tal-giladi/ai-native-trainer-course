# Instructor notes — 06.2 The five core skills

**Teaching objective.** Students install and run the five core skills as a pipeline whose stages hand off through files, choose invocation per skill from side effects, and can show why a chat-only handoff fails on the first reset.

**Likely confusion.** "Why not one big `/do-ticket` skill?" Because the plan checkpoint is a human decision, and one skill that runs research, plan and implementation removes it. Also: a single large body is paid for on every later turn, and a failure inside it is hard to locate.

**Common misconception.** "The skill ran, so the output is right." `plan-lint` passing on the break's plan is the moment to stop and ask the room what `plan-lint` actually checks (form, evidence paths exist) and what it cannot (whether "issued" means the right thing).

**Key analogy.** A relay race where the baton is a file. If runners pass the baton by shouting the time to each other, a new runner (reset, colleague, different tool) starts with nothing.

**Common failure in the exercise.** Students skip `/clear` in the Show me run "because it is faster", so the break never happens for them. Make the `/clear` mandatory. Second: they treat the two inventory overlaps as bugs and rewrite descriptions; the overlap between a skill and the agent it delegates to is expected. Overlap between two *skills* is the problem (06.4).

**Expected exercise outcome.** Five skills and two agents lint clean. BILL-154 run end to end with `/clear` between phases; brief and plan pass contracts and golden rules; 9 tests, `VERDICT: PASS`. `/spec` produces one or two drafts and an index naming BILL-150, 151 and 154 as already ticketed. The 0.9.0 break reproduces the `!= Paid` plan and the `Statements/` folder. Total 60–90 minutes; the first run of `/prime` is the slowest.

**Extension exercise.** Make `validate` fork into a sub-agent (`context: fork`) so that test output never enters the main session. Measure the main session's context before and after with `/context`, and decide whether the saving is worth losing the verbatim lines in the main transcript.

**Discussion question.** A team lead wants `pr-review` to run automatically on every change the agent makes. Using what 05.3 said about fresh-context reviewers over-reporting, argue for and against, and propose where it should run instead.
