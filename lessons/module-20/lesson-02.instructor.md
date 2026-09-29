# Instructor notes — 20.2 Pricing with uncertainty

**Teaching objective.** Students price each rung between a cost floor and a share of the client's value range, compute that range from their own measured interval (with defect cost, adoption, tool spend and transfer widening), read corners and a Monte Carlo interval, and report ROI as an interval with the probability of loss. When one team's value straddles zero, they price the pilot as a measurement and say so.

**Likely confusion.** The difference between the effect interval and the value interval. The effect interval (−25% to −5%) is from the experiment; the value interval also carries the client's uncertain inputs and the defect cost, so it is much wider and can include losses even when the effect interval excludes zero. Second: why the worksheet widens the ratio past 1.0 when the experiment's interval did not reach it. Transfer: another team, another codebase.

**Common misconception.** "A range will lose the sale; buyers want one number." Buyers want a number they can defend to their CFO. A range with a stated chance of loss and a cheap way to find out (the pilot) is easier to defend than a vendor's single figure that the first skeptic dismantles. Second: "Monte Carlo is more accurate." It is a different summary (a coverage interval under stated assumptions, including independence), not more truth; corners are a bound that is easier to explain.

**Key analogy.** Capacity planning with p95 latency. Nobody sizes a system on the mean request; you look at the distribution and the tail. The worksheet does the same for money: the median is not the plan, the tail is the risk.

**Common failure in the exercise.** Entering the client's numbers as points because "that's what they told me"; ask what range they would bet on. Omitting the defect row because the student's own guardrail passed; keep it with its interval, even if it includes zero. Tuning inputs until the median covers the fee; ask students to record the first run's numbers before any change and justify every change with a source.

**Expected exercise outcome.** A floor computation, three dated market references, one worksheet for the real pitch target, `price` clean, and two sentences for the proposal: the monthly value range and the probability of loss. For many students the pilot will not pay back on one team; the expected response is to reframe it as a measurement, not to change inputs.

**Extension exercise.** Add a correlation to the worksheet by hand: assume ticket volume and baseline hours move in opposite directions (more tickets, smaller tickets). Rerun with narrower independent ranges that mimic it and compare the 90% interval. Discuss what the independence assumption does to the tails.

**Discussion question.** Your employer wants to "buy" your pilot internally with no money changing hands. What does pricing mean then, and which of the three numbers (floor, market reference, value) still matters?
