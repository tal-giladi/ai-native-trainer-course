# Instructor notes — 12.1 Hosting options

**Teaching objective.** Students can place any hosting option in one of four models by who operates the model and whose account and network the request uses, eliminate options on hard constraints, and compare survivors on cost per successful task including fixed cost and review of failures.

**Likely confusion.** "Hosted in the EU" versus "processed in the EU." A cloud provider can bill you from an EU account and still route inference globally (global profiles and endpoints). Make students name the processing geography of each route, not the account's home region.

**Common misconception.** "Self-hosting is cheaper at scale." Only if the model passes your tasks. Walk through the hint in *Try it*: at a 62% pass rate no volume makes the large self-hosted model beat a 64% hosted one, because the review term does not shrink with volume.

**Key analogy.** Choosing a data centre: you would not pick the cheapest rack in a country your data may not enter, and you would not compare colocation to cloud on electricity price alone. Constraints first, total cost of a working service second.

**Common failure.** Students copy the lab's numbers into their own organisation. Insist that the pass rates come from their own task set (Module 7) and that every geography or feature claim carries an "as of" date and a source link.

**Expected exercise outcome.** A reproduced `ArchCheck hosting` run; a note mapping each eliminated option to a stakeholder's requirement; the sensitivity result that the survivor order is stable for review costs from 5 to 40 (the older model gets closer at 5 but stays behind); a short constraints list for their own context.

**Extension exercise.** Add a seventh option: the vendor API with a hypothetical EU `inference_geo` value at 1.1× price. Where does it land? What would have to be true of its feature set for it to beat the cloud-hosted primary?

**Discussion question.** Your CTO says "no lock-in" is worth a 70% higher cost per success. How do you turn that into a constraint, a weight, or a consequence in the ADR, and who should sign it?
