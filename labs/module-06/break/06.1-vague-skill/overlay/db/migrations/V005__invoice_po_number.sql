-- BILL-153: purchase-order number on invoices.
ALTER TABLE dbo.Invoice ADD PoNumber nvarchar(35) NOT NULL;
