# Instructor notes — 14.2 Incident to principle to name

**Teaching objective.** Students turn their own logs into two or three concept cards: clustered by mechanism, written as four-part falsifiable claims with boundaries and counter-evidence, given a status that matches the evidence, named last with the five tests, traced with `MethodCheck trace`, and checked with a non-engineer.

**Likely confusion.** "Isn't this just the failure taxonomy again?" The taxonomy (02.5) classifies one failure; a concept is a recurring pattern across failures, with a claim about what prevents it and where it stops. Taxonomy classes are useful cluster keys, not concepts. Second confusion: students treat the Wilson interval as a test of the concept; it is a statement of how often, with honest uncertainty.

**Common misconception.** "More concepts make a stronger method." Two or three well-evidenced concepts carry a workshop; ten thin ones make students doubt all of them. Second: "a good name will make it stick". A good name makes a good claim stick; it also makes a bad claim spread.

**Key analogy.** A detective's case board. Incidents are the pinned evidence; the string between them is the mechanism; the suspect's name goes up last. Naming the suspect first is how innocent people get convicted.

**Common failure in the exercise.** Clustering by symptom ("rounding bugs", "date bugs") and finding no cluster over two. Ask "what was missing?" for each incident. Second: claims without an intervention ("agents duplicate code") — ask "so what should the team do?". Third: skipping the paraphrase test or running it with an engineer. Insist on a non-engineer; it is the module's exit test.

**Expected exercise outcome.** A `concepts.md` with 2–3 cards passing `trace`, at least one supported, often one hypothesis; at least one Wilson interval with its denominator; a paraphrase table with the reader's verbatim words and at least one revision made because of them. For the break: all six errors explained, C2 deleted and turned into a boundary, C3 folded into existing evidence.

**Extension exercise.** Swap anonymized failure logs with another student and cluster theirs without seeing their cards. Compare clusters. Where you disagree, whose evidence trail makes the judgment easier to inspect, and what would each of you need to log differently?

**Discussion question.** A workshop attendee says: "Your Shadow rule is just DRY, which we've known for twenty-five years." What do you concede, what is actually new in your claim, and does the name survive?
