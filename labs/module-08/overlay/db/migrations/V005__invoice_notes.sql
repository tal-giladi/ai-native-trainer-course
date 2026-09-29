-- 2025-06-10. BILL-120: collections agents keep a free-text note on an invoice.
ALTER TABLE dbo.Invoice ADD Notes nvarchar(400) NULL;
