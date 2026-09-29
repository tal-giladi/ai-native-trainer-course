"""Canonical lesson manifest: module titles, stages and lesson titles.
Single source of truth for _sidebar.md, lesson ids and H1s. Run `py scripts/manifest.py` to regenerate _sidebar.md."""
from pathlib import Path

STAGES = {
    1: "Stage A — Becoming an AI-native engineering practitioner",
    2: "Stage B — Understand the technology",
    4: "Stage C — Context engineering",
    5: "Stage D — Reliable agentic software development",
    8: "Stage E — Tools, integration and security",
    12: "Stage F — Enterprise AI architecture",
    13: "Stage G — Measurement and evidence",
    14: "Stage H — Methodology",
    15: "Stage I — Teaching",
    19: "Stage J — Organizational adoption",
    20: "Stage K — Consulting and business",
}

MODULES = [
    ("The AI-Native Trainer Role & Picking Your Wedge", [
        "The roles and the flywheel",
        "Engineering pain vs AI hype",
        "Finding your wedge",
        "Customer discovery interviews"]),
    ("LLM and Agent Fundamentals", [
        "From text to next token",
        "Sampling and (non-)determinism",
        "The instruction hierarchy",
        "Tool calling and the agent loop",
        "Failure taxonomy and model selection"]),
    ("Anatomy of an AI-Native Codebase: The AI Layer", [
        "The fourth citizen: AI behavior as an engineering artifact",
        "Component map and portability",
        "The brownfield audit",
        "Writing a grounded rules file"]),
    ("Context Engineering", [
        "What context is and what it costs",
        "Layering context",
        "Context pathologies",
        "Compression and compaction",
        "The context audit"]),
    ("Research → Plan → Implement → Validate", [
        "Research discipline",
        "Plans worth reviewing",
        "Validation gates and reset decisions",
        "Failure diagnosis across the loop"]),
    ("Skills, Sub-Agents and Workflow Automation", [
        "Skill anatomy",
        "The five core skills",
        "Sub-agents, and when not to build one",
        "Testing, versioning and failure analysis for skills"]),
    ("Agent Evaluation", [
        "Why evaluate agents",
        "Task datasets",
        "Graders: deterministic checks, rubrics, humans and LLM judges",
        "Statistics for stochastic systems",
        "Comparisons and paired designs",
        "Evals in the loop"]),
    ("MCP, APIs and Hooks", [
        "MCP architecture, authentication and authorization",
        "A real integration on your wedge stack",
        "Building a custom MCP server in C#",
        "Hooks: enforcement and audit"]),
    ("Agent Security", [
        "Threat modeling agents",
        "Prompt injection, direct and indirect",
        "Tool poisoning and the supply chain",
        "Excessive agency and exfiltration",
        "Layered defenses",
        "Red-team your own layer"]),
    ("Multi-Agent Systems", [
        "Multi-agent topologies",
        "Coordination: handoffs, shared state and conflicts",
        "Cost, latency and failure propagation",
        "When multi-agent is worse"]),
    ("Agents in CI and Production", [
        "Headless agents",
        "CI review: signal vs noise",
        "Recurring automation",
        "Operating agents: approval, rollback, observability and cost",
        "Governance of the AI layer"]),
    ("Enterprise AI-Agent Architecture", [
        "Hosting options",
        "Model gateways and routing",
        "Identity, secrets, networking and audit",
        "Data, residency and compliance",
        "ADRs and the reference architecture"]),
    ("Measuring AI Engineering Impact", [
        "Delivery metrics",
        "The evidence base on AI productivity",
        "Experiment design",
        "Threats to validity",
        "Statistics for engineering comparisons",
        "Running and reporting the comparison"]),
    ("Naming Your Method", [
        "Why vocabulary is the IP",
        "Incident to principle to name",
        "Diagrams, versioning and the evolution policy",
        "Originality, attribution and intellectual property"]),
    ("The Demo Repo and Live Demo Craft", [
        "Designing a credible brownfield demo",
        "Company scaffolding and the before state",
        "The unedited recording and the stranger test",
        "When the demo breaks"]),
    ("Instructional Design for Engineers", [
        "How adults and engineers learn",
        "Objectives, alignment and cognitive load",
        "Show, do, reflect",
        "Managing the room",
        "Measuring learning"]),
    ("Workshop Design and Delivery", [
        "The workshop agenda",
        "Workshop materials",
        "The hard-questions bank",
        "Rehearsal protocol"]),
    ("Teaching in Public", [
        "Content as a funnel",
        "Evidence-based technical writing",
        "Content formats",
        "Lunch-and-learn, free pilot, and questions into content"]),
    ("AI Adoption and Change Management", [
        "Why rollouts fail",
        "Champions and tool fragmentation",
        "Enablement mechanics",
        "Measuring adoption and preventing regression"]),
    ("Offers, Pricing, Selling and Consulting", [
        "Positioning and the offer ladder",
        "Pricing with uncertainty",
        "Qualification and discovery calls",
        "Proposals, SOWs and scope control",
        "Legal, admin and difficult clients"]),
    ("Delivering Engagements", [
        "Discovery, kickoff and stakeholder mapping",
        "Technical audit and baseline",
        "Architecture and build with the team",
        "Enable, measure, hand over, follow up",
        "Writing the anonymized case study"]),
    ("Productization and Scaling", [
        "From hours to repeatable service",
        "Workshop to course to community",
        "Templates, kits and assessments as products",
        "Capacity-limited consulting and the productization plan"]),
]


def lessons():
    """Yield (module_no, lesson_no, global_no, id, title, path)."""
    g = 0
    for m, (_, ls) in enumerate(MODULES, 1):
        for l, t in enumerate(ls, 1):
            g += 1
            yield m, l, g, f"{m:02d}.{l}", t, f"lessons/module-{m:02d}/lesson-{l:02d}.md"


def sidebar():
    out = ["- [Home](/)", "- [Course map](COURSE-MAP.md)", "- [My progress](PROGRESS.md)",
           "- [Glossary](glossary.md)", "- [Templates](templates/README.md)",
           "- [Simulations](simulations/README.md)", "- [References](references/README.md)",
           "- [Capstone brief](assessments/capstone.md)", "- [Capstone rubric](assessments/capstone-rubric.md)", ""]
    all_ls = list(lessons())
    for m, (title, _) in enumerate(MODULES, 1):
        if m in STAGES:
            out += ["", f"- {STAGES[m]}"]
        out.append(f"- **Module {m} — {title}**")
        for mm, l, g, lid, t, p in all_ls:
            if mm == m:
                out.append(f"  - [{g:02d} · {t}]({p})")
        out.append(f"  - [Module {m} quiz](assessments/module-{m:02d}-quiz.md)")
    return "\n".join(out).replace("\n\n\n", "\n\n") + "\n"


if __name__ == "__main__":
    root = Path(__file__).resolve().parent.parent
    (root / "_sidebar.md").write_text(sidebar(), encoding="utf-8", newline="\n")
    print(f"{len(list(lessons()))} lessons, {len(MODULES)} modules")
