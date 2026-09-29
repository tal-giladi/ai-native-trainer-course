-- 2025-02-03. BILL-88: every issued invoice has a due date (backfilled to IssuedUtc + 30 days).
UPDATE dbo.Invoice SET DueUtc = DATEADD(day, 30, IssuedUtc) WHERE DueUtc IS NULL;
ALTER TABLE dbo.Invoice ALTER COLUMN DueUtc datetimeoffset(0) NOT NULL;
