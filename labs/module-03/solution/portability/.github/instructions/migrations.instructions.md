---
applyTo: "db/migrations/**"
---

# Migrations (GitHub Copilot path-specific instructions)

- New migration = `V###__name.sql` plus matching `U###__name.sql` undo script.
- Never edit a merged `V###` script; add a new one.
- Time columns are `datetimeoffset(0)` in UTC; money is `decimal(19,4)`.
