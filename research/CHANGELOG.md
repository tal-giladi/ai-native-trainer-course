# Curriculum changelog

How the course evolves. Changes happen at most once a week, only when a development clears the bar
in [the weekly review protocol](PROTOCOL-weekly.md), or when a fact in an existing lesson went stale.
Paths and quiz ids never change. "No curriculum changes this week" is a normal entry.

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
