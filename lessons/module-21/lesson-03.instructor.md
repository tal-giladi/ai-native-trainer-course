# Instructor notes — 21.3 Architecture and build with the team

**Teaching objective.** Students build the AI layer with the client team: decisions recommended by the consultant and taken by client people, a fading scaffold from pairing to review to stepping back, and two checks from the PR log (client-authored share by thirds, client truck factor) that tell them whether the team will own what was built.

**Likely confusion.** Why the demo's one-author layer (Module 15) was right and the same thing is wrong here: a demo's audience watches; a client's team must change it next month. Second: the truck factor counts client people only; including the consultant always inflates it.

**Common misconception.** "The fastest way to help is to build it and explain it." Explanations produce awareness; authorship produces the ability to change. Second: "Code review is enough transfer." Bacchelli and Bird found knowledge transfer is a real outcome of review, but reviewing is not the same as having written and debugged the thing.

**Key analogy.** Teaching someone to drive. You drive the first lap and talk; then they drive and you talk; then you sit in the back. Nobody learns to drive by watching you drive for twelve weeks, however well you drive.

**Common failure in the exercise.** Logging pairing sessions where the consultant typed as client-authored. Writing ADRs with the decision already made ("We will use snapshot mode") and asking for a signature. In the break exercise, trusting the truck factor after someone leaves, when the log still credits them.

**Expected exercise outcome.** A decisions table with four client deciders and records; a PR log whose last third is at least 70% client-authored, with at least two client authors and a truck factor of at least 2; `EngageCheck build` clean. Break exercise: 67% in the last third (fails), one orphan file; pairing removes the orphan, only Dana authoring passes the share gate.

**Extension exercise.** Compute the truck factor of your own team's AI layer (or any config area: CI, infrastructure) from `git log --name-only`, counting only current employees. Who is the single point of failure, and what is the next change they should pair on?

**Discussion question.** A client says: "We are paying for your expertise; why are our developers typing?" How do you answer, and what in the SOW supports you?
