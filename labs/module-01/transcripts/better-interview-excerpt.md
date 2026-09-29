# Transcript excerpt — the same participant, re-interviewed

Fictional, continuing [`bad-interview.md`](bad-interview.md). Two weeks later the student asked Noa for 30 minutes more and used the [customer discovery script](../../../templates/customer-discovery-script.md). This is minutes 7–19, in the plain `I:` / `P:` format that `TranscriptLint` also reads.

I: Last time you mentioned a migration that dropped a default constraint. Can you walk me through it, from when it was written?

P: Sure. A junior had a ticket to add a column to Claims. He asked Copilot for the migration, it generated the ALTER plus a rebuild of the table, and the rebuild didn't carry over the default on StatusCode. Review was me, on a Friday afternoon, and I looked at the C# more than the SQL.

I: What happened next?

P: Nothing until Thursday, when staging started rejecting new claims. It took two of us most of the day to find it, because the error was in a trigger, not in the migration.

I: What did you change afterwards?

P: We made a rule that juniors don't use the agent for migrations. And our DBA lead now reviews every migration by hand, which means they wait for him. Last month that was eleven migrations.

I: How did the DBA lead take that?

P: He hates it. He said in the retro that he's become a human linter.

I: Who else felt it?

P: Our manager, because the release slipped a day. He asked me for a list of what the agent is allowed to touch. I never wrote it.

I: What would it take for me to see what that list would have needed?

P: I could ask the DBA lead to talk to you. He has a list of the conventions in his head — defaults, the audit columns, the naming of constraints.
