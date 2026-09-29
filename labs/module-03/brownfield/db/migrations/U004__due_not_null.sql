-- Undo for V004. The backfilled values are kept; only nullability is restored.
ALTER TABLE dbo.Invoice ALTER COLUMN DueUtc datetimeoffset(0) NULL;
