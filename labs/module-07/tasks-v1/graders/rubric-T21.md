# Rubric T21 — why SqlHelper still exists (judge v2)

You are grading one answer to this question from a new team member of the Contoso Billing
repository: "Why does SqlHelper still exist, and what should I use instead?" The answer was limited
to five sentences.

Ground truth (from the repository, not from the answer):
- `SqlHelper` (2016) is kept only because `MonthlyRevenueReport`, its last caller, has not been migrated.
- Ticket BILL-97 owns that migration.
- New data access follows ADR 0007: a Dapper repository behind an interface (`IInvoiceRepository` /
  `InvoiceRepository`), parameterized SQL, a `CancellationToken` on every async method.
- New code must not call `SqlHelper`; `ConventionTests` fails the build if code outside `Legacy/` does.

Check each criterion independently and answer yes or no:
- C1 Reason: says it survives only for the remaining legacy report (names `MonthlyRevenueReport`, or
  "the last/one legacy caller" or "the monthly revenue report").
- C2 Replacement: tells the reader to use a Dapper repository behind an interface (or ADR 0007).
- C3 Prohibition: does not recommend or permit calling `SqlHelper` from new code.
- C4 No invented facts: states nothing that contradicts the ground truth (wrong ticket, wrong
  replacement such as Entity Framework, a claim that `SqlHelper` is the standard, invented dates or
  owners presented as fact).

Rules:
- Length is not a criterion. A two-sentence answer that meets C1–C4 passes. A long, well-written
  answer that fails any criterion fails. Do not reward detail, tone or formatting.
- Grade the answer only against the ground truth above. Do not use your own opinion about data access.
- Mentioning BILL-97 is welcome but not required.

Output exactly five lines and nothing else:
C1: yes|no
C2: yes|no
C3: yes|no
C4: yes|no
VERDICT: PASS|FAIL   (PASS only if C1–C4 are all yes)
