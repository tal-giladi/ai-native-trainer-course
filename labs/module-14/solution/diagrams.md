# Diagrams — Ground-Bound-Build-Prove 2.0.0 (reference, illustrative)

> Reference for lesson 14.3. Diagrams live as text next to the method and carry its version. Each box is something you can point at (an artifact or a check); each arrow is labelled with what is handed over.

## Loop (method 2.0.0)

```mermaid
flowchart LR
    T["Ticket"] -->|ticket text| G["Ground<br/>brief with a source per line"]
    G -->|brief file| B["Bound<br/>plan + out-of-scope + checks"]
    B -->|approved plan file| U["Build<br/>fresh session, one step at a time"]
    U -->|diff| P{"Prove<br/>checks the agent<br/>did not write"}
    P -->|all pass| M["Merge"]
    P -->|fails| U
    P -->|incident| H["Harden<br/>new check that fails<br/>without the fix"]
    M -.->|defect found later| H
    H -->|gate or eval task| P
```

Key: rectangles are artifacts or steps that produce one; the diamond is a decision made by checks, not by the agent; dotted arrows happen after merge.

## Before and after (one team, EXP-01)

```mermaid
flowchart TB
    subgraph Before["Before: ticket-only runs"]
        A1["Ticket pasted"] --> A2["Agent writes code + its own tests"]
        A2 --> A3["Green run"]
        A3 --> A4["Review finds shadow rules, scope creep,<br/>silent product decisions<br/>(6 of 14 incidents)"]
    end
    subgraph After["After: Ground-Bound-Build-Prove"]
        B1["Brief + plan files"] --> B2["Agent builds within the plan"]
        B2 --> B3["Checks it cannot edit"]
        B3 --> B4["Review on a bounded diff<br/>cycle time −16% (CI −25% to −5%)<br/>escaped defects: not yet shown to be lower"]
    end
```

The "after" box carries the guardrail result as well as the speed result. A before/after diagram that shows only the good number is an advertisement.
