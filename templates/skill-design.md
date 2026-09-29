# Skill design template

Fill this in **before** writing `SKILL.md`. A skill is a procedure the agent loads on demand; if what you are about to write is a fact, a guarantee or an external system, it belongs somewhere else ([03.2 decision rule](../lessons/module-03/lesson-02.md)). Introduced in [06.1 · Skill anatomy](../lessons/module-06/lesson-01.md); tested as in [06.4](../lessons/module-06/lesson-04.md). Checked by `SkillCheck lint` (`labs/module-06/tools/SkillCheck`).

## 1. Why this skill exists

- The repeated procedure it captures (what you kept typing into prompts): …
- Evidence it is repeated: tickets, sessions or dates (at least 3)
- What goes wrong today without it: …
- Why not a rule, a test, a hook or a sub-agent instead: …

## 2. Trigger

| Question | Answer |
|---|---|
| Who invokes it? | model and user · user only (`disable-model-invocation: true`) · model only (`user-invocable: false`) |
| Why that choice? | side effects? cost? must a person decide when? |
| Description (third person, what + when + not-when, under 1,024 chars) | … |
| 5 queries that must load it | … |
| 5 near-miss queries that must not | … |
| Scope limit (`paths:` globs), if any | … |

## 3. Inputs

| Input | Source (`$0`, `$ARGUMENTS`, file, injected command) | Required? | What happens if missing |
|---|---|---|---|
| | | | stop and say … |

## 4. Output contract

The observable result a caller or a check can rely on. Write it so a script can verify most of it.

- Artifact(s) and path(s): …
- Required sections / fields: …
- Invariants (things that must always be true): …
- What the chat reply contains (keep artifacts in files, not chat): …
- Checked by: `SkillCheck contract <file> --rules <skill>/contract.rules` · `LoopGate …` · other: …

## 5. Steps

| # | Step | Deterministic (script/command) or model? | Why that side | Verify |
|---|---|---|---|---|
| 1 | | | | |

Rule of thumb: anything with one right answer (numbering, file names, running gates, collecting a diff) is a script; anything that needs judgment (writing SQL, choosing an approach) is a model step followed by a deterministic check. Stop rule: after two failed checks, stop and report.

## 6. Context cost

- Description length: … chars (always in context while model-invocable)
- `SKILL.md` body: … lines (~… tokens), re-sent on every later turn once loaded
- Reference files loaded only when: …

## 7. Tests (before release)

- [ ] `SkillCheck lint` passes.
- [ ] Trigger test: … queries × … runs; recall ≥ …, false-trigger rate ≤ …
- [ ] Ticket test set covers every ticket category the skill will meet (list them): …
- [ ] Golden checks for the riskiest ticket: …
- [ ] A colleague used it cold, without help; notes: …

## 8. Version and ownership

- `metadata.version`: … (SemVer: major = a caller or check relying on the old output breaks; minor = new step, rule or capability; patch = wording)
- `CHANGELOG.md` entry written: yes / no
- Owner (CODEOWNERS): …
- Portability: works in (Claude Code / Cursor / Copilot / Codex-class) — tool-specific fields used: …
