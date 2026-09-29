# Illustrative runs (not measurements)

`illustrative-before/` and `illustrative-after/` hold **hand-written** answers in the shape of
`claude -p --output-format json`, one run per task. They exist so you can practise `ContextLab grade`
and `ContextLab report` without an API key. The token and cost fields are invented to look plausible;
**never quote them as results.** Your own runs are the only numbers that count.

```bash
dotnet run --project ../tools/ContextLab -- grade ../tasks-v0/tasks.json illustrative-before --label before --rules-tokens 7028 --out demo.csv
dotnet run --project ../tools/ContextLab -- grade ../tasks-v0/tasks.json illustrative-after --label after --rules-tokens 531 --out demo.csv
dotnet run --project ../tools/ContextLab -- report demo.csv
```

Exercise: the "after" set has two failures. One is a real context failure and one is a grader false
negative. Read both answers and decide which is which before you change anything.
