# Concept card template (`method-notes/concepts.md`)

One card per named concept in your method. Introduced in [14.2](../lessons/module-14/lesson-02.md); versioned under the rules of [14.3](../lessons/module-14/lesson-03.md). The format is what `MethodCheck trace` (in `labs/module-14/tools/MethodCheck`) reads, so keep the field names.

Rules:

- A concept is a **claim about how agent work fails or succeeds**, with a boundary and a dated evidence trail. A tip, a tool feature or a slogan is not a concept.
- **Name after evidence.** The `Named` date must be on or after at least one evidence item.
- **Status follows evidence:** `hypothesis` (1–2 incidents), `supported` (3+ incidents on 2+ dates, or an experiment with an interval), `retired` (kept in the file with a `Retired:` line; the name is never reused).
- **Numbers need intervals.** Any percentage or multiplier in `Claim` or `Plain words` needs "CI" and an `EXP-` evidence item.
- **Plain words** are for a non-engineer: 30 words or fewer, no tool names, no jargon.

## Card

```markdown
## C1 · <Name, two to four words, a noun phrase>
- Status: hypothesis | supported | retired
- Named: YYYY-MM-DD
- Plain words: <one sentence a product manager could repeat>
- Claim: <when X, Y happens (because Z); doing W prevents it — falsifiable, scoped>
- Boundary: <where it does not apply>
- Counter-evidence: <incidents that cut against it, or "none yet" and what would count>
- Evidence:
  - YYYY-MM-DD · INC-07 · <one line> ([NOTES](../path/NOTES.md))
  - YYYY-MM-DD · EXP-01 · <one line> ([report](experiment-01.md))
- Retired: <YYYY-MM-DD, why, where the idea went — only for retired concepts>
```

## Name checklist

- [ ] Describes the mechanism or the symptom, not a feeling ("Shadow rule", not "The Phoenix Effect").
- [ ] Two to four words; works in a sentence: "that's a ___".
- [ ] Not already in use for something else: searched the web, the three frameworks your audience knows, and your trademark register (see [14.4](../lessons/module-14/lesson-04.md)).
- [ ] Does not blame a person or anthropomorphize the model ("lie", "lazy").
- [ ] A colleague who saw the incidents would recognize it without the definition.

## Paraphrase test (the module's exit test)

Give a non-engineer (a PM, your manager, a designer) `concepts.md` and your loop, 10 minutes, no help. Then ask them, without the page:

1. "Walk me through the steps and what each one produces."
2. For each concept: "When would you see this happen, and what would you do about it?"

| Item | Correct | Partial | Wrong | Their words (verbatim) |
|---|---|---|---|---|
| Loop step 1 | | | | |
| Loop step 2 | | | | |
| … | | | | |
| C1 | | | | |
| C2 | | | | |

Pass: every loop step and every concept "correct" or "partial" with at most one "partial". For every "wrong", fix the page, not the reader, and retest with someone new.
