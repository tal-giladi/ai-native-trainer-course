# Application form

A one-page form a prospect fills in before you agree to a call or an engagement. It exists to answer one question honestly, for both sides: is this a problem you can help with, now, for someone who can decide? Introduced in [20.3 · Qualification and discovery calls](../lessons/module-20/lesson-03.md). The answers go into `applications.csv`, which `OfferCheck qualify` scores ([Module 20 labs](../labs/module-20/README.md)).

The form qualifies; it does not perform scarcity. If you say you take a limited number of engagements, state the real cap on the form and keep it.

## Rules

1. **Ask only what the decision uses.** Every field below maps to a qualification criterion. Name and work email are needed to reply; nothing else personal is. No phone number, home address, date of birth, salary, photo or ID number. Under data-protection laws such as the GDPR (orientation only, jurisdiction-dependent), collecting more than you need is itself a problem.
2. **Ask about the past, not opinions.** "The last time this happened" separates a real problem from curiosity, as in the [customer discovery script](customer-discovery-script.md).
3. **Say what happens next, and when.** Every applicant gets an answer within five working days, including a "no" with a pointer elsewhere.
4. **Say what you do not do.** It saves both sides a call.

## The form (copy and adapt)

> **Before you apply.** I help legacy .NET and SQL Server teams that already use a coding agent get PRs that pass review, and measure on their own tickets whether it helped. I take at most [N] engagements at a time. I do not promise productivity figures, and I never need access to production systems. I answer every application within five working days.

| # | Question | Field in `applications.csv` | Why it is asked |
|---|---|---|---|
| 1 | Your role and your team's size | `role`, `team_size` | Who you are talking to; whether the offer fits the size |
| 2 | In one or two sentences, what problem do you want to solve? | `problem` | Fit with your wedge |
| 3 | Tell me about the last time it happened: what happened, and what did it cost? | `last_incident` | A real, dated problem vs curiosity |
| 4 | Who would sponsor this, and who decides? (roles, not names) | `sponsor` | Someone can say yes |
| 5 | Is there a budget for this? (approved / we know the process / none yet / don't know) | `budget` | Whether money can move |
| 6 | When would you want to start, in weeks from now? | `timing_weeks` | Whether now is real |
| 7 | Does the team use a coding agent today? (daily / weekly / no) | `uses_agent` | Your offers assume it |
| 8 | What would a good result look like for you? | `expectation` | Mismatches (guarantees, headcount cuts) surface before the call |
| 9 | Is your organization a customer, competitor or supplier of [your employer]? (yes / no / not sure) | `conflict` | Employer conflicts ([20.5](../lessons/module-20/lesson-05.md)) |
| 10 | Your name and work email | not stored in the CSV | To reply |

## Scoring (what `OfferCheck qualify` does)

| Criterion | 1 point if |
|---|---|
| Problem | `last_incident` describes a specific past event |
| Sponsor | a role is named |
| Budget | `approved` or `process-known` |
| Timing | start within 12 weeks |
| Agent use | daily or weekly |

| Result | Decision | What you send |
|---|---|---|
| conflict = yes | **REFER** | "I can't take this on because of my employment; here are two people who can." |
| 4–5 points | **CALL** | a link to book the 30-minute discovery call; if `expectation` is flagged, say in the invitation what you do not promise |
| 2–3 points | **NURTURE** | a useful piece of your content and "come back when the timing/budget is real" |
| 0–1 points | **DECLINE** | a kind, specific no, with a pointer to a better fit |

A form that sends everyone to a call is not qualifying anyone.

## Privacy note to put under the form

> I use your answers only to decide whether and how I can help, keep them for 12 months, and delete them on request. I do not share them. [Adapt to the data-protection law that applies to you and your applicants; this is orientation, not legal advice.]
