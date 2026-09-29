# Instructor notes — 12.2 Model gateways and routing

**Teaching objective.** Students can say what a gateway centralises and what it costs, size per-team token-bucket quotas, compute route availability including the gateway, and diagnose unattributed spend and noisy neighbours as one design error: a caller identity coarser than the unit being managed.

**Likely confusion.** Token bucket versus fixed window. Students picture a counter that resets every minute. Draw the bucket refilling continuously; a burst passes if the bucket is full, and sustained excess demand is what throttles. The provider docs say this explicitly.

**Common misconception.** "Just ask the provider for a higher limit." A higher limit moves the cliff; the next runaway loop is bigger. The quota boundary must match the unit that causes load (team, CI job), not the organisation.

**Key analogy.** Office electrical circuits. One breaker for the whole floor means one kettle trips everyone's computers. Per-room breakers (per-team buckets) trip only where the load is, and the main breaker (org limit) still protects the building. The gateway is the distribution board: if it fails, every room is dark, so it needs redundancy more than any single circuit.

**Common failure.** Students allocate team quotas that sum to exactly the organisation limit and then see false 429s in quiet teams. Discuss over-allocation (109% here) and why it is safe only while the org-level 429 rate is watched.

**Expected exercise outcome.** Both simulations reproduced; halving the CI key raises only the CI line's 429s; at `activeShare` 0.55 the larger teams approach their quotas and raising only the org limit does not help (the team buckets bind); an availability calculation where the gateway term dominates; a two-sentence finance note explaining the visible rise in per-developer cost.

**Extension exercise.** Add a `fallback` concept to the simulation mentally: if the primary returns 5xx for 5 minutes, all traffic moves to a route with a lower TPM limit. What happens to the per-team fairness you just designed? Sketch how the gateway should share the fallback's smaller bucket.

**Discussion question.** Should interactive developer keys ever be hard-stopped at budget? Who in your organisation should make that call, and what would make you change it?
