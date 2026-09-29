# Illustrative trigger runs

These two files show the **format** `scripts/run-triggers.sh` writes and `SkillCheck triggers` reads, with plausible numbers for the lesson 06.1 break. They are **illustrative, not measurements**: produce your own with your agent, your model and your repository, and expect different numbers.

| File | Skill | Queries | Runs |
|---|---|---|---|
| `triggers-db-helper-v0.tsv` | the vague v0 in `break/06.1-vague-skill/db-helper` | `tests/triggers/new-migration.tsv` | 10 queries x 3 |
| `triggers-new-migration-v1.tsv` | `layer/.claude/skills/new-migration` 1.0.0 | same | 10 queries x 3 |

```bash
dotnet run --project tools/SkillCheck -- triggers samples/triggers-db-helper-v0.tsv       # recall 0.20, false-trigger 0.13 -> FAIL
dotnet run --project tools/SkillCheck -- triggers samples/triggers-new-migration-v1.tsv   # recall 0.93, false-trigger 0.07 -> PASS
```
