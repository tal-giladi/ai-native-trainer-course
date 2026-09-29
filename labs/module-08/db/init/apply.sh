#!/usr/bin/env bash
# Applies Contoso Billing migrations V001..V$MIGRATE_TO to ContosoBilling, records each in dbo.SchemaHistory
# (the fact Flyway keeps in flyway_schema_history), runs init/before-V###.sql and after-V###.sql around a
# migration when present (seed rows, one lab workaround), and creates the
# least-privilege login the MCP server uses: it can read the catalog, not the data.
set -euo pipefail
SQL=(/opt/mssql-tools18/bin/sqlcmd -C -S sql -U sa -P "$MSSQL_SA_PASSWORD" -b)
TO=${MIGRATE_TO:-6}

"${SQL[@]}" -Q "IF DB_ID('ContosoBilling') IS NOT NULL BEGIN ALTER DATABASE ContosoBilling SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ContosoBilling; END; CREATE DATABASE ContosoBilling;"
"${SQL[@]}" -d ContosoBilling -Q "CREATE TABLE dbo.SchemaHistory (Version int NOT NULL PRIMARY KEY, Script nvarchar(200) NOT NULL, AppliedUtc datetimeoffset(0) NOT NULL DEFAULT SYSUTCDATETIME());"

for f in $(ls /lab/migrations/base/V*.sql /lab/migrations/overlay/V*.sql | sort -t/ -k5); do
  name=$(basename "$f")
  v=$((10#$(echo "$name" | sed -E 's/^V([0-9]+)__.*/\1/')))
  [ "$v" -le "$TO" ] || continue
  vv=$(printf '%03d' "$v")
  if [ -f "/lab/init/before-V$vv.sql" ]; then "${SQL[@]}" -d ContosoBilling -i "/lab/init/before-V$vv.sql"; fi
  echo "applying $name"
  "${SQL[@]}" -d ContosoBilling -i "$f"
  "${SQL[@]}" -d ContosoBilling -Q "INSERT dbo.SchemaHistory (Version, Script) VALUES ($v, N'$name');"
  if [ -f "/lab/init/after-V$vv.sql" ]; then "${SQL[@]}" -d ContosoBilling -i "/lab/init/after-V$vv.sql"; fi
done

"${SQL[@]}" -Q "IF SUSER_ID('schema_reader') IS NULL CREATE LOGIN schema_reader WITH PASSWORD = '$SCHEMA_READER_PASSWORD', CHECK_POLICY = OFF;"
"${SQL[@]}" -d ContosoBilling -Q "CREATE USER schema_reader FOR LOGIN schema_reader; GRANT VIEW DEFINITION TO schema_reader; GRANT SELECT ON dbo.SchemaHistory TO schema_reader;"
echo "ContosoBilling is at V$(printf '%03d' "$TO"). schema_reader: VIEW DEFINITION + SELECT on dbo.SchemaHistory only."
