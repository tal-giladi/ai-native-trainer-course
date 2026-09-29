# Instructor notes — 07.5 Comparisons and paired designs

**Teaching objective.** Students run comparisons in which only the treatment differs (pinned agent, model and task set, recorded in a manifest), pair by task, interleave, check the instrument with an A/A run, and write the Module 4 re-score as a report with an interval and limitations.

**Likely confusion.** "Paired" vs "same number of trials". Pairing means each task is compared with itself across arms. Two arms with 120 trials each on different task lists are not paired, however equal the counts.

**Common misconception.** "The interval excludes zero, so the result is solid." Intervals quantify sampling noise only. The `reduced-later` break has an interval well clear of zero and is still uninterpretable, because two things changed. Warnings first, intervals second.

**Key analogy.** Testing two tyres by putting one on a race car and the other on a delivery van. Pairing is putting both tyres on the same car, on the same track, on the same afternoon; A/A is running the same tyre twice to see how much the stopwatch varies.

**Common failure in the exercise.** Students forget to pin the agent and an auto-update lands mid-experiment; the manifest warning on resume catches it only if they read the output. Second: they use `--bare` for "clean" runs and are surprised when the layer makes no difference — bare mode does not load `CLAUDE.md`. Third: stopping and restarting runs out of order, so one arm finishes a day later. Encourage the interleaved loop.

**Expected exercise outcome.** An A/A interval that includes 0 (typically about ±10 points on 24 tasks × 2 rounds); an A/B with no warnings; a report with the paired difference, dev and holdout separately, cost per passing trial, and at least three limitations. On real runs, most students see the reduced layer hold or improve with lower cost per passing trial; if the reduced layer loses on one task, they should name the fact (usually T09's BILL-97 fact, as in Module 4).

**Extension exercise.** Run a model comparison with the layer fixed: two `--model` values, interleaved, 24 tasks × 3 trials. Report pass rate, cost per passing trial and median latency. Would you switch? At what price ratio would the answer change?

**Discussion question.** A client's platform team says "we upgraded the model last week and the agents got better". What would you need to see before agreeing, and how would you design the check without stopping their work?
