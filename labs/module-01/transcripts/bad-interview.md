# Transcript — a bad discovery interview

Fictional. The interviewer is a course student three weeks into picking a wedge. The participant, "Noa", is a senior developer (11 years of .NET) on the claims platform of a mid-sized insurance software company: about 35 developers, Jira and Confluence, GitHub, SQL Server 2019, a .NET Framework 4.8 monolith plus newer .NET 8 services. Everything here is invented for the exercise; any resemblance to a real company is accidental.

The call was 25 minutes. The student left it delighted: "she loved it, she'd attend, her manager is big on AI, and I got three new module ideas."

Your job in [01.4](../../../lessons/module-01/lesson-04.md): mark every interviewer question with its defect codes, find the evidence that is actually usable, and list the threads the interviewer dropped. Do it by hand first, then run `TranscriptLint` and compare.

---

**Q1 · Interviewer:** Thanks so much for doing this! So, I'm building a workshop that teaches teams to make their whole SDLC AI-native — rules files, skills, MCP, CI review, the lot. I honestly think it could be huge for teams like yours. Before I get into it, do you use AI coding tools?

**Participant:** Yeah, we have Copilot licences. A few of us use Claude Code on the side.

**Q2 · Interviewer:** Great. And don't you find that most developers are using them completely wrong?

**Participant:** Ha, probably. I mean, I don't know about "wrong". People use them differently.

**Q3 · Interviewer:** Right, exactly. Would you say AI makes you more productive?

**Participant:** I guess? It feels faster for some stuff. Hard to say.

**Q4 · Interviewer:** How much faster, would you guess — 30%? 50%?

**Participant:** Maybe 30%? On a good day.

**Q5 · Interviewer:** That's in line with what I hear. And do you usually use it for writing code or for reviewing code?

**Participant:** Writing, mostly. We tried an AI reviewer on pull requests in the spring and it was noisy. Someone turned it off after about three weeks.

**Q6 · Interviewer:** Totally, the out-of-the-box review bots are bad. That's why my workshop has a whole module on tuning CI review. Would that be useful to you?

**Participant:** Sure, that sounds useful.

**Q7 · Interviewer:** What's the biggest problem with AI tools for your team, and how would you fix it?

**Participant:** Trust, I guess. Last month one of the juniors merged a migration that Copilot wrote, and it dropped a default constraint on the Claims table. We only caught it in staging on the Thursday. It took two of us most of a day to find and fix. Anyway, you'd fix it with better prompts?

**Q8 · Interviewer:** Better context, actually — that's exactly what the rules-file part of my workshop solves. Do you think your manager would see the value in something like that?

**Participant:** Probably. He's big on AI right now; the CEO keeps asking what our AI strategy is.

**Q9 · Interviewer:** Awesome. Would your team attend a two-day workshop on this?

**Participant:** I think so, if it's during work hours.

**Q10 · Interviewer:** And do you ever have problems with agents not understanding your legacy code?

**Participant:** All the time, yeah.

**Q11 · Interviewer:** Like what kind of problems?

**Participant:** Oh, you know, it doesn't know our conventions. Everything in claims goes through stored procedures, and it keeps wanting to write EF queries.

**Q12 · Interviewer:** Right, so a rules file that says "use the stored procedures" would really help, wouldn't it?

**Participant:** Yeah, that would help.

**Q13 · Interviewer:** How much would you pay for a workshop like this? Would five thousand be crazy?

**Participant:** I'm not the one who pays. Maybe? I'd have to ask.

**Q14 · Interviewer:** Sure. And on a scale of 1 to 10, how painful is code review for you in general?

**Participant:** Seven?

**Q15 · Interviewer:** Seven, wow. So pretty painful. Is that because reviewers are overloaded and because they don't understand the AI-generated code?

**Participant:** Both, kind of. Mostly it's that two people know the claims engine, and every PR that touches it waits for one of them. Last sprint I had one sit for four days.

**Q16 · Interviewer:** Makes sense. Do you like the idea of the workshop overall?

**Participant:** Yeah, it sounds great, honestly. I'd love to see it.

**Q17 · Interviewer:** Amazing, that's really encouraging. Anything else you think I should include?

**Participant:** Maybe something on security? Our CISO is nervous about the agent seeing customer data in the claims database.

**Q18 · Interviewer:** Good idea, I'll add a security module. Thanks so much, this was super helpful!

**Participant:** Sure, good luck with it!
