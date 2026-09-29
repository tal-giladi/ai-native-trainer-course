# Open items (not blocking)

- No lab has been run against a real model/`claude` CLI (cost); all lab tools were tested offline with stand-ins. Sample results in `labs/*/samples/` are labelled illustrative.
- Model ids and prices are "as of 2026-09" (volatility: implementation) — first quarterly review due 2026-12.
- `labs/module-03/.../V004__due_not_null.sql` fails on real SQL Server on purpose-by-accident; Module 8 teaches it as a CI finding with a workaround. Fix V004 only if you drop that teaching point.
- Some sources were confirmed via search because the publisher page returned 403 (SWE-bench Verified page, Loftus & Palmer, Guest et al., BMJ/ACM items, Kingman/Wilson DOIs, GDPR EUR-Lex). Listed in `references/research-log.md`.
- A few lessons run 2,500–3,100 words (19.1, 19.4, 21.1, 21.2), over the 2,000-word guideline.
- `ai-native-trainer-path.html` (the source teardown of another trainer's paid workshop) is kept out of the public repo via .gitignore.
