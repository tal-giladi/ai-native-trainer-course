# Cross-service lab (lesson 10.5)

Three fictional Contoso services owned by three teams, connected by messages. Each service publishes
what it sends and what it reads, per version, in `versions/<version>/contract.json`:

| Service | Team | Sends | Reads |
|---|---|---|---|
| `billing-api` | Billing | `invoice.issued` | — |
| `ledger-worker` | Finance Platform | `ledger.posted` | `invoice.issued` |
| `notifier` | Customer Comms | — | `invoice.issued`, `ledger.posted` |

`services/` has only the deployed version (`v1`) of each. Your job in 10.5 is to add the new versions
and a rollout plan for ticket BILL-412 (integer minor units plus currency instead of a floating
`amount`), and prove with `ContractCheck` that every step is safe while old and new versions run side
by side.

```bash
# from labs/module-10
dotnet run --project tools/ContractCheck -- rollout <services-dir> <plan.json>   # every step, both versions live
dotnet run --project tools/ContractCheck -- owners  <services-dir> <plan.json>   # who approves each step
```

A plan is JSON: the versions running now (`start`) and the ordered deploy steps, one per pull
request. See [`../break/10.5-big-bang/plan.json`](../break/10.5-big-bang/plan.json) for the format.

`solution/` holds a worked answer (`phased.plan.json`) and a second wrong order
(`consumers-first.plan.json`). Try the lab before you open it.
