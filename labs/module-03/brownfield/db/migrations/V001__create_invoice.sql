-- 2016-03-02. Original schema. Note: no rollback script existed yet (convention starts at V003).
CREATE TABLE dbo.Invoice
(
    InvoiceId  bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Invoice PRIMARY KEY,
    CustomerId int            NOT NULL,
    Amount     decimal(19,4)  NOT NULL,
    IssuedUtc  datetime       NOT NULL
);
