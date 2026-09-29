# Objections FAQ — Ground-Bound-Build-Prove (reference, illustrative)

> Reference for lesson 14.3. Each answer cites evidence or says plainly what is not known. Roles are anonymized.

| ID | Objection (their words) | Asked by | Answer | Evidence | What we do not know |
|---|---|---|---|---|---|
| FAQ-01 | "Isn't this just research-plan-implement with new names?" | staff engineer, lunch-and-learn | Yes, the shape is common and credited. What is specific is what each step must output on legacy code, and Prove's rule that the agent's own tests do not count. | Credits; C1, C2 | Whether the named outputs matter more than the shape; not tested separately. |
| FAQ-02 | "The loop takes longer. My small tickets were faster before." | developer, team retro | Often true for one-sentence changes; the method says to skip Ground and Bound there. | INC-14; S-ticket medians in NOTES | Where exactly the break-even lies; six S tickets are too few. |
| FAQ-03 | "You measured 16% faster. Will we get 16%?" | engineering manager, other team | No promise. One team, one repository, interval 5–25%; defects may have increased. | EXP-01 | Anything about your team until measured there. |
| FAQ-04 | "If the model gets better, does the method go away?" | CTO, discovery call | Parts may. Ground and Prove address missing context and self-grading, which a better model does not remove on its own; C3 is about measurement, not the model. Every number is tied to an agent version. | C1, C2, C3; evolution policy trigger 3 | How much a stronger model reduces shadow rules; not measured. |
