# Instructor notes — 09.4 Excessive agency and exfiltration

**Teaching objective.** Students classify excessive agency into functionality/permissions/autonomy, enumerate exfiltration channels with a control each, recognize that generated code can be vulnerable (catch it with CA2100 + review), and explain approval gates as blast-radius control.

**Likely confusion.** Conflating injection with agency. Injection is how instructions arrive; agency is how much they can do. Keep the two separate: you reduce injection's *impact* by cutting agency, not by resisting the text.

**Common misconception.** "The agent is honest, so its code is safe." Pearce's ~40% figure lands here: vulnerable code appears with no attacker. Treat generated code like a junior's PR.

**Key analogy.** Bash(*) is a master key. It feels like one item on the keyring, but it opens every door — network, files, settings. Scoping it is handing out only the keys the job needs.

**Common failure in the exercise.** Students scope MCP tools but leave Bash(*) and think they are done (the Break). Show that Bash(*) re-adds the exfiltration channel by itself. Also, some put `.claude/**` behind `ask` instead of `deny`; push them to justify deny (never a legitimate task step).

**Expected exercise outcome.** A per-line classification of both configs, A03/A06 scored blocked on hardened, a CA2100 build failure on concatenated SQL that passes once parameterized, and a correct explanation of deny-vs-ask for self-permission edits.

**Extension exercise.** Have them enable a broader analyzer set (e.g. the .NET security rules or `dotnet format analyzers`) and see what else it flags in Contoso Billing's legacy `SqlHelper`. Tie back to Module 3's brownfield.

**Discussion question.** Where is the line between an approval gate that protects and one that trains people to click "yes" without reading? Which actions truly need a gate?
