-- Undo for V006. Restores the column and the latest note per invoice; authors and older notes are lost.
DROP PROCEDURE dbo.usp_InvoiceNote_LatestByCustomer;
GO
ALTER TABLE dbo.Invoice ADD Notes nvarchar(400) NULL;
GO
UPDATE i SET Notes = n.Body
FROM dbo.Invoice AS i
CROSS APPLY (SELECT TOP (1) x.Body FROM dbo.InvoiceNote AS x WHERE x.InvoiceId = i.InvoiceId ORDER BY x.CreatedUtc DESC) AS n;
DROP TABLE dbo.InvoiceNote;
