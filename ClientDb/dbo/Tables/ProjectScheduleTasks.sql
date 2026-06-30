CREATE TABLE [dbo].[ProjectScheduleTasks] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [ProjectScheduleId]  UNIQUEIDENTIFIER NOT NULL,
    [ConstructionTaskId] UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (150)   NOT NULL,
    [Sequence]           INT              NOT NULL,
    [Duration]           INT              NOT NULL,
    [StartDate]          DATE             NOT NULL,
    [EndDate]            DATE             NOT NULL,
    [Pred1]              UNIQUEIDENTIFIER NULL,
    [Lag1]               INT              NULL,
    [Pred2]              UNIQUEIDENTIFIER NULL,
    [Lag2]               INT              NULL,
    [Pred3]              UNIQUEIDENTIFIER NULL,
    [Lag3]               INT              NULL,
    [CreatedBy]          INT              NOT NULL,
    [UpdatedBy]          INT              NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [DateUpdated]        DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProjectScheduleTasks_ToTable] FOREIGN KEY ([ProjectScheduleId]) REFERENCES [dbo].[ProjectSchedules] ([Id])
);

