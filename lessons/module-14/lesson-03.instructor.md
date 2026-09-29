# Instructor notes — 14.3 Diagrams, versioning and the evolution policy

**Teaching objective.** Students produce a loop diagram and a before/after diagram that explain rather than decorate, put their method under a version with bump rules and a changelog, write a one-page evolution policy for the method, and start an objections FAQ that is honest about what they do not know.

**Likely confusion.** The two evolution loops. Students conflate 11.5 (the AI layer of one repository: incident → regression task → fix) with this lesson (the method they teach: evidence and objections → review → version → materials). Draw both on the board, one above the other; an incident can feed both.

**Common misconception.** "Versioning a methodology is pretentious." The version exists for other people: anyone who attended a talk or read a post needs to know whether what they learned still holds. Second misconception: "a good diagram is a pretty diagram". The test is whether it can be checked and redrawn, not whether it looks like a consultancy slide.

**Key analogy.** A published API. Your attendees are its clients. Renaming an endpoint without a redirect breaks them silently; retiring it with a deprecation notice lets them move on their schedule.

**Common failure in the exercise.** Loop diagrams with no failure path. Ask "where does the work go when Prove fails?" until an arrow appears. Second: before/after diagrams with only the speed number. Third: FAQs where every answer is confident; ask for the "what we do not know" column to be non-empty in at least one row.

**Expected exercise outcome.** `diagrams/loop.md` and `diagrams/before-after.md` in Mermaid with keys and failure paths; a colleague's successful redraw; `method-v1.md` at 1.0.0 or later with a changelog; a one-page policy naming a reviewer; a FAQ with two or more real objections; one `MethodCheck diff` run on a real change. For the break: the MAJOR requirement and the name reuse found, and a 2.0.0 release that keeps C2's name, retires C4 and names C5 afresh.

**Extension exercise.** Take a well-known public method or framework you respect and reconstruct its version history from its public pages or repository. Where did it make breaking changes without saying so, and what did that do to how people talk about it?

**Discussion question.** A company that bought your workshop last year asks whether their internal training, built on your 1.x loop, is "still correct" now that you are on 2.0. What do you owe them, and what does your policy say?
