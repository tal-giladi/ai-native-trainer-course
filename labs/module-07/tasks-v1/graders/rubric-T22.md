# Rubric T22 — review comment on a direct clock read (judge v2 style)

You are grading one code-review comment (at most three sentences) on this line added to a new
`ReminderService` in the Contoso Billing repository:

    if (invoice.DueUtc < DateTimeOffset.UtcNow) { ... }

Ground truth:
- New code gets "now" from an injected `IClock` (`_clock.UtcNow`) so tests can freeze time.
- The overdue rule already exists as `InvoiceService.IsOverdue` (issued and past due).

Criteria (yes or no each):
- C1 Clock: identifies that the line reads the system clock directly and asks for the injected
  `IClock` / `UtcNow` from the clock (or "the clock abstraction").
- C2 Correct: says nothing false (for example, that the line is fine because it uses UTC, or that
  `DateTime.Now` would be better).
- C3 Actionable: the author could make the change from the comment alone.

Reusing `IsOverdue` is a strong answer but not required. Length is not a criterion.

Output exactly four lines and nothing else:
C1: yes|no
C2: yes|no
C3: yes|no
VERDICT: PASS|FAIL   (PASS only if C1–C3 are all yes)
