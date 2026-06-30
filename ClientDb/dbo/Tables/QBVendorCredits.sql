CREATE TABLE [dbo].[QBVendorCredits] (
    [ID]            NVARCHAR (450)  NOT NULL,
    [VendorName]    NVARCHAR (MAX)  NULL,
    [APAccountName] NVARCHAR (MAX)  NULL,
    [Balance]       DECIMAL (18, 2) NULL,
    [TotalAmt]      DECIMAL (18, 2) NULL,
    [TxnDate]       DATETIME2 (7)   NULL,
    [TimeCreated]   DATETIME2 (7)   NULL,
    [TimeModified]  DATETIME2 (7)   NULL,
    [CreatedBy]     NVARCHAR (MAX)  NULL,
    [UpdatedBy]     NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_QBVendorCredits] PRIMARY KEY CLUSTERED ([ID] ASC)
);

