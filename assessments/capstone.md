# Capstone: one real engagement, end to end

**Duration:** 6–10 weeks · **Depends on:** all 22 modules · **Assessed by:** [the capstone rubric](capstone-rubric.md), evidence required for every score.

The capstone is the whole course compressed into one real piece of work: you pick a wedge, build and secure an AI layer on a real brownfield repository, prove what it does with evaluation and a controlled comparison, name your method, teach it to a real team, and package it as an offer with an honest case study.

> [!CAUTION]
> Use only code you are legally allowed to work on. Security testing runs only in your local [security-lab](../projects/security-lab/README.md) — never against real organizations, production systems or third-party infrastructure. Anonymize every client or employer detail before anything becomes public.

## The twenty steps

| # | Step | Built in | Evidence |
|---|---|---|---|
| 1 | Select a real engineering wedge | [Module 1](../lessons/module-01/lesson-03.md) | `wedge.md`, interview quotes |
| 2 | Audit a brownfield repository | [Module 3](../lessons/module-03/lesson-03.md) | Audit report |
| 3 | Create the AI layer | [Module 3](../lessons/module-03/lesson-04.md), [Module 4](../lessons/module-04/lesson-05.md) | Rules, context audit, portability matrix |
| 4 | Implement skills | [Module 6](../lessons/module-06/lesson-02.md) | Skills with test tickets and changelog |
| 5 | Implement an MCP integration | [Module 8](../lessons/module-08/lesson-02.md) | Integration + custom server with tests |
| 6 | Implement hooks | [Module 8](../lessons/module-08/lesson-04.md) | Enforcement and audit hooks |
| 7 | Implement evaluation tests | [Module 7](../lessons/module-07/lesson-06.md) | Harness, task set, calibrated grader |
| 8 | Conduct security testing | [Module 9](../lessons/module-09/lesson-06.md) | Threat model, ≥5 attacks, residual-risk register |
| 9 | Create a CI/headless workflow | [Module 11](../lessons/module-11/lesson-01.md) | Workflow file, precision/recall report |
| 10 | Measure baseline performance | [Module 13](../lessons/module-13/lesson-01.md) | Pre-registered metrics, baseline data |
| 11 | Run a controlled comparison | [Module 13](../lessons/module-13/lesson-06.md) | Experiment report with intervals |
| 12 | Document limitations | [Module 13](../lessons/module-13/lesson-04.md) | Threats-to-validity section |
| 13 | Create a named methodology | [Module 14](../lessons/module-14/lesson-02.md) | `concepts.md`, `method-v1.md`, attribution audit |
| 14 | Build a public demo | [Module 15](../lessons/module-15/lesson-01.md) | `brownfield-demo`, unedited recording, stranger test |
| 15 | Deliver the 2-hour workshop to a real team | [Module 17](../lessons/module-17/lesson-01.md) | Recording, observer rubric |
| 16 | Collect participant feedback | [Module 16](../lessons/module-16/lesson-05.md) | Feedback analysis |
| 17 | Measure learning (pre/post) | [Module 16](../lessons/module-16/lesson-05.md) | Normalized gain |
| 18 | Create an adoption plan | [Module 19](../lessons/module-19/lesson-03.md) | Stakeholder map, champions, metrics, anti-regression |
| 19 | Produce an offer | [Module 20](../lessons/module-20/lesson-01.md) | Offer sheet, pricing rationale, SOW |
| 20 | Produce a case study | [Module 21](../lessons/module-21/lesson-05.md) | Anonymized before/after case study |

## Submission

One index document (`capstone.md` in your `case-studies` repo) linking to: the technical repository · the AI layer · the evaluation suite · the security assessment · the architecture diagram · the methodology · workshop materials · the workshop recording · participant feedback · the before/after measurement · the adoption plan · the offer sheet · the SOW · the anonymized case study.

## Honesty rules

- Every number you cite is your own measured number, with its interval and its sample size.
- Every claim the evidence does not support is either removed or labelled as a hypothesis.
- If the controlled comparison shows no effect, say so — a well-run null result scores higher than an unsupported win.

## Self-assessment

Score yourself against [the rubric](capstone-rubric.md) before asking anyone else to. For each category write one sentence: "I score N because the evidence at [link] shows …".
