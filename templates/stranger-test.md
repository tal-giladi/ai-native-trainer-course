# Stranger test template (`brownfield-demo/recordings/stranger-test-notes.md`)

Can someone who has never seen your AI layer get a working pull request out of it, alone, in 30 minutes? Introduced in [15.3](../lessons/module-15/lesson-03.md). The timeline format is what `DemoCheck timeline --mode stranger` (in `labs/module-15/tools/DemoCheck`) reads.

## Who counts as a stranger

- A working developer who has **not** seen your demo, your skills or your repository, and did not help build them.
- Comfortable with git and a terminal. Their stack does not have to be .NET; note it if not.
- Not your direct report, and ideally not someone who wants to please you.

## Protocol

1. **Before.** Fresh clone at the after tag on their machine or a clean one; they bring their own agent login. Screen and voice recording on, with their consent.
2. **Brief (read aloud, word for word).** "Pick any ticket from `tickets/`. Get it to a pull request with passing tests using what is in this repository. Please think aloud: say what you are looking for and what you expect. I will not help; if you are stuck, say so and keep going or stop. This tests my repository, not you."
3. **During.** Stay silent. Allowed: "please keep talking". Not allowed: hints, pointing, "just a hint", "you want the…", nodding at the right file. Any of those is **help**: log it, and the clock for the exit test stops there.
4. **Stop** at a PR with green tests, at 30 minutes, or when they give up. Then ask: "Where did you expect something that was not there?"
5. **Within 24 hours.** Write the timeline from the recording (not from memory), then the notes below.

## Timeline format

```text
mm:ss<TAB>kind<TAB>note
```

Kinds: `start`, `prompt`, `output`, `stuck`, `help`, `error`, `recover`, `green` (tests pass), `pr`, `end`. Start every `stuck` and `help` note with the component in brackets — `[readme]`, `[prime]`, `[plan-feature]`, `[hooks]` — so the weak points group themselves.

## Notes

```markdown
# Stranger test — <first name or initials>, YYYY-MM-DD

- Ticket: <id> · Agent: <tool and version> · Commit: <sha or tag>
- Result: PASS | FAIL (<PR with green tests at mm:ss> / <first help at mm:ss>)
- Weak points (from `DemoCheck timeline --mode stranger`):
  | Component | Stuck | Help | What they expected | Change made (AI-LAYER-CHANGELOG entry) |
  |---|---|---|---|---|
- Their words: "<the one sentence that surprised you, verbatim>"
```

## How many strangers

With a problem that hits a fraction $L$ of users, the chance that at least one of $n$ strangers hits it is $1-(1-L)^n$. One stranger finds a problem that bites a third of users only a third of the time; three find it about 70% of the time. Run the test with one person, fix what they hit, then with a new person. A stranger who saw the previous version is no longer a stranger.
