CREATE TYPE [dbo].[BankingAccountType] AS TABLE (
    [Date]           DATE           NULL,
    [TxnID]          NVARCHAR (255) NULL,
    [RefNo]          NVARCHAR (255) NULL,
    [Payee]          NVARCHAR (255) NULL,
    [BankingAccount] NVARCHAR (255) NULL);

