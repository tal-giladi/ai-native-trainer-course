# Instructor notes — 04.1 What context is and what it costs

**Teaching objective.** Students treat context as a budget with named terms, compute $B = W - S - R - T - H - O$ and the monthly cost of $R$ for a real layer, and can find the term that grew in a live session with `/context`.

**Likely confusion.** "The agent remembers the conversation." It does not; the harness re-sends everything on every request. Draw the request stack on the board and add one tool result at a time — students see $H$ grow and understand why turn 30 is more expensive than turn 1 (link back to the quadratic growth in 02.4).

**Common misconception.** "Caching makes context free." Caching changes the price of re-reading a stable prefix; it does not free window space and it does nothing for attention. Ask: "After caching, is the coffee-machine paragraph still competing with the migration rule?"

**Key analogy.** A carry-on suitcase. The airline (window) sets the size; the toiletries bag (system prompt) and the laptop (tools) go in first; every souvenir you pick up during the trip (history) takes space from what you can bring home. Packing a 300-page manual "just in case" (bloated rules) is paid on every flight.

**Common failure in the exercise.** Students compare `/context` with the tool and conclude the tool is wrong. Make them explain the gap (proxy tokenizer, auto memory, skill descriptions, MCP names). Second failure: they run the break, then ask the follow-up questions in a new session and see no effect — the point is that the log stays in *this* session.

**Expected exercise outcome.** `context/budget.md` with $R \approx 7{,}000$ (bloated) vs $\approx 500$ (Module 3 layer) in o200k tokens, $B$ at turns 1 and 30, monthly cost of $R$ uncached vs cached for their team size, and a `/context` screenshot showing the messages jump after `-v diag`.

**Extension exercise.** Measure $H$ over a real 20-turn session on their own repository (note `/context` every 5 turns), fit $H \approx s \cdot i$, and compare $s$ with the 1,500 tokens per turn assumed in 02.4.

**Discussion question.** If windows keep growing and cache reads keep getting cheaper, which part of this lesson stays true in five years, and which part becomes irrelevant?
