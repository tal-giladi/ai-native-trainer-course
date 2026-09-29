# Instructor notes — 12.4 Data, residency and compliance

**Teaching objective.** Students produce a data inventory that follows every copy of prompt data (location, retention, readers, governing terms), apply residency to every path rather than the primary route, and can state the price and capability cost of an EU-only constraint, while leaving legal conclusions to legal.

**Likely confusion.** Inference geography versus storage geography. On the vendor API they are separate settings; on Bedrock, data at rest stays in the source region while inference and abuse-detection storage may use destination regions within the geography. Have students write two columns, "processed in" and "stored in", for every row.

**Common misconception.** "Compliance is a certificate the vendor has." A SOC 2 report or ISO/IEC 42001 certificate covers the vendor's service as scoped; it says nothing about your fallback route, your MCP servers or a developer's personal account. The inventory is yours.

**Key analogy.** Tracing a parcel. The shipping label says "Berlin to Amsterdam", but the parcel also passes a sorting hub, gets photographed at customs and has its label scanned into a tracking database. Legal wants the whole route, not the label.

**Common failure.** Students skip rows 8 and 9 (feedback reports and auxiliary checks) because they never used those features. The point is that defaults, not usage, decide what can leave. Make them check the current docs, not memory.

**Expected exercise outcome.** Both lint runs, with each change in the solution mapped to a finding; a complete nine-row inventory for Fabrikam v1 citing ADRs and managed settings; an inventory of their own setup with at least one empty cell they had to go and find out; a residency cost estimate with both price and capability lag.

**Extension exercise.** Legal adds a second constraint: Israeli customer data may not leave Israel. Which rows change, which hosting options survive, and what would you ask legal before proposing a design?

**Discussion question.** The WebFetch safety check sends only a hostname to a vendor endpoint. Is that "code leaving the EU"? Who should decide, and how would you document the decision so the next reviewer does not reopen it?
