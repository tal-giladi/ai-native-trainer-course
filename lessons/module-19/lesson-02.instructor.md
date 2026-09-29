# Instructor notes — 19.2 Champions and tool fragmentation

**Teaching objective.** Students explain why pilot volunteers mislead, design a network of sanctioned peer champions in every team with succession, quantify fragmentation with $H$ and $N_{\text{eff}}$, and write a tool strategy with one default, ADR-backed exceptions and a portable rules core.

**Likely confusion.** Champion versus evangelist: the champion's job is making the tool useful for their team, not raising usage numbers. Second: $H$ versus the number of tools. Two tools at 50/50 give $N_{\text{eff}} = 2$; at 95/5 about 1.1.

**Common misconception.** "More champions from the pilot means faster adoption." Pilot enthusiasts are often discounted by the majority precisely because they are enthusiasts. Second: "letting teams choose their tool increases adoption." It may raise early satisfaction but splits every downstream investment and hides usage outside the gateway.

**Key analogy.** On-call rotations. Nobody runs on-call on goodwill and evenings: it has named people, agreed time, a secondary, a handover and a runbook. A champion network is an on-call rotation for "the agent does not work for me".

**Common failure in the exercise.** Naming champions without asking their managers, so the hours column is aspirational. Second: leaving the pilot's tool mix in place "for now". Third: making the student themselves a champion in their own client plan.

**Expected exercise outcome.** A section 2 with champions in every team (co-champions in teams over 25), hours agreed in writing, a default tool with a rationale, ADRs or retirement dates for every other tool, `champions` clean and $N_{\text{eff}} \le 2$. Break: the two events add at least two errors and push $N_{\text{eff}}$ up.

**Extension exercise.** Draw the informal help network of one team: ask five developers who they asked the last three times they were stuck with anything technical. Compare the most-named people with your champion list.

**Discussion question.** A team lead says: "My best people are too valuable to spend three hours a week helping others." What does that tell you about the choice, and what do you propose?
