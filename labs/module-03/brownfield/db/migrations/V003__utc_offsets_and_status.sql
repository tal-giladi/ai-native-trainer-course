-- 2024-05-14. BILL-61: move to datetimeoffset (UTC) and add Status.
-- From this migration on, every V### script ships with a matching U### undo script,
-- and a PR that adds a V### without its U### is rejected in review.
ALTER TABLE dbo.Invoice ALTER COLUMN IssuedUtc datetimeoffset(0) NOT NULL;
ALTER TABLE dbo.Invoice ALTER COLUMN DueUtc datetimeoffset(0) NULL;
ALTER TABLE dbo.Invoice ADD Status tinyint NOT NULL CONSTRAINT DF_Invoice_Status DEFAULT (1);
CREATE INDEX IX_Invoice_CustomerId_Status ON dbo.Invoice (CustomerId, Status) INCLUDE (Amount, DueUtc);
