-- 2026-09-28. BILL-150: record payment reminders. New table; dbo.Invoice is unchanged.
CREATE TABLE dbo.InvoiceReminder
(
    InvoiceReminderId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_InvoiceReminder PRIMARY KEY,
    InvoiceId         bigint            NOT NULL CONSTRAINT FK_InvoiceReminder_Invoice REFERENCES dbo.Invoice (InvoiceId),
    SentUtc           datetimeoffset(0) NOT NULL
);
CREATE INDEX IX_InvoiceReminder_InvoiceId_SentUtc ON dbo.InvoiceReminder (InvoiceId, SentUtc);
