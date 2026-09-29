# Calibration set for task T21

Twenty-four answers to T21 ("why does SqlHelper still exist, and what should I use instead?"), written by hand to cover the failure modes a judge must catch: right and short, right and long, wrong and short, and wrong but long and fluent (recommending EF Core, permitting new `SqlHelper` calls, inventing a ticket or an API).

| File | What it is |
|---|---|
| `answers/C01.txt` … `C24.txt` | the answers |
| `human-labels.csv` | the course author's pass/fail label per answer, with a one-line reason; 12 pass, 12 fail |
| `illustrative-judge-v1.csv` | **illustrative** verdicts in the shape `EvalHarness judge` writes, showing the typical behaviour of the biased prompt `../tasks-v1/graders/judge-v1.md` |
| `illustrative-judge-v2.csv` | **illustrative** verdicts for the rubric `../tasks-v1/graders/rubric-T21.md` |

The two verdict files are hand-written to make the lesson's pattern visible without an API key. They are not measurements of any model. Produce your own:

```bash
H="dotnet run --project ../tools/EvalHarness --"
$H judge ../tasks-v1/graders/judge-v1.md answers --out my-judge-v1.csv
$H judge ../tasks-v1/graders/rubric-T21.md answers --out my-judge-v2.csv
$H calibrate human-labels.csv my-judge-v1.csv --answers answers
$H calibrate human-labels.csv my-judge-v2.csv --answers answers
```

Label the answers yourself **before** you open `human-labels.csv`, and compare. Where you and the file disagree, you have found the ambiguous parts of the rubric.
