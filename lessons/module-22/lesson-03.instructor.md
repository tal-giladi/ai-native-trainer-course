# Instructor notes — 22.3 Templates, kits and assessments as products

**Teaching objective.** Students package their templates, starter pack and assessment as a versioned kit with a manifest, changelog, LICENSE files, a support line, dated stranger tests with Wilson intervals, and a clean deny-list scan; they run item analysis on real answers and act on the flags. `ScaleCheck kit` and `items` clean or with explained warnings.

**Likely confusion.** Versioning documents. Engineers accept SemVer for code but not for templates; the key move is defining the kit's public API as "what buyers have filled in or checked out". Second: discrimination vs difficulty. A 50% item is not automatically good; it must also separate stronger from weaker learners.

**Common misconception.** "An item everyone gets right is a good item; it shows the teaching worked." On a post-test it may be fine as a confidence builder, but it measures nothing and should not be counted as evidence of learning. Second: "Certification is just a word." It is a claim about people that employers may rely on; the Standards treat it as needing its own evidence.

**Key analogy.** A NuGet package. Nobody would ship one without a version, a licence file, release notes and a test run against the current SDK; a kit sold to companies deserves the same.

**Common failure in the exercise.** The deny-list scan finds an employer's name in a template's example, left from the first time the student used it at work. Treat it as a success of the check, not a failure of the student, and use it to stress 20.5. Second: answer files too small for item analysis; accept hints, not decisions. Third: changelogs written as commit logs; ask for the buyer's view.

**Expected exercise outcome.** `kit.md` with every item versioned, licensed, verified within 90 days and stranger-tested (or booked); `CHANGELOG.md`; LICENSE files; deny-list scan with zero hits; an item analysis with a recorded decision for each flag; an assessment page stating what a score means.

**Extension exercise.** Write parallel form B for your assessment by replacing each item with one that tests the same objective, administer both to the next group (half A-then-B, half B-then-A), and compare item statistics across forms.

**Discussion question.** A large client wants to fork your starter pack into their internal platform and maintain it themselves. Which version do they get, what does your licence allow, and what do you still owe them when you release 2.0.0?
