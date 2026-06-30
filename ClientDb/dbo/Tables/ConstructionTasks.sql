CREATE TABLE [dbo].[ConstructionTasks] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (MAX)   NOT NULL,
    [Description]  NVARCHAR (MAX)   NULL,
    [Sequence]     INT              NOT NULL,
    [ParentTaskID] UNIQUEIDENTIFIER NULL,
    [Duration]     INT              NOT NULL,
    [Created]      DATETIME2 (7)    NULL,
    [CreatedBy]    NVARCHAR (MAX)   NULL,
    [Updated]      DATETIME2 (7)    NULL,
    [UpdatedBy]    NVARCHAR (MAX)   NULL,
    [QBClassID]    UNIQUEIDENTIFIER NULL,
    [Pred1ID]      UNIQUEIDENTIFIER NULL,
    [Pred1Lag]     INT              DEFAULT ((0)) NOT NULL,
    [Pred2ID]      UNIQUEIDENTIFIER NULL,
    [Pred2Lag]     INT              NULL,
    [Pred3ID]      UNIQUEIDENTIFIER NULL,
    [Pred3Lag]     INT              NULL,
    CONSTRAINT [PK_ConstructionTasks] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ConstructionTasks_ConstructionTasks_ParentTaskID] FOREIGN KEY ([ParentTaskID]) REFERENCES [dbo].[ConstructionTasks] ([ID]),
    CONSTRAINT [FK_ConstructionTasks_QBClasses_QBClassID] FOREIGN KEY ([QBClassID]) REFERENCES [dbo].[QBClasses] ([ID])
);


GO
ALTER TABLE [dbo].[ConstructionTasks] NOCHECK CONSTRAINT [FK_ConstructionTasks_ConstructionTasks_ParentTaskID];


GO
ALTER TABLE [dbo].[ConstructionTasks] NOCHECK CONSTRAINT [FK_ConstructionTasks_QBClasses_QBClassID];


GO
CREATE NONCLUSTERED INDEX [IX_ConstructionTasks_ParentTaskID]
    ON [dbo].[ConstructionTasks]([ParentTaskID] ASC);


GO
ALTER INDEX [IX_ConstructionTasks_ParentTaskID]
    ON [dbo].[ConstructionTasks] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_ConstructionTasks_QBClassID]
    ON [dbo].[ConstructionTasks]([QBClassID] ASC);


GO
ALTER INDEX [IX_ConstructionTasks_QBClassID]
    ON [dbo].[ConstructionTasks] DISABLE;

