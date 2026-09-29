# Instructor notes — 08.2 A real integration on your wedge stack

**Teaching objective.** Students connect the tracker and the code host of their wedge through the vendors' MCP servers with a read-only identity, classify every action by who notices and whether it can be undone, and prove that each of the three permission layers stops a write on its own.

**Likely confusion.** "Read-only mode" versus "read-only token". Students switch one on and believe the integration is read-only. The exercise that tests each layer separately exists to make the difference physical: without the header the tool appears; without the token scope the write succeeds.

**Common misconception.** "OAuth means least privilege." Atlassian's server uses OAuth 2.1 and acts with all of the user's existing permissions. OAuth answers *who*; it does not answer *what*. For a project admin, "as me" is a lot.

**Key analogy.** Giving an intern your laptop, logged in. They can read your mail (useful), reply to it (visible, in your name) and delete it (destructive). You would not hand it over without deciding which of the three you mean. A fine-grained token is giving them a separate account with read access to one folder.

**Common failure in the exercise.** Tokens in files: `.mcp.json` with the literal value "just for testing", or a `.env` that is not ignored. Run `git grep` on their repository together. Second: students create the fine-grained token with *All repositories*. Third: on Windows, the environment variable is set in one terminal and Claude Code is started from another.

**Expected exercise outcome.** Tickets imported as issues with the `contoso-ticket` label; GitHub server connected read-only with toolsets limited; permission rules with an allowlist; three recorded outcomes for a write attempt (host deny, tool absent, 403); T28 run with 3 trials; checklist filled for the GitHub server. The break diagnosis names ambiguity, missing layers and blast radius, not "the model misbehaved".

**Extension exercise.** Replace the personal token with a GitHub App installation or a machine account for a CI use (preview of Module 11). What changes in attribution, and what new secret-management problem appears?

**Discussion question.** A team lead says "our developers are adults; they approve every prompt anyway, so `ask` on everything is enough". What does the evidence on prompt fatigue in your own sessions say, and which actions would you still move to `deny`?
