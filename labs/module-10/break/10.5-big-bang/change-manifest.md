# Change manifest — BILL-412 (written by the agent)

> This is the 10.5 break. A coding agent with all three repositories open produced this plan and the
> three pull requests in about twelve minutes. Every repository's own CI is green. Read it, run
> `ContractCheck rollout` on it, and find what the plan gets wrong before you read the lesson's Fix it.

**Ticket.** BILL-412 — send invoice amounts as integer minor units (`amountMinor`, e.g. 1250 for 12.50)
plus an ISO 4217 `currency`, instead of the floating `amount`. Finance reports rounding differences
of a few cents per month.

**Repositories and owners.**

| Repository | Team | Change |
|---|---|---|
| `billing-api` | Billing | `invoice.issued` now sends `amountMinor` and `currency`; `amount` removed |
| `ledger-worker` | Finance Platform | reads `amountMinor` and `currency` instead of `amount` |
| `notifier` | Customer Comms | formats the e-mail amount from `amountMinor` and `currency` |

**Pull requests.** `billing#212`, `ledger#87`, `notifier#41`. Each one updates its own unit tests and
its own JSON fixtures; all three pipelines pass.

**Rollout.** Merge all three on Friday afternoon so the change is atomic. The pipelines deploy in the
order the merges land: billing first, then ledger, then notifier.

**Rollback.** Revert the three pull requests.

**Verification.** Green CI in each repository.

**Approvals.** One reviewer per repository.

The plan as data: [`plan.json`](plan.json). The contracts each version publishes: [`services/`](services/).
