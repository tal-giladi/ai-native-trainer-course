# Instructor notes — 22.1 From hours to repeatable service

**Teaching objective.** Students turn one or more real (or dry-run) deliveries into a service card: one price, one scope, standard assets per repeating step, one budgeted custom step, three or more exclusions, a version. They compute effective day rates from their own time log and read a learning curve. `ScaleCheck service` clean.

**Likely confusion.** "Productized" vs "product". A productized service still needs the trainer in the room; what changes is that most of the work is standard and the hours fall. Students jump straight to courses (22.2) because that feels like scale; point out that the course is built from the same assets, so this step comes first. Second: the custom step. Students think admitting custom work breaks the idea; it is the opposite, it is what keeps the rest standard.

**Common misconception.** "Raising the price is how you fix a delivery that ran over." Sometimes, but the first delivery's overrun is usually the cost of learning. Price for the delivery you will do next time, and use the time log to check. Second: "A fixed fee is always better than a day rate." Only with a fixed scope; on a vague scope it moves all the risk to you.

**Key analogy.** A stable API. Callers see one contract (price, scope, deliverables); the implementation behind it improves every release. A change the client would notice is a breaking change and gets a MAJOR version.

**Common failure in the exercise.** No time log, so the hours are guessed after the fact; accept estimates for one delivery but insist on logging the next one by step. Second: assets named but not existing ("report template" that is last client's report with names deleted); that is a leak risk too (22.3). Third: exclusions that nobody ever asked for; good exclusions come from past scope pushes.

**Expected exercise outcome.** A `service-card.md` for the student's most-sold rung, with a steps table whose standard steps each name a versioned asset, a custom share at or under 25%, an effective day rate at plan above the floor from 20.2, and, if three deliveries exist, a learning rate and a prediction. `service` clean, or clean except the "fewer than three deliveries" warning.

**Extension exercise.** Ask a peer to read only the card and describe what a client receives, what they do not, and how long it takes. Every difference from what you meant is a wording fix. Then time how long it takes you to prepare the kickoff from the assets alone.

**Discussion question.** A long-standing client asks for "the pilot, but for our Java team". Is that the same service with a new custom step, a MAJOR version of the card, or a consulting engagement? What evidence from your time log decides it?
