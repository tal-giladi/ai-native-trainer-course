-- Lab workaround. V004 (labs/module-03) was written without running it: SQL Server refuses to
-- ALTER COLUMN DueUtc while IX_Invoice_CustomerId_Status INCLUDEs it (Msg 5074). The lab drops the
-- index before V004 and after-V004.sql recreates it. A real fix would be a reviewed change to V004
-- before it ever reached production; here it shows why migrations belong in CI against a real engine.
DROP INDEX IX_Invoice_CustomerId_Status ON dbo.Invoice;
