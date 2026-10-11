# Curriculum changelog

How the course evolves. Changes happen at most once a week, only when a development clears the bar
in [the weekly review protocol](PROTOCOL-weekly.md), or when a fact in an existing lesson went stale.
Paths and quiz ids never change. "No curriculum changes this week" is a normal entry.

## 2026-W41 (2026-10-11)

- **Corrected** [09.5 Layered defenses](../lessons/module-09/lesson-05.md) — Limitations now cites two
  Claude Code security advisories (write-time symlink TOCTOU, fixed 2.1.129; pre-approved
  huggingface.co WebFetch exfiltration, fixed 2.1.163): the sandbox is the boundary, but keep it
  patched, and an allowed domain anyone can publish to is an outbound channel.
  [Weekly report](weekly/2026-W41.md).

## 2026-09-30 — review process started

- The course (22 modules, 101 lessons) was built on 2026-09-28 before this process existed; from
  now on changes flow through the daily/weekly protocols in this folder.
