CREATE TABLE [dbo].[Estimates] (
    [ID]                    UNIQUEIDENTIFIER NOT NULL,
    [Amount]                DECIMAL (18, 2)  NULL,
    [EstimateSubCategoryID] UNIQUEIDENTIFIER NULL,
    [QBClassID]             UNIQUEIDENTIFIER NULL,
    [Created]               DATETIME2 (7)    NULL,
    [CreatedBy]             NVARCHAR (MAX)   NULL,
    [ProjectCompletionDate] DATETIME2 (7)    NULL,
    [Updated]               DATETIME2 (7)    NULL,
    [UpdatedBy]             NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Estimates] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Estimates_EstimateCategories_EstimateSubCategoryID] FOREIGN KEY ([EstimateSubCategoryID]) REFERENCES [dbo].[EstimateCategories] ([ID]),
    CONSTRAINT [FK_Estimates_QBClasses_QBClassID] FOREIGN KEY ([QBClassID]) REFERENCES [dbo].[QBClasses] ([ID])
);


GO
ALTER TABLE [dbo].[Estimates] NOCHECK CONSTRAINT [FK_Estimates_EstimateCategories_EstimateSubCategoryID];


GO
ALTER TABLE [dbo].[Estimates] NOCHECK CONSTRAINT [FK_Estimates_QBClasses_QBClassID];


GO
CREATE NONCLUSTERED INDEX [IX_Estimates_EstimateSubCategoryID]
    ON [dbo].[Estimates]([EstimateSubCategoryID] ASC);


GO
ALTER INDEX [IX_Estimates_EstimateSubCategoryID]
    ON [dbo].[Estimates] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_Estimates_QBClassID]
    ON [dbo].[Estimates]([QBClassID] ASC);


GO
ALTER INDEX [IX_Estimates_QBClassID]
    ON [dbo].[Estimates] DISABLE;

