CREATE TABLE [dbo].[EstimateCategories] (
    [ID]                       UNIQUEIDENTIFIER NOT NULL,
    [Name]                     NVARCHAR (MAX)   DEFAULT (N'') NOT NULL,
    [Sequence]                 INT              DEFAULT ((0)) NOT NULL,
    [ParentEstimateCategoryID] UNIQUEIDENTIFIER NULL,
    [Created]                  DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (MAX)   NULL,
    [Updated]                  DATETIME2 (7)    NULL,
    [UpdatedBy]                NVARCHAR (MAX)   NULL,
    [Description]              NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_EstimateCategories] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EstimateCategories_EstimateCategories_ParentEstimateCategoryID] FOREIGN KEY ([ParentEstimateCategoryID]) REFERENCES [dbo].[EstimateCategories] ([ID])
);


GO
ALTER TABLE [dbo].[EstimateCategories] NOCHECK CONSTRAINT [FK_EstimateCategories_EstimateCategories_ParentEstimateCategoryID];


GO
CREATE NONCLUSTERED INDEX [IX_EstimateCategories_ParentEstimateCategoryID]
    ON [dbo].[EstimateCategories]([ParentEstimateCategoryID] ASC);


GO
ALTER INDEX [IX_EstimateCategories_ParentEstimateCategoryID]
    ON [dbo].[EstimateCategories] DISABLE;

