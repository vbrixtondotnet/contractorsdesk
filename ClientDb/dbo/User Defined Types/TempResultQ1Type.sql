CREATE TYPE [dbo].[TempResultQ1Type] AS TABLE (
    [Date]              DATE            NULL,
    [Type]              NVARCHAR (255)  NULL,
    [TxnID]             NVARCHAR (255)  NULL,
    [RefNo]             NVARCHAR (255)  NULL,
    [Payee]             NVARCHAR (255)  NULL,
    [Memo]              NVARCHAR (MAX)  NULL,
    [Account/Category]  NVARCHAR (255)  NULL,
    [BankingAccount]    NVARCHAR (255)  NULL,
    [Class]             NVARCHAR (255)  NULL,
    [Division/Location] NVARCHAR (255)  NULL,
    [Amount]            DECIMAL (21, 2) NULL);

