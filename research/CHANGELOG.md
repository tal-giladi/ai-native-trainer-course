# Curriculum changelog

How the course evolves. Changes happen at most once a week, only when a development clears the bar
in [the weekly review protocol](PROTOCOL-weekly.md), or when a fact in an existing lesson went stale.
Paths and quiz ids never change. "No curriculum changes this week" is a normal entry.

## 2026-11-11 — industry signals and stale law wording

Source: KDnuggets, "Data Scientists Are Becoming AI Managers, Not Model Builders" (2026-07-06), every
figure re-checked at its primary source; notes in `curriculum/research-notes/2026-11-11-ai-managers-factcheck.md`.

- **Extended** [01.2](../lessons/module-01/lesson-02.md) — reading market numbers: interest, forecast,
  adoption, outcome (Gartner, McKinsey). Quiz q5.
- **Extended** [01.3](../lessons/module-01/lesson-03.md) — audiences beyond developers (Lightcast, PwC 2026,
  McKinsey); data and analytics teams as a wedge.
- **Extended** [10.1](../lessons/module-10/lesson-01.md) — agent frameworks you will meet at clients, PyPI
  versions as of 2026-11-11; AutoGen archived, successor Microsoft Agent Framework.
- **Extended** [10.3](../lessons/module-10/lesson-03.md) — whether an end-to-end rate is good enough
  depends on what the output may do. Quiz q5.
- **Extended** [11.4](../lessons/module-11/lesson-04.md) — three oversight levels (log, sample, gate),
  sizing a sampled review, the supervision/autonomy tension (MIT SMR and BCG, MIT Sloan, McKinsey). Quiz q5.
- **Extended** [20.3](../lessons/module-20/lesson-03.md) — find the AI-governance owner early (IAPP and
  Credo AI 2025). Quiz q5.
- **Fixed** [12.4](../lessons/module-12/lesson-04.md) — high-risk deadlines were attributed to "a 2025
  amendment package"; now the AI Omnibus, Regulation (EU) 2026/1744 (OJ 24 July 2026), with the new
  2 December 2026 prohibition date.
- **Fixed** [19.3](../lessons/module-19/lesson-03.md) — Article 4 AI literacy wording updated to the
  Omnibus text (support the development of AI literacy; no specific level mandated).

## 2026-11-10 — new lesson

- **Added** [10.5 Changes that cross services and teams](../lessons/module-10/lesson-05.md) — the
  mixed-version window, expand → migrate → contract, contract checks across the seam, one change
  owner, lead time as the critical path, and agent setup across repositories. New lab
  `labs/module-10/cross-service` with the `ContractCheck` tool, template
  `templates/cross-service-change.md`, and module 10 quiz question q10.

## 2026-W45 (2026-11-08)

- **Extended** [08.1 MCP architecture, authentication and authorization](../lessons/module-08/lesson-01.md) — "Where credentials enter"
  adds the server-side check: do not assume your SDK enforces token audience. The official MCP TypeScript SDK's
  bearer-auth helper accepted tokens issued for other services until an opt-in fix (CVE-2026-111771, fixed sdk
  1.32.0 / server 2.3.0); test your server with a token for another service and expect 401.
  [Weekly report](weekly/2026-W45.md).

## 2026-W44 (2026-11-01)

- **Extended** [07.3 Graders](../lessons/module-07/lesson-03.md) — new subsection "How sure is the corrected
  number?": a pass rate corrected for the judge's TPR and FPR also inherits the noise from the few
  human labels that measured them. With the lesson's 24-label calibration set the interval is about
  ±19 points, so report the interval and size the label set to the precision you need (Lee et al.,
  ICML 2026). New quiz question q5. [Weekly report](weekly/2026-W44.md).
- **Corrected** [09.5 Layered defenses](../lessons/module-09/lesson-05.md) — Limitations adds a second
  symlink race, this time in the Edit tool: code inside the Bash sandbox could redirect the unsandboxed
  Edit tool outside the workspace (fixed 2.1.251). The sandbox confines commands, not the agent's own
  file tools. [Weekly report](weekly/2026-W44.md).

## 2026-W43 (2026-10-25)

- **Extended** [08.1 MCP architecture, authentication and authorization](../lessons/module-08/lesson-01.md) — "Where credentials enter" now
  covers the client's side: because the server names the authorization server, a client must bind each
  stored credential to the server that issued it, or a malicious MCP server can collect your refresh
  tokens (the official TypeScript SDK's CVE-2026-104850, fixed 1.31.0 / 2.2.0).
  [Weekly report](weekly/2026-W43.md).

## 2026-W42 (2026-10-18)

- **Extended** [13.2 The evidence base on AI productivity](../lessons/module-13/lesson-02.md) — two
  learning-outcome trials (Shen and Tamkin 2026; Bastani et al. 2025, PNAS) join the evidence table,
  with a short "Speed is not learning" section: AI help can lower what learners can do on their own,
  so speed and competence are measured separately, and workshop exercises should use the agent to
  predict and explain, not to do the exercise. [Weekly report](weekly/2026-W42.md).

## 2026-W41 (2026-10-11)

- **Corrected** [09.5 Layered defenses](../lessons/module-09/lesson-05.md) — Limitations now cites two
  Claude Code security advisories (write-time symlink TOCTOU, fixed 2.1.129; pre-approved
  huggingface.co WebFetch exfiltration, fixed 2.1.163): the sandbox is the boundary, but keep it
  patched, and an allowed domain anyone can publish to is an outbound channel.
  [Weekly report](weekly/2026-W41.md).

## 2026-09-30 — review process started

- The course (22 modules, 101 lessons) was built on 2026-09-28 before this process existed; from
  now on changes flow through the daily/weekly protocols in this folder.
