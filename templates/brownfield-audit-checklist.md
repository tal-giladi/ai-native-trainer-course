# Brownfield audit checklist

Use before writing or changing a repository's AI layer, and at the start of every engagement.
Goal: find what a coding agent would get wrong in this repository, with evidence. Introduced in [03.3 · The brownfield audit](../lessons/module-03/lesson-03.md).

> [!WARNING]
> Audit reports on employer or client code are confidential. Keep them in a private repository and anonymize before reusing anything.

## Header

- Repository / commit audited:
- Date:
- Auditor:
- Timebox used (target 60–90 min):

## Evidence ladder (strongest first)

1. Executable: test, analyzer, CI check, a command you ran
2. Recent code (last 6–12 months of commits)
3. Decision records (ADRs) with dates
4. Commit / PR history and review comments
5. Docs in the repo (note last-modified date)
6. Wiki pages, slide decks
7. Memory ("we always…")

A finding is **confirmed** when two sources agree and one is from rungs 1–3.

## Pass 1 — Solution topology

- [ ] Solutions (`*.sln` / `*.slnx`) and which one CI builds
- [ ] Projects: target frameworks, project references, packages and versions
- [ ] Deployable units vs libraries vs test projects
- [ ] `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
- [ ] Generated code and folders an agent must not edit

## Pass 2 — Build and test (run them)

- [ ] Exact build command, executed; result:
- [ ] Exact test command, executed; result and duration:
- [ ] Tests needing external resources (SQL Server, containers, secrets) and how to skip or provide them
- [ ] Commands CI runs that developers do not (lint, analyzers, format checks)

## Pass 3 — SQL Server conventions

- [ ] How schema changes ship (migrations tool, numbered scripts, DACPAC/`.sqlproj`)
- [ ] Undo/rollback convention and since when
- [ ] Naming (tables, procedures, indexes, constraints)
- [ ] Types for money, time (UTC? `datetimeoffset`?), identifiers
- [ ] Data access pattern(s) in use and which is current

## Pass 4 — Legacy patterns

- [ ] `[Obsolete]` types/members and their remaining callers
- [ ] ADRs that deprecate something; is the old thing still the majority pattern?
- [ ] `Legacy/`-style folders, analyzer suppressions, `#pragma warning disable`
- [ ] Docs that name files, projects or APIs that no longer exist

## Pass 5 — Tribal knowledge

- [ ] Asked ≥2 engineers: "What do new people always get wrong here?"
- [ ] Recurring PR review comments (search the last 50 PRs)
- [ ] Incidents / postmortems that produced an unwritten rule

## Findings table

| # | Finding | Evidence (rung) | Confirmed? | Enforceable by test/hook/CI? | Fresh-agent probe: wrong in k runs? | Rules file? | Action |
|---|---|---|---|---|---|---|---|
| 1 | | | | | _/5 | | |

## Outputs

- [ ] Rules-file candidates (only confirmed, non-inferable findings)
- [ ] Checks to add (enforceable findings without a test yet)
- [ ] Docs to fix or delete (stale sources an agent may read)
- [ ] Baseline problems (red build, flaky tests) — fix before rolling out agents
