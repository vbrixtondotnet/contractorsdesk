CREATE TABLE [dbo].[QBCreditMemos] (
    [ID]                    NVARCHAR (450)  NOT NULL,
    [DocNumber]             NVARCHAR (MAX)  NULL,
    [CustomerName]          NVARCHAR (MAX)  NULL,
    [RemainingCredit]       DECIMAL (18, 2) NULL,
    [TotalTax]              DECIMAL (18, 2) NULL,
    [TotalAmt]              DECIMAL (18, 2) NULL,
    [ApplyTaxAfterDiscount] BIT             NULL,
    [Balance]               DECIMAL (18, 2) NULL,
    [Currency]              NVARCHAR (MAX)  NULL,
    [TxnDate]               DATETIME2 (7)   NULL,
    [DueDate]               DATETIME2 (7)   NULL,
    [TimeCreated]           DATETIME2 (7)   NULL,
    [TimeModified]          DATETIME2 (7)   NULL,
    [CreatedBy]             NVARCHAR (MAX)  NULL,
    [UpdatedBy]             NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_QBCreditMemos] PRIMARY KEY CLUSTERED ([ID] ASC)
);

