# Cross-service change manifest

One page per change that touches more than one repository, service or team. The coordinating agent
drafts it; the change owner (a person) signs it; every per-repository session gets it as its handoff.
Plain markdown so any team's agent and harness can read it. Used in
[10.5](../lessons/module-10/lesson-05.md).

## 1. The change

- **Ticket / goal:**
- **Change owner (one person):**
- **Shared contract that changes** (message, API, table, file format) and its old and new shape:

## 2. Who is touched

| Repository / service | Owning team | Reads or writes the contract? | Its change | Its reviewers |
|---|---|---|---|---|
|  |  |  |  |  |

How you found every reader (search across repositories, contract registry, consumer list) and what
you might have missed:

## 3. Phases

Each phase is one pull request in one repository, safe to deploy while the previous version still
runs. Default shape: expand → migrate readers → contract.

| # | Repository | PR | What it changes | Safe while old version live because | Rollback |
|---|---|---|---|---|---|
| 1 |  |  |  |  |  |

**Gate before the contract (removal) phase:** every reader is on the new shape in production, and
messages already queued in the old shape have drained (how you know):

## 4. Verification

- **Inside each repository:** its own tests and reviewers (CODEOWNERS).
- **Across the seam:** the contract check that runs on every phase (command, where it runs, who
  owns its result):
- **In production:** the signal that says a phase broke a reader (error rate, dead-letter queue,
  reconciliation job), and who watches it:

## 5. Timeline

| Phase | Team | Expected lead time | Depends on |
|---|---|---|---|
|  |  |  |  |

Critical path (the slowest chain, not the sum):

## 6. Agents

- Coordinating session (read access to all repositories, writes only this manifest and contract files):
- Per-repository sessions (each with that repository's own rules and permissions):
- What no agent may do (merge, deploy, write to another team's repository, hold one token for all):
