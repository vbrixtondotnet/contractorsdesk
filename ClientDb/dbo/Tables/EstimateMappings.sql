CREATE TABLE [dbo].[EstimateMappings] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [AccountType]           NVARCHAR (MAX)   NULL,
    [AccountSubType]        NVARCHAR (MAX)   NULL,
    [Description]           NVARCHAR (MAX)   NULL,
    [QBAccountID]           UNIQUEIDENTIFIER NULL,
    [EstimateSubCategoryID] UNIQUEIDENTIFIER NULL,
    [Created]               DATETIME2 (7)    NULL,
    [CreatedBy]             NVARCHAR (MAX)   NULL,
    [QBClassID]             UNIQUEIDENTIFIER NULL,
    [Updated]               DATETIME2 (7)    NULL,
    [UpdatedBy]             NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_EstimateMappings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EstimateMappings_EstimateCategories_EstimateSubCategoryID] FOREIGN KEY ([EstimateSubCategoryID]) REFERENCES [dbo].[EstimateCategories] ([ID]),
    CONSTRAINT [FK_EstimateMappings_QBAccounts_QBAccountID] FOREIGN KEY ([QBAccountID]) REFERENCES [dbo].[QBAccounts] ([ID]),
    CONSTRAINT [FK_EstimateMappings_QBClasses_QBClassID] FOREIGN KEY ([QBClassID]) REFERENCES [dbo].[QBClasses] ([ID])
);


GO
ALTER TABLE [dbo].[EstimateMappings] NOCHECK CONSTRAINT [FK_EstimateMappings_EstimateCategories_EstimateSubCategoryID];


GO
ALTER TABLE [dbo].[EstimateMappings] NOCHECK CONSTRAINT [FK_EstimateMappings_QBAccounts_QBAccountID];


GO
ALTER TABLE [dbo].[EstimateMappings] NOCHECK CONSTRAINT [FK_EstimateMappings_QBClasses_QBClassID];


GO
CREATE NONCLUSTERED INDEX [IX_EstimateMappings_EstimateSubCategoryID]
    ON [dbo].[EstimateMappings]([EstimateSubCategoryID] ASC);


GO
ALTER INDEX [IX_EstimateMappings_EstimateSubCategoryID]
    ON [dbo].[EstimateMappings] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_EstimateMappings_QBAccountID]
    ON [dbo].[EstimateMappings]([QBAccountID] ASC);


GO
ALTER INDEX [IX_EstimateMappings_QBAccountID]
    ON [dbo].[EstimateMappings] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_EstimateMappings_QBClassID]
    ON [dbo].[EstimateMappings]([QBClassID] ASC);


GO
ALTER INDEX [IX_EstimateMappings_QBClassID]
    ON [dbo].[EstimateMappings] DISABLE;

