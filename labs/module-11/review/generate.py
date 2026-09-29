"""Generates the ILLUSTRATIVE CI-review dataset for lesson 11.2 (simulated, not measurements).

prs.json            20 historical Contoso Billing PRs with the defects humans found (review + 30-day follow-up)
comments-v1.json    what a naive "review this diff thoroughly" prompt posted (the flood)
comments-v2.json    what the evidence-first prompt (review-v2.md) posted
adjudicated.json    agent comments a human later confirmed as real defects the humans had missed

Deterministic: run `py generate.py` (or python3) from this folder and the files are byte-identical.
"""
import json, random
from pathlib import Path

HERE = Path(__file__).parent
rng = random.Random(1107)

S = "src/Contoso.Billing/"
PRS = [
    # id, split, title, files
    ("PR-301", "dev", "BILL-150 record payment reminders (service)", [S + "Invoices/InvoiceService.cs", S + "Reminders/ReminderService.cs"]),
    ("PR-302", "dev", "BILL-142 list overdue invoices", [S + "Invoices/InvoiceRepository.cs", S + "Invoices/IInvoiceRepository.cs"]),
    ("PR-303", "dev", "BILL-150 reminder table", ["db/migrations/V006__payment_reminder.sql", S + "Reminders/ReminderRepository.cs"]),
    ("PR-304", "dev", "Tidy README and build badge", ["README.md", "docs/ARCHITECTURE.md"]),
    ("PR-305", "dev", "BILL-156 days-overdue in reminder text", [S + "Reminders/ReminderService.cs", S + "Reminders/ReminderText.cs"]),
    ("PR-306", "dev", "BILL-158 search invoices by reference", [S + "Invoices/InvoiceRepository.cs"]),
    ("PR-307", "dev", "Bump xunit to 2.9.2", ["tests/Contoso.Billing.Tests/Contoso.Billing.Tests.csproj"]),
    ("PR-308", "dev", "BILL-160 allocate partial payments", [S + "Payments/PaymentAllocator.cs", S + "Payments/Payment.cs"]),
    ("PR-309", "dev", "BILL-151 collections summary", [S + "Collections/CollectionsSummary.cs", S + "Collections/CollectionsService.cs"]),
    ("PR-310", "dev", "BILL-161 cancellation in repository calls", [S + "Invoices/InvoiceRepository.cs"]),
    ("PR-311", "dev", "BILL-142 tests", ["tests/Contoso.Billing.Tests/InvoiceServiceTests.cs"]),
    ("PR-312", "dev", "Rename Cust to Customer in legacy report", [S + "Legacy/MonthlyRevenueReport.cs"]),
    ("PR-313", "holdout", "BILL-163 monthly statement builder", [S + "Statements/StatementBuilder.cs", S + "Statements/Statement.cs"]),
    ("PR-314", "holdout", "BILL-164 purchase-order number column", ["db/migrations/V007__po_number.sql", "db/migrations/U007__po_number.sql"]),
    ("PR-315", "holdout", "Logging cleanup", [S + "Common/LogEvents.cs"]),
    ("PR-316", "holdout", "BILL-165 nightly reminder job", [S + "Reminders/ReminderJob.cs"]),
    ("PR-317", "holdout", "BILL-166 audit trail for invoice reads", [S + "Invoices/InvoiceRepository.cs", S + "Common/Audit.cs"]),
    ("PR-318", "holdout", "BILL-167 late fees", [S + "Fees/LateFeeCalculator.cs"]),
    ("PR-319", "holdout", "Docs: ADR 0008 reminders", ["docs/adr/0008-reminders.md"]),
    ("PR-320", "holdout", "BILL-168 statement due-date banner", [S + "Invoices/InvoiceService.cs", S + "Statements/StatementBuilder.cs"]),
]

# Defects. found_by: "human" (review or 30-day follow-up) or "adjudicated" (an agent comment a human confirmed later).
DEFECTS = [
    ("D01", "PR-301", S + "Reminders/ReminderService.cs", 42, "time", "DateTimeOffset.UtcNow instead of IClock; tests cannot freeze time", "human"),
    ("D02", "PR-302", S + "Invoices/InvoiceRepository.cs", 58, "sql", "loads every invoice for the customer and filters in memory (BILL-142 AC4)", "human"),
    ("D03", "PR-303", "db/migrations/V006__payment_reminder.sql", 1, "sql", "V006 has no U006 undo script (convention since V003)", "human"),
    ("D04", "PR-305", S + "Reminders/ReminderService.cs", 31, "correctness", "days overdue off by one at the due instant (< vs <=)", "human"),
    ("D05", "PR-306", S + "Invoices/InvoiceRepository.cs", 77, "security", "reference concatenated into SQL text (injection)", "human"),
    ("D06", "PR-308", S + "Payments/PaymentAllocator.cs", 64, "money", "remaining balance computed in double", "human"),
    ("D07", "PR-309", S + "Collections/CollectionsService.cs", 22, "architecture", "re-implements OutstandingAsync and counts Void invoices", "human"),
    ("D08", "PR-311", "tests/Contoso.Billing.Tests/InvoiceServiceTests.cs", 90, "tests", "paid-invoice test asserts nothing", "human"),
    ("D09", "PR-313", S + "Statements/StatementBuilder.cs", 48, "correctness", "draft invoices with no due date throw in the banner path", "human"),
    ("D10", "PR-314", "db/migrations/V007__po_number.sql", 3, "sql", "NOT NULL column added to a populated table without a default", "human"),
    ("D11", "PR-316", S + "Reminders/ReminderJob.cs", 40, "concurrency", "async void handler: exceptions are lost and the job reports success", "human"),
    ("D12", "PR-317", S + "Common/Audit.cs", 19, "security", "audit line writes the customer's e-mail address (PII) to logs", "human"),
    ("D13", "PR-318", S + "Fees/LateFeeCalculator.cs", 27, "money", "Math.Round default is banker's rounding; the fee policy says away from zero", "human"),
    ("D14", "PR-320", S + "Invoices/InvoiceService.cs", 55, "time", "DateTime.Now (local clock) in the banner cut-off", "adjudicated"),
]
DEF = {d[0]: d for d in DEFECTS}
PR_FILES = {p[0]: p[3] for p in PRS}
PR_IDS = [p[0] for p in PRS]

FP_TEXT = {
    "style": ["Consider using 'var' here for readability.", "Prefer expression-bodied member.", "Line exceeds 120 characters.",
              "Consider a switch expression instead of if/else.", "Use string interpolation instead of concatenation.",
              "Braces could be omitted for this single-line if.", "Consider pattern matching with 'is not null'."],
    "naming": ["Consider renaming 'svc' to something more descriptive.", "Method name could be clearer, e.g. 'GetItems'.",
               "Private field should use camelCase without underscore.", "Consider renaming 'dto' to 'model'.",
               "Async suffix is redundant here."],
    "docs": ["Add an XML doc comment to this public method.", "Consider documenting the exceptions this can throw.",
             "Add a summary comment explaining the class purpose.", "The README could mention this new option."],
    "speculative": ["This could throw a NullReferenceException if the invoice is null.",
                    "Potential race condition if called concurrently.",
                    "This may cause performance issues with large datasets.",
                    "Consider validating the input here; negative values might cause problems.",
                    "This could fail if the database is unavailable; add retry logic."],
    "convention": ["Consider using Entity Framework Core instead of raw SQL for maintainability.",
                   "Use DateTime instead of DateTimeOffset for simplicity.",
                   "Consider reusing SqlHelper for consistency with MonthlyRevenueReport.",
                   "Store amounts as double to avoid decimal overhead."],
    "perf": ["Consider caching this result.", "Consider making this method parallel.", "Use a StringBuilder here."],
    "tests": ["Consider adding a test for the empty case.", "Consider a test for very large amounts.", "Add a test for cancellation."],
    "duplicate": ["Same issue as above: this SQL is built by concatenation."],
}


def fp_line(pr, path):
    bad = {d[3] for d in DEFECTS if d[1] == pr and d[2] == path}
    while True:
        line = rng.randint(5, 140)
        if all(abs(line - b) > 3 for b in bad):
            return line


def build(version, tps, fps):
    """tps: [(defect, severity, line_offset)]; fps: [(pr, category, severity)]"""
    out = []
    for did, sev, off in tps:
        d = DEF[did]
        cat = d[4]
        out.append({"pr": d[1], "file": d[2], "line": d[3] + off, "severity": sev, "category": cat,
                    "body": "Evidence: " + d[5] + "." if version == "v2" else d[5][0].upper() + d[5][1:] + "."})
    for pr, cat, sev in fps:
        files = [f for f in PR_FILES[pr] if f.endswith((".cs", ".sql", ".md", ".csproj"))]
        path = rng.choice(files)
        body = rng.choice(FP_TEXT[cat])
        line = fp_line(pr, path)
        if cat == "duplicate":
            d = DEF["D05"]; path = d[2]; line = d[3] + 2; cat = "security"
        # The agent names categories itself: a speculative null warning calls itself "correctness", not "speculative".
        shown = {"speculative": ["correctness", "concurrency", "reliability"], "convention": ["maintainability", "architecture"]}.get(cat)
        out.append({"pr": pr, "file": path, "line": line, "severity": sev, "category": shown[len(out) % len(shown)] if shown else cat, "body": body})
    # stable order: by PR then line; ids afterwards
    out.sort(key=lambda c: (c["pr"], c["file"], c["line"]))
    for i, c in enumerate(out, 1):
        c["id"] = f"{version}-{i:03d}"
    return [{"id": c["id"], **{k: v for k, v in c.items() if k != "id"}} for c in out]


def spread(n, cats_sevs, prs=None):
    prs = prs or PR_IDS
    res = []
    for i in range(n):
        cat, sev = cats_sevs[i % len(cats_sevs)]
        res.append((prs[(i * 7 + 3) % len(prs)], cat, sev))
    return res


# v1: "Review this diff thoroughly and list every issue you find." 64 comments.
v1_tps = [("D01", "high", 0), ("D02", "medium", 1), ("D03", "medium", 0), ("D05", "high", 0), ("D06", "high", -1),
          ("D09", "medium", 2), ("D10", "high", 0), ("D12", "medium", 1), ("D14", "low", 0)]
v1_fps = (spread(14, [("style", "low"), ("style", "nit")]) + spread(10, [("naming", "low")]) +
          spread(9, [("docs", "low"), ("docs", "nit")]) +
          spread(10, [("speculative", "high"), ("speculative", "medium"), ("speculative", "medium"), ("speculative", "high"), ("speculative", "medium")]) +
          spread(6, [("convention", "medium"), ("convention", "high"), ("convention", "medium")]) +
          spread(5, [("perf", "medium")]) + [("PR-306", "duplicate", "high")])
assert len(v1_tps) + len(v1_fps) == 64, len(v1_fps)

# v2: evidence-first prompt grounded in AGENTS.md; no style/naming/docs; <= 3 per PR. 24 comments.
v2_tps = [("D01", "high", 0), ("D02", "medium", 0), ("D03", "high", 0), ("D04", "high", 1), ("D05", "high", 0),
          ("D06", "high", 0), ("D07", "medium", -1), ("D09", "high", 0), ("D10", "high", 0), ("D11", "medium", 2),
          ("D12", "low", 0), ("D14", "high", 0)]
v2_fps = ([("PR-302", "speculative", "medium"), ("PR-316", "speculative", "medium"), ("PR-320", "speculative", "medium"),
           ("PR-308", "speculative", "low"), ("PR-313", "speculative", "low"),
           ("PR-310", "convention", "low"), ("PR-317", "convention", "low"),
           ("PR-301", "perf", "low"), ("PR-313", "perf", "low"),
           ("PR-305", "tests", "low"), ("PR-309", "tests", "low"), ("PR-318", "tests", "low")])
assert len(v2_tps) + len(v2_fps) == 24

prs = {"note": "ILLUSTRATIVE data (simulated, not measurements). Ground truth = defects found in human review or in the 30-day follow-up, plus adjudicated agent findings.",
       "match_rule": "an agent comment matches a defect when pr and file are equal and |line - defect.line| <= 3; each defect matches once, later matches count as duplicates (false positives)",
       "prs": [{"id": p[0], "split": p[1], "title": p[2], "files": p[3]} for p in PRS],
       "defects": [{"id": d[0], "pr": d[1], "file": d[2], "line": d[3], "category": d[4], "summary": d[5], "found_by": d[6]} for d in DEFECTS if d[6] == "human"]}
adjudicated = {"note": "Agent comments a reviewer re-examined after the fact and confirmed as real defects the human review missed. They join the ground truth.",
               "defects": [{"id": d[0], "pr": d[1], "file": d[2], "line": d[3], "category": d[4], "summary": d[5], "found_by": "adjudicated (v1 comment, confirmed by @contoso/billing-leads)"} for d in DEFECTS if d[6] == "adjudicated"]}

def dump(name, obj):
    (HERE / name).write_text(json.dumps(obj, indent=2) + "\n", encoding="utf-8", newline="\n")

dump("prs.json", prs)
dump("adjudicated.json", adjudicated)
dump("comments-v1.json", {"prompt": "prompts/review-v1.md", "note": "ILLUSTRATIVE", "comments": build("v1", v1_tps, v1_fps)})
dump("comments-v2.json", {"prompt": "prompts/review-v2.md", "note": "ILLUSTRATIVE", "comments": build("v2", v2_tps, v2_fps)})
print("wrote prs.json, adjudicated.json, comments-v1.json (64), comments-v2.json (24)")
