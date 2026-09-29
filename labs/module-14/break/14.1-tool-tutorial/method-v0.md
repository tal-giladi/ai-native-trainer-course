# My AI-native method (v0, deliberately flawed)

> **Break for lesson 14.1.** A student's first attempt at "writing down the method", in March 2026. Run the tool-change test: `MethodCheck tooldep break/14.1-tool-tutorial/method-v0.md --tools data/tool-terms.txt`, then ask what is left of it if the agent is replaced or its next release renames its features.

## The method

1. Start every ticket in Claude Code with plan mode on (press Shift+Tab twice).
2. Ask the Explore subagent to find the files the ticket touches.
3. Put the team conventions in CLAUDE.md and keep it under 200 lines.
4. When plan mode shows the plan, press Ctrl+G to edit it before approving.
5. After two failed corrections, run /clear and start again with a better prompt.
6. Use Opus for planning and Sonnet for implementation to save money.
7. Keep reusable prompts as skills in .claude/skills with a SKILL.md each.
8. Run /compact when the context indicator passes 60%.
9. Review the diff before committing.
10. Add a PostToolUse hook in settings.json that runs `dotnet test` after every edit.

## Why it works

Claude Code is much better when plan mode is on. The Explore subagent keeps the main context clean. CLAUDE.md gives it the conventions. Opus plans better than Sonnet. With these settings my tickets go faster and I find fewer bugs in review.

## Tips

- Use `/context` to see what is using the context window.
- Name skills with verbs.
- Cursor users can put the same conventions in .cursor/rules.
- Copilot users can use copilot-instructions.md.
