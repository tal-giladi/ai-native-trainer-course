# Staging — things worth considering

Temporary document for the current week. The daily run appends class **A** and **B**
candidates here (full records); the weekly review evaluates them, archives this content into
`weekly/YYYY-Www.md`, and resets this file. Nothing here is course material.

## Candidates

### C-20261111-01 · .NET 8 reached end of support on 2026-11-10; labs target net8.0 and CI templates install SDK 8.0.x

- **Class:** A
- **Kind:** correction
- **Date discovered:** 2026-11-11
- **Date published:** 2026-11-10
- **Source:** https://github.com/dotnet/core/blob/main/releases.md (primary, fetched this session; dotnet.microsoft.com support-policy page does not resolve from this session's network)
- **Organization/researchers:** Microsoft (.NET team)
- **Category:** ci/production
- **What changed:** The official .NET releases table now lists .NET 8.0 (LTS, final patch 8.0.34) and .NET 9.0 (STS, final patch 9.0.23) as end-of-life with end of support 2026-11-10. .NET 11.0 (STS) shipped 2026-11-10; supported releases are now .NET 10.0 (LTS, 10.0.15) and .NET 11.0, both supported to 2028-11-14. Related: `actions/setup-dotnet` is at v6.1.0 (2026-11-03; v5.0.0 moved to Node.js 24), while course workflows use `@v4` — https://github.com/actions/setup-dotnet/releases (primary, fetched this session).
- **Technical summary:** 35 lab `.csproj` files target `net8.0` (32 with `RollForward=Major`, so they still run on a newer runtime; the three without are class libraries). An SDK 10/11 still builds `net8.0` but warns that the target framework is out of support, and no further security patches ship for the 8.0 runtime. The CI workflows learners copy into their own repos install the now-unsupported SDK with `dotnet-version: "8.0.x"`: `labs/module-11/workflows/{agent-fix,agent-triage,ai-layer-governance,budget-guard}.yml`, `labs/module-11/ops/break/dep-fix-hourly.yml`, `labs/module-05/hooks/loop-gates.yml`, `labs/module-07/ci/agent-evals.yml`, and the inline workflow in lesson 12.5 (`actions/setup-dotnet@v4` + `dotnet-version: "8.0.x"`, line ~164). Lessons 08.1, 12.1, 12.5 and labs/module-08, labs/module-12 READMEs hard-code `bin/Release/net8.0/...` paths, which stay correct only while the TFM is unchanged. Lesson 02.1 says "You need the .NET 8+ SDK" (still true as a minimum).
- **Why it might matter:** A course about running agents safely in CI teaches templates that install an end-of-support SDK; a learner who copies them into a production pipeline adopts an unpatched toolchain. The module-03 brownfield repo is a deliberate legacy fixture and can keep net8.0 as story; the CI templates and tool projects are not fixtures.
- **Evidence of adoption:** n/a (platform lifecycle fact).
- **Major organizations using it:** n/a
- **Open-source implementation:** https://github.com/dotnet/core
- **Paper:** none
- **Code:** none
- **Relationship to existing course material:** lessons 02.1, 08.1, 12.1, 12.5; labs/module-02, 05, 07, 08, 11, 12 (workflows and tool csproj files); `labs/module-22/solution/practitioner-kit/CHANGELOG.md` already records a .NET 10-only setup failure (T-02). `lookup "net8.0"` shows no registry topic.
- **Potential course lesson:** FIX, not ADD. Sentences/lines now stale: lesson 12.5 inline workflow `dotnet-version: "8.0.x"`; the seven lab workflow files above. Decide whether to retarget tool/lab projects to `net10.0` (LTS) and update the `bin/Release/net8.0/` paths in 08.1, 12.1, 12.5 and the module-08/12 READMEs together, keeping ids stable. Consider bumping `actions/setup-dotnet@v4` in the same pass.
- **Confidence:** high (dates from the official dotnet/core releases table)
- **Recommendation:** review for FIX: CI templates to `dotnet-version: "10.0.x"`; weigh a single TFM sweep to net10.0 (with path updates) vs keeping net8.0 + RollForward and adding an "as of 2026-11, .NET 8 is out of support" note.
