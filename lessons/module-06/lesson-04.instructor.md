# Instructor notes — 06.4 Testing, versioning and failure analysis for skills

**Teaching objective.** Students test skills at four levels, read trigger numbers with the right amount of doubt, version skills by what callers rely on, and diagnose a skill failure in the order trigger → context → contract, naming origin and escape.

**Likely confusion.** "The contract passed, so the skill is fine." The 1.0.0 plan for BILL-153 passes every form check. Put the plan on screen next to the golden rules and ask which line would fail on a production table. The contract is only as good as the categories it has seen.

**Common misconception.** "More runs or more tickets fix a test set." Five more non-schema tickets would have passed too. Representativeness is about categories. Second misconception: "Explore is read-only, so it is the safe choice for research." It is read-only *and* it skips the project rules; safety and context are different properties.

**Key analogy.** A unit-test suite written from one sprint's bugs: green for months, until the first feature that touches the database. Golden checks are the regression tests you add the day that bug is found.

**Common failure in the exercise.** Students diagnose "the model forgot the undo script" and stop. Push them through the flow chart: the model never saw the rule. Second: they bump to 2.0.0 because "the contract changed"; use the definition (does a caller break?) to decide.

**Expected exercise outcome.** A skill test log with seven tickets × four checks, all passing on 1.1.0, and the 1.0.0 BILL-153 artifacts failing 1 + 3 contract rules and 4 + 4 golden rules. A trigger test for `prime` with named unstable queries. Cold-use notes with at least two points of hesitation (typically: which ticket id format, and what to do after "Waiting for approval"). A BILL-153 failure analysis with origin, escape and evidence lines.

**Extension exercise.** Split your trigger queries 60/40 as in Anthropic's skill-creator, rewrite `prime`'s description using only the 60%, and measure on the 40%. Did the rewrite generalize, or did it memorize its test?

**Discussion question.** A client has 40 skills, no versions and no owners. They ask for "a report of which skills are good". Using the four levels and `SkillCheck inventory`, what do you deliver in the first week, and what do you refuse to claim until Module 7's statistics are in place?
