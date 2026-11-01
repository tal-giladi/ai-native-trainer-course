# Cross-skill composition risk (individually clean skills, unsafe combinations)

- **Topic ID:** cross-skill-composition-risk
- **Status:** WAIT
- **Next review:** 2026-12-27
- **Course change:** none
- **Candidates:** C-20261026-01

## Summary

KnotScryer (arXiv preprint) builds a capability-flow graph across agent skills and finds chains where one benign skill's output feeds another into an insecure sink; per-skill scanners miss these. Authors report 612 insecure compositions among 13,598 marketplace skills that individually passed scanning (vendor-independent but self-reported).

## Evidence

- https://arxiv.org/abs/2610.32155 — single non-peer-reviewed preprint, fetched by the daily run 2026-10-26 and 2026-10-31; numbers from the authors' own pipeline; code not confirmed.

## What would change the decision

Independent replication, a vendor advisory or skill-marketplace review policy that names composition risk, or a public tool. Then one checklist line in module 06's skill review ("review what skills can do together") or 09.4 excessive agency — not a lesson.

## History

- 2026-11-01 — WAIT — single unreplicated preprint; the idea is plausible and not taught, but needs independent evidence — weekly/2026-W44.md — candidates C-20261026-01
