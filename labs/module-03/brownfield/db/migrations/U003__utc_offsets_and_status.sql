-- Undo for V003.
DROP INDEX IX_Invoice_CustomerId_Status ON dbo.Invoice;
ALTER TABLE dbo.Invoice DROP CONSTRAINT DF_Invoice_Status;
ALTER TABLE dbo.Invoice DROP COLUMN Status;
ALTER TABLE dbo.Invoice ALTER COLUMN DueUtc datetime NULL;
ALTER TABLE dbo.Invoice ALTER COLUMN IssuedUtc datetime NOT NULL;
