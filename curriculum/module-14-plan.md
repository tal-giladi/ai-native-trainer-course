# Module 14 plan — Naming Your Method

4 lessons · ~60 min instruction · ~4 h practice · depends on Module 13 (`experiment-01.md`, the claims ladder), Module 5 (`NOTES.md`, failure-diagnosis log), Module 11 (AI-layer changelog, system-evolution loop) and Module 2 (failure taxonomy).

Modules 2–13 produced evidence: incidents in `NOTES.md`, changelog entries, eval results, one controlled comparison. Module 14 turns that evidence into a teachable method — a small vocabulary of named concepts, each traced to dated incidents or experiments, a loop with outputs per step, diagrams, versioning and an evolution policy — and audits it for borrowed vocabulary before anything is taught in public (Modules 15–18).

Shared lab material: `labs/module-14/` — `tools/MethodCheck` (dependency-free C#: `tooldep`, `trace`, `diff`, `borrowed`), illustrative evidence (`evidence/NOTES-contoso.md` with 14 diagnosed incidents, `evidence/experiments.md` with EXP-01 from the Module 13 worked example), `data/tool-terms.txt`, `data/terms.csv` (HumanLayer ACE, Anthropic, GitHub Spec Kit, AWS AI-DLC, this course's working names), four breaks and a reference `solution/`.

| Lesson | Objectives (short) | Prereqs | Lab | Break | Artifact |
|---|---|---|---|---|---|
| 14.1 Why vocabulary is the IP | Seven parts of a methodology; why names carry the IP (handles, quotability, semantic diffusion); separate concept from implementation; tool-change test | 13.6, 05.4 | Vocabulary inventory from own notes; method skeleton; `tooldep` | A "method" that is a Claude Code tutorial: 84% of sentences tool-dependent | `method-notes/method-v1.md` skeleton + `implementation-YYYY-MM.md` |
| 14.2 Incident to principle to name | Cluster incidents; write a falsifiable, bounded claim; status from evidence strength (Wilson interval on recurrence); name tests; paraphrase test | 14.1, 05.4, 07.4 | Mine own NOTES/changelog/report into 2–3 concept cards; `trace`; paraphrase test with a non-engineer | Concept named before its evidence, "3x faster" from one ticket, no boundary, jargon | `method-notes/concepts.md` |
| 14.3 Diagrams, versioning and evolution | Diagram rules (artifact boxes, labelled arrows, key, failure edges); semantic versioning for a method; retire never delete; evolution policy triggers; objections FAQ | 14.2, 11.5 | Loop + before/after diagrams in Mermaid; version + changelog; `diff`; evolution policy; FAQ | New step + rename + reused name shipped as 1.0.1 | `diagrams/`, `evolution-policy.md`, `faq.md`, versioned `method-v1.md` |
| 14.4 Originality, attribution and IP | Idea/expression line (17 U.S.C. §102(b)); trademarks for names; attribute / replace / justify; original synthesis vs copied framework; employer material | 14.1, 14.2 | Audit against ≥3 public frameworks + this course; credits paragraph; employer checklist | Draft with 11 borrowed terms, course loop resold as own | `attribution-audit.md`, credits in `method-v1.md` |

Math (§10): none required. 14.2 reuses the Wilson interval (07.4) for how often a failure recurs.

Simulation: none for this module.

Templates created: `templates/concept-card.md` (incl. the paraphrase exit test), `templates/method-evolution-policy.md` (incl. objections FAQ), `templates/attribution-audit.md`.

Exit test (outline): a non-engineer paraphrases the loop correctly from `concepts.md` — protocol and scoring in the concept card template, run in 14.2, rerun after 14.4.
