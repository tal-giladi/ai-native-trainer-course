# Discovery call — A01, Fabrikam Freight (fictional)

> **Illustrative.** 30-minute call with the engineering manager (EM) who filled in application A01, captured in notes with consent and written up the same day; roles only, no names. Script: [discovery-call script](../../../../../templates/discovery-call-script.md). Check with `OfferCheck call`.

- Date: 2026-09-15
- Application: A01
- Consent: notes only, no recording, agreed by email 2026-09-12

## Transcript (lightly condensed)

[00:00] Me: Thanks for making the time. Before anything about what I do, can you tell me about the team and what it is working on?
[00:40] Them: Sure. Six teams, forty-eight developers, all on the dispatch and billing platform. .NET Framework 4.8 mostly, a couple of .NET 8 services, SQL Server with about nine hundred stored procedures. Everyone has had a coding agent licence since spring.
[02:10] Me: You wrote that agent PRs duplicate billing rules. Tell me about the last time that happened.
[02:30] Them: Last sprint. Two PRs re-implemented the credit-hold rule in C#. The rule already lives in a stored procedure, usp_Invoice_ApplyCreditHold. Review caught one. The other shipped, and a customer got an invoice they should not have, so finance issued a refund and we spent two days on the fix and the post-mortem. The customer's account manager was not happy either. It was the third time this year something like that got through, but the first one a customer saw.
[04:00] Me: What happened in the post-mortem?
[04:20] Them: The honest answer was that the agent never looked at the database. The developer asked for the feature and it wrote it where it was asked. Nobody told it the rule existed, and the developer did not know either, because he joined in March. Our reviewers are now suspicious of every agent PR, so review takes longer than before we had the tool.
[05:40] Me: How do you know review takes longer?
[05:55] Them: I pulled it from GitHub last month. Median time from PR opened to approved went from about a day to about a day and a half since spring. I have not controlled for anything, it is just the number.
[07:00] Me: What have you tried so far?
[07:15] Them: A rules file with forty lines. Nobody maintains it. One team wrote a prompt template that says "check the database first". People forget to use it. And the rules file says nothing about the database at all, it is mostly formatting and naming rules someone copied from a blog post.
[08:30] Me: Who else feels this, apart from you and the reviewers?
[08:45] Them: The DBA lead, definitely. She reverts inline SQL every week, sometimes twice, and she has started asking to review every PR that touches data access, which is a bottleneck of its own. And finance, since the refund.
[09:40] Me: If you imagine this solved, how would you know? What number would move?
[10:00] Them: Fewer reverts from the DBA. Review time back to where it was. And no more refunds caused by duplicated rules, obviously.
[11:10] Me: Your form mentions that your VP wants a guaranteed 30% productivity improvement. Where does the 30% come from?
[11:30] Them: A vendor webinar, I think. He said if the tool makes people 30% faster, the training should guarantee it.
[12:15] Me: I can't promise 30%, and I'd be wary of anyone who does. My one measured result: one team, ninety-six tickets, cycle time down 16%, interval 5% to 25%, and escaped defects may have gone up. That is not your team. I can measure it on your tickets and report the range, even if it is zero.
[13:30] Them: He will ask why he should pay for something that might show nothing.
[13:50] Me: Fair question. What would it cost him to roll the agent and a method out to six teams and find out a year later it did not help?
[14:20] Them: Seats alone are about a hundred and fifty thousand a year. Plus the refunds. I see your point, I would rather know after six weeks.
[15:30] Me: Who decides on something like a six-week pilot, and who signs it?
[15:45] Them: I can propose it, the VP decides, and anything over ten thousand goes through procurement. They need a statement of work and a supplier form, which takes about two weeks.
[17:00] Me: Is there a budget line for this kind of thing, or would it come from somewhere else?
[17:20] Them: There is a training budget and a tooling budget. A pilot that includes building the rules and skills could come from tooling. I would need a number to take to him.
[18:30] Me: And timing: by when would you want to know whether this works?
[18:45] Them: The licence renewal is in January. If we are going to expand or cut it, I want evidence by mid-December. Nobody will approve anything over the holidays, and the vendor wants an answer in the first week of January.
[20:00] Me: Anything that would make a pilot impossible? Access to code, security reviews, people's time?
[20:20] Them: Security will not let anyone outside see production data. Code access for a contractor is possible under NDA, read-only, on our laptops. The team can give maybe two hours a week each for the pilot.
[21:30] Me: That works. I never need production access, and the comparison uses Jira timestamps and your defect tickets. Two hours a week is enough for one team, not for six.
[22:30] Them: So what would you suggest?
[23:00] Me: A pilot on one team fits: six weeks, rules and skills built with them, and a randomized comparison on their tickets with a go/no-go rule written first. DBA reverts and review time go in as measures next to cycle time and escaped defects. If you only needed awareness, the workshop alone would be cheaper, but I don't think that is your problem.
[24:30] Them: What would that cost?
[24:45] Me: The pilot is a fixed eighteen thousand dollars. I will put it in a short proposal with two options, the pilot and a workshop-only option, and what each does and does not show.
[25:30] Them: Can you include what the VP asked for, the productivity figure?
[25:50] Me: I'll include my measured range, what it would be worth if it transferred to one team, and the chance it doesn't pay back on its own. No guarantee. If that is a deal-breaker for him, I am not the right fit, and better you know now.
[27:00] Them: I think he can live with that if the numbers are honest.
[27:30] Me: Next step: I send the proposal by Friday 2026-09-18, and we go through it on Tuesday 2026-09-22 before you take it to him. Does that work?
[28:00] Them: Yes. Send me the supplier form requirements too, and I will start procurement in parallel. They will want your insurance details and a data-processing questionnaire, so have those ready.
[28:40] Me: Will do. Thank you. One last question: is there anything I should have asked?
[29:10] Them: Maybe that two of the six teams are on .NET 8 and do things differently. The pilot should probably be on a Framework team, where the stored procedures are.

## After the call

- Qualification: a dated, costed incident (refund, two days of work); sponsor (VP) and decision path (procurement over 10k, about two weeks); budget line (tooling); deadline (evidence by mid-December for a January renewal); constraints (no production data, read-only code access under NDA, two hours a week per developer).
- Expectation reset: the 30% guarantee was declined on the call; the proposal carries the range and the probability of loss.
- Their words to reuse in the proposal: "the agent never looked at the database"; "I would rather know after six weeks".
- Pilot team: a .NET Framework team that owns the billing stored procedures.
