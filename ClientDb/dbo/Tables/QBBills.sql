CREATE TABLE [dbo].[QBBills] (
    [ID]           NVARCHAR (450)  NOT NULL,
    [DueDate]      DATETIME2 (7)   NULL,
    [Balance]      DECIMAL (18, 2) NULL,
    [TotalAmt]     DECIMAL (18, 2) NULL,
    [TxnDate]      DATETIME2 (7)   NULL,
    [TimeCreated]  DATETIME2 (7)   NULL,
    [TimeModified] DATETIME2 (7)   NULL,
    [CreatedBy]    NVARCHAR (MAX)  NULL,
    [UpdatedBy]    NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_QBBills] PRIMARY KEY CLUSTERED ([ID] ASC)
);

