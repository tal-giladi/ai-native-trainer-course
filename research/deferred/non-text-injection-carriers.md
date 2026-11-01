# Prompt injection through non-text carriers decoded by tools (steganographic images via MCP)

- **Topic ID:** non-text-injection-carriers
- **Status:** WAIT
- **Next review:** 2026-12-27
- **Course change:** none
- **Candidates:** C-20261026-02

## Summary

A preprint shows instructions hidden in images (LSB, DCT, metadata) pass text scanners and surface only after an MCP tool decodes them; keyword/regex filtering let 62.5% of 24 adaptive payloads through, and a proposed decode-time provenance-binding + taint-tracking scheme blocked all 24 (authors' testbed).

## Evidence

- https://arxiv.org/abs/2610.31974 — single preprint (daily run 2026-10-26); small attack set; code link unverified; no production MCP client implements the defense.

## What would change the decision

Independent replication or a real-world advisory of image/attachment-borne injection against a coding agent or MCP client. Then one sentence in 09.2 or 09.5 ("text filters do not see content that a tool decodes later"); the course already teaches that no filter is a boundary, so a lesson is unlikely.

## History

- 2026-11-01 — WAIT — single preprint with a small attack set; the general point is mostly covered by 09.2/09.5 — weekly/2026-W44.md — candidates C-20261026-02
