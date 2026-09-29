---
id: "21.5"
module: 21
minutes: 15
practice_minutes: 90
prerequisites: ["21.4", "13.6", "14.4", "20.5"]
objectives:
  - Write a one-page case study from the engagement record in which every number, interval and status can be traced to the record.
  - Anonymize past the point of recognition by estimating how many organizations match the description, and replace identifying values with bands.
  - Report what a reader can generally expect rather than the best case, with limitations as a section, and obtain the client's written approval of the exact text.
  - Check the case study with EngageCheck casestudy against the record and a private deny list.
volatility: concept
sources:
  - title: "UK Information Commissioner's Office — Anonymisation (guidance, including the motivated intruder test)"
    url: https://ico.org.uk/for-organisations/uk-gdpr-guidance-and-resources/data-sharing/anonymisation/
  - title: "16 CFR 255.2 — Consumer endorsements (FTC Guides Concerning the Use of Endorsements and Testimonials in Advertising)"
    url: https://www.law.cornell.edu/cfr/text/16/255.2
  - title: "Becker, Rush, Barnes and Rein (2025) — Measuring the Impact of Early-2025 AI on Experienced Open-Source Developer Productivity"
    url: https://arxiv.org/abs/2507.09089
last_verified: "2026-09-28"
---

# 21.5 · Writing the anonymized case study

## Why it matters

The career path this course is built on is blunt about it: your first paid engagements come from your network, and each one has to produce a measured before/after you can show the next prospect. The case study is that artifact. It is also the easiest place in the whole business to lose your credibility, in two opposite ways.

The first is leaking. "We cut PR review time 60% at a fintech in Tel Aviv with 19 .NET developers" names the client to anyone in that city's industry, whether or not you wrote their name. The second is overclaiming. The evening after the read-out, the most exciting number is the pooled median, −23%, and "up to 40% faster on migration tickets" is technically something that happened on one ticket. Neither is what the engagement showed.

This lesson is the last phase: a one-page case study written from the record, anonymized until the client cannot be recognized, saying what a reader can generally expect, and approved in writing before anyone else sees it.

> [!NOTE]
> Content tags. **Concept** (stable): tracing claims to the record, identifiability and bands, typical versus best-case results, limitations as a section, written approval. **Implementation**: `EngageCheck casestudy` and the Contoso reference. The legal material is orientation only; your SOW's publicity clause and your jurisdiction decide.

## How it works

### Write from the record, not from memory

Draft within a week of the read-out, while the details are fresh, and update the adoption numbers after the 90-day follow-up. Write it *from* the engagement record: every number in the case study is a number in the record's baseline or results table, with its interval and its status. If you want to say something the record does not contain, the record is incomplete, or you should not say it.

The claims ladder from [13.6](../module-13/lesson-06.md) bounds the whole document. A single engagement supports rung 2 at most: "in this engagement, on these tickets, the estimate was X with interval Y". It never supports rung 4, "teams like yours will see…", however much a prospect wants to read it that way.

### Anonymize past the point of recognition

Removing the name is pseudonymization, not anonymization. The UK ICO's guidance asks whether a **motivated intruder** — someone reasonably competent, with access to public information, who wants to identify the subject — could do it from what you published ([ICO](https://ico.org.uk/for-organisations/uk-gdpr-guidance-and-resources/data-sharing/anonymisation/)). For a case study, the intruder is a competitor, a former employee or a recruiter who knows the local market.

*Intuition.* Every detail you add shrinks the set of organizations that match the description. When that set is small enough that a reader can name the client, the case study identifies them, with or without the name.

*Equation.* With $N$ candidate organizations and detail $i$ true of a fraction $p_i$ of them (assuming, roughly, independence), the expected number that match is

$$k \approx N \prod_i p_i$$

*Tiny example (illustrative numbers).* Take $N = 20{,}000$ companies with software teams in one country. "Sells maintenance contracts to businesses" ($p = 0.02$), "exactly 19 developers" ($p = 0.03$), ".NET and SQL Server" ($p = 0.3$): $k \approx 20{,}000 \times 0.02 \times 0.03 \times 0.3 \approx 3.6$. Add the city and $k$ drops below 1: identified. The banded version, "a business-to-business software company" ($p = 0.5$), "15–25 developers" ($p = 0.25$), ".NET and SQL Server" ($p = 0.3$): $k \approx 750$.

*Implementation.* `EngageCheck casestudy` builds a deny list from the record itself (the client name, every stakeholder's full name and surname) plus your private list (product names, hosts, internal project names), flags e-mail addresses and tracker keys like `BILL-97`, and warns on exact counts ("19 developers", "1400 customers").

*Interpretation.* The numbers are rough; the direction is not. Replace exact values with bands, drop the city, and remove anything unique: a public incident, a product quirk, a distinctive number. A detail that only one company could have (Contoso's "month-end freeze on the revenue report after a 0.4% error in 2023") collapses $k$ to 1 on its own.

### Report what is generally expected

The US FTC's endorsement guides treat a testimonial about a central attribute of a product as a claim that it represents what people can generally expect. If it does not, the advertiser must clearly disclose the generally expected performance, and a generic "results not typical" disclaimer is not enough ([16 CFR 255.2](https://www.law.cornell.edu/cfr/text/16/255.2)). A case study is not a consumer testimonial, and B2B services are not the guides' main target; the principle still applies to anything you use to sell. Three consequences:

- **The headline is the pre-registered estimate with its interval**, not the best ticket, the best week or the best team.
- **"Up to" is banned.** "Up to 40% faster" presents the maximum as the claim.
- **Inconclusive stays inconclusive.** A null result is a result; it also tells a prospect you will not spin theirs. Engagements happen against a real evidence base in which experienced developers were measured 19% slower with early-2025 AI tools despite forecasting a speed-up ([Becker et al.](https://arxiv.org/abs/2507.09089)); a case study that says "we could not detect a delivery change" is more credible next to that, not less.

### Approval, and whose permission you need

The SOW's IP clause ([20.4](../module-20/lesson-04.md)) says you may describe the engagement "in anonymized form only with the Client's written approval of the text". Send the exact text, get a dated written yes, and record it in the case study's `Client approval:` line. If the client was your employer's customer, or the work used your employer's material, you also need your employer's permission ([20.5](../module-20/lesson-05.md), [14.4](../module-14/lesson-04.md)). A client who says no, or asks for changes, gets exactly that; you do not publish a "lightly edited" version.

### The one-page shape

The [case-study template](../../templates/case-study.md): context (banded), problem (in their words), what you did (repeatably specific), results (table with intervals and readings), what this does not show (a heading, not a footnote), what the client did next (if they allow it).

## Show me

The evening-after draft ([`break/21.5-leaky-case-study`](../../labs/module-21/break/21.5-leaky-case-study/case-study.md)). Before running: how many identifying details can you find, and which claim is not in the record at all?

```bash
cd labs/module-21
dotnet run --project tools/EngageCheck -- casestudy break/21.5-leaky-case-study/case-study.md --record solution/engagement-record.md --deny solution/deny-terms.example.txt
```

```text
ERROR line 9: names "Contoso Ltd"
ERROR line 9: names "Tamar Golan"
WARN  line 9: exact count "19 developers" narrows who it is; use a band (e.g. 15-25 developers)
WARN  line 16: tracker key BILL-512 points at the client's project; remove it
ERROR line 16: "Up to 40" presents the best case as the claim; report the estimate and its interval
ERROR line 18: names "Ruth Amar"
ERROR line 18: e-mail address avi.bendavid@contoso.com
WARN  line 20: a "results may vary" disclaimer does not fix an unrepresentative claim; ...
ERROR no 'Client approval:' date: publish only after the client approves the exact text in writing
ERROR line 13: "23%" does not appear in the engagement record's baseline or results
ERROR line 13: claims a change in cycle time, ai vs manual (...) that the record calls inconclusive
ERROR line 14: claims a change in escaped defects, ai vs manual that the record calls inconclusive
ERROR no limitations heading ("What this does not show")
...
17 error(s), 8 warning(s)
```

The 23% is the pooled median from 21.4, which the record deliberately does not report as a result. "Escaped defects fell to zero" is 0 of 12 tickets, inconclusive. And the draft quotes the finance controller by name about the revenue report, the single most identifying detail of the engagement.

The reference ([`solution/case-study.md`](../../labs/module-21/solution/case-study.md)) describes "a business-to-business software company with 15-25 developers in three teams", leads its results table with the eval improvement and its interval, reports cycle time as "−8%, −24% to +12%, inconclusive" with the sentence "only a change of about 38% or more was likely to be detected", and has four bullets under "What this does not show". It was approved in writing on 2027-04-02. `casestudy` on it: 0 errors, 0 warnings.

## Try it

Budget: 90 minutes.

1. **Deny list (10 min).** Outside any repository, write your private deny list for the dry run: client, product and system names, people, tracker keys.
2. **Draft (45 min).** From your dry-run record, fill the [case-study template](../../templates/case-study.md). Every number from the record; every effect with its interval.
3. **Identifiability (15 min).** List every descriptive detail in your context section. Estimate $p$ for each and $k$ for the whole description. Band or remove until $k$ is in the hundreds.
4. **Approval (10 min).** Send the exact text to the peer who played the sponsor. Record the dated approval, or their changes.
5. **Check (10 min).** `EngageCheck casestudy --record … --deny …` until clean.

<details>
<summary>Hint: what if the only interesting result is inconclusive?</summary>

Lead with what the engagement did establish (convention adherence, adoption held at 90 days, a team that owns the layer), then state the inconclusive result plainly with the detectable effect. Prospects who have been burned by vendor numbers read that as a reason to trust you.
</details>

## Break it

> [!CAUTION]
> Never test anonymization by publishing. Run these experiments on your private copy only; anything pushed to a public repository or posted stays findable after you delete it.

Take the reference case study and add one sentence: "The team works in Haifa and closes its books with a three-day freeze after a 2023 revenue-reporting error." Run `casestudy`. Does the checker flag it? Estimate $k$ before and after the sentence.

## Fix it

**Diagnose.** The evening-after draft fails on identity (client, three people, an e-mail, tracker keys, exact counts), on truth (a number not in the record, two inconclusive outcomes written as improvements, "up to 40"), and on process (no approval, no limitations).

**Modify.** Rebuild it from the record with the template: bands, no names, no keys; the table with intervals and readings; "What this does not show" as a heading; approval requested with the exact text.

**Rerun.** `EngageCheck casestudy` on the reference: 0 errors, 0 warnings.

<details>
<summary>Solution: the Haifa sentence</summary>

The checker passes it: no name, no key, no exact count. That is the point of the exercise: the tool catches the mechanical leaks, not the unique ones. A city plus "three-day freeze after a 2023 revenue error" is true of perhaps one company; $k$ drops from hundreds to about 1. Only your judgment, and the client's review of the exact text, catches that.
</details>

## How do I know it works?

- [ ] Every number in the case study appears in the record, with its interval and the record's status.
- [ ] No names, e-mails, tracker keys, product names or exact counts; your estimate of $k$ is in the hundreds.
- [ ] The results are the pre-registered estimates, not the best case; no "up to", no "results may vary".
- [ ] "What this does not show" is a section.
- [ ] The client approved the exact text in writing, with a date; employer permission if relevant.
- [ ] `EngageCheck casestudy` is clean with your private deny list.

## Use / don't use

**Use** the case study as the evidence rung of your offer ladder ([20.1](../module-20/lesson-01.md)), linked from the proposal, with the interval visible. **Use** a null result honestly; it is a differentiator.

**Don't** publish before written approval, even anonymized. **Don't** combine several engagements' best numbers into one story. **Don't** let a prospect's question turn a rung-2 claim into a rung-4 promise on a call.

**Limitations.**

- The $k$ estimate is rough and assumes independent details; real details are correlated, which usually makes identification easier, not harder.
- `EngageCheck` catches names, keys, counts and untraceable numbers; it cannot catch a unique story. Read the draft as the client's competitor would.
- The FTC guides are US rules about advertising; outside the US and for B2B services other rules apply. Treat the principle, not the citation, as the standard.
- A case study describes one engagement. Its value to the next client is the method and the honesty, not the number.

## Reflect

1. Which detail of your last project would identify the client even without the name?
2. What is the least impressive honest result you would still be willing to publish, and why?
3. Who, besides the client, would you need permission from before publishing about your first engagement?

## Sources

- [ICO — Anonymisation](https://ico.org.uk/for-organisations/uk-gdpr-guidance-and-resources/data-sharing/anonymisation/) — identifiability, the motivated intruder test, and the difference between anonymisation and pseudonymisation.
- [16 CFR 255.2 — Consumer endorsements](https://www.law.cornell.edu/cfr/text/16/255.2) — endorsements on central attributes are read as representative; otherwise disclose the generally expected performance; "results not typical" disclaimers are ineffective.
- [Becker et al. (2025)](https://arxiv.org/abs/2507.09089) — randomized trial with experienced developers: forecast speed-up, measured 19% slowdown.
