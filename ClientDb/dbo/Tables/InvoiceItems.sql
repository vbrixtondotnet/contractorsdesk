CREATE TABLE [dbo].[InvoiceItems]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[InvoiceId] UNIQUEIDENTIFIER NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Quantity] INT NOT NULL,
	[Rate] DECIMAL(18, 2) NOT NULL,
	[Amount] DECIMAL(18, 2) NOT NULL,
	[Sequence] INT NOT NULL,
	[DateCreated] DATETIME NOT NULL DEFAULT GETDATE(),
	[DateUpdated] DATETIME NULL,
	[CreatedBy] INT NOT NULL,
	[UpdatedBy] INT NULL, 
    CONSTRAINT [FK_InvoiceItems_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices]([Id])
)
