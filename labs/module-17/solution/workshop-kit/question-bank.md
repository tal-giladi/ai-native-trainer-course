# Hard-questions bank — Ground before you generate

> Reference (illustrative). The questions a room of .NET developers and their leads asked in rehearsals, lunch-and-learns and discovery calls, each answered in about a minute with this student's own evidence and an honest limit. Format: [workshop template](../../../../templates/workshop-template.md#question-bank). Check it with `KitCheck questions`. Evidence IDs point to the student's `method-notes` ([Module 14 reference](../../../module-14/solution/concepts.md)) and experiment ([EXP-01](../../../module-14/evidence/experiments.md)).

### Q01 · "Is AI going to replace us?"
- Category: replace
- Answer: I don't know, and I would distrust anyone who says they do. What I can show you is what the work looked like on one legacy system: every incident in my notes was caught by someone who knew the business rule the agent could not see. The steps that took judgment, finding the existing rule and deciding what counts as proof, were the ones the agent did worst unaided. That is what today practises.
- Evidence: INC-01, INC-08, INC-12 (C1); INC-02, INC-07 (C2); opinion on the future, said as such
- Not known: anything about the labour market; my data is one team for twelve weeks.
- Bridge: "Keep that question in mind in Hands-on 1: notice which part the agent could not do."

### Q02 · "Then what happens to juniors, if the agent does their tickets?"
- Category: replace
- Answer: A fair worry. In published studies, less experienced developers gained the most speed on well-specified tasks, which is also the work that used to teach them the codebase. My suggestion is the Ground brief: a junior who writes it learns where the rules live, and a reviewer can see whether they did.
- Evidence: Cui et al. and Peng et al. in the course's evidence table (13.2); INC-05
- Not known: whether juniors who work this way learn the system as well; nobody I know has measured it.
- Bridge: "Pair the least experienced person with the keyboard in the next exercise."

### Q03 · "What stops a ticket or a doc from telling the agent to do something bad?"
- Category: security
- Answer: Nothing inside the model reliably does; prompt injection through tickets and docs is OWASP's first risk for LLM applications. What we rely on is outside the model: the agent runs with a deny list of commands, no network except the model, hooks that block writes outside the repository, and a human approves the plan. In my security lab, the injected ticket got the agent to try an exfiltration; the permission layer stopped it.
- Evidence: OWASP LLM01:2025 (https://genai.owasp.org/llmrisk/llm01-prompt-injection/); security-lab attack A-03 notes (Module 9)
- Not known: attacks I have not tried; the residual-risk register lists four.
- Bridge: "You will see those hooks fire in Live 3."

### Q04 · "Can it read our secrets or push to production?"
- Category: security
- Answer: Only what its permissions allow, so set them before the first run. In the demo it cannot read `.env` or user secrets, cannot push, and cannot run anything not on the allow list; every tool call is logged by a hook. The agent works on a branch and opens a pull request; production stays behind your normal review and pipeline.
- Evidence: brownfield-demo `.claude/settings.json` (Module 9 permissions, Module 8 hooks); OWASP LLM06:2025 Excessive Agency (https://genai.owasp.org/llmrisk/llm062025-excessive-agency/)
- Not known: how your company's agent is configured today; that is worth checking this week.
- Bridge: "I'll show the settings file during the break if you want to copy it."

### Q05 · "Does our code go to the vendor?"
- Category: confidentiality
- Answer: Yes: whatever the agent reads is sent to the model provider, or to the cloud platform that hosts the model for you. What happens to it next depends on your contract and hosting: retention, training use, region. Those are questions for your legal and security teams, and I can give you the checklist I use. For today, the starter pack is a fictional company, so nothing of yours leaves the room.
- Evidence: Module 12 hosting and data-residency checklist (12.1, 12.4); starter pack README
- Not known: your company's contract terms; I have not seen them.
- Bridge: "That's exactly why we practise on Contoso and not on your repository."

### Q06 · "You measured 16% faster. Will we get 16%?"
- Category: roi
- Answer: No promise. That number is one team, one repository, ninety-six tickets, and the interval runs from 5% to 25% faster. Escaped defects may also have gone up; that guardrail failed. What I would promise is a way to measure it on your team: the same comparison, on your tickets, in about six weeks.
- Evidence: EXP-01 (experiments.md); FAQ-03
- Not known: anything about your team until it is measured there.
- Bridge: "The defect part is why we spend twenty minutes on Prove."

### Q07 · "What's the ROI of this workshop?"
- Category: roi
- Answer: Two hours cannot change a team's delivery metrics on their own, and I won't claim it. What I measure is whether you can do three things you couldn't this morning, with a pre- and post-test, and whether you did them at work two weeks later. If you want the delivery number, that is a separate, longer piece of work with a baseline.
- Evidence: the pre/post forms and follow-up in forms.md; Kirkpatrick levels (16.1, 16.5 notes)
- Not known: your two-week follow-up rate; my last group was 3 of 6, which is a wide interval.
- Bridge: "That is why the post-test is not optional."

### Q08 · "Isn't this just DRY with a new name?"
- Category: skeptic
- Answer: Duplication is old, and DRY names it. What is different is the cause: the agent re-implements a rule because the rule was not in its context, not because anyone was careless. The costliest case in my notes crossed C# and T-SQL, which a C#-only search misses. The fix is a research step before planning, not a review reminder. It does not apply to greenfield code.
- Evidence: C1 Shadow rule, INC-01, INC-05, INC-08, INC-12; FAQ-01
- Not known: how often it happens on repositories unlike this one.
- Bridge: "Search one business term in your own repo this week and tell me what you find."

### Q09 · "The next model will fix this. Why learn a method?"
- Category: skeptic
- Answer: Parts of it may go away, and I version the method so that I can drop them. A stronger model still cannot see a stored procedure that is missing from its context, and a model grading its own tests is still grading itself. Every number I show is tied to the agent version I measured with.
- Evidence: C1, C2; evolution policy trigger 3; FAQ-04
- Not known: how much a stronger model reduces shadow rules; not measured.
- Bridge: "Watch whether the agent finds BILL-155 on its own in Hands-on 1."

### Q10 · "Didn't a study find developers were slower with AI?"
- Category: skeptic
- Answer: Yes. METR's 2025 randomized study found experienced open-source maintainers 19% slower on their own mature repositories with early-2025 tools, while they believed they were faster. Their 2026 follow-up points to a speed-up but says the data became unreliable. That setting, experts in a big codebase they know, is close to yours, which is why I don't quote speed-ups I haven't measured.
- Evidence: METR 2025 study (https://arxiv.org/abs/2507.09089); METR 2026 update (https://metr.org/blog/2026-02-24-uplift-update/)
- Not known: how the newest tools do on your codebase.
- Bridge: "Your own measurement beats both of us; the follow-up is where that starts."

### Q11 · "We use Copilot, not Claude Code. Does this apply?"
- Category: tools
- Answer: The method does; the commands don't. Ground, Bound and Prove are steps with outputs, a brief, a plan and checks the agent cannot edit, and they work in any agent that can read files and run tests. The slash commands in the demo are one implementation; the kit maps each step to AGENTS.md and to Copilot's instruction files.
- Evidence: implementation-2026-09.md (Module 14 reference); Module 3 portability map
- Not known: how well each tool follows a long brief; I have measured one.
- Bridge: "In Hands-on 1, write the brief by hand; that part is tool-free."

### Q12 · "What does it cost per developer?"
- Category: cost
- Answer: It depends on the plan and the model, and prices change every quarter, so I won't quote one from memory. What I can give you is the method: count tokens per ticket for a week, multiply by your price, and compare it with the time saved you measured, not the time saved you hoped for.
- Evidence: Module 4 cost-per-task arithmetic; Module 13 ROI range
- Not known: your prices and your ticket mix.
- Bridge: "The troubleshooting sheet has the token arithmetic for this room as an example."

### Q13 · "When does this not work?"
- Category: limits
- Answer: One-sentence changes that touch no business term: Ground and Bound cost more than they save, and my notes show it. Greenfield code with no existing rules. Teams without a test suite they trust, because Prove has nothing to stand on. And any claim about speed on your team, until you measure it.
- Evidence: INC-14; method-v1.0.0 "What this method does not claim"; FAQ-02
- Not known: where exactly the break-even lies; six small tickets are too few.
- Bridge: "The reflection card asks you for one ticket where you would skip it."
