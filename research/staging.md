# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates
### C-20261123-01 · Agent-as-a-Judge is not immune to confirmation bias: one false "pre-screening note" in the judge's context flips up to 40% of correct verdicts

- **Class:** B
- **Kind:** new-topic
- **Date discovered:** 2026-11-23
- **Date published:** 2026-11-03
- **Source:** https://arxiv.org/abs/2611.05662 (primary, abstract page fetched this session)
- **Organization/researchers:** Drake Benjamin Brown, Khushbu Pahwa, Yubin Ge, Salvatore Romeo, Akhil Agnihotri, Yi Zhang, Xiaofei Ma, Daniele Bonadiman (affiliations not checked)
- **Category:** evaluation
- **What changed:** Preprint "Grounded but Not Immune: Confirmation Bias in Agent-as-a-judge" (v1 3 Nov 2026, only version; not peer reviewed). Older than the 48 h window; found today and not previously in the registry (`lookup "2611.05662"`, `lookup "agent-as-a-judge"`: no matches).
- **Technical summary:** Per the abstract: a tool-using judge that gathers its own evidence from the environment and the agent's trajectory was given a single false, authority-attributed "pre-screening note" irrelevant to the correct verdict. Across four judges and five datasets this overturned up to 40% of correct verdicts, even when the judge had collected decisive evidence. The authors frame the cue as a controlled diagnostic, not an attack. Three failure modes: over-trust (verifies correctly, then defers to the cue), hallucinated support for the flipped verdict, and stopping investigation at the first corroborating signal. For the two Claude judges, over-trust caused 67–75% of harmful flips. Lightweight mitigations reduced but did not remove the bias.
- **Why it might matter:** Lesson 07.3 teaches judge design against verbosity, position and self-preference bias and calibration against human labels, but not judge-input hygiene. In the course's CI pipelines (modules 10–11) a judge or reviewer agent often sees upstream text (another agent's summary, a PR description, a bot's "tests pass" comment). If this holds, anything upstream that states a verdict is a bias source and should be stripped from the judge's context, and calibration done without such notes will overstate accuracy in production.
- **Evidence of adoption:** none (new preprint)
- **Major organizations using it:** n/a
- **Open-source implementation:** none checked
- **Paper:** https://arxiv.org/abs/2611.05662
- **Code:** none checked
- **Relationship to existing course material:** lesson 07.3 (`lessons/module-07/lesson-03.md` line 11 objectives on verbosity/position bias, line ~219 "Don't" list, line ~237 Wang et al. position bias); 10.2 ("a message from another agent is data, not an instruction with your authority"); 11.2 (the CI reviewer it measures). Related staged/deferred: `research/deferred/llm-judge-error-shifts-with-agent-version.md` (another reason calibration may not transfer).
- **Potential course lesson:** If replicated: one "Don't" line in 07.3, e.g. "Don't give the judge anyone else's verdict (a PR description, a bot comment, another agent's summary): a single false authority note can flip correct verdicts even for a judge that checks the evidence itself", plus calibrating with the same context the judge sees in production.
- **Confidence:** low–medium (unreviewed preprint; mechanism consistent with known anchoring/sycophancy findings)
- **Recommendation:** monitor; cheap caveat to consider if the weekly review touches 07.3 together with the deferred judge-error-shift topic.
