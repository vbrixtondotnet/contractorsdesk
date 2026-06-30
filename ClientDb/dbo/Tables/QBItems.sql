CREATE TABLE [dbo].[QBItems] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [ListID]           NVARCHAR (MAX)   NULL,
    [Name]             NVARCHAR (MAX)   NULL,
    [FullName]         NVARCHAR (MAX)   NULL,
    [ExpenseAccountID] NVARCHAR (MAX)   NULL,
    [TaxCode]          NVARCHAR (MAX)   NULL,
    [TaxRate]          NVARCHAR (MAX)   NULL,
    [IsActive]         BIT              NULL,
    [Type]             NVARCHAR (MAX)   NULL,
    [TimeCreated]      DATETIME2 (7)    NULL,
    [TimeModified]     DATETIME2 (7)    NULL,
    [CreatedBy]        NVARCHAR (MAX)   NULL,
    [UpdatedBy]        NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_QBItems] PRIMARY KEY CLUSTERED ([ID] ASC)
);

