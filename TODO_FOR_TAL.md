# Open items (not blocking)

- No lab has been run against a real model/`claude` CLI (cost); all lab tools were tested offline with stand-ins. Sample results in `labs/*/samples/` are labelled illustrative.
- Model ids and prices are "as of 2026-09" (volatility: implementation) — first quarterly review due 2026-12.
- `labs/module-03/.../V004__due_not_null.sql` fails on real SQL Server on purpose-by-accident; Module 8 teaches it as a CI finding with a workaround. Fix V004 only if you drop that teaching point.
- Some sources were confirmed via search because the publisher page returned 403 (SWE-bench Verified page, Loftus & Palmer, Guest et al., BMJ/ACM items, Kingman/Wilson DOIs, GDPR EUR-Lex). Listed in `references/research-log.md`.
- A few lessons run 2,500–3,100 words (19.1, 19.4, 21.1, 21.2), over the 2,000-word guideline.
- `ai-native-trainer-path.html` (the source teardown of another trainer's paid workshop) is kept out of the public repo via .gitignore.

# KDnuggets "AI managers" article — done 2026-11-11 (not committed)

Every figure re-checked at its primary source: `curriculum/research-notes/2026-11-11-ai-managers-factcheck.md`.
Dropped as unverifiable: SecondTalent "+135.8% prompt-engineering roles", the "Monte Carlo 75-90% per step -> 50%" claim, PwC 56% (now 62%, 2026 edition).

- ai-native-org: 01.2, 01.3, 10.1, 10.3, 11.4, 20.3 extended (4 new quiz questions); 12.4 and 19.3 EU AI Act wording fixed to the AI Omnibus. Logged in research/CHANGELOG.md. check.py and Academy check-course: 0 problems.
- llm-evals-course: 10.3 "the bar depends on what the answer does next"; quiz q5 replaced by q6. Academy check: 0 problems.
- ai-transformation-field-course: 04.6 and references/laws-and-guidance.md (Art 4 / Art 50(2) wording), 13.4 (McKinsey data-barrier figures), 14.4 (three oversight levels; quiz q3 replaced by q6). Academy check: 0 problems.
- Skipped: frontier LLM research course (no fit); ai-security-course (already covers it; has someone's uncommitted work in progress); capstone portfolio items (already in the capstone).
- Open issue: the OWASP LLM Top 10 2026 edition exists (v1.0, August 2026). ai-security-course already teaches it; ai-native-org (09.1, 09.4, 09.6) and ai-transformation-field-course (04.5, 14.4) still present the 2025 list as current.
