# Module 14 labs — Naming your method

Everything the Module 14 labs need. Lessons: [14.1](../../lessons/module-14/lesson-01.md) · [14.2](../../lessons/module-14/lesson-02.md) · [14.3](../../lessons/module-14/lesson-03.md) · [14.4](../../lessons/module-14/lesson-04.md).

The labs turn your `NOTES.md` from [Module 5](../module-05/README.md), your AI-layer changelog from [Module 11](../module-11/README.md) and your experiment report from [Module 13](../module-13/README.md) into `method-notes/`: named concepts traced to dated evidence, a versioned method, diagrams, an evolution policy, an objections FAQ and an attribution audit.

> [!WARNING]
> Your incidents come from your employer's code and your team's work. Keep `method-notes/` **private** until it is anonymized: no customer names, internal URLs, code, ticket text or colleagues' names, and no employer metrics without permission. Lesson 14.4 has the checklist.

## Requirements

- .NET SDK 8 or newer (`RollForward=Major`, so a newer runtime works too). Verified with SDK 10.0.400.
- No NuGet packages. `MethodCheck` is read-only and deterministic; it works offline.

## Contents

| Path | What it is |
|---|---|
| `tools/MethodCheck/` | Dependency-free C# checker: `tooldep` (tool-change test), `trace` (concept cards and their evidence trail), `diff` (version bump, changelog, name reuse), `borrowed` (terms from public frameworks vs your attribution audit). |
| `evidence/` | **Illustrative** evidence from one student's Contoso Billing work: `NOTES-contoso.md` (ticket log and 14 diagnosed incidents) and `experiments.md` (EXP-01, the Module 13 worked example). |
| `data/tool-terms.txt` | Tool- and release-specific words for `tooldep` (as of 2026-09). |
| `data/terms.csv` | Terms from four public sources (HumanLayer, Anthropic, GitHub Spec Kit, AWS AI-DLC) and from this course's own working names, for `borrowed`. |
| `break/` | One deliberately flawed file per lesson: a tool tutorial posing as a method (14.1), anecdote-based concept cards (14.2), a breaking change released as a patch (14.3), a draft full of borrowed vocabulary (14.4). |
| `solution/` | Reference `concepts.md`, `method-v1.0.0.md`, `method-v2.0.0.md`, `implementation-2026-09.md`, `diagrams.md`, `evolution-policy.md`, `faq.md`, `attribution-audit.md`. One student's answers on illustrative data: compare the structure, never copy the names. |

## Quick start

From this folder:

```bash
M="dotnet run --project tools/MethodCheck --"

# 14.1 — the tool-change test
$M tooldep break/14.1-tool-tutorial/method-v0.md --tools data/tool-terms.txt   # 84%, fails
$M tooldep solution/method-v1.0.0.md --tools data/tool-terms.txt               # 3%, passes

# 14.2 — concept cards and their evidence
$M trace break/14.2-anecdote/concepts.md    # 6 errors, 2 warnings
$M trace solution/concepts.md               # clean

# 14.3 — versioning
$M diff solution/method-v1.0.0.md break/14.3-silent-rename/method-v1.0.1.md   # 2 errors
$M diff solution/method-v1.0.0.md solution/method-v2.0.0.md                    # clean

# 14.4 — attribution audit
$M borrowed break/14.4-borrowed/method-draft.md --terms data/terms.csv          # 11 open
$M borrowed solution/method-v1.0.0.md --terms data/terms.csv --audit solution/attribution-audit.md   # 0 open
```

(PowerShell: type the full `dotnet run --project tools/MethodCheck -- <command>` instead of `$M`.) Exit code 0 means clean, 1 means errors, 2 means a usage problem, so every check can run in CI on your `method-notes` repository.

## Your own `method-notes/`

```text
method-notes/
├── experiment-01.md            # Module 13
├── concepts.md                 # 14.2 — cards from templates/concept-card.md
├── method-v1.md                # 14.1 skeleton, 14.3 version and changelog, 14.4 credits
├── implementation-2026-09.md   # 14.1 — the tool mapping, outside the method's version
├── diagrams/loop.md            # 14.3 — Mermaid, same version as the method
├── diagrams/before-after.md    # 14.3
├── evolution-policy.md         # 14.3 — templates/method-evolution-policy.md
├── faq.md                      # 14.3 — objections and honest answers
└── attribution-audit.md        # 14.4 — templates/attribution-audit.md
```

Copy `data/tool-terms.txt` and `data/terms.csv` into it and extend them with the tools your team uses and the frameworks your audience knows.

## Lab order

1. **14.1 — Why vocabulary is the IP.** Vocabulary inventory from your own notes; method skeleton; tool-change test. *Break:* a tool tutorial posing as a method.
2. **14.2 — Incident to principle to name.** Mine incidents into 2–3 concept cards; paraphrase test with a non-engineer. *Break:* a concept named before its evidence, with a number and no interval.
3. **14.3 — Diagrams, versioning and evolution.** Loop and before/after diagrams, `Version:` and changelog, evolution policy, objections FAQ. *Break:* a new loop step, a rename and a reused name released as a patch.
4. **14.4 — Originality, attribution and IP.** Audit against three or more public frameworks and this course; credits paragraph; employer check. *Break:* a draft that resells borrowed vocabulary.

## Expected results

| Check | Break | Reference |
|---|---|---|
| `tooldep` | 16 of 19 sentences (84%) | 1 of 34 (3%) |
| `trace` | 6 errors, 2 warnings | 0 |
| `diff` | required MAJOR, actual PATCH; name reuse | required MAJOR, actual MAJOR |
| `borrowed` | 11 borrowed terms, 0 resolved | 4 found, 4 resolved |
