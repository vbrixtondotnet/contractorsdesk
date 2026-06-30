CREATE TABLE [dbo].[QBTransactions] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [AccountID]       UNIQUEIDENTIFIER NULL,
    [Amount]          DECIMAL (18, 2)  NULL,
    [VendorID]        UNIQUEIDENTIFIER NULL,
    [ClassID]         UNIQUEIDENTIFIER NULL,
    [CustomerID]      UNIQUEIDENTIFIER NULL,
    [TxnID]           NVARCHAR (MAX)   NULL,
    [TxnNumber]       NVARCHAR (MAX)   NULL,
    [TxnType]         NVARCHAR (MAX)   NULL,
    [Memo]            NVARCHAR (MAX)   NULL,
    [Name]            NVARCHAR (MAX)   NULL,
    [Currency]        NVARCHAR (MAX)   NULL,
    [Status]          NVARCHAR (MAX)   NULL,
    [IsCleared]       NVARCHAR (MAX)   NULL,
    [ExchangeRate]    DECIMAL (18, 6)  NULL,
    [TransactionDate] DATETIME2 (7)    NULL,
    [Created]         DATETIME2 (7)    NULL,
    [Updated]         DATETIME2 (7)    NULL,
    [CreatedBy]       NVARCHAR (MAX)   NULL,
    [UpdatedBy]       NVARCHAR (MAX)   NULL,
    [SplitAccountID]  UNIQUEIDENTIFIER NULL,
    [Location]        NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_QBTransactions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_QBTransactions_QBAccounts_AccountID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[QBAccounts] ([ID]),
    CONSTRAINT [FK_QBTransactions_QBClasses_ClassID] FOREIGN KEY ([ClassID]) REFERENCES [dbo].[QBClasses] ([ID]),
    CONSTRAINT [FK_QBTransactions_QBCustomers_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[QBCustomers] ([ID]),
    CONSTRAINT [FK_QBTransactions_QBVendors_VendorID] FOREIGN KEY ([VendorID]) REFERENCES [dbo].[QBVendors] ([ID])
);


GO
ALTER TABLE [dbo].[QBTransactions] NOCHECK CONSTRAINT [FK_QBTransactions_QBAccounts_AccountID];


GO
ALTER TABLE [dbo].[QBTransactions] NOCHECK CONSTRAINT [FK_QBTransactions_QBClasses_ClassID];


GO
ALTER TABLE [dbo].[QBTransactions] NOCHECK CONSTRAINT [FK_QBTransactions_QBCustomers_CustomerID];


GO
ALTER TABLE [dbo].[QBTransactions] NOCHECK CONSTRAINT [FK_QBTransactions_QBVendors_VendorID];


GO
CREATE NONCLUSTERED INDEX [IX_QBTransactions_AccountID]
    ON [dbo].[QBTransactions]([AccountID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_QBTransactions_ClassID]
    ON [dbo].[QBTransactions]([ClassID] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_QBTransactions_CustomerID]
    ON [dbo].[QBTransactions]([CustomerID] ASC);


GO
ALTER INDEX [IX_QBTransactions_CustomerID]
    ON [dbo].[QBTransactions] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_QBTransactions_VendorID]
    ON [dbo].[QBTransactions]([VendorID] ASC);


GO
ALTER INDEX [IX_QBTransactions_VendorID]
    ON [dbo].[QBTransactions] DISABLE;
