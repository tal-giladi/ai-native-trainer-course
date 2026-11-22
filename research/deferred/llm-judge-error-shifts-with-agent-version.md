# LLM-judge error shifts with the agent version under evaluation

- **Topic ID:** llm-judge-error-shifts-with-agent-version
- **Status:** WAIT
- **Next review:** 2027-01-17
- **Course change:** none
- **Candidates:** C-20261120-01

## Summary

A September 2026 preprint reports that an LLM judge's error rates differ between versions of the agent it grades, even on the same tasks, and that judges accept failed coding patches more often as agents get more capable. If it holds, a judge calibrated on an old agent version gives biased upgrade comparisons, so 07.3's "recalibrate when the task or the judge changes" should also cover "or the agent version you compare".

## Evidence

- https://arxiv.org/abs/2609.34198 — Li (2026), "Frozen Judges, Moving Agents" (v1 2026-09-28, v2 2026-09-29), abstract fetched by the weekly review 2026-11-22: single author, arXiv preprint, no venue. 35 SWE-bench Verified submissions (250 issues), two tau-bench agents (155 tasks), 1,106 AgentRewardBench trajectories; all judges tested reject task-conditioned error invariance; eight judge-only SWE-bench intervals claimed upgrades execution grading could not establish; transported calibration raised comparison error from 3.8 to 19.5 points; independent human patch review pending.

## What would change the decision

Peer review, or an independent replication (another group, other judges or tasks) of version-dependent judge error. Then one sentence in 07.3 extending the recalibration rule to agent-version changes and recommending a fresh labelled sample of the new version's outputs (or execution-based grading) for upgrade decisions. Not a new lesson. The mechanism is consistent with 07.3's own invariance assumption, so a replication would be enough.

## History

- 2026-11-22 — WAIT — plausible and directly relevant to 07.3, but a single-author unreviewed preprint with human review still pending — weekly/2026-W47.md — candidates C-20261120-01
