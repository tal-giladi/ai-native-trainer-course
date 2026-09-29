# Method evolution policy template (`method-notes/evolution-policy.md`)

How your **method** changes: the loop, the concepts and the claims you teach. Introduced in [14.3](../lessons/module-14/lesson-03.md). Not the same as the team's AI-layer governance policy ([governance template](governance-policy.md), [11.5](../lessons/module-11/lesson-05.md)), which governs rules files, skills and gates in one repository. One page.

## 1. What is versioned

- `method-vN.md` — problem, loop, concepts (id, name, status), evidence summary, what it does not claim, credits, changelog. Carries `Version: MAJOR.MINOR.PATCH`.
- `concepts.md` — the cards ([concept card template](concept-card.md)).
- `diagrams/` — diagrams as text (Mermaid), the same version as the method.
- `implementation-YYYY-MM.md` — tool mapping; changes freely, reviewed quarterly, **not** part of the method's version.

## 2. Version rules

| Change | Bump | Examples |
|---|---|---|
| Loop step added, removed, renamed or reordered | MAJOR | 4 steps → 5 |
| Concept removed or retired | MAJOR | a concept you teach is withdrawn |
| Concept renamed without keeping the old name as an alias | MAJOR | (avoid; prefer an alias) |
| Concept added; status changed (except to retired); renamed with `formerly "…"` | MINOR | hypothesis → supported |
| Wording, examples, evidence added without a status change | PATCH | a new incident on a supported concept |

Never reuse a retired or former name for a different meaning. Never delete a concept; retire it.

## 3. Triggers for a review

- [ ] An incident that contradicts a concept (counter-evidence) — within 2 weeks.
- [ ] A new experiment report — before you next teach.
- [ ] A major change in the agent or model you use — check `implementation-*.md` and every claim with a number.
- [ ] An objection from an audience you could not answer from evidence — add to the FAQ, decide within a month.
- [ ] A published study or framework that covers the same ground — attribution audit (14.4).
- [ ] Otherwise: quarterly.

## 4. Who and how

- Owner: [you]. Reviewer for MAJOR changes: [a peer who has seen the evidence].
- Every change: a changelog entry with the evidence ID that caused it (INC-, EXP-, FAQ-).
- MAJOR changes: list the teaching materials that must change (slides, posts, workshop exercises) and update them before the next delivery. Published posts keep their date and the version they describe.

## 5. Objections FAQ (`method-notes/faq.md`)

For each hard objection you have heard: the objection in the asker's words, who asked (role, anonymized), your answer, the evidence behind it, and what you do **not** know.

| ID | Objection (their words) | Asked by | Answer | Evidence | What we do not know |
|---|---|---|---|---|---|
| FAQ-01 | | | | | |
