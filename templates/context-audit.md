# Context audit template

Use when an agent's always-loaded context has grown past ~200 lines, before a model or tool change, and at the start of every engagement that touches an existing AI layer.
Goal: every piece of context is in the right bucket — **must always know / retrieve when needed / never load / generate dynamically** — and the change is proven on a fixed task set. Introduced in [04.5 · The context audit](../lessons/module-04/lesson-05.md); layers from [04.2](../lessons/module-04/lesson-02.md), pathologies from [04.3](../lessons/module-04/lesson-03.md).

> [!WARNING]
> Context layers of employer or client repositories often quote internal systems, people and incidents. Keep the audit in a private repository, and never send client code to a model provider the client has not approved.

## Header

- Repository / commit:
- Agent(s) and model(s) the layer serves (with versions):
- Date:
- Auditor:
- Task set used (name, number of tasks, runs per task):

## 0. Inventory (from `ContextLab budget`, `/context` or equivalent)

| File | Loads (always / on demand / not loaded) | Trigger | Lines | Tokens |
|---|---|---|---|---|
| | | | | |

Always-loaded total $R$: ___ lines, ___ tokens · $B = W - S - R - T - H - O$ at turn 1: ___

## 1. Critical facts

One row per fact the task set depends on. A fact is **critical** if a task fails without it.

| Fact | Statement | Inferable from code? (probe k runs) | Evidence (path) | Bucket | Task(s) |
|---|---|---|---|---|---|
| F1 | | | | | |

Buckets: **always** (needed before any trigger fires, not inferable) · **retrieve** (on demand: path rule, nested file, skill, path reference) · **generate** (derived from the repo at query time) · **never**.

## 2. Every block of the current layer

| Block (file:lines) | Pathology (pollution / stale / contradictory / redundant / irrelevant / none) | Decision (keep / move / generate / delete) | Evidence or reason |
|---|---|---|---|
| | | | |

Decision-before-trigger check for every **move**: will the agent read a file under the trigger before it needs this fact? If not, keep the fact or a pointer in the root.

## 3. Compaction and session hygiene

- [ ] Compact instructions written (what must survive a summary)
- [ ] Constraints that used to live in chat are now in files or tests
- [ ] Handoff note format agreed for long tasks (`HANDOFF.md` or equivalent)

## 4. Measurement

Same task set, same agent and model, same runs per task, fresh sessions, before and after.

| | Before | After | Change |
|---|---|---|---|
| Always-loaded lines | | | |
| Always-loaded tokens | | | |
| Pass rate (passes / runs) | | | |
| Mean input tokens per run | | | |
| Mean cost per run | | | |
| Cost per passing answer | | | |

Ablation: for the 3 riskiest deletions, remove the fact alone and rerun its task. Record which ones the task set would have caught.

## 5. Decision

- Ship / revise / revert:
- Tasks that still fail, and why (context, grader or model):
- Follow-ups (skills, hooks, tests to add):
- Changelog entry: link
