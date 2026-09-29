---
id: "22.2"
module: 22
minutes: 16
practice_minutes: 60
prerequisites: ["22.1", "14.4", "16.5", "18.4"]
objectives:
  - Compare a live workshop, a self-paced course, a team licence and a paid community by what each can carry (feedback, pace, completion, your hours) and choose the next format from your own evidence.
  - Compute revenue per live day and the share of revenue that grows without more live days for a catalog of three to five products.
  - Assign a licence to every artifact you publish or sell (software licence for code, Creative Commons for documents, your own terms for paid material) and write team-licence terms with seats, term, resale and attribution, as orientation only.
  - Write community rules with response time, office hours, exclusions, moderation and a closing promise, and check the catalog with ScaleCheck catalog.
volatility: concept
sources:
  - title: "Reich and Ruipérez-Valiente (2019) — The MOOC pivot (Science 363(6423))"
    url: https://pubmed.ncbi.nlm.nih.gov/30630920/
  - title: "Creative Commons — About CC Licenses"
    url: https://creativecommons.org/share-your-work/cclicenses/
  - title: "Creative Commons — Frequently Asked Questions (can I apply a CC license to software?)"
    url: https://creativecommons.org/faq/
  - title: "Open Source Initiative — The MIT License"
    url: https://opensource.org/license/mit
  - title: "U.S. Copyright Office — Circular 1: Copyright Basics"
    url: https://www.copyright.gov/circs/circ01.pdf
last_verified: "2026-09-28"
---

# 22.2 · Workshop to course to community

## Why it matters

The trainer whose business this course studied runs a four-stage flywheel: practitioner work, free public teaching, a paid community with courses and office hours, and private workshops capped per month. The source path's advice for months 6 to 12 is to turn the workshop kit into a self-serve course, open a small paid community "even 10–20 people", and treat that community as the R&D loop for what to teach next. The exit signal is revenue from the course and community that is non-trivial next to workshop revenue, without more live hours.

That advice is sound, and it hides three traps. First, a recording is not a course: the live room's feedback, pace and peer pressure are what produced your learning gain, and they do not come with the video. Second, the moment you publish or sell material, questions you never had to answer arrive: who may copy it, share it inside a 400-person company, resell it, or keep using it after you stop. Third, a community sold as "direct access to me" is a retainer for every member at once. This lesson walks the formats, the numbers, the licences and the rules, so that the step from workshop to course to community multiplies your evidence instead of diluting it.

> [!CAUTION]
> **Licensing content is orientation only and jurisdiction-dependent.** Copyright, contract and consumer law differ by country, and your employment contract and client SOWs may already decide who owns what (20.5, 14.4). The licences named here are widely used public licences, described by their own publishers; they are not advice for your situation. Before you sell a course or a team licence, have a lawyer where you work look at your terms.

> [!NOTE]
> Content tags. **Concept** (stable): what each format carries, revenue per live day, licence layers, team-licence terms, community rules. **Implementation**: the `ScaleCheck catalog` rules, the licence versions (MIT, CC BY 4.0) and the example prices (USD, as of 2026-09).

## How it works

### What each format can carry

| Format | Carries | Loses | Your hours | Evidence you can claim |
|---|---|---|---|---|
| Live workshop | feedback in the moment, pace set by the room, peers, your judgement on their questions | scale; it caps at your calendar | 1–2 days per delivery | your measured gain (16.5) |
| Self-paced course | the content and the exercises, at the learner's pace, at any volume | the room: nobody notices a learner who is stuck | upkeep only | its own gain and completion, measured separately |
| Team licence | the course plus a short live kickoff for one company | per-learner price; control over who takes it | a kickoff per sale | completion inside the team |
| Paid community | questions, office hours, peers who share your wedge | depth on any one member's code | fixed hours a month | the questions log (18.4) and what you build from it |

The number that should worry you is completion. Reich and Ruipérez-Valiente's analysis of MIT and Harvard courses on edX found that only about 3% of participants completed their course in 2017–18, and 52% of those who registered never started. Paid, short, focused courses do better than free MOOCs, but the lesson transfers: **your workshop's gain is not the course's gain**. The reference student measured it: the same parallel forms gave 0.57 live and 0.41 for the self-paced beta cohort, different people, and the course page says so.

So choose the next format from evidence, not from the business model you admire. A course needs a workshop whose outline, exercises and assessment already work with strangers (22.3). A community needs people who already ask you good questions; the source path's advice is that the first ten members should be people from your public teaching and pilots who asked good questions for free (Module 18).

### Revenue per live day

*Intuition.* Scaling means revenue that grows without your calendar growing with it. The measurable version: revenue divided by the live days it takes, and the share of revenue that does not need more live days when volume rises.

*Equation.* For products $i$ with price $P_i$, units per month $u_i$, live days per unit $\ell_i$ and fixed live days per month $f_i$:

$$\text{revenue per live day} = \frac{\sum_i P_i u_i}{\sum_i (\ell_i u_i + f_i)}, \qquad \text{scalable share} = \frac{\sum_{i:\,\ell_i = 0} P_i u_i}{\sum_i P_i u_i}.$$

*Tiny example.* One workshop (6,500, 1.5 live days) alone is 4,333 per live day, and none of it scales. Add 15 course seats at 390 (5,850, no live days) and 30 community members at 39 (1,170, a fixed 0.75 days a month): $13{,}520 / 2.25 = 6{,}009$ per live day, and $7{,}020/13{,}520 = 52\%$ of revenue grows without more live days.

*Interpretation.* The community's fixed days are real: it counts as scalable in the share (more members do not need more days, up to a point), but not in the "free" sense. And the course's number is only as good as its seats forecast, which is a range.

### Licences, layer by layer

Copyright in your text, slides, videos and code generally exists from the moment you create and fix the work, without registration (US Copyright Office, Circular 1; similar rules apply in most countries). A licence is how you tell others what they may do with it. Think in layers:

| Layer | Typical choice | Why |
|---|---|---|
| Public code (starter pack, hooks, scripts) | a software licence such as **MIT** | Creative Commons itself recommends against CC licences for software; MIT's one condition is that the copyright and permission notice travel with copies |
| Public documents (posts, templates, exercises) | **CC BY 4.0** | anyone may share and adapt with credit; your name travels with every copy |
| Your method's name, concepts, diagrams | all rights reserved; quoting with attribution welcome | the names are what make it yours (14.1); others may cite, not rebrand |
| Paid course | your own terms: personal licence, one seat, no redistribution, updates for a stated period | what the buyer paid for, and nothing more |
| Team licence | your own terms: internal use, **named seats**, **term**, **no resale or sublicensing**, **attribution kept**, **which updates** | the five questions a company's procurement will ask anyway |
| Client deliverables | per the SOW: background IP retained, licence to what is embedded (20.5) | already agreed |

Two licence traps. **NC** (non-commercial) sounds protective, but CC defines it as permitting "only noncommercial use", and whether a company's internal training is commercial is exactly the kind of question you do not want every reader to answer differently. **"Free to use"** is not a licence at all; it answers none of the questions. And for anything you did not create, keep a third-party register with TASL (title, author, source, licence), as in 14.4.

### What never goes into a product

A recording of a client's workshop, the client's slides, their code or their questions with their names: all were created under an SOW and a confidentiality clause, and none of it is yours to sell (20.4, 20.5). Build courses from `brownfield-demo` and rehearsals (Modules 15, 17), and ask for written consent before you use even an anonymized quote, as for a case study ([21.5](../module-21/lesson-05.md)).

### Community rules

A community sells access. Without rules, "access" means you, at any hour, on anyone's code. Five lines make it a product:

- **Response time**: how fast questions are answered, and what happens to the rest.
- **Office hours**: how many, how long, recorded or not, and with whose consent.
- **Not included**: private code reviews, client-specific advice, anything needing an NDA; those are a pilot or a paid call.
- **Moderation**: code of conduct; no employer or client code or data posted.
- **If it closes**: notice, refund, members' export of their own posts.

## Show me

The reference catalog, [`catalog.md`](../../labs/module-22/solution/catalog.md), has five products: the free starter pack and posts (S0), the private workshop (W1), a self-paced course (C1), a team licence (T1) and a community (M1). It also has a licence table, a third-party register and community rules.

```bash
cd labs/module-22
dotnet run --project tools/ScaleCheck -- catalog solution/catalog.md
```

```text
  id   product                        revenue/month        live days/month  licence
  S0   Starter pack and public posts  0–0                  0–0              MIT (code), CC BY 4.0 (docs)
  W1   Private workshop: Ground be... 0–6,500              0–1.5            internal-use licence to handouts
  C1   Self-paced course              1,560–5,850          0–0              personal, one seat
  T1   Course team licence            0–3,900              0–0.25           team licence
  M1   Practitioners' community       585–1,170            0.75–0.75        membership terms

  revenue per month: 2,145 to 17,420; live days: 0.75 to 2.5
  revenue per live day: 2,860 to 6,968
  share of revenue that grows without more live days: 100% to 40%

0 error(s), 0 warning(s)
```

Three details are worth copying. The course's evidence column shows its lower gain instead of borrowing the workshop's. The team licence row in the licence table answers seats (15 named), term (12 months), resale (none), attribution (kept) and updates (same MAJOR version). And the third-party register lists even a comparison slide that names another framework (with no files copied) and the icon set with its ISC licence.

## Try it

Budget: about an hour.

1. **Choose the next format (10 min).** From your evidence, which format could you sell next without borrowing the workshop's numbers? Write one sentence on why.
2. **Catalog (20 min).** Copy the catalog format from the [productization plan template](../../templates/productization-plan.md). Three to five products; units per month as ranges; live days per unit and fixed live days honestly counted.
3. **Licences (20 min).** One row per artifact you publish or sell. For the team licence, write the five terms. List every third-party item with TASL.
4. **Community rules (5 min)**, if you offer one.
5. **Check (5 min).** Run `catalog` until it is clean. Then ask someone who buys training for a company to read the team-licence row and tell you what they would ask.

<details>
<summary>Hint: I only have the workshop so far</summary>

That is the normal case at this stage. Your catalog is S0 plus W1, and perhaps a free recorded talk. Write the course as a planned product with "none yet" as evidence and a first beta cohort as the next step (22.4). Do not sell pre-orders for a course whose exercises have not passed a stranger test.
</details>

## Break it

[`break/22.2-course-dump/catalog.md`](../../labs/module-22/break/22.2-course-dump/catalog.md) is a weekend's work: "Become a Certified AI-Native Engineer", "Lifetime access, direct access to me 24/7", a course that is a "recording of the Fabrikam workshop, cut into 12 videos, plus the client's slides", a company licence with "unlimited seats, perpetual", the starter pack's code under CC BY-NC 4.0 "so nobody can sell it", templates "free to use", a diagram "found online", and a guarantee of 30% faster delivery.

```bash
dotnet run --project tools/ScaleCheck -- catalog break/22.2-course-dump/catalog.md
```

Before running it: which product would you take down today, and whom would you have to tell?

## Fix it

**Diagnose.** `catalog` reports 18 errors and 10 warnings, in four groups:

- **Someone else's material.** A client's workshop recording and slides sold as a course; a diagram and music with no author or licence. The recording is the one to take down today, and the client is who you tell (20.5).
- **Unbounded promises.** "24/7", "direct access to me", "anytime", "lifetime", a guarantee and a certification with no assessment behind it (22.3).
- **Licences that answer nothing.** Code under a CC licence, "free to use", a company licence with no seat limit, no term and no resale clause; two products with no licence at all.
- **Capacity.** At high demand the catalog needs 6 live days a month against a stated 3, and the community has no fixed days and no rules.

**Modify.** Take the course down and rebuild it from `brownfield-demo` and your rehearsal recordings. Replace "certified" with "certificate of completion". Relicense the starter-pack code under MIT, the documents under CC BY 4.0, and write your own terms for the paid material and the team licence. Replace every third-party item you cannot attribute. Write the community rules and give it fixed days. Cut the workshop forecast to what your capacity allows.

**Rerun.** `catalog` clean. Then read each licence row as a buyer's procurement team would.

## How do I know it works?

- [ ] Every product has a licence, evidence of its own (not borrowed from another format), a unit forecast as a range and honest live days.
- [ ] Code is under a software licence; public documents under a named licence; paid material and team licences under written terms with seats, term, resale, attribution and updates.
- [ ] Nothing in any product was created for an employer or a client; every third-party item has title, author, source and licence.
- [ ] If you run a community, its rules state response time, office hours, exclusions, moderation and what happens if it closes.
- [ ] `ScaleCheck catalog` is clean, and at high demand the catalog fits your capacity.

## Use / don't use

**Use** a self-paced course when the workshop's exercises already work without you, and measure its gain separately. **Use** a team licence for companies that want the course for a whole team; it is also how a course reaches buyers who have procurement. **Use** the community as the place questions come from, then answer the recurring ones in public (18.4).

**Don't** sell a recording of a live delivery as a course. **Don't** promise "lifetime" anything; you will not run it for a lifetime. **Don't** choose a licence to stop people copying; public material spreads your name, paid material is protected by its terms, and your method is protected by being yours (14.1).

**Limitations.**

- Completion and gain data for your course will be small and noisy for months; report intervals (16.5).
- MOOC completion rates come from free, open-enrollment courses; paid professional courses differ, but no public source tells you your own rate. Measure it.
- Licence terms here are orientation. Consumer-protection rules for online sales (refunds, cancellation) apply in many countries and are not covered.

## Reflect

1. Which of your products could a stranger complete without you today, and how do you know?
2. What would you answer if a 400-person company asked to put your course on its internal learning platform?
3. Who are the first ten people who would join your community, and what have they already asked you?

## Sources

- [Reich and Ruipérez-Valiente (2019) — The MOOC pivot](https://pubmed.ncbi.nlm.nih.gov/30630920/) — edX courses from MIT and Harvard, 2012–2018: 3.13% of participants completed in 2017–18; 52% of registrants never started.
- [Creative Commons — About CC Licenses](https://creativecommons.org/share-your-work/cclicenses/) — the six licences; BY requires credit, NC permits only noncommercial use, ND no adaptations, SA share-alike.
- [Creative Commons — FAQ](https://creativecommons.org/faq/) — CC recommends against using its licences for software and points to software licences instead (CC0 excepted).
- [Open Source Initiative — The MIT License](https://opensource.org/license/mit) — permissive licence whose condition is that the copyright and permission notice are included in copies.
- [U.S. Copyright Office — Circular 1: Copyright Basics](https://www.copyright.gov/circs/circ01.pdf) — copyright protection applies to original works once fixed in a tangible form; registration is not required for protection.
