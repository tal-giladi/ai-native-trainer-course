# Instructor notes — 08.3 Building a custom MCP server in C#

**Teaching objective.** Students build and test a small read-only MCP server in C#, make the database login the real least-privilege boundary, and design every data answer to carry provenance, with stale data refused as a tool error rather than decorated with a warning.

**Likely confusion.** "Freshness" as a timestamp. Students add `capturedAt` and consider the job done. Age is a weak signal; the strong one is comparison with a reference (the repository's migration head). Ask: "a snapshot taken one hour ago, before this morning's merge: fresh or stale?"

**Common misconception.** "If the tool says WARNING, the model will notice." In the break transcript it does not: it quotes the columns and ignores the header. Models treat tool results as facts and metadata as noise unless the result itself is unusable. That is the reason for `isError`.

**Key analogy.** A satellite navigation system with an old map. A small grey note "map data from 2019" in the corner changes nobody's driving. A screen that says "this road no longer exists, map outdated, use the detour signs" does.

**Common failure in the exercise.** Paths: the `.mcp.json` args are relative to the directory Claude Code starts in, and on Windows the environment variable is set in a different terminal. Have students run the exact command from `.mcp.json` by hand through `mcp-talk.sh` first. Second: students run the server with `dotnet run --project`, which adds seconds of build to every host start; publish once instead.

**Expected exercise outcome.** 17 tests green; `info` against the live database reports `FRESH` at V006; `schema_reader` gets Msg 229 on a row read; the server registered and allowed in Claude Code; BILL-161 implemented on `dbo.usp_InvoiceNote_LatestByCustomer`; T25–T27 run with 3 trials each. In the break, the student names three causes (stale source, no reference, research skipped `db/migrations`) and the missing SQL oracle in validation.

**Extension exercise.** Add a `schema://tables/{name}` MCP resource next to the tools and compare: who decides when it is read (the application) versus a tool (the model)? Or add a Streamable HTTP transport (`ModelContextProtocol.AspNetCore`) and write down everything that changes about authorization.

**Discussion question.** Which other internal sources does your team's agent read as if they were current (wiki exports, generated docs, cached API specs), and what would be the reference for each one's freshness check?
