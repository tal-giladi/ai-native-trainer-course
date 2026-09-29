---
id: "14.4"
module: 14
minutes: 15
practice_minutes: 60
prerequisites: ["14.1", "14.2"]
objectives:
  - Explain, as orientation, what copyright, trademark and contracts do and do not protect in a methodology.
  - Distinguish original synthesis from a copied framework with the four-question test.
  - Run an attribution audit against at least three public frameworks and this course, deciding attribute, replace or justify for every borrowed term.
  - Check employer and client material before it appears in anything you teach or publish.
volatility: concept
sources:
  - title: "17 U.S. Code § 102 — Subject matter of copyright: In general"
    url: https://www.law.cornell.edu/uscode/text/17/102
  - title: "U.S. Copyright Office — Circular 33: Works Not Protected by Copyright"
    url: https://www.copyright.gov/circs/circ33.pdf
  - title: "USPTO — Why search for similar trademarks"
    url: https://www.uspto.gov/trademarks/basics/why-search-similar-trademarks
  - title: "Creative Commons — Recommended practices for attribution"
    url: https://wiki.creativecommons.org/wiki/Recommended_practices_for_attribution
  - title: "HumanLayer — Advanced Context Engineering for Coding Agents"
    url: https://github.com/humanlayer/advanced-context-engineering-for-coding-agents/blob/main/ace-fca.md
  - title: "GitHub — Spec Kit"
    url: https://github.com/github/spec-kit
  - title: "AWS DevOps Blog — AI-Driven Development Life Cycle: Reimagining Software Engineering"
    url: https://aws.amazon.com/blogs/devops/ai-driven-development-life-cycle/
last_verified: "2026-09-29"
---

# 14.4 · Originality, attribution and intellectual property

## Why it matters

The career path this course grew from ends with a guardrail titled "Don't sell a copy": cloning a trainer's public repository and reselling training built on it — "same rule names, same skill files, same diagrams" — is passing off someone else's method as your own expertise. It adds that it is also fragile: "the first hard question from a client will expose that you didn't build it."

That applies to this course too. For thirteen modules you have used its working names: research → plan → implement → validate, AI layer, system evolution, claims ladder. They were chosen to teach you the mechanisms, not to be resold. And the space is crowded. As of 2026-09 at least four public sources describe an understand → plan → build → verify loop for coding agents, each with its own vocabulary. Your audience has read some of them. If your method uses their words without credit, the most informed person in the room will notice first — and that is usually the person who decides whether to hire you.

The good news is that honesty here costs almost nothing. You do not need a loop nobody has thought of. You need to credit the shape, keep your own evidence at the centre, and be precise about what you add.

> [!CAUTION]
> This lesson is **orientation, not legal advice**. Copyright, trademark and employment law differ by country and change; the statutes cited are US examples. Before you register a name, sign a licensing deal or publish material that touches an employer's or client's work, talk to a lawyer in your jurisdiction.

## How it works

### What protects what

| Protection | Covers | Does not cover | What it means for you |
|---|---|---|---|
| **Copyright** | Expression: your text, slides, diagrams as drawn, code, recordings | "any idea, procedure, process, system, method of operation, concept, principle" (US 17 U.S.C. §102(b)); words, short phrases, names and titles (Copyright Office Circular 33) | Nobody owns a loop, including you. Copying someone's pages, slides or diagrams is a different matter. |
| **Trademark** | Words, phrases, symbols used to identify goods or services in commerce | Descriptive use of ordinary words | A catchy method name may already be someone's mark. The USPTO names "likelihood of confusion" with an existing mark as a main reason for refusal; search before you brand. |
| **Contracts and licenses** | Whatever you agreed: employment IP assignment and confidentiality, client NDAs, open-source and Creative Commons licenses | Anything outside their terms | Your employer may own material you made at work. An MIT-licensed template may be reused with its notice; a CC BY text needs attribution. |

Most jurisdictions draw a similar line between ideas and their expression, but the details differ — which is why the callout above is not a formality.

Two consequences run in opposite directions. The law will not stop anyone from teaching a loop like yours, so a method's value cannot rest on legal protection. And the law will not stop *you* from using someone's loop either — which is exactly why attribution is a matter of honesty and credibility, not compliance.

### The public landscape (as of 2026-09)

| Source | Loop / phases | Coined terms |
|---|---|---|
| HumanLayer, *Advanced Context Engineering for Coding Agents* (Dex Horthy, 2025) | research → plan → implement | frequent intentional compaction |
| Anthropic, Claude Code best practices | explore → plan → implement → commit | — |
| GitHub Spec Kit (MIT license) | constitution → specify → plan → tasks → implement (recent versions add a converge step) | spec-driven development, constitution |
| AWS, *AI-Driven Development Life Cycle* (2025) | Inception → Construction → Operations | mob elaboration, mob construction, bolts, units of work |
| This course (working names) | research → plan → implement → validate | AI layer, system evolution, claims ladder, paste-and-go |

Everyone converges on the same shape. That convergence is evidence the shape is right, and proof that the shape cannot be your contribution.

### Original synthesis vs copied framework: four questions

1. **Shape.** Is the structure the same as a public one? (For a loop, almost certainly yes.)
2. **Words.** Are the step and concept names someone else's?
3. **Evidence.** Is every concept traced to your own dated incidents or experiments ([14.2](lesson-02.md))?
4. **Contribution.** Can you say in one sentence what you add — and would the source's author agree it is not already in their work?

A **copied framework** answers yes, yes, no, no. An **original synthesis** may answer yes to shape, but credits it, uses its own words for its own ideas, and has evidence and a stated contribution. The Contoso student's contribution sentence: "what each step must output on legacy .NET code, and the rule that the agent's own tests do not count as proof — from fourteen diagnosed incidents and one randomized comparison."

### Attribute, replace or justify

For every borrowed term, one decision:

- **Attribute** — the term is the best word and your audience knows it: keep it and credit the source in your method's Credits section.
- **Replace** — you used it for something your evidence describes better: use your own word, tied to your own concept.
- **Justify** — a generic term with no single owner ("context engineering", "code review"): say why in the audit.

The same applies beyond words: templates, diagrams, exercises and examples. For Creative Commons material, the recommended attribution is TASL — title, author, source, license. For MIT-licensed files, keep the license notice. For ideas you learned from a person — a talk, a colleague, this course — name them in the credits even though nothing requires it.

### Employer and client material

Your concepts are built on incidents from real work. Before any of it leaves your private `method-notes`:

- **Code, ticket text, screenshots, customer names, internal URLs:** never. Reproduce the incident on a demo repository instead (Module 15 builds one).
- **Numbers** ("cycle time −16%"): only with explicit permission, or anonymized past recognition — company, team size and domain all changed or removed.
- **The incident itself** as a story: anonymize the organization and people; check your employment agreement's confidentiality and IP clauses.
- **Your method as a whole:** if you built it on company time with company data, read your IP assignment clause before you sell it.

## Show me

The student's draft (`labs/module-14/break/14.4-borrowed/method-draft.md`) against `data/terms.csv`, then the fix:

```text
$ dotnet run --project tools/MethodCheck -- borrowed solution/method-v1.0.0.md \
    --terms data/terms.csv --audit solution/attribution-audit.md
  attributed research -> plan -> implement  (HumanLayer)
  replaced   frequent intentional compaction  (HumanLayer)
  attributed explore -> plan -> implement -> commit  (Anthropic)
  justified  context engineering  (Anthropic): General industry term used by many vendors…
  replaced   spec-driven development  (GitHub Spec Kit)
  replaced   constitution  (GitHub Spec Kit)
  replaced   research -> plan -> implement -> validate  (AI-Native Trainer course)
  attributed ai layer  (AI-Native Trainer course)
  replaced   system evolution  (AI-Native Trainer course)
  replaced   productivity mirage  (AI-Native Trainer course)
  replaced   claims ladder  (AI-Native Trainer course)
  replaced   paste-and-go  (AI-Native Trainer course)
4 borrowed term(s) found, 4 resolved, 0 open
```

The credits paragraph in `method-v1.0.0.md`: "The four-step shape is the common research, plan, implement pattern, described as research → plan → implement by HumanLayer (Advanced Context Engineering for Coding Agents) and as explore → plan → implement → commit in Anthropic's Claude Code best practices; this method's contribution is what each step must output on legacy code and the evidence for why. 'AI layer' is used as in the AI-Native Trainer course."

Note what the replacements are: *productivity mirage* became the student's own *Late-PR illusion*, which has its own experiment; *paste-and-go* became *ticket-only run* in teaching, with the failure itself now *Shadow rule*. Every replacement points at evidence.

## Try it

Budget: 60 minutes.

1. **Choose frameworks (10 min).** At least three public frameworks your audience is likely to know, plus this course and any other course, book or trainer you learned from. Read each one's main page again, today.
2. **Extend the term list (10 min).** Copy `labs/module-14/data/terms.csv` to `method-notes/` and add terms from your frameworks.
3. **Word audit (15 min).** Run `borrowed` on `method-v1.md`, `concepts.md` and your diagram files. Fill section 2 of the [attribution audit template](../../templates/attribution-audit.md) with a decision for every hit.
4. **Shape audit (10 min).** Fill section 3 — loop shape, diagrams, templates, examples — by hand. Write your contribution sentence and the credits paragraph.
5. **Names and employer check (15 min).** Search the web and your country's trademark register for each concept name and your method's name. Fill section 4 for every incident, number and screenshot in your method.

```bash
cd labs/module-14
dotnet run --project tools/MethodCheck -- borrowed ~/method-notes/method-v1.md \
  --terms ~/method-notes/terms.csv --audit ~/method-notes/attribution-audit.md
```

<details>
<summary>Hint: after replacing the course's words, my loop has no good names left</summary>

Name steps by their **output** or their **question**, taken from your own evidence: what did the step exist to stop? The Contoso student's *Ground* exists to stop shadow rules (find what already exists); *Prove* exists to stop self-graded green. If a step has no incident behind it, a plain verb is fine. It is also fine to keep a common word and credit it: "plan" does not need a synonym.
</details>

## Break it

Open `labs/module-14/break/14.4-borrowed/method-draft.md`, written the week before a lunch-and-learn. It is accurate about what the student does. It calls itself "my approach", has a Research → Plan → Implement → Validate loop, calls the rules file "the team's constitution", mentions spec-driven development, frequent intentional compaction, paste-and-go, the productivity mirage, system evolution and the claims ladder, and has no credits section. Its diagram is the course's four boxes with a "System evolution" back-edge.

Predict how many borrowed terms the checker finds, and which part of the problem it cannot see. Then:

```bash
dotnet run --project tools/MethodCheck -- borrowed break/14.4-borrowed/method-draft.md --terms data/terms.csv
```

## Fix it

**Diagnose.** Eleven borrowed terms from four sources, none resolved. The checker cannot see the rest: the draft's loop and diagram are this course's, with nothing added, and the whole thing answers the four questions yes, yes, no, no. Two terms also misdescribe the student's work — they do not write specs as the source of truth, and they do not manage a context-utilization band — so leaving them in would be inaccurate as well as uncredited.

**Modify.** Decide every term (`solution/attribution-audit.md`): attribute the loop shape to HumanLayer and Anthropic and "AI layer" to this course; justify "context engineering" as a generic term; replace the rest with the student's own words, each tied to a concept or an incident. Rename the loop from the student's evidence (Ground, Bound, Build, Prove), write the contribution sentence and the credits paragraph, redraw the diagram from their own loop, and check that no incident in the method carries a customer name or an unapproved number.

**Rerun.** `borrowed solution/method-v1.0.0.md --audit solution/attribution-audit.md`: four borrowed terms found, four resolved, none open. Then run the paraphrase test from 14.2 again with a new non-engineer: renaming everything is only a fix if the new names are still understood.

## How do I know it works?

- [ ] `borrowed` reports no open terms against at least three public frameworks and this course.
- [ ] Section 3 of the audit covers the loop shape, diagrams, templates and examples, and the method has a credits paragraph with a contribution sentence.
- [ ] Every concept and the method name have been searched on the web and in a trademark register, with the date recorded.
- [ ] No code, ticket text, customer or colleague name, or unapproved employer number appears in anything outside your private repository.
- [ ] The source author of your closest public framework could read your credits and agree with them.

## Use / don't use

**Use** the audit before every public release — talk, post, workshop — and whenever you read a new framework in your space. **Use** credits generously: naming your sources signals that you know the field, which is part of what clients pay for.

**Don't** rename borrowed ideas to hide them; a synonym for someone else's concept, uncredited, is still a copy, and an easier one to catch. **Don't** register or advertise a method name before the trademark search. **Don't** treat "it's not illegal" as the standard: the standard is whether the most informed person in your audience would think you were honest.

**Limitations.**

- The term list is only as complete as your reading. The checker finds words you told it about; a framework you have not read is invisible to it.
- Independent invention happens, and in a converging field it is common. It does not remove the need to credit what you did read, and it is hard to prove, so err toward crediting.
- Legal content here is US-centred orientation and changes over time; it is no substitute for advice where you work.

## Reflect

1. Which term was hardest to give up, and what did you replace it with?
2. What is your one-sentence contribution, and would the closest framework's author agree?
3. Which incident in your method would your employer recognize, and what have you done about it?

## Sources

- [17 U.S. Code § 102](https://www.law.cornell.edu/uscode/text/17/102) — §102(b): copyright does not extend to any idea, procedure, process, system, method of operation, concept or principle.
- [U.S. Copyright Office — Circular 33](https://www.copyright.gov/circs/circ33.pdf) — ideas, methods and systems, and words, short phrases, names and titles, are not protected by copyright.
- [USPTO — Why search for similar trademarks](https://www.uspto.gov/trademarks/basics/why-search-similar-trademarks) — likelihood of confusion with an existing mark as a main reason for refusal; search federal, state and internet sources first.
- [Creative Commons — Recommended practices for attribution](https://wiki.creativecommons.org/wiki/Recommended_practices_for_attribution) — TASL: title, author, source, license.
- [HumanLayer — Advanced Context Engineering for Coding Agents](https://github.com/humanlayer/advanced-context-engineering-for-coding-agents/blob/main/ace-fca.md) — research, plan, implement phases and "frequent intentional compaction" (Dex Horthy, 2025).
- [GitHub — Spec Kit](https://github.com/github/spec-kit) — spec-driven development with constitution, specify, plan, tasks and implement steps; MIT license.
- [AWS DevOps Blog — AI-Driven Development Life Cycle](https://aws.amazon.com/blogs/devops/ai-driven-development-life-cycle/) — Inception, Construction and Operations phases; mob elaboration, mob construction, bolts and units of work (2025).
