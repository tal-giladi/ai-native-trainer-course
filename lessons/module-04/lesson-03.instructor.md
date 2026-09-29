# Instructor notes — 04.3 Context pathologies

**Teaching objective.** Students name five context pathologies, map each to its typical symptom, confirm automated leads against evidence, and diagnose a trajectory-dependent failure caused by a contradictory directory-level rule.

**Likely confusion.** Pollution vs irrelevance, and stale vs contradictory. Use two questions: "Does `/clear` make it go away?" (yes → pollution) and "Is the wrong behavior stable across runs?" (stable → stale or missing; flipping → contradiction or trigger timing).

**Common misconception.** "Intermittent means the model is random." Sampling does cause variation (02.2), but an instruction layer that differs by trajectory produces far larger swings. Make the class check context before blaming temperature.

**Key analogy.** A codebase with two `appsettings` files that set the same key: which value wins depends on load order and environment, and the app "sometimes" connects to the wrong database. Nobody calls that randomness; they call it configuration drift.

**Common failure in the exercise.** Students accept every audit lead as a defect and produce a 100-row log. Enforce "two per category, confirmed or rejected with evidence". Second failure: they grep the root files only and miss the nested `Invoices/CLAUDE.md` — have them list every instruction file first.

**Expected exercise outcome.** A pathology log with ten leads (roughly 7–8 confirmed, 2–3 rejected — typically framework names flagged as stale), two semantic contradictions (DateTime.Now vs IClock; `Billing.sln` vs `Contoso.Billing.sln` in the CI section), a pollution transcript, and T01–T03 failures on the bloated layer that match predictions. After the break fix: T02 5/5.

**Extension exercise.** Write an LLM-based contradiction reviewer prompt, run it over the bloated layer, and score it against the human log: how many of the confirmed contradictions does it find, and how many false ones does it invent? Keep the numbers for Module 7 (grader precision and recall).

**Discussion question.** Is a directory-level rule that contradicts the root ever the right design? What would the root have to say for such an exception to be safe?
