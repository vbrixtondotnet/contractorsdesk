CREATE TABLE [dbo].[ScheduleRevisionItems]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[ScheduleRevisionId] uniqueidentifier NOT NULL,
	[Reason] nvarchar(100) NOT NULL,
	[Description] nvarchar(500) NULL,
	[ConstructionTaskId] uniqueidentifier NOT NULL,
	[OldDuration] int NOT NULL,
	[NewDuration] int NOT NULL,
	[OldStartDate] date NOT NULL,
	[NewStartDate] date NOT NULL,
	[OldEndDate] date NOT NULL,
	[NewEndDate] date NOT NULL, 
    CONSTRAINT [FK_ScheduleRevisionItems_ScheduleRevisions] FOREIGN KEY ([ScheduleRevisionId]) REFERENCES [ScheduleRevisions]([Id])
)
