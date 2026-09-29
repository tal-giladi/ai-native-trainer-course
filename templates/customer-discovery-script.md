# Customer discovery script

A 30-minute interview script for learning whether a pain is real, who feels it, and who pays — without pitching. Use it before you have an offer, and again whenever your wedge changes. Introduced in [01.4 · Customer discovery interviews](../lessons/module-01/lesson-04.md); the question rules come from Rob Fitzpatrick's *The Mom Test* and standard survey-design practice.

This is **not** a sales call. The discovery-call script for qualifying a prospect who already wants to buy comes later, with offers (Module 20).

## Three rules

1. **Talk about their life, not your idea.** Do not describe your workshop, method or product until the last two minutes — ideally not at all.
2. **Ask about specific past events, not opinions or the future.** "Tell me about the last time…" beats "Would you…", "Do you ever…" and "How painful is…".
3. **Listen more than you talk.** Target: you speak less than 30% of the words. When they mention an event, follow it; the script can wait.

## Before the call

- [ ] Target: someone in your wedge's audience, **outside your own team**, not a close friend (friends are kind; kindness is noise).
- [ ] Consent message sent, e.g.: *"I'm researching how .NET teams actually work with coding agents. 30 minutes, I'm not selling anything. I'll take notes and may quote you anonymously — nothing about your company, code or customers. OK?"*
- [ ] Recording only with explicit consent, and only if you will actually re-listen. Notes are usually enough.
- [ ] Never ask for confidential code, customer data, or internal numbers they are not allowed to share. Ask for aggregates ("roughly how many PRs a week?"), not exports, unless they offer and are allowed.
- [ ] Interview id (`I01`, `I02`…) assigned; no names in your files.
- [ ] Your wedge hypothesis written down *before* the call, so you can tell afterwards whether you heard it or put it there.

## The script (30 minutes)

| Min | Section | Questions (pick, don't recite) | You are listening for |
|---|---|---|---|
| 0–2 | **Open** | "Thanks. As I said, I'm researching, not selling. Can you tell me about your team and what you're working on?" | Stack facts for the inventory; team size; who they report to |
| 2–7 | **Context** | "Walk me through what happened with the last ticket you finished, from picking it up to it being in production." · "Where did it wait?" | Where time goes, in their words |
| 7–17 | **The last time** | "Tell me about the last time [a coding agent / a review / a migration / onboarding] went badly." · "What happened next?" · "How did you find out?" · "Who else got involved?" | A dated event, a cost, a person — this is the evidence |
| 17–23 | **Workarounds and cost** | "What have you tried to fix it?" · "What happened to that?" · "How long did it take you to deal with it that time?" · "How often does something like that happen?" | Existing workarounds (strongest signal), abandoned tools, numbers they actually know |
| 23–27 | **People** | "Who else feels this?" · "Who would decide to spend time or money on it?" · "Who would worry if it changed?" | User, champion, buyer, blocker |
| 27–30 | **Close** | "Is there anything I should have asked?" · **Commitment ask** (below) · "Can I follow up in two weeks?" | Whether they spend something (time, reputation, data) on this |

### Commitment asks (pick one that costs them something real)

- An introduction to the person who owns the problem (their manager, the DBA lead, the CISO).
- An anonymized aggregate they are allowed to share (PR open/review/merge timestamps with no titles, a count of rollbacks last quarter).
- A second, 20-minute session with a teammate who lived the incident.
- Permission to send them your one-page summary for correction.

A "yes, sounds great" is not a commitment. An intro is.

## Question rewrites

| Don't ask | Why | Ask instead |
|---|---|---|
| "Don't you find agents misunderstand legacy code?" | leading | "Tell me about the last time the agent got something wrong in the legacy code." |
| "Would your team attend a workshop on this?" | future, polite | "What did the team do the last time it tried to learn a new tool together?" |
| "How much would you pay for…?" | hypothetical; not your job yet | "What have you spent — time or money — trying to fix this so far?" |
| "Do you ever have review delays?" | generic ("ever" is always yes) | "What happened with the last PR that waited more than a day?" |
| "On a scale of 1–10, how painful is…?" | generic opinion | "When did it last cost you an evening or a weekend?" |
| "Is it because reviewers are overloaded and don't understand AI code?" | double-barrelled and leading | "Why do you think that PR waited?" — then stay quiet |
| "Do you like the idea of…?" | compliment fishing | (don't) |
| "My workshop covers X — would that help?" | pitch | "What have you already tried for X?" |

## After the call (within 24 hours)

- [ ] Write up in the interview log: past-specific answers vs opinion/future answers, pains heard, **verbatim quotes** (exact words, in quotation marks), commitments asked and given.
- [ ] Mark each pain as heard or not heard before; update the pain map only with past, specific events.
- [ ] Note every thread you dropped. That is your next interview's first question.
- [ ] Send the thank-you and anything you promised. Do not attach a pitch.
- [ ] Optional: run the transcript or notes through `TranscriptLint` ([Module 1 labs](../labs/module-01/README.md)) and read every flag.

## After 3–5 calls: the verdict

| Signal | Keep | Change | Kill |
|---|---|---|---|
| Your hypothesized pain heard as a past event | in ≥ 2 independent interviews | a *different* pain in ≥ 2 | in ≤ 1, and nothing else recurs |
| Workarounds or abandoned tools | present | present for the other pain | absent (nobody bothers) |
| Commitments | ≥ 1 intro or data | ≥ 1 for the other pain | none |

Five interviews find common pains; they cannot tell you how common. Write "3 of 5 interviewees", never "60% of teams".
