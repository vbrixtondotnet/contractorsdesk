CREATE TABLE [dbo].[QBAccounts] (
    [ID]                 UNIQUEIDENTIFIER NOT NULL,
    [ListID]             NVARCHAR (MAX)   NULL,
    [Name]               NVARCHAR (MAX)   NULL,
    [AccountNumber]      NVARCHAR (MAX)   NULL,
    [ParentID]           NVARCHAR (MAX)   NULL,
    [AccountType]        NVARCHAR (MAX)   NULL,
    [TimeCreated]        DATETIME2 (7)    NULL,
    [TimeModified]       DATETIME2 (7)    NULL,
    [CreatedBy]          NVARCHAR (MAX)   NULL,
    [UpdatedBy]          NVARCHAR (MAX)   NULL,
    [FullyQualifiedName] NVARCHAR (MAX)   NULL,
    [IsSubAccount]       BIT              NULL,
    [DetailType]         NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_QBAccounts] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_QBAccounts_ID]
    ON [dbo].[QBAccounts]([ID] ASC);


GO
ALTER INDEX [IX_QBAccounts_ID]
    ON [dbo].[QBAccounts] DISABLE;

