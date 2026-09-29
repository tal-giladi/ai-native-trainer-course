# Capstone rubric

100 points, 10 categories × 10 points. **Evidence is required for every score** — a claim without a link scores in the lowest band. Used with the [capstone brief](capstone.md).

| Category | Pts | Evidence required for full marks |
|---|---|---|
| LLM/agent understanding | 10 | Written diagnosis of ≥3 real agent failures, each classified by mechanism |
| AI-layer architecture | 10 | Every file justified; portability shown; ownership/review in place |
| Workflow engineering | 10 | Loop + skills + sub-agents with tests, changelog, failure analysis |
| Evaluation methodology | 10 | Harness with repeated trials, calibrated grader, intervals, regression gate |
| Security | 10 | Threat model, ≥5 executed attacks, mitigations, retest results, residual-risk register |
| Enterprise architecture | 10 | Diagram with all required components and boundaries, ADRs, security-review answers |
| Measurement rigor | 10 | Controlled comparison, validity threats addressed, uncertainty stated, no unsupported claims |
| Original methodology | 10 | Concepts traced to own incidents/data; attribution audit |
| Teaching quality | 10 | Recording, observer rubric, pre/post gain, feedback analysis |
| Consulting/commercial readiness | 10 | Offer, pricing with rationale, SOW, adoption plan, case study |

## Bands

Each category is scored in one of four bands: **0–3** missing or unsupported · **4–6** present but weakly evidenced · **7–8** solid · **9–10** exemplary.

### LLM/agent understanding

| Band | Descriptor |
|---|---|
| 0–3 | No failure analysis, or failures described only as "the AI got it wrong" |
| 4–6 | Failures described but classification vague or not tied to mechanism (tokens, sampling, context, instruction hierarchy, tool loop) |
| 7–8 | ≥3 failures classified by mechanism, with transcripts |
| 9–10 | As 7–8, plus each diagnosis led to a layer change that was re-evaluated |

### AI-layer architecture

| Band | Descriptor |
|---|---|
| 0–3 | Layer missing, or copied from a template without justification |
| 4–6 | Layer present; some files unjustified; no portability or ownership |
| 7–8 | Every file traced to an audit finding; portability matrix across ≥3 tools; CODEOWNERS in place |
| 9–10 | As 7–8, plus a colleague used the layer cold and the notes show what changed |

### Workflow engineering

| Band | Descriptor |
|---|---|
| 0–3 | Ad-hoc prompting only |
| 4–6 | Skills exist but are untested or have no contracts |
| 7–8 | Five core skills + ≥1 sub-agent with test tickets, changelog and failure analysis |
| 9–10 | As 7–8, plus an edge-case failure diagnosed and fixed with a regression test |

### Evaluation methodology

| Band | Descriptor |
|---|---|
| 0–3 | Single runs, eyeballed results |
| 4–6 | Harness exists but single trials or an uncalibrated judge |
| 7–8 | N trials, Wilson intervals, judge calibrated against human labels, gate in CI |
| 9–10 | As 7–8, plus a documented case where the gate blocked a change that looked better |

### Security

| Band | Descriptor |
|---|---|
| 0–3 | No threat model or attacks |
| 4–6 | Threat model, but attacks theoretical or not retested |
| 7–8 | ≥5 attacks executed in the lab, mitigated, retested, residual risk recorded |
| 9–10 | As 7–8, plus the attacks run as a permanent regression suite |

### Enterprise architecture

| Band | Descriptor |
|---|---|
| 0–3 | No architecture |
| 4–6 | Diagram missing components or trust boundaries; ADRs thin |
| 7–8 | All components and trust boundaries; ≥5 ADRs; 30 security-review answers |
| 9–10 | As 7–8, plus a reviewer outside the course challenged it and the ADRs record the outcome |

### Measurement rigor

| Band | Descriptor |
|---|---|
| 0–3 | Before/after anecdote or vendor numbers |
| 4–6 | Comparison without a control or without intervals |
| 7–8 | Pre-registered metrics, control, intervals, threats to validity addressed |
| 9–10 | As 7–8, plus the report states plainly what it does not show and a skeptic accepted it |

### Original methodology

| Band | Descriptor |
|---|---|
| 0–3 | A borrowed framework renamed |
| 4–6 | Named concepts, but weak evidence trail or no attribution audit |
| 7–8 | Every concept traced to a dated incident or experiment; attribution audit done |
| 9–10 | As 7–8, plus a non-engineer paraphrased the loop correctly |

### Teaching quality

| Band | Descriptor |
|---|---|
| 0–3 | No workshop delivered |
| 4–6 | Delivered, satisfaction scores only |
| 7–8 | Recording, observer rubric, pre/post assessment with normalized gain, feedback analysis |
| 9–10 | As 7–8, plus a behavior follow-up weeks later |

### Consulting/commercial readiness

| Band | Descriptor |
|---|---|
| 0–3 | No offer |
| 4–6 | Offer without pricing rationale or SOW |
| 7–8 | Offer ladder, value pricing with stated uncertainty, SOW with exclusions, adoption plan, case study |
| 9–10 | As 7–8, plus a real paid or pilot engagement delivered against the SOW |

## Passing

70 points overall, with no category below 4. A category below 4 is resubmitted, not averaged away.
