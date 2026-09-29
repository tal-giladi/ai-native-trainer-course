# Key — hype or pain?

For [`exercises/hype-or-pain.md`](../exercises/hype-or-pain.md). O = observable, R = recurring, C = costly, W = owned (someone with authority feels it).

| # | Class | Tests passed | Next question |
|---|---|---|---|
| 1 | **Hype** (with a real buyer) | W | "What happened that made the board ask? What would the board need to see in six months?" — the CTO is a buyer under pressure; the pain is still unknown. |
| 2 | **Pain** | O, R, C, W | "What happened to the two PRs that missed the train? Who reviews most of the 38?" |
| 3 | **Hype** (fear) | — | Don't argue. "When did the agent last do something that worried you?" turns fear into an event. |
| 4 | **Pain** + workaround | O, C, W (R unknown) | "How did you find the rounding bug, and how long did it take?" A workaround (restricting use) is the strongest pain signal. |
| 5 | **Symptom** | O, W | "Can I talk to three of the 89 who stopped?" Low adoption is a number, not a cause: it could be trust, onboarding, policy or irrelevance. |
| 6 | **Hype** (opinion) | — | "Tell me about the last ticket where it was fastest — and the last where it was slowest." Self-estimated speed-ups are unreliable (see the METR study in [13.2](../../../lessons/module-13/lesson-02.md)). |
| 7 | **Pain** + failed workarounds | O, R, C, W | "Walk me through the last release: who wrote the scripts, how long, what did they miss?" Two abandoned tools mean the pain is real and the easy fix does not work. |
| 8 | **Blocker** | W | "What would you need to see to say yes to a read-only pilot on anonymized data?" Design for it early; security is Module 9 and data residency Module 12. |
| 9 | **Hype** (solution-first) | — | "Which system does the team copy data out of by hand most often?" MCP is a mechanism (Module 8), not a problem. |
| 10 | **Pain** | O, R, C, W | "What does a new hire do in weeks 1–10? What did the last one get stuck on?" Six hires × ~10 weeks of reduced output is sizable. |
| 11 | **Symptom** (hypothetical) | — | "When did it last ignore a convention? Which one, and what did it cost?" "Would be nice" is a future wish; the past event decides. |
| 12 | **Hype** (competitive fear) | W (weakly) | "If you were 'AI-first' a year from now, what would be measurably different?" The answer may reveal a real metric — or none. |

Pattern: every **pain** row names an event, a number and a person. Every **hype** row names a technology or a feeling. Symptoms and blockers are not noise — a symptom tells you where to dig, and a blocker tells you who must be in the room before anything is sold.
