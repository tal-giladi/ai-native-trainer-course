-- 2019-11-18. Added due date. Still datetime (local server time) at this point.
ALTER TABLE dbo.Invoice ADD DueUtc datetime NULL;
