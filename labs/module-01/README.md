# Module 1 labs — The AI-native trainer role and picking your wedge

Everything the Module 1 labs need. Lessons: [01.1](../../lessons/module-01/lesson-01.md) · [01.2](../../lessons/module-01/lesson-02.md) · [01.3](../../lessons/module-01/lesson-03.md) · [01.4](../../lessons/module-01/lesson-04.md).

This is the field-work module. The labs are mostly writing and talking to real people: a proof-chain inventory, a pain-sizing sheet, `wedge.md`, a stack inventory, 3–5 discovery interviews and a pain/problem map. One small C# tool helps you critique interview transcripts.

> [!WARNING]
> The interviews involve real people and, indirectly, their employers. Ask for consent before taking notes, record only with explicit consent, never ask for confidential code, customer data or numbers they are not allowed to share, and keep notes under interview ids (`I01`…) with no names or company names. Keep everything from this module in a **private** folder. Do not interview people who report to you, and do not use your current employer's internal data without your manager's agreement.

## Requirements

- .NET SDK 8 or newer for `TranscriptLint` (`RollForward=Major`, so a newer runtime works too). Verified with SDK 10.0.400. No NuGet packages.
- Optional: the `gh` CLI or read access to a SQL Server warehouse with PR data, for sizing a review-queue pain (01.2).
- A calendar and 3–5 people in your wedge's audience who will give you 30 minutes.

## Contents

| Path | What it is | Lesson |
|---|---|---|
| `transcripts/bad-interview.md` | An 18-question fictional discovery interview full of leading, hypothetical, generic and pitching questions, with real evidence buried in it | 01.4 |
| `solutions/bad-interview-key.md` | Defect codes per question, the usable evidence, the dropped threads, the questions that should have been asked | 01.4 |
| `tools/TranscriptLint/` | Dependency-free C# heuristic linter: flags interviewer questions (leading, hypothetical, generic, pitch, double-barrelled, compliment-fishing, anchoring), counts past-specific asks, talk share, and participant answers with past evidence | 01.4 |
| `exercises/hype-or-pain.md` + `solutions/hype-or-pain-key.md` | Twelve statements to classify as pain, hype, symptom or blocker | 01.2 |
| `sql/review-wait.sql` | T-SQL (and a `gh` one-liner) to size a review-queue pain from PR data: median and p90 hours to first review, PRs waiting > 48 h, reviewer concentration | 01.2 |
| `worksheets/` | `wedge.md`, `stack-inventory.md`, `interview-log.csv`, `pain-map.md` — copy into your private `wedge/` folder | 01.1–01.4 |
| `examples/` | A worked wedge (v2, after interviews) and pain map for a fictional .NET / SQL Server wedge | 01.3, 01.4 |

The interview script itself is a reusable template: [`templates/customer-discovery-script.md`](../../templates/customer-discovery-script.md).

## Quick start

From this folder:

```bash
# 01.4 — lint the bad transcript (after you have critiqued it by hand)
dotnet run --project tools/TranscriptLint -- transcripts/bad-interview.md --legend

# 01.4 — lint your own interview notes (use "I:" and "P:" line prefixes)
dotnet run --project tools/TranscriptLint -- ~/wedge/interviews/I01.md
```

`TranscriptLint` reads markdown with `**Qn · Interviewer:**` / `**Participant:**` lines, or plain `I:` / `P:` lines. It is a regex heuristic: it misses presupposing questions, cannot see a dropped thread, and will occasionally flag a fine question. Treat every flag as a prompt to look, not a verdict.

## Your folder at the end of the module

```text
wedge/                       # private
├── role-and-stage.md        # 01.1 proof-chain inventory
├── pain-sizing.md           # 01.2 one pain sized in engineer-hours and flow, with a range
├── wedge.md                 # 01.3 v1, updated to v2 after 01.4
├── stack-inventory.md       # 01.3
├── interview-log.csv        # 01.4
├── interviews/I01.md …      # 01.4 notes with verbatim quotes, anonymized
└── pain-map.md              # 01.4
```

This folder feeds the rest of the course: the stack inventory decides what you integrate in Module 8, the pains become eval tasks in Module 7 and hard questions in your workshop, and the verbatim quotes become the hooks of your talks.
