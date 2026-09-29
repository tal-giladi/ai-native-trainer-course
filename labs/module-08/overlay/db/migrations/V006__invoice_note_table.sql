-- 2026-09-14. BILL-158: one note per invoice was not enough, and nobody knew who wrote it.
-- Notes move to dbo.InvoiceNote (many per invoice, with author and time). Existing notes are
-- copied with author 'migrated' and the invoice's issue time. dbo.Invoice.Notes is dropped.
CREATE TABLE dbo.InvoiceNote
(
    InvoiceNoteId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_InvoiceNote PRIMARY KEY,
    InvoiceId     bigint            NOT NULL CONSTRAINT FK_InvoiceNote_Invoice REFERENCES dbo.Invoice (InvoiceId),
    AuthorUpn     nvarchar(256)     NOT NULL,
    Body          nvarchar(400)     NOT NULL,
    CreatedUtc    datetimeoffset(0) NOT NULL
);
CREATE INDEX IX_InvoiceNote_InvoiceId_CreatedUtc ON dbo.InvoiceNote (InvoiceId, CreatedUtc DESC) INCLUDE (Body, AuthorUpn);
GO
INSERT INTO dbo.InvoiceNote (InvoiceId, AuthorUpn, Body, CreatedUtc)
SELECT InvoiceId, N'migrated', Notes, IssuedUtc FROM dbo.Invoice WHERE Notes IS NOT NULL;
ALTER TABLE dbo.Invoice DROP COLUMN Notes;
GO
-- The latest note of each of a customer's issued invoices (collections view).
CREATE PROCEDURE dbo.usp_InvoiceNote_LatestByCustomer
    @customerId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT i.InvoiceId, n.Body, n.AuthorUpn, n.CreatedUtc
    FROM dbo.Invoice AS i
    CROSS APPLY (SELECT TOP (1) x.Body, x.AuthorUpn, x.CreatedUtc
                 FROM dbo.InvoiceNote AS x
                 WHERE x.InvoiceId = i.InvoiceId
                 ORDER BY x.CreatedUtc DESC) AS n
    WHERE i.CustomerId = @customerId AND i.Status = 1;
END;
