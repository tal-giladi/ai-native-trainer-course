"""Writes the scripted scenarios for AgentTeam's offline agent (--agent fake:scenarios/<name>.json).

A scenario is a list of steps keyed by task, role and round: the text the "agent" returns, the edits it
applies to its working copy, and the cost, tokens and milliseconds it reports. The edits are the Module 7
reference solutions for T18-T20 split into anchored replacements, so the real golden tests decide the
outcome. Costs and timings are invented. You do not need to run this; it is kept so the scenarios' origin
is transparent. Usage: py make_scenarios.py
"""
import json
from pathlib import Path

HERE = Path(__file__).resolve().parent
SVC = "src/Contoso.Billing/Invoices/InvoiceService.cs"
TST = "tests/Contoso.Billing.Tests/InvoiceServiceTests.cs"
ANCHOR_T = "    [Theory]"
ANCHOR_M = "    /// <summary>An invoice is overdue when"

T18_CODE = {"path": SVC, "old": "invoice.DueUtc < _clock.UtcNow;", "new": "invoice.DueUtc <= _clock.UtcNow;"}
T18_TEST = {"path": TST, "old": ANCHOR_T, "new": """    [Fact]
    public void IsOverdue_from_the_due_instant_onward()
    {
        var sut = new InvoiceService(new InMemoryInvoices(), new FixedClock());
        Assert.True(sut.IsOverdue(new Invoice(1, 7, 10m, Now.AddDays(-30), Now, InvoiceStatus.Issued)));
    }

""" + ANCHOR_T}
T20_CODE = {"path": SVC, "old": ANCHOR_M, "new": """    /// <summary>Whole days an invoice has been overdue, in UTC; 0 when it is not overdue.</summary>
    public int DaysOverdue(Invoice invoice) =>
        IsOverdue(invoice) ? (int)Math.Floor((_clock.UtcNow - invoice.DueUtc).TotalDays) : 0;

""" + ANCHOR_M}
T20_TEST = {"path": TST, "old": ANCHOR_T, "new": """    [Fact]
    public void DaysOverdue_counts_whole_days_in_utc()
    {
        var sut = new InvoiceService(new InMemoryInvoices(), new FixedClock());
        Assert.Equal(3, sut.DaysOverdue(Inv(1, 10m, InvoiceStatus.Issued, -3)));
        Assert.Equal(0, sut.DaysOverdue(Inv(2, 10m, InvoiceStatus.Issued, 2)));
    }

""" + ANCHOR_T}
T19_CODE1 = {"path": SVC, "old": "return invoices.Where(i => i.Status == InvoiceStatus.Issued).Sum(i => i.Amount);",
             "new": "return Owed(invoices).Sum(i => i.Amount);"}
T19_CODE2 = {"path": SVC, "old": ANCHOR_M, "new": """    /// <summary>BILL-151: what the customer owes and the overdue part of it, with the same meanings as above.</summary>
    public async Task<CollectionsSummary> CollectionsSummaryAsync(int customerId, CancellationToken ct)
    {
        var owed = Owed(await _invoices.ListByCustomerAsync(customerId, ct)).ToList();
        return new CollectionsSummary(owed.Sum(i => i.Amount), owed.Where(IsOverdue).Sum(i => i.Amount));
    }

    private static IEnumerable<Invoice> Owed(IEnumerable<Invoice> invoices) =>
        invoices.Where(i => i.Status == InvoiceStatus.Issued);

""" + ANCHOR_M}
T19_RECORD = {"path": "src/Contoso.Billing/Invoices/CollectionsSummary.cs", "content":
              "namespace Contoso.Billing.Invoices;\n\n/// <summary>BILL-151: amount owed by a customer and the overdue part of it.</summary>\n"
              "public sealed record CollectionsSummary(decimal Owed, decimal Overdue);\n"}
T19_TEST = {"path": TST, "old": ANCHOR_T, "new": """    [Fact]
    public async Task CollectionsSummary_splits_owed_and_overdue()
    {
        var sut = new InvoiceService(new InMemoryInvoices(
            Inv(1, 100m, InvoiceStatus.Issued, -3), Inv(2, 40m, InvoiceStatus.Issued, 10)), new FixedClock());
        Assert.Equal(new CollectionsSummary(140m, 100m), await sut.CollectionsSummaryAsync(7, CancellationToken.None));
    }

""" + ANCHOR_T}


def step(task, role, rnd, text, cost, tin, ms, edits=None, writes=None):
    d = {"task": task, "role": role, "round": rnd, "text": text, "cost": cost, "input_tokens": tin,
         "output_tokens": int(tin * 0.03), "ms": ms}
    if edits:
        d["edits"] = edits
    if writes:
        d["writes"] = writes
    return d


def write(name, desc, steps):
    body = {"description": desc, "agent_version": "fake-agent 1.0 (scripted, offline)", "steps": steps}
    (HERE / name).write_text(json.dumps(body, indent=1) + "\n", encoding="utf-8", newline="\n")


# Lesson 10.2: three parallel workers, all touching the same two files.
W = "Checks: dotnet test Contoso.Billing.sln -> passed"
write("clobber.json",
      "Lesson 10.2. Three workers implement T18, T19 and T20 in parallel; all three edit InvoiceService.cs and "
      "InvoiceServiceTests.cs. Timings are scripted: T18 finishes first, T19 last.", [
          step("T18", "worker", 1, f"{SVC}\n{TST}\n{W}", 0.11, 52000, 64000, [T18_CODE, T18_TEST]),
          step("T20", "worker", 1, f"{SVC}\n{TST}\n{W}", 0.13, 58000, 97000, [T20_CODE, T20_TEST]),
          step("T19", "worker", 1, f"{SVC}\n{T19_RECORD['path']}\n{TST}\n{W}", 0.19, 71000, 142000,
               [T19_CODE1, T19_CODE2, T19_TEST], [T19_RECORD]),
      ])

# Lesson 10.1: T19 split between parallel specialists who only see the plan.
PLAN19 = """## Plan
1. Add the collections summary to InvoiceService, reusing the existing meanings of owed (Issued) and overdue (IsOverdue).
2. Add a summary record with Owed and Overdue amounts.
3. Unit tests: no invoices; owed includes not-yet-due; paid, void and draft excluded.
Touch: src/Contoso.Billing/Invoices/InvoiceService.cs, src/Contoso.Billing/Invoices/*.cs, tests/Contoso.Billing.Tests/*.cs

## Interfaces
- InvoiceService: an async method that returns the collections summary for a customer (customerId, CancellationToken)
- a summary record with decimal Owed and decimal Overdue

## Checks
dotnet test Contoso.Billing.sln

## Open questions
none"""
TESTER_FILE = {"path": "tests/Contoso.Billing.Tests/CollectionSummaryTests.cs", "content": """using Contoso.Billing.Common;
using Contoso.Billing.Invoices;
using Xunit;

namespace Contoso.Billing.Tests;

public class CollectionSummaryTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);
    private sealed class FixedClock : IClock { public DateTimeOffset UtcNow => Now; }
    private sealed class Rows(params Invoice[] rows) : IInvoiceRepository
    {
        public Task<Invoice?> GetAsync(long invoiceId, CancellationToken ct) => Task.FromResult(rows.FirstOrDefault(r => r.InvoiceId == invoiceId));
        public Task<IReadOnlyList<Invoice>> ListByCustomerAsync(int customerId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Invoice>>(rows.Where(r => r.CustomerId == customerId).ToList());
    }

    [Fact]
    public async Task No_invoices_means_nothing_owed()
    {
        var sut = new InvoiceService(new Rows(), new FixedClock());
        CollectionSummary summary = await sut.GetCollectionSummaryAsync(7, CancellationToken.None);
        Assert.Equal(0m, summary.Owed);
    }
}
"""}
write("specialists-t19.json",
      "Lesson 10.1. T19 (BILL-151) split between a coder and a tester who work in parallel from the planner's plan "
      "and never see each other or the ticket. Also contains the planner -> worker and single-agent steps for the fix.", [
          step("T19", "planner", 1, PLAN19, 0.05, 24000, 38000),
          step("T19", "coder", 1, f"{SVC}\n{T19_RECORD['path']}", 0.12, 51000, 88000, [T19_CODE1, T19_CODE2], [T19_RECORD]),
          step("T19", "tester", 1, TESTER_FILE["path"], 0.09, 43000, 71000, None, [TESTER_FILE]),
          step("T19", "worker", 1, f"{SVC}\n{T19_RECORD['path']}\n{TST}\n{W}", 0.17, 66000, 131000,
               [T19_CODE1, T19_CODE2, T19_TEST], [T19_RECORD]),
          step("T19", "solo", 1, f"{SVC}\n{T19_RECORD['path']}\n{TST}\n{W}", 0.16, 62000, 118000,
               [T19_CODE1, T19_CODE2, T19_TEST], [T19_RECORD]),
      ])

# Lessons 10.1 and 10.3: T18 as one agent and as a pipeline; the deadlock variant.
PLAN18 = """## Plan
1. In InvoiceService.IsOverdue, count the due instant itself as overdue: compare DueUtc <= clock.UtcNow.
2. Add a boundary test (due exactly now -> overdue) to InvoiceServiceTests.
Touch: src/Contoso.Billing/Invoices/InvoiceService.cs, tests/Contoso.Billing.Tests/InvoiceServiceTests.cs

## Interfaces
- none (IsOverdue keeps its signature)

## Checks
dotnet test Contoso.Billing.sln

## Open questions
none"""
W18 = f"{SVC}\n{TST}\n{W}"
COMMON18 = [
    step("T18", "solo", 1, W18, 0.14, 61000, 112000, [T18_CODE, T18_TEST]),
    step("T18", "planner", 1, PLAN18, 0.05, 23000, 41000),
    step("T18", "worker", 1, W18, 0.12, 52000, 96000, [T18_CODE, T18_TEST]),
    step("T18", "worker", 2, "No change: the plan was kept (PLAN: KEEP); IsOverdue stays <= as the task requires.\n" + W, 0.06, 39000, 52000),
]
write("t18-pipeline.json",
      "Lessons 10.1 and 10.3. T18 (BILL-152) as a single agent and as planner -> worker -> reviewer. "
      "The reviewer follows roles/reviewer.md 1.0.0 and approves.",
      COMMON18 + [step("T18", "reviewer", 1, "## Findings\nNo findings\nVERDICT: APPROVE", 0.04, 27000, 33000)])
write("t18-deadlock.json",
      "Lesson 10.3 break. The same T18 pipeline with reviewer 0.9.0 (break/10.3-deadlock/reviewer.md), whose rule R7 "
      "contradicts the task. Planner and reviewer never agree.",
      COMMON18 + [
          step("T18", "reviewer", 1, "## Findings\n- R7 time comparisons: src/Contoso.Billing/Invoices/InvoiceService.cs:25 compares "
               "DueUtc <= UtcNow; comparisons against the clock must be strict (<) to avoid boundary flapping.\nVERDICT: REQUEST_CHANGES",
               0.04, 27000, 34000),
          step("T18", "planner", 2, "PLAN: KEEP\nThe task says an invoice is overdue from its due instant onward (Finance). "
               "That acceptance criterion outranks reviewer rule R7; <= stays.", 0.05, 26000, 29000),
      ])
print("wrote 4 scenarios")
