# Simulation brief

Build interactive educational simulations for the course at `C:\Users\TalGiladi\OneDrive\repos\course-creator\ai-native-org`.

Read first: `curriculum/course-outline.md` §11 and §19.6 (binding), and every lesson that links to your simulation (`grep -rn "simulations/<name>" lessons`) — the simulation must teach exactly what those lessons say, with the same numbers, vocabulary and presets.

## Rules (§19.6)
- `simulations/<name>/index.html` + its own `.js`/`.css` next to it; shared code only in `simulations/common/` (check what exists; add, don't clobber — e.g. `common/sim.css`, `common/sim.js` for the label, query parsing, seeded RNG, safe storage).
- Plain HTML + CSS + vanilla ES modules. No framework, bundler, npm, CDN, external fonts or any network request.
- NOTE: ES modules do not load from `file://` in Chrome. §19.9 requires opening from `file://`, so use classic `<script src>` files (IIFE / global namespace), not `type="module"`.
- Works in a sandboxed iframe (`allow-scripts` only): no cookies, no parent access, localStorage optional in try/catch, no alert/prompt/popups/top navigation.
- Responsive from 360 px; keyboard-usable (real buttons/inputs, labels, focus styles); light and dark (`prefers-color-scheme`).
- Visible label: "Educational model — not real model behaviour".
- Presets from the URL query `?preset=<x>`; support every preset the lessons link, plus a sensible default. Show a short explanation of what the preset demonstrates.
- Deterministic where possible (seeded RNG, "re-run" button changes seed).

## Also
- Write/extend `simulations/README.md`: one line per simulation with a relative link, the module lessons that use it, and its presets. Other agents may be editing it: re-read right before writing and only add your rows.
- Verify: open each page with a headless check if possible (e.g. `node` to at least parse the JS: `node --check file.js`), and confirm zero network references (`grep -n "http" simulations/<name>`). Run `py scripts/check.py` for the modules that link to your simulations — their simulation-link errors must be gone.
- Checkpoint: append `<name> done` to `curriculum/status/simulations.log` after each simulation.
- Report back in under 120 words.
