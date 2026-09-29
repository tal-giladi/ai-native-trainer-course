# Hands-on 1 — Ground brief for BILL-180 (12 minutes)

> Reference (illustrative). Objective O2. Handout page 3.

- Start: the starter pack at `ws-0-start`; ticket `tickets/BILL-180.md` ("Tell Collections whether an invoice can still be voided").
- Task: before the agent plans anything, find every place in the repository that already says something about voiding an invoice, in C#, T-SQL, tickets and docs. Then run `/prime BILL-180` and compare the agent's brief with your list.
- Done when: `research/BILL-180.md` (yours or the agent's, edited by you) names every existing implementation or definition with a path, says "reuse, do not re-implement", and lists at least one open question for Finance. Your pair agrees on it.
- Time: 12 minutes; one-minute warning at 11.
- Hints:
  1. "Void" is a business term. Where does this company define its terms?
  2. Search tickets and docs as well as code: `git grep -n -i "void" -- "*.cs" "*.sql" "*.md"`.
  3. There is an open ticket that makes a *different* promise about which invoices can be voided. Read its acceptance criteria next to BILL-180's.
- Extension: write the one sentence you would send Finance to settle the open question, and say which of the two tickets should own the rule.
- Catch-up: `git checkout ws-1-ground` and read `research/BILL-180.md`; compare it with what you found.
- Paper path: handout page 4 has the search output and the agent's brief from a rehearsal run; mark what the brief missed.

## Reference answer (instructor only; not on the handout)

| Found | Where | Why it matters |
|---|---|---|
| `InvoiceStatus` with `Void = 3` | `src/Contoso.Billing/Invoices/Invoice.cs` | The status the rule is about; no `CanVoid` exists yet |
| `Status tinyint`, default 1 (Issued) | `db/migrations/V003__utc_offsets_and_status.sql` | Rows with no explicit status are issued |
| Glossary: "Void — cancelled after issue" | `company/glossary.md` | Says drafts are not voided, they are never issued |
| BILL-155, open: "Only an issued invoice can be voided; a draft … is refused" | `tickets/BILL-155.md` | Conflicts with BILL-180 AC 2 (drafts can be voided) |
| Status rules live in `InvoiceService` (`OutstandingAsync`, `IsOverdue`) | `src/Contoso.Billing/Invoices/InvoiceService.cs` | The exemplar: one method per rule, on the service |

Open question for Finance: can a **draft** be voided (BILL-180) or not (BILL-155, glossary)? Until answered, the plan implements one `CanVoid` that BILL-155 must reuse, with the draft case behind the answer. The shadow rule this prevents: BILL-155 later writing its own `status == Issued` check next to `CanVoid`.
