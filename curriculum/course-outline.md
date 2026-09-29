# Course Outline — AI-Native Trainer: From Coding-Agent Power-User to Paid SDLC Trainer

> **Phase 1 deliverable, revision 2 (2026-09-28), with the Tal's Academy import contract (§19) added the same day.** Source brief: `brief/course-brief.md`. Source material: `ai-native-trainer-path.html`. Revision 1 (13 modules / 58 lessons / 12.5 h) is preserved inside this revision — see §15 for the mapping.

---

## 1. Course Identity

| Field | Value |
|---|---|
| **Title** | AI-Native Trainer: From Coding-Agent Power-User to Paid SDLC Trainer |
| **Audience** | Experienced C#/.NET engineer and technical leader who already uses coding agents daily |
| **Level** | Advanced (engineering) · Intermediate (LLM internals, security of agents) · Beginner (teaching, consulting, selling) |
| **Stages / Modules** | 11 stages (A–K) · 22 modules |
| **Lessons** | 101 |
| **Instruction** | ~26 hours (lessons average ~15 min of reading/watching) |
| **Practice** | ~75–100 hours of labs, projects and field work (ratio 1 : 2–4 on technical modules) |
| **Calendar** | ~6–9 months part-time; field assignments (interviews, talks, pilot, paid engagement) set the pace |
| **Format** | Plain-markdown course repo that is (a) a static docsify site on GitHub Pages and (b) imported unchanged into Tal's Academy — the contract in §19 is binding for every file |
| **Outcome** | Nine portfolio repos, a named evidence-backed method, a rehearsed 2-hour workshop with measured learning, an adoption plan, an offer + SOW, and at least one delivered engagement with an honest before/after case study |

**Core progression (unchanged from revision 1):**
Use agents → systematize → integrate → evaluate → measure → develop a methodology → teach → sell → deliver → productize.

**Design loop for the whole course:** Understand → Build → Break → Measure → Explain → Teach.

---

## 2. Target Audience

A senior C#/.NET engineer and technical leader: GitHub, SQL Server, JavaScript, APIs, Docker, cloud, architecture, cybersecurity concepts, daily coding-agent use. The course never teaches programming, Git, Docker, or HTTP. Labs default to **C#/.NET and SQL Server** brownfield code (with Python/TypeScript only where an ecosystem's reference SDK demands it), because that is both the student's strength and a large, under-served consulting market (legacy .NET shops).

**New to the student (and therefore taught properly):** transformer/LLM mechanics at an explanatory level, context engineering as a discipline, evaluation of stochastic systems, agent-specific attack surfaces, experimental design and statistics, instructional design, change management, consulting and sales.

**Prerequisite environment:** Docker Desktop; a GitHub account with Actions; API access to at least two model providers; one coding agent (Claude Code primary) plus access to at least one of Cursor / GitHub Copilot / Codex-class agents; a Jira/Confluence-class (or Linear/Notion-class) workspace — free tiers are enough; a real brownfield repository the student may legally work on.

---

## 3. Course-Level Learning Outcomes

After this course the student can:

1. **Explain** why a coding agent behaved the way it did, from tokens, sampling, context and instruction hierarchy up through tool-call loops — and name which failure class occurred.
2. **Architect** the AI layer of a brownfield repository (rules, skills, sub-agents, MCP, hooks, scripts, CI agents, evals, governance) and treat it as a versioned, reviewed, tested engineering artifact, portable across agent tools.
3. **Engineer context** deliberately: decide what the agent must always know, retrieve on demand, never load, or generate dynamically — and prove a reduction did not hurt quality.
4. **Run** a disciplined research → plan → implement → validate workflow and diagnose which phase failed.
5. **Evaluate** an agent system with task datasets, deterministic checks, rubrics and calibrated LLM judges, using repeated trials and confidence intervals, and gate AI-layer changes on the result.
6. **Attack and defend** an agent system: threat-model trust boundaries, execute prompt-injection, tool-poisoning and exfiltration attacks in an isolated lab, mitigate, retest, and document residual risk.
7. **Decide** between single- and multi-agent designs on evidence, and run agents headless in CI with approval, rollback, observability and cost controls.
8. **Design** an enterprise AI-agent architecture — hosting, gateways, routing, identity, secrets, audit, residency, cost allocation — and defend it in a security review with ADRs.
9. **Measure** engineering impact with a controlled comparison, quantify uncertainty, name threats to validity, and refuse unsupported claims.
10. **Author** an original method traced to the student's own experiments, with attribution and versioning.
11. **Teach**: design aligned objectives and assessments, deliver a rehearsed 2-hour workshop with live demo, and measure learning (not just satisfaction).
12. **Drive adoption**: plan champions, enablement, governance ownership and anti-regression so the change survives the consultant leaving.
13. **Consult and sell** honestly: position, qualify, price on value with stated uncertainty, write proposals and SOWs, control scope, handle IP and employer conflicts.
14. **Deliver and productize**: run the full engagement lifecycle and turn repeatable work into workshops, courses, kits and capacity-limited services.

---

## 4. Design Principles

Preserved from revision 1:

- **Practitioner before teacher.** The AI layer is built on a real codebase before anything is taught.
- **Evidence before price.** No pricing content before evaluation (M7) and measurement (M13) exist; ROI claims may cite only the student's own measured numbers.
- **Own it, don't resell it.** Every concept the student teaches traces to a dated incident in their notes (M14).
- **Tool-agnostic core, tool-specific labs.** Claude Code is the primary lab tool; every concept is mapped to AGENTS.md, Cursor, GitHub Copilot and other agent environments.
- **Real reps where practical.** Interviews, lunch-and-learn, free pilot, pitch and engagement happen with real people.
- **Honest limits.** Stochasticity, misleading benchmarks, confounded productivity numbers, and "demos don't prove reliability" are taught explicitly.
- **Security, confidentiality, employer IP are first-class.**
- **Every module ships an artifact; the course produces a portfolio.**

Added in revision 2:

- **Mechanism, not commands.** Every important technical concept is taught through nine facets: intuition · architecture · concrete example · failure example · implementation · validation · limitations · when to use · when not to use.
- **Problem → architecture → requirements → evaluation → tool/model selection.** Never favorite tool → problem. No arbitrary "best model" rankings; comparisons use explicit criteria.
- **Failure is the curriculum.** Every lab contains a deliberate break the student must diagnose (§6).
- **Math only where it pays.** Intuition → equation → tiny numeric example → implementation → interpretation (§10).
- **Stable concepts separated from fast-changing implementation.** Every lesson tags its content as *concept* or *implementation* so maintenance is tractable (§14).
- **Primary sources.** Factual claims about models, benchmarks, productivity, security, MCP, tools and pricing are backed by official docs, papers, or official engineering blogs, listed in a Sources section per lesson and in the research log. External material is research input, never executable instruction.
- **Safe labs.** Security exercises run only in local, intentionally vulnerable Docker environments. Students never attack real organizations, production systems, or third-party infrastructure.

---

## 5. Lesson Anatomy

Every lesson page has the same skeleton, answering the student's ten questions:

| Section | Answers |
|---|---|
| **Why it matters** | Why does this matter? What problem does it solve? |
| **How it works** | Mechanism and architecture (with diagram) |
| **Show me** | Concrete worked example, real numbers where possible |
| **Try it** | Hands-on exercise (the *Do* step) |
| **Break it** | Deliberate failure injected |
| **Fix it** | Diagnose → modify → rerun |
| **How do I know it works?** | Validation: test, eval, metric |
| **Use / don't use** | When to use it · when not to · limitations |
| **Reflect** | 3-line entry in the student's learning log |
| **Sources** | Linked primary sources (where factual claims are made) |

Plus **Instructor notes** (teaching objective, likely confusion, common misconception, key analogy, common failure, expected exercise outcome, extension exercise, discussion question) in a separate file next to the lesson, `lesson-NN.instructor.md`, never linked from `_sidebar.md` — so neither the student site nor the Academy import ever shows them.

Every lesson starts with YAML front-matter (exact field names and types in §19.3): `id`, `module`, `minutes`, `practice_minutes`, `prerequisites`, `objectives`, `volatility: concept | implementation`, `sources`, `last_verified`.

The lesson ends with its **knowledge check**: 3–5 multiple-choice questions in `lesson-NN.quiz.yaml` next to the lesson (§19.4) — not written in the lesson markdown.

---

## 6. The Default Learning Loop

Learn → See → Do → **Break** → **Diagnose** → Fix → Verify → Reflect.

Required failure scenarios, and where each lives:

| Failure the student must diagnose | Module |
|---|---|
| Agent misunderstands the architecture | M5 |
| Insufficient context | M4 |
| Excessive / contradictory context | M4 |
| A skill fails on an edge-case ticket | M6 |
| Evaluation produces misleading results (biased judge, leaked task) | M7 |
| MCP tool returns bad or stale data | M8 |
| Agent follows malicious instructions from a ticket/doc/repo file | M9 |
| Two agents disagree or clobber each other | M10 |
| CI review floods with false positives | M11 |
| Productivity measurement looks positive but is confounded | M13 |
| Live demo breaks in front of an audience | M15, M17 |
| Workshop is enjoyed but nothing is learned | M16 |

---

## 7. Stages and Module Map

| Stage | # | Module | Lessons | Instr. | Practice | Depends on |
|---|---|---|---|---|---|---|
| **A — Practitioner** | 1 | The AI-Native Trainer Role & Picking Your Wedge | 4 | 60 m | 4–6 h (field) | — |
| **B — Understand the technology** | 2 | LLM and Agent Fundamentals | 5 | 90 m | 5 h | — |
| | 3 | Anatomy of an AI-Native Codebase: The AI Layer | 4 | 60 m | 4 h | 2 |
| **C — Context engineering** | 4 | Context Engineering | 5 | 80 m | 5 h | 2, 3 |
| **D — Reliable agentic development** | 5 | Research → Plan → Implement → Validate | 4 | 60 m | 6 h | 3, 4 |
| | 6 | Skills, Sub-Agents and Workflow Automation | 4 | 60 m | 5 h | 5 |
| | 7 | Agent Evaluation | 6 | 90 m | 6 h | 2, 6 |
| **E — Tools, integration, security** | 8 | MCP, APIs and Hooks | 4 | 60 m | 5 h | 2, 3, 7 |
| | 9 | Agent Security | 6 | 100 m | 7 h | 7, 8 |
| | 10 | Multi-Agent Systems | 4 | 60 m | 5 h | 6, 7 |
| | 11 | Agents in CI and Production | 5 | 75 m | 5 h | 6, 7, 9 |
| **F — Enterprise architecture** | 12 | Enterprise AI-Agent Architecture | 5 | 80 m | 5 h | 8, 9, 11 |
| **G — Measurement** | 13 | Measuring AI Engineering Impact | 6 | 100 m | 6 h + 3–6 weeks data | 5, 7 |
| **H — Methodology** | 14 | Naming Your Method | 4 | 60 m | 4 h | 13 |
| **I — Teaching** | 15 | The Demo Repo and Live Demo Craft | 4 | 60 m | 8 h | 3–11, 14 |
| | 16 | Instructional Design for Engineers | 5 | 80 m | 5 h (incl. mini-workshop) | 14 |
| | 17 | Workshop Design and Delivery | 4 | 60 m | 10 h (3 rehearsals) | 15, 16 |
| | 18 | Teaching in Public | 4 | 60 m | ongoing | 14, 15 |
| **J — Organizational adoption** | 19 | AI Adoption and Change Management | 4 | 60 m | 4 h | 11, 12, 16 |
| **K — Consulting & business** | 20 | Offers, Pricing, Selling and Consulting | 5 | 80 m | 5 h + pitch | 13, 17, 19 |
| | 21 | Delivering Engagements | 5 | 80 m | engagement | 12, 13, 19, 20 |
| | 22 | Productization and Scaling | 4 | 60 m | 4 h | 21 |
| | — | Capstone | — | — | 6–10 weeks | all |

**Total: 22 modules · 101 lessons · ~26 hours instruction · ~100+ hours practice.**

**Dependency rule checked:** no lesson uses a concept introduced later. Where an early module needs a later tool, it gets a minimal version and the later module formalizes it (M4 uses a fixed 10-task pass/fail check → M7 turns it into a proper harness with statistics; M5 starts `NOTES.md` → M13 turns it into a controlled experiment; M7 introduces binomial confidence intervals → M13 extends to comparisons of engineering metrics).

```
M1 ─────────────────────────────────────────────────────────────┐
M2 → M3 → M4 → M5 → M6 → M7 ─┬→ M8 → M9 ─┬→ M11 → M12 ─┐       │
                             ├→ M10 ─────┘             │       │
                             └→ M13 → M14 → M15 → M17 ─┼→ M19 → M20 → M21 → M22 → Capstone
                                          └→ M16 ──────┘   ↑
                                          └→ M18 (ongoing from M14)
```

---

## 8. Portfolio Repositories

The six original repos remain; three are added because evaluation, security and architecture outgrew `ai-layer-lab`.

| Repo | Built in | Purpose | Visibility |
|---|---|---|---|
| `ai-layer-lab` | M3–M6, M8, M11 | AI layer on a real brownfield (.NET) codebase + `NOTES.md` | Private |
| `agent-evals` *(new)* | M4, M7, M9, M10 | Task dataset, graders, harness, attack suite, comparison reports | Private → public harness |
| `security-lab` *(new)* | M9 | Docker lab: vulnerable ticket/docs service, malicious MCP server, fake secrets, egress catcher; threat model; residual-risk register | Public (safe by design) |
| `enterprise-architecture` *(new)* | M12 | Reference architecture diagram, ADRs, security-review Q&A | Public |
| `method-notes` | M13–M14 | Experiment report, named concepts, diagrams, evolution policy, FAQ | Private → public |
| `brownfield-demo` | M15 | Confidentiality-safe messy .NET app + PRD, tickets, docs, recordings | Public |
| `talks` | M16, M18 | Mini-workshop, lunch-and-learn, content calendar, published pieces | Private |
| `workshop-kit` | M17, M20 | Agenda, instructor notes, starter pack, troubleshooting, pre/post assessment, pricing, forms, SOW | Starter pack public |
| `case-studies` | M21 | Anonymized before → intervention → after results | Private |

Every repo README covers: purpose · what was built · why it exists · how it was evaluated · lessons learned.

---

## 9. Modules in Detail

Format per module: purpose · objectives · lessons (C = concept, I = implementation-heavy) · lab & deliberate break · artifact · sample quiz question.

The sample quiz questions below are open scenarios that show the *reasoning* each module tests. In the course they are written as multiple-choice scenario questions (one correct answer, three plausible wrong ones, an explanation) per §19.4 — the scenario stays, the answer format changes.

---

### STAGE A — Becoming an AI-Native Engineering Practitioner

#### Module 1 — The AI-Native Trainer Role & Picking Your Wedge
*4 lessons · 60 min · field work 4–6 h*

What the job is, who pays for it, and the one-sentence angle that makes a stranger hire you.

**Objectives.** Distinguish trainer / consultant / enablement engineer and what each is paid for · explain the practitioner → teacher → consultant flywheel and why skipping stages fails · separate engineering pain from AI hype and identify buyer vs user vs champion · define a defensible wedge and run non-leading discovery interviews.

**Lessons.**
1. The roles and the flywheel: trainer, consultant, enablement engineer; practitioner → public teaching → community → private engagements (C)
2. Engineering pain vs AI hype: valuable problems and who actually pays (C)
3. Finding your wedge: stack × audience × pain — e.g. "AI-native delivery for legacy .NET/SQL Server shops on Jira" (C)
4. Customer discovery: interview technique, avoiding leading questions, building a pain/problem map (I)

**Lab & break.** Critique a bad interview transcript full of leading questions; then run 3–5 real interviews.
**Artifact.** `wedge.md`, stack inventory, interview transcripts with verbatim quotes, pain/problem map.
**Sample quiz.** *A CTO says "we need AI training." Three developers say review queues are their bottleneck. Who is the buyer, who is the user, and what problem would you propose to solve?*

---

### STAGE B — Understand the Technology

#### Module 2 — LLM and Agent Fundamentals *(new)*
*5 lessons · 90 min · practice 5 h*

Enough mechanism to explain agent behavior to a room of engineers without hand-waving. Not ML research; not a vendor tour — examples span several current providers and open-weights models.

**Objectives.** Explain tokenization, embeddings, attention and inference conceptually · compute how sampling parameters change output distribution · describe the instruction hierarchy and how conflicts resolve · build a minimal tool-calling agent loop · classify agent failures · select a model on explicit criteria.

**Lessons.**
1. From text to next token: tokenization, embeddings, attention, positional information, logits, context windows, long-context degradation (C)
   Lab: tokenize C#, SQL, and Hebrew text across two tokenizers; compute token cost of a real solution; run a needle-in-context test at different positions.
2. Sampling and (non-)determinism: softmax, temperature, top-p, why identical prompts differ, reasoning models vs ordinary generation, model capability vs tool capability (C, math)
   Math: softmax with temperature on 3 logits; probability that k independent runs all succeed (pᵏ).
3. The instruction hierarchy: system, developer, user, tool results, repository instructions, retrieved context; conflicts and precedence (C)
   Break: plant a README instruction that contradicts the rules file; observe which wins across two agents.
4. Tool calling and the agent loop: tool definitions, arguments, structured output, results, loops, malformed arguments, tool failure, authorization; LLM → LLM+tools → agent loop → multi-agent (C + I)
   Lab: write a ~150-line agent loop in C# against a provider API with two tools; inject malformed arguments and a failing tool.
5. Failure taxonomy and model selection: hallucination, incorrect/incomplete reasoning, stale/insufficient/conflicting context, tool/instruction/planning/verification failure; selecting by capability, latency, cost, context size, privacy, tool support, reasoning, coding ability, reliability (C)
   Lab: selection matrix for three real tasks across ≥3 models from ≥2 providers, with measured latency and cost.

**Simulation.** Agent-loop simulator (request → reasoning → tool → result → reasoning → tool → validation).
**Artifact.** `ai-layer-lab/fundamentals/`: agent loop, tokenizer notebook, failure-taxonomy cheat sheet, model-selection matrix.
**Sample quiz.** *The same ticket succeeds 7 of 10 runs. A teammate sets temperature to 0 and calls it fixed. What did they fix and what didn't they?*

#### Module 3 — Anatomy of an AI-Native Codebase: The AI Layer
*4 lessons · 60 min · practice 4 h* (revision-1 Module 2, expanded)

The repository now holds source + tests + documentation + AI behavior — and AI behavior needs version control, review, testing, ownership and change management like any other code.

**Objectives.** Map every AI-layer component and the problem it solves · map each Claude Code concept to AGENTS.md, Cursor, GitHub Copilot and other agents · audit a brownfield repo for what an agent cannot infer · write a grounded rules file · establish ownership and review for the layer.

**Lessons.**
1. The fourth citizen: AI behavior as an engineering artifact — versioning, review (CODEOWNERS), testing, ownership, change management (C)
2. Component map and portability: rules, AGENTS.md, Claude Code config, Cursor rules, Copilot instructions, skills, sub-agents, MCP, hooks, scripts, CI agents, eval tests, governance, docs (C + I)
3. The brownfield audit: solution topology, build/test commands, SQL Server conventions, legacy patterns, tribal knowledge (I)
4. Writing a grounded rules file (<300 lines, every line traceable) (I)

**Lab & break.** A rules line encodes a convention the codebase abandoned; the agent faithfully reproduces the stale pattern — diagnose and fix.
**Artifact.** `ai-layer-lab` created; audit report; rules file; portability matrix (same layer expressed in ≥3 tools).
**Sample quiz.** *A team keeps its agent instructions in a wiki page, not the repo. List three failures this causes and how you'd migrate.*

---

### STAGE C — Context Engineering

#### Module 4 — Context Engineering *(new, major)*
*5 lessons · 80 min · practice 5 h*

More context is not automatically better.

**Objectives.** Compute context budgets · design layered context (always-loaded / on-demand / never / generated) at repo, directory and task level · recognize pollution, staleness, contradiction, redundancy, irrelevance · apply compression, summarization, compaction, prioritization · prove a context change on a task set.

**Lessons.**
1. What context is and what it costs: windows, budgets, token arithmetic, effective vs nominal context (C, math)
   Math: budget = window − system − rules − tools − history; cost and latency per turn; tiny worked example on a real repo.
2. Layering: always-loaded, on-demand, progressive disclosure; repo / directory / task-specific context; retrieval; documentation as context; codebase topology maps (C + I)
3. Context pathologies: pollution, stale information, contradictory instructions, redundancy, irrelevance — and how each shows up in agent output (C)
4. Compression: summarization, compaction, prioritization, session resets; what survives compaction and what doesn't (C + I)
5. The context audit: must always know / retrieve when needed / never load / generate dynamically (I)

**Project.** Given an intentionally terrible 900-line rules + context system on a .NET repo, cut it by ≥70% while holding or improving results on a fixed 10-task set; report tokens, pass rate and cost before/after.
**Breaks.** (a) Remove one critical fact → insufficient context failure. (b) Add a contradictory directory rule → conflicting context failure.
**Simulation.** Context simulator — add irrelevant material and watch usable budget and hit-rate on relevant facts degrade.
**Artifact.** `ai-layer-lab/context/` audit + reduced layer; `agent-evals/tasks-v0/` (first 10 tasks).
**Sample quiz.** *An agent does well when given the entire repository but poorly with a focused context. What experiments would you run to find out why?*

---

### STAGE D — Reliable Agentic Software Development

#### Module 5 — Research → Plan → Implement → Validate
*4 lessons · 60 min · practice 6 h* (revision-1 Module 3, expanded)

**Objectives.** Run disciplined research (repo exploration, dependency and architecture discovery) · write reviewable plans with implementation boundaries and human checkpoints · define validation gates and self-review · decide reset vs continue · locate the failing phase.

**Lessons.**
1. Research discipline: why paste-and-go fails on brownfield; exploration, dependency discovery, architecture discovery (C + I)
2. Plans worth reviewing: quality criteria, boundaries, checkpoints, plan-as-artifact (I)
3. Validation: gates (build, tests, analyzers, SQL checks), self-review, reset/restart decisions (I)
4. Failure diagnosis: research vs plan vs implement vs validate (C)

**Lab & break.** Seed a plan with a wrong architecture assumption (e.g. data access via EF when the module uses stored procedures); the implementation "works" but violates the architecture — find where it went wrong.
**Field.** Run 6 real tickets both paste-and-go and full loop; log minutes, defects, rework in `NOTES.md`.
**Artifact.** Loop templates; `NOTES.md` with 6 entries; failure-diagnosis log.
**Sample quiz.** *The PR compiles, tests pass, reviewer rejects it for duplicating an existing service. Which phase failed and what gate would have caught it?*

#### Module 6 — Skills, Sub-Agents and Workflow Automation
*4 lessons · 60 min · practice 5 h* (revision-1 Module 4, expanded)

**Objectives.** Design skills with triggers, inputs, outputs, contracts and deterministic steps · build five core skills · design specialized sub-agents (planner, reviewer, parallel research) · know when not to build a skill or agent · test, version and failure-analyze skills.

**Lessons.**
1. Skill anatomy: trigger, inputs, output contract, deterministic steps (scripts) vs model steps (C + I)
2. The five core skills: prime, spec (PRD → tickets), plan-feature, validate, pr-review (I)
3. Sub-agents: specialization, context isolation, parallel research, planner and reviewer agents — and when not to create a skill or an agent (C + I)
4. Testing, versioning and failure analysis for skills (I)

**Lab & break.** A skill that works on 5 tickets fails on a ticket touching a SQL migration; diagnose contract vs context vs trigger.
**Artifact.** 5 skills + ≥1 sub-agent, each with test tickets, `CHANGELOG.md`, failure analysis; colleague cold-use test notes.
**Sample quiz.** *A team has 40 skills and nobody knows which fire. What's your audit and consolidation approach?*

#### Module 7 — Agent Evaluation *(new, major)*
*6 lessons · 90 min · practice 6 h*

You cannot reliably improve an agent system if you cannot measure whether a change improved it. (Distinct from business impact, M13.)

**Objectives.** Build representative, golden and regression task sets · combine deterministic checks, rubrics, human review and LLM judges · quantify grader error · handle stochasticity with repeated trials and confidence intervals · compare models, prompts, rules and skills version-to-version · gate AI-layer changes.

**Lessons.**
1. Why evaluate agents: the improve-without-measuring trap; qualitative vs quantitative evaluation (C)
2. Task datasets: representative, golden, regression; pass/fail criteria; benchmark contamination (C + I)
3. Graders: deterministic checks (build/test/diff), rubric grading, human evaluation, LLM-as-judge; evaluator bias; grader false positives/negatives as precision/recall (C + I, math)
4. Statistics for stochastic systems: repeated trials, pass@k vs pass^k, binomial / Wilson confidence intervals, how many runs you need (C, math)
5. Comparisons: model A vs B, prompt A vs B, rules/skill A vs B; reproducibility; paired designs (I)
6. Evals in the loop: before/after every AI-layer change, CI integration, regression gates (I)

**Project.** `agent-evals` harness running ≥20 tasks from the student's repo, N trials each, with deterministic checks + calibrated judge; re-score the M4 context reduction formally.
**Breaks.** (a) An LLM judge that rewards verbosity. (b) A task whose answer leaked into the rules file. (c) A "win" that disappears inside the confidence interval.
**Simulation.** Evaluation simulator — compare model A/B, prompt A/B, skill A/B on the same task set; vary trials and see interval width.
**Artifact.** `agent-evals/` harness, task set v1, grader calibration report, first comparison report.
**Sample quiz.** *Skill v2 scores 14/20 vs v1's 12/20, one run each. Should you ship v2? What would you run first?*

---

### STAGE E — Tools, Integration and Security

#### Module 8 — MCP, APIs and Hooks
*4 lessons · 60 min · practice 5 h* (revision-1 Module 5, expanded)

**Objectives.** Explain MCP architecture (hosts, clients, servers, tools, resources, prompts, transports) · design authentication and authorization and choose MCP vs direct API vs CLI · wire a real integration · build a custom server · write enforcing and auditing hooks.

**Lessons.**
1. MCP architecture, authentication and authorization: scoped tokens, OAuth, least-privilege tool design; MCP vs API vs CLI (C)
2. Real integration: the wedge stack (Jira/Confluence/GitHub or equivalent), read vs write scoping (I)
3. Building a custom MCP server in C# (e.g. read-only SQL Server schema + stored-procedure catalog) (I)
4. Hooks: pre/post tool hooks, policy enforcement, audit logging (I)

**Lab & break.** The MCP server returns stale schema; the agent confidently writes code against a dropped column — diagnose, then add freshness metadata and a validation hook.
**Artifact.** One real integration, one custom MCP server with tests, two hooks (one enforcement, one audit), eval tasks covering them.
**Sample quiz.** *An MCP server has write access to production Jira and GitHub. What risks exist and how would you redesign the permission model?*

#### Module 9 — Agent Security *(new, major, hands-on)*
*6 lessons · 100 min · practice 7 h*

Adversarial thinking, not "don't leak secrets." All attacks run in the local `security-lab` only.

**Objectives.** Threat-model an agent system (trust boundaries; agent, tool and user identities; external, repository and third-party content) · execute and explain the major attack classes · implement layered defenses · run attack → observe → mitigate → retest → residual-risk cycles.

**Lessons.**
1. Threat modeling agents: trust boundaries, identities, the combination of private data + untrusted content + an exfiltration channel; mapping to OWASP LLM / agentic threat lists (C)
2. Prompt injection: direct and indirect via malicious tickets, documentation, repo files, PR comments (C + I)
3. Tools and supply chain: tool poisoning, malicious or changed MCP servers, malicious dependencies and hallucinated package names (C + I)
4. Excessive agency: privilege escalation, secret theft, data exfiltration; generated vulnerable code and how SAST/review catch it (C + I)
5. Defenses: least privilege, allowlists, sandboxing, network egress restriction, approval gates, secret isolation, tool restrictions, output filtering, validation, logging, auditing, isolation (I)
6. Red-team your own layer: 5+ attacks → observe → mitigate → rerun → document residual risk (I)

**Lab.** `security-lab` (Docker Compose): fake ticket service with an injected ticket, docs service with a poisoned page, a malicious MCP server, canary secrets, an egress catcher. Attacks join `agent-evals` as a permanent regression suite.
**Simulation.** Security simulator — injection travels ticket → MCP → agent → tool; toggle defenses and watch which stage stops it.
**Artifact.** Threat model, attack suite, mitigation diff, residual-risk register, security section in `ai-layer-lab` README.
**Sample quiz.** *Your agent reads public GitHub issues and can push to branches. Draw the attack path and list defenses in the order you'd implement them.*

#### Module 10 — Multi-Agent Systems *(new)*
*4 lessons · 60 min · practice 5 h*

Most importantly: when multi-agent is worse than one agent.

**Objectives.** Describe topologies (planner/worker, planner/reviewer, specialists, parallel, hierarchical) · design handoffs and shared state · reason about failure propagation, cost and latency · debug via traces · decide on evidence.

**Lessons.**
1. Topologies: single, planner/worker, planner/reviewer, specialists, parallel, hierarchical (C)
2. Coordination: handoffs, shared state, synchronization, conflicting outputs (C + I)
3. Cost, latency, failure propagation and debugging — math: expected cost = Σ calls × tokens × price; end-to-end success ≈ Πpᵢ; tracing multi-agent runs (C, math)
4. When multi-agent is worse: running the comparison (I)

**Project.** Implement planner/worker/reviewer and a single-agent baseline on the same `agent-evals` tasks; compare pass rate, cost, latency with intervals.
**Break.** Two workers edit the same file / reviewer and planner disagree indefinitely.
**Simulation.** Multi-agent simulator — planner/worker/reviewer message flow with adjustable per-agent reliability and cost.
**Artifact.** `agent-evals/reports/multi-vs-single.md` with a recommendation.
**Sample quiz.** *A vendor pitches a 7-agent pipeline for PR review. What three numbers do you ask for before believing it beats one agent?*

#### Module 11 — Agents in CI and Production
*5 lessons · 75 min · practice 5 h* (revision-1 Module 6, expanded)

**Objectives.** Run headless agents safely · measure CI-review signal vs noise · automate recurring work (triage, changelog, dependencies, tests, docs, scheduled agents) · add approval, rollback, observability, cost controls, secrets and failure handling · write governance.

**Lessons.**
1. Headless agents: invocation, permissions, determinism, secrets in CI (I)
2. CI review: measuring false positives against human reviews (I, math: precision/recall)
3. Recurring automation: triage, changelog, dependency work, test generation, documentation, scheduled agents (I)
4. Operating it: approval workflows, rollback, observability, cost ceilings, failure handling (C + I)
5. Governance: who owns the AI layer, change control, system-evolution policy (agent mistake → rules update → eval) (C)

**Lab & break.** Run CI review on 20 historical PRs; it floods with false positives — tune until precision is acceptable, measured.
**Artifact.** `.github/workflows/agent-review.yml`, one recurring automation with approval gate, `governance.md`, CI-review precision/recall report.
**Sample quiz.** *CI review catches 3 real bugs a month but posts 60 comments. Developers have started ignoring it. What do you change and how do you measure it?*

---

### STAGE F — Enterprise AI Architecture

#### Module 12 — Enterprise AI-Agent Architecture *(new)*
*5 lessons · 80 min · practice 5 h*

The questions a real enterprise's architects, security and legal teams will ask.

**Objectives.** Compare hosting options · design gateways, routing, quotas, rate limits and cost allocation · design identity, RBAC, secrets, private networking and tenant isolation · address retention, residency, compliance · produce ADRs and a reference architecture that survives security review.

**Lessons.**
1. Hosting options: SaaS model APIs, cloud-hosted models (hyperscaler platforms), private deployments, self-hosted open weights (C)
2. Model gateways and routing: quotas, rate limits, fallback, cost allocation per team (C + I)
3. Identity, RBAC, secrets management, private networking, tenant isolation, audit logs, observability (C)
4. Data and compliance: retention, residency, training-use terms, compliance frameworks orientation, the security-review questionnaire (C)
5. Architecture decision records and the reference diagram (I)

**Project.** Architecture for a hypothetical 400-developer enterprise: developers, coding agents, model providers, gateway, MCP servers, GitHub, CI/CD, ticketing, documentation, identity, secrets, logging, security boundaries; ≥5 ADRs; answers to a 30-question security review.
**Break.** Review finds agent tokens shared across teams and prompts logged with secrets — redesign.
**Artifact.** `enterprise-architecture/` diagram, ADRs, security-review Q&A.
**Sample quiz.** *Legal says "no code may leave the EU." What changes in your architecture, and what does it cost you in model choice?*

---

### STAGE G — Measurement and Evidence

#### Module 13 — Measuring AI Engineering Impact
*6 lessons · 100 min · practice 6 h + 3–6 weeks of data* (revision-1 Module 7, substantially strengthened)

No "AI made us 40% faster" unless the experiment supports it.

**Objectives.** Choose and compute delivery metrics · design a controlled comparison · name and mitigate threats to validity · quantify uncertainty · distinguish correlation from causation and practical from statistical significance · critique published and vendor claims · report honestly.

**Lessons.**
1. Metrics: cycle time, lead time, PR review time, rework, defects, escaped defects, effort, throughput, DORA, quality-adjusted productivity, cost per successful task (C)
2. The evidence base: what published RCTs, field studies and industry reports found, why they disagree, and how to critique exaggerated claims (C)
3. Experiment design: baseline, treatment, control, randomization, within-developer designs (C + I)
4. Threats to validity: selection bias, task difficulty, developer differences, learning effects, Hawthorne effect, regression to the mean, model-version changes, contamination (C)
5. Statistics: sample size, bootstrap confidence intervals, permutation tests, effect sizes, practical vs statistical significance, correlation vs causation (C, math)
6. Running and reporting the comparison (I)

**Project.** Real controlled comparison on the student's team or own work (≥20 tasks per arm where feasible), with pre-registered metrics and an honest report including what it does not show.
**Break.** Provided dataset shows a 40% speedup that vanishes after controlling for task difficulty.
**Simulation.** Measurement simulator — vary sample size, difficulty mix, outliers and selection bias; watch the apparent improvement swing.
**Artifact.** `method-notes/experiment-01.md` with data, analysis script and limitations; one-page internal write-up.
**Sample quiz.** *A team reports AI cut coding time 50%, but escaped defects doubled. What additional measurements do you request before claiming improvement?*

---

### STAGE H — Methodology

#### Module 14 — Naming Your Method
*4 lessons · 60 min · practice 4 h* (revision-1 Module 8, expanded)

**Objectives.** Extract principles from incidents and experiments · name concepts and draw explanatory diagrams · design, version and evolve a methodology · handle attribution and IP; distinguish original synthesis from a copied framework.

**Lessons.**
1. Why vocabulary is the IP; what makes a methodology (C)
2. Incident → principle → name, with an evidence trail (I)
3. Diagrams, versioning and the evolution policy for the method itself (I)
4. Originality, attribution and intellectual property (C)

**Lab & break.** Audit your draft against three public frameworks — flag every term you borrowed and either attribute, replace, or justify.
**Artifact.** `method-notes/`: `concepts.md` (each concept → dated incident/experiment), diagrams, `method-v1.md`, evolution policy, objections FAQ.
**Exit test.** A non-engineer paraphrases your loop correctly from `concepts.md`.
**Sample quiz.** *Your "4-phase loop" is identical to a popular trainer's. What do you keep, what do you change, and how do you credit?*

---

### STAGE I — Teaching

#### Module 15 — The Demo Repo and Live Demo Craft
*4 lessons · 60 min · practice 8 h* (revision-1 Module 9)

**Objectives.** Build a credible brownfield demo with real engineering value · scaffold a realistic company around it · record an unedited run · pass the stranger test · recover live from failure.

**Lessons.**
1. Designing a credible brownfield demo: a legacy .NET + SQL Server app with messy history and undocumented conventions (C + I)
2. Company scaffolding and the "before" state: PRD, tickets, docs, empty AI-layer commit, then the configured layer (I)
3. The unedited recording and the stranger test (I)
4. When the demo breaks: recovery patterns, fallback recordings, narrating failure as teaching (C)

**Lab & break.** Deliberately trigger a demo failure (rate limit, wrong plan) during a recorded run and recover on camera.
**Artifact.** `brownfield-demo/` with history, PRD, tickets, docs, AI layer, `run-01-unedited`, `stranger-test-notes.md`.
**Exit test.** A stranger gets a working PR from your skills in 30 minutes, unaided.

#### Module 16 — Instructional Design for Engineers *(expanded)*
*5 lessons · 80 min · practice 5 h incl. 30-minute mini-workshop*

People enjoying a workshop ≠ people learning something.

**Objectives.** Write measurable objectives and align assessments (backward design) · manage cognitive load and progressive difficulty, surface misconceptions · design show → do → reflect with labs, live coding and demos · handle mixed levels, skeptics, remote and in-person rooms · measure learning with pre/post assessment and feedback analysis.

**Lessons.**
1. How adults and engineers learn; enjoyment vs learning (C)
2. Objectives, assessment alignment, cognitive load, progressive difficulty, misconceptions (C)
3. Show → do → reflect: exercises, hands-on labs, live coding, demos, pacing (C + I)
4. The room: Q&A, mixed skill levels, skeptics, remote vs in-person (C)
5. Measuring learning: pre/post assessment, behavior follow-up, feedback analysis (C + I, math: normalized gain)

**Field.** Deliver a 30-minute mini-workshop on one concept to 3–8 people: pre-assessment → workshop → exercise → post-assessment → feedback analysis.
**Artifact.** `talks/mini-workshop/` with objectives, assessments, results, analysis.
**Sample quiz.** *Feedback averages 4.8/5 but post-test scores didn't move. What happened, and what do you change?*

#### Module 17 — Workshop Design and Delivery
*4 lessons · 60 min · practice ~10 h (3 rehearsals)* (revision-1 Module 10, split)

**Objectives.** Build the full 2-hour workshop with instructor notes, demo, exercises, starter pack, troubleshooting and fallback · run Q&A in agent wait-time · answer hard questions honestly · rehearse to a standard.

**Lessons.**
1. The agenda: timestamped arc (credibility → problem → live before/after → exercise → Q&A → close), instructor notes (I)
2. Materials: starter pack, exercises, troubleshooting guide, fallback recording, pre/post assessment (I)
3. The hard-questions bank: "AI will replace us", security, ROI, skeptics — answered with your own evidence; Q&A during agent wait-time (C)
4. Rehearsal protocol and observation rubric (I)

**Artifact.** `workshop-kit/`: agenda, instructor notes, starter pack, exercises, troubleshooting, fallback video, question bank, 3 rehearsal logs with observer scores.
**Exit test.** Full 2 hours from your outline, notes checked ≤2 times.

#### Module 18 — Teaching in Public
*4 lessons · 60 min · ongoing* (revision-1 Module 11)

**Objectives.** Plan an evidence-based content strategy · write clearly and avoid hype · produce posts, videos, talks, demos and case studies · turn audience questions into content and credibility.

**Lessons.**
1. Content as funnel; audience research; credibility over time (C)
2. Technical writing and evidence-based claims; avoiding hype (C + I)
3. Formats: short posts, videos, talks, live demos, case-study pieces (I)
4. Lunch-and-learn, free pilot, and questions → content (I)

**Artifact.** `talks/`: content calendar, ≥4 published pieces with links, lunch-and-learn deck + feedback, pilot feedback.
**Exit test.** Someone outside your network asks a follow-up question unprompted.

---

### STAGE J — Organizational Adoption

#### Module 19 — AI Adoption and Change Management *(new)*
*4 lessons · 60 min · practice 4 h*

Why technically successful AI systems fail organizationally.

**Objectives.** Diagnose adoption failure · address developer, manager, security and legal objections · build champion networks and handle tool fragmentation · design enablement (training, office hours, feedback loops, governance ownership) · measure adoption and prevent regression after you leave.

**Lessons.**
1. Why rollouts fail; stakeholders and objections: trust, fear of replacement, manager skepticism, security and legal (C)
2. Champions, early adopters, team-wide adoption, tool fragmentation (C)
3. Enablement mechanics: training, office hours, feedback loops, governance ownership (I)
4. Measuring adoption and preventing regression after the consultant leaves (C + I)

**Artifact.** Adoption plan for the student's team (or the M12 hypothetical enterprise): stakeholder map, champions, cadence, adoption metrics, anti-regression mechanisms.
**Sample quiz.** *Three months after your engagement, usage is back to 10% of devs. What do you look at first?*

---

### STAGE K — Consulting and Business

#### Module 20 — Offers, Pricing, Selling and Consulting
*5 lessons · 80 min · practice 5 h + real pitch* (revision-1 Module 12, expanded)

Professional qualification and honest communication of uncertainty — no manipulative tactics.

**Objectives.** Define problems, positioning and value proposition · design an offer ladder (workshop, pilot, implementation, retainer) · choose pricing models and compute value with ranges · qualify and run discovery · write proposals and SOWs with scope control · handle procurement, legal basics, employer conflicts, IP, invoicing, difficult clients.

**Lessons.**
1. Positioning and the offer ladder: problem definition, value proposition, workshop → pilot → implementation → retainer (C)
2. Pricing models and value pricing with uncertainty — math: ROI range from measured effect ± interval (C, math)
3. Qualification, application forms, discovery calls — honest, non-manipulative (I)
4. Proposal, SOW, scope control, exclusions, change requests, procurement (I)
5. Legal and admin: employer conflicts, IP, contracts, invoicing; difficult clients (C)

**Field.** Pitch your employer (internal budget) or one network contact.
**Artifact.** `workshop-kit/`: `offer-ladder.md`, `pricing.md`, application form, discovery-call script, proposal, SOW template.
**Sample quiz.** *A prospect asks you to guarantee a 30% productivity improvement. How do you respond and what do you put in the SOW instead?*

#### Module 21 — Delivering Engagements
*5 lessons · 80 min · practice: real engagement* (revision-1 Module 13, expanded)

Discovery → Audit → Baseline → Architecture → Build → Enable → Measure → Handover → Follow-up.

**Objectives.** Run kickoff and stakeholder mapping · audit and baseline a client · architect and build with the team · enable and drive adoption · measure, hand over, support · write the case study.

**Lessons.**
1. Discovery, kickoff and stakeholder mapping (I)
2. Technical audit and baseline (I)
3. Architecture and build — with the team, not for them (I)
4. Enable, measure, hand over, support, follow up (I)
5. Writing the anonymized case study (I)

**Lab.** Full dry-run engagement against `brownfield-demo` as a simulated client, then a real (paid or pilot) engagement.
**Artifact.** `engagement-playbook.md`, engagement records, `case-studies/`.

#### Module 22 — Productization and Scaling *(new)*
*4 lessons · 60 min · practice 4 h*

**Objectives.** Turn consulting into repeatable services · move workshop → course → community · package templates, kits and assessments; understand licensing · set capacity limits and retainers so you're not only selling hours.

**Lessons.**
1. From hours to repeatable service (C)
2. Workshop → course → community; licensing your material (C)
3. Templates, starter kits and assessments as products (I)
4. Capacity-limited consulting, retainers, and the productization plan (I)

**Artifact.** Productization plan with capacity cap, pricing, and 12-month roadmap.

---

## 10. Math Placement

Only where it builds understanding; each as intuition → equation → tiny example → implementation → interpretation.

| Topic | Module |
|---|---|
| Softmax with temperature; top-p truncation | M2 |
| Probability all k runs succeed (pᵏ); compounding step failure | M2, M10 |
| Token / context budget arithmetic; expected cost per task | M4, M10 |
| Grader precision / recall; CI-review precision / recall | M7, M11 |
| Binomial and Wilson confidence intervals; runs needed; pass@k vs pass^k | M7 |
| Bootstrap CIs, permutation tests, effect size, sample size | M13 |
| Normalized learning gain (pre/post) | M16 |
| ROI range with uncertainty; cost per successful task | M13, M20 |

---

## 11. Simulations

Standalone, educational, clearly labeled as not reproducing real model behavior. Built as plain HTML + CSS + vanilla JavaScript with no framework and no build step (no React, no bundler, no npm), so they run unchanged on GitHub Pages and embedded in Tal's Academy lessons — technical rules in §19.6.

| Simulation | Module | Teaches |
|---|---|---|
| Agent-loop | M2 | request → reasoning → tool → result → … → validation; where loops break |
| Context budget | M4 | irrelevant context eats usable budget and relevant-fact recall |
| Evaluation | M7 | model/prompt/skill A vs B on one task set; trial count vs interval width |
| Security | M9 | injection path ticket → MCP → agent → tool; toggle defenses per stage |
| Multi-agent | M10 | planner/worker/reviewer flow; reliability, cost, latency compounding |
| Measurement | M13 | sample size, difficulty mix, outliers, selection bias distorting apparent gains |

---

## 12. Capstone

Depends naturally on the previous 22 modules; 6–10 weeks.

The student must:
1. Select a real engineering wedge
2. Audit a brownfield repository
3. Create the AI layer
4. Implement skills
5. Implement an MCP integration
6. Implement hooks
7. Implement evaluation tests
8. Conduct security testing
9. Create a CI/headless workflow
10. Measure baseline performance
11. Run a controlled comparison
12. Document limitations
13. Create a named methodology
14. Build a public demo
15. Deliver the 2-hour workshop to a real team
16. Collect participant feedback
17. Measure learning (pre/post)
18. Create an adoption plan
19. Produce an offer
20. Produce a case study

**Submission:** technical repository · AI layer · evaluation suite · security assessment · architecture diagram · methodology · workshop materials · workshop recording · participant feedback · before/after measurement · adoption plan · offer sheet · SOW · anonymized case study.

### Capstone Rubric (100 points, evidence required for every score)

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

Each category has 4 bands (0–3 missing/unsupported, 4–6 present but weakly evidenced, 7–8 solid, 9–10 exemplary) with descriptors in `assessments/capstone-rubric.md`.

---

## 13. Assessments, Templates, Glossary

**Quizzes.** All multiple choice, graded by plain code (no LLM, no free-text answers), format in §19.4:
- **Knowledge check** per lesson: 3–5 questions in `lesson-NN.quiz.yaml`.
- **Module quiz** per module: 8–10 scenario questions, reasoning over recall, in `assessments/module-NN-quiz.quiz.yaml`.
- Pass mark 70%, unlimited retries; the correct answer and explanation are shown only after the student submits.
- Open-ended work (field assignments, projects, the capstone) is assessed by rubric and evidence, not by quizzes.

**Templates** (`templates/`, usable outside the course): brownfield audit checklist · AI-layer architecture template · context audit template · skill design template · sub-agent design template · MCP security checklist · agent threat model · evaluation dataset template · evaluation rubric · benchmark template · metrics template · experiment design template · notes log · ADR · governance policy · security review checklist · workshop template · lesson template · pre/post assessment · customer discovery script · discovery-call script · application form · proposal · SOW · pricing worksheet · case-study template · adoption plan · engagement playbook.

**Glossary** (`glossary.md`): agent, agent loop, context engineering, prompt engineering, tool calling, MCP, skill, sub-agent, orchestration, RAG, structured output, evaluation, LLM-as-judge, prompt injection, indirect prompt injection, tool poisoning, excessive agency, sandboxing, human-in-the-loop, headless agent, model routing, governance, observability, AI layer, brownfield, DORA, cycle time, rework, escaped defect, statistical significance, confidence interval, causal inference, change management — plus terms introduced along the way. Short, precise, each linked to its lesson.

---

## 14. Research, Sources and Maintenance

**Research log** (`references/research-log.md`): source · date accessed · topic · claim supported · lesson(s) · primary (Y/N) · volatility (stable / quarterly / annual).

**Source policy.** Primary first: official docs (MCP specification, agent tools' docs, provider API docs), original papers (e.g. transformer architecture, nucleus sampling, long-context position effects, indirect prompt injection, SWE-bench-class agent benchmarks, LLM-as-judge bias), official engineering blogs, OWASP, NIST, DORA reports, published RCTs on developer productivity. Every claim is verified at write time; none are taken from memory.

**`COURSE-MAINTENANCE.md`.**
- *Quarterly:* tool-specific lessons (agent config formats, MCP spec changes, CI actions, model lineups, pricing), lab setup scripts, security attack/defense state.
- *Annual:* productivity evidence base, enterprise hosting/compliance landscape, glossary, capstone rubric.
- *Stable concepts* (tokens, sampling, context layering, eval statistics, experimental design, instructional design, change management) reviewed only when sources change.
- Lab update procedure when APIs change: pinned versions, smoke tests per lab, `last_verified` front-matter, changelog.

---

## 15. Mapping from Revision 1

| Rev 1 | Rev 2 |
|---|---|
| M1 Role & Wedge | M1 (expanded: pain vs hype, who pays, pain map) |
| M2 AI Layer | M3 (expanded) + context parts moved to M4 |
| M3 The Loop | M5 (expanded) |
| M4 Skills & Sub-Agents | M6 (expanded) |
| M5 MCP & Hooks | M8 (expanded); security talk → M9 |
| M6 CI & Governance | M11 (expanded) |
| M7 Measuring | M13 (substantially strengthened) |
| M8 Naming Your Method | M14 |
| M9 Demo | M15 |
| M10 Workshop | M16 + M17 |
| M11 Teaching in Public | M18 |
| M12 Offers & Pricing | M20 (expanded) |
| M13 Engagements & Scaling | M21 + M22 |
| — | New: M2 Fundamentals, M4 Context, M7 Evaluation, M9 Security, M10 Multi-Agent, M12 Enterprise, M19 Change Management |

---

## 16. Course Website & Repository Structure

Static site on GitHub Pages (docsify, consistent with the sibling courses), with: responsive layout, sidebar by module, per-lesson and per-module progress with "resume where I stopped" (localStorage), prerequisites/time/objectives header on every lesson, collapsible exercises, quiz engine with answer-after-attempt, embedded simulations, downloadable templates, code blocks with copy, diagrams (SVG), per-lesson sources, glossary with backlinks, references page, completion tracking.

All of that site behaviour lives in the docsify shell (`index.html` + `assets/site/*.js|css`): the progress buttons, the header built from front-matter, the quiz engine that renders `*.quiz.yaml`, the simulation embeds. The content files themselves stay plain markdown and YAML (§19), so Tal's Academy can import them and supply its own progress, quizzes and certificates.

```text
/
├── README.md  COURSE-MAP.md  PROGRESS.md  COURSE-MAINTENANCE.md  glossary.md
├── index.html  _sidebar.md  .nojekyll
├── brief/  curriculum/            # brief, this outline, per-module plans (not imported)
├── references/                    # research-log.md, bibliography
├── lessons/module-01/ … module-22/
│     lesson-01.md                 # the lesson (front-matter + markdown)
│     lesson-01.quiz.yaml          # its knowledge check
│     lesson-01.instructor.md      # instructor notes (never in the sidebar)
├── assessments/module-01-quiz.md + module-01-quiz.quiz.yaml … capstone-rubric.md
├── labs/module-01/ … module-22/   # everything a module's labs need = that module's download
├── simulations/<name>/index.html  simulations/common/
├── templates/  exercises/
├── projects/ ai-layer-lab/ agent-evals/ security-lab/ enterprise-architecture/
│            method-notes/ brownfield-demo/ talks/ workshop-kit/ case-studies/   # scaffolds + READMEs
└── assets/                        # diagrams (SVG/PNG); assets/site/ = docsify shell JS/CSS
```

---

## 17. Build Plan

| Phase | Output |
|---|---|
| 1 Audit | Curriculum map of revision 1: strong / expand / duplicate / missing / outdated (done in this revision) |
| 2 Architecture | This outline + dependency graph; per-module plans `curriculum/module-NN-plan.md` |
| 3 Research | Research log populated per module, sources verified |
| 4 Foundations | Site shell, navigation, progress system, lesson/quiz/exercise frameworks, source system |
| 5 Technical modules | M2–M13 written first |
| 6 Labs | Lab repos and Docker environments (security-lab, eval harness, brownfield .NET app, 900-line bad context) |
| 7 Simulations | The six simulations |
| 8 Teaching/business modules | M1, M14–M22, built on the technical artifacts |
| 9 Capstone | Brief, rubric, submission checklist |
| 10 QA | Every link, code sample, exercise, quiz, diagram, simulation, prerequisite and navigation path; a full end-to-end run-through; the §19.9 checklist passes |

---

## 18. Known Limitations

- The agent-tool and model landscape changes quarterly; implementation lessons are marked and scheduled for review.
- Legal, tax and compliance content is orientation only and jurisdiction-dependent.
- Real controlled comparisons need a team and weeks of data; solo students get a reduced within-developer design and are told what it cannot show.
- Labs default to .NET/SQL Server brownfield code; other wedges need lab substitutions.
- Simulations are pedagogical models, not predictions of real model behavior.

---

## 19. Tal's Academy Import Contract (binding)

The course is imported into Tal's Academy (Next.js + Supabase) straight from its GitHub repo, with no hand edits. GitHub stays the source; the Academy re-imports after every change. Everything below exists so that import is mechanical. When a rule here conflicts with an earlier section, this section wins.

### 19.1 General rules

- Content is **plain markdown and YAML**. No React, no JSX/MDX, no Vue, no build step, no generated HTML pages, no content that only exists after JavaScript runs.
- English only. UTF-8, LF line endings.
- Never rename or renumber a published lesson, quiz or module path: paths are the stable ids that learners' progress hangs on. Add new lessons at the end of a module, or accept that a renumbered one starts fresh.
- Nothing a learner needs may depend on localStorage, cookies or the docsify shell. The shell may add conveniences; the markdown must stand alone.

### 19.2 Structure and `_sidebar.md`

- Lessons: `lessons/module-NN/lesson-MM.md` (two digits each; `MM` restarts at 01 in every module). Stable id = `NN.M`, e.g. `07.3`.
- Module quiz: `assessments/module-NN-quiz.md` (a short intro page) + `assessments/module-NN-quiz.quiz.yaml` (the questions).
- `_sidebar.md` is the single table of contents and is what the importer reads:

```markdown
- [Home](/)
- [Glossary](glossary.md)
- [Templates](templates/README.md)

- Stage B — Understand the technology
- **Module 2 — LLM and Agent Fundamentals**
  - [06 · From text to next token](lessons/module-02/lesson-01.md)
  - [07 · Sampling and (non-)determinism](lessons/module-02/lesson-02.md)
  - [Module 2 quiz](assessments/module-02-quiz.md)
```

- Top-level links (Home, Glossary, Templates, References, Capstone brief/rubric) become the Academy's "Reference" section.
- A stage is a plain top-level line with **no link and no bold** (`- Stage B — …`); the importer ignores it.
- Each module is one bold line exactly `- **Module N — Title**`, followed by its lessons and then its quiz, indented two spaces.
- Lesson link text is `NN · Title` (global running number, then the title); the title matches the lesson's H1.
- Pages under `curriculum/`, `brief/` and `output/` are never imported; put no learner content there. Module plans may still appear in the sidebar.

### 19.3 Lesson file

```markdown
---
id: "07.3"
module: 7
minutes: 15
practice_minutes: 60
prerequisites: ["07.2", "04.1"]
objectives:
  - Explain grader false positives and false negatives as precision and recall.
  - Calibrate an LLM judge against 20 human-graded answers.
volatility: concept          # concept | implementation
sources:
  - title: "Judging LLM-as-a-Judge with MT-Bench and Chatbot Arena"
    url: https://arxiv.org/abs/2306.05685
last_verified: "2026-10-01"
---

# 07.3 · Graders: deterministic checks, rubrics, humans and LLM judges

## Why it matters
…
```

- Exactly one H1, the first line after the front-matter: `# NN.M · Title`.
- Sections are `##` headings in the §5 order: `## Why it matters`, `## How it works`, `## Show me`, `## Try it`, `## Break it`, `## Fix it`, `## How do I know it works?`, `## Use / don't use`, `## Reflect`, `## Sources`. The knowledge check is not a section; it lives in the `.quiz.yaml` (19.4).
- `prerequisites` lists lesson ids (`"NN.M"`), never titles or paths.
- Allowed markdown: GitHub-flavoured markdown (tables, task lists, fenced code with a language), `$…$` / `$$…$$` math (KaTeX), `mermaid` fenced diagrams, images, and GitHub alert callouts `> [!NOTE]`, `> [!TIP]`, `> [!IMPORTANT]`, `> [!WARNING]`, `> [!CAUTION]`. Every lab that can cause harm (security-lab, secrets, production systems, clients' code) opens with a `> [!WARNING]` or `> [!CAUTION]`.
- Allowed raw HTML: only `<details>`/`<summary>` (hints, solutions, collapsible exercises), `<kbd>`, `<sub>`, `<sup>`, `<br>`. No `<div class=…>` callouts, `<script>`, `<style>`, `<iframe>`, inline `<svg>`, `<button>`, forms or event attributes — they are stripped on import.
- Links: relative repo paths only (`../module-04/lesson-02.md`, `../../templates/adr.md`, `../../labs/module-07/`). The importer rewrites them to Academy URLs, module downloads or GitHub. No absolute GitHub Pages URLs to the course's own pages.
- Images: files under `assets/` (SVG or PNG), linked as `![alt text](../../assets/m07-grader-matrix.svg)`, always with meaningful alt text.
- Commands learners type go in fenced code blocks; no lesson calls a progress tool (`course.py complete …` or similar).

### 19.4 Quizzes (`*.quiz.yaml`)

The YAML file is the only copy of the questions and answers — no answer keys in markdown.

```yaml
# lessons/module-07/lesson-03.quiz.yaml
- id: q1                     # unique within the file, never reused for a different question
  question: >-
    Your LLM judge approves 18 of 20 answers that humans rejected as wrong.
    Which grader property is failing?
  options:
    - Its recall is too low, so it misses answers that are actually correct.
    - It has a high false-positive rate, so its "pass" cannot be trusted.
    - It is too strict, so real improvements will look like regressions.
    - Nothing; LLM judges are only meant to rank answers, not grade them.
  correct: 1                 # 0-based index into options
  explanation: >-
    Approving answers humans rejected is a false positive. With 18 of 20 wrong answers passing,
    a "pass" from this judge carries almost no information until it is recalibrated.
```

- Exactly 4 options, exactly one correct, `correct` is 0–3. Single answer only: no "select all that apply", free text, numeric entry or ordering questions.
- Knowledge check: 3–5 questions per lesson. Module quiz: 8–10 scenario questions, reasoning over recall.
- Quality rules (the Academy checks these by script):
  - The correct option is faithful to the lesson and not debatable.
  - The three wrong options are plausible misconceptions someone who skimmed the lesson would pick — never absurd or joke answers.
  - All four options have similar length, tone and specificity; the correct one is not the longest or most hedged.
  - No "all of the above" or "none of the above".
  - Across a file, spread `correct` over 0–3 (about 25% each, no pattern).
- YAML: use `>-` block scalars for any text containing `:`, `#`, quotes or backslashes. Inside block scalars write LaTeX normally (`$\frac{a}{b}$`); in a quoted string every backslash must be doubled. Parse every file before committing.
- `explanation` is shown after the learner submits; write it to teach, in one to three sentences.

### 19.5 Labs, templates and downloads

- Everything a module's labs need lives under `labs/module-NN/`; the Academy offers that folder as the module's zip download. Shared lab code goes in `labs/common/` and is referenced from each module's README.
- Templates live in `templates/` as markdown files; lessons link to them relatively.
- Portfolio scaffolds under `projects/` are linked, not embedded.

### 19.6 Simulations

- Each simulation is `simulations/<name>/index.html` with its own `.js`/`.css` next to it, plus shared code only from `simulations/common/`. Plain HTML + CSS + vanilla ES modules; no framework, bundler or npm; no CDN or other network requests (vendor everything); no external fonts.
- It works inside a **sandboxed iframe** (`allow-scripts` only): no cookies, no access to the parent page, `localStorage` optional and wrapped in try/catch, no `alert()`/`prompt()`, no popups, no top-level navigation.
- Responsive from 360 px wide; keyboard-usable; a visible "educational model, not real model behaviour" label.
- Parameters and presets may come from the URL query (`?preset=biased-judge`), so one simulation can be embedded at different starting points.
- A lesson embeds a simulation with a normal link on a line of its own whose text starts with `Simulation:` — `[Simulation: Evaluation — trials vs interval width](../../simulations/evaluation/index.html?preset=one-run)`. docsify shows a link (or the shell turns it into an embed); the Academy turns it into an embedded simulation. Never write an `<iframe>` in content.

### 19.7 Course metadata

- `README.md`: H1 = course title; the first plain paragraph after it (not bold, not a list, not a heading) is the catalog description, one to three sentences.
- `glossary.md` entries link to the lesson that introduces them, relatively.

### 19.8 What the docsify shell may do (and the Academy ignores)

`index.html` and `assets/site/` may render front-matter as the lesson header, render `*.quiz.yaml` as an interactive quiz, keep progress in localStorage, and turn `Simulation:` links into iframes. None of that is imported; the Academy provides its own header, quizzes, progress, certificates and embeds from the same files.

### 19.9 Pre-publish checklist (run before every push)

- [ ] Every lesson in `_sidebar.md` exists, has valid front-matter and exactly one H1 matching its sidebar title.
- [ ] Every lesson has a `lesson-MM.quiz.yaml` with 3–5 valid questions; every module has a module quiz with 8–10.
- [ ] Every `*.quiz.yaml` parses; 4 distinct options each; `correct` in 0–3; positions spread; no correct option noticeably longer than the wrong ones.
- [ ] No disallowed HTML (19.3), no React/JSX/MDX, no absolute links to the course's own pages, no broken relative links or images.
- [ ] No published path was renamed or renumbered.
- [ ] Every simulation opens from `file://` and in a sandboxed iframe with no console errors and no network requests.
- [ ] Instructor notes are only in `*.instructor.md` and never linked from `_sidebar.md`.
