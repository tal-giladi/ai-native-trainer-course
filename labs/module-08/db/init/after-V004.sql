CREATE INDEX IX_Invoice_CustomerId_Status ON dbo.Invoice (CustomerId, Status) INCLUDE (Amount, DueUtc);
-- Fictional rows so the schema has something in it. Status: 1 Issued, 2 Paid.
INSERT dbo.Invoice (CustomerId, Amount, IssuedUtc, DueUtc, Status) VALUES
 (1001, 1200.0000, '2026-06-01T08:00:00+00:00', '2026-07-01T08:00:00+00:00', 1),
 (1001,  450.5000, '2026-07-15T08:00:00+00:00', '2026-08-14T08:00:00+00:00', 1),
 (1002,  980.0000, '2026-05-10T08:00:00+00:00', '2026-06-09T08:00:00+00:00', 2);
