CREATE TABLE [dbo].[EstimateHistories] (
    [ID]         UNIQUEIDENTIFIER NOT NULL,
    [Amount]     DECIMAL (18, 2)  NULL,
    [EstimateID] UNIQUEIDENTIFIER NULL,
    [Created]    DATETIME2 (7)    NULL,
    [CreatedBy]  NVARCHAR (MAX)   NULL,
    [Updated]    DATETIME2 (7)    NULL,
    [UpdatedBy]  NVARCHAR (MAX)   NULL,
    [QBClassID]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_EstimateHistories] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EstimateHistories_Estimates_EstimateID] FOREIGN KEY ([EstimateID]) REFERENCES [dbo].[Estimates] ([ID]),
    CONSTRAINT [FK_EstimateHistories_QBClasses_QBClassID] FOREIGN KEY ([QBClassID]) REFERENCES [dbo].[QBClasses] ([ID])
);


GO
ALTER TABLE [dbo].[EstimateHistories] NOCHECK CONSTRAINT [FK_EstimateHistories_Estimates_EstimateID];


GO
ALTER TABLE [dbo].[EstimateHistories] NOCHECK CONSTRAINT [FK_EstimateHistories_QBClasses_QBClassID];


GO
CREATE NONCLUSTERED INDEX [IX_EstimateHistories_EstimateID]
    ON [dbo].[EstimateHistories]([EstimateID] ASC);


GO
ALTER INDEX [IX_EstimateHistories_EstimateID]
    ON [dbo].[EstimateHistories] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_EstimateHistories_QBClassID]
    ON [dbo].[EstimateHistories]([QBClassID] ASC);


GO
ALTER INDEX [IX_EstimateHistories_QBClassID]
    ON [dbo].[EstimateHistories] DISABLE;

