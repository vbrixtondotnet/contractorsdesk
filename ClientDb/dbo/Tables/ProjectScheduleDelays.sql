CREATE TABLE [dbo].[ProjectScheduleDelays]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectScheduleId] UNIQUEIDENTIFIER NOT NULL,
    [TaskId] UNIQUEIDENTIFIER NULL,
    [Start] DATE NOT NULL, 
    [Reason] NVARCHAR(150) NOT NULL, 
    [Description] NVARCHAR(250) NULL, 
    [Days] INT NOT NULL, 
    [ApplyToOtherProjects] BIT NULL DEFAULT 0, 
    [CreatedBy] INT NOT NULL, 
    [UpdatedBy] INT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL, 
    CONSTRAINT [FK_ProjectScheduleDelays_ToTable] FOREIGN KEY ([ProjectScheduleId]) REFERENCES [ProjectSchedules]([Id]), 
    CONSTRAINT [FK_ProjectScheduleDelays_ProjectScheduleTasks] FOREIGN KEY ([TaskId]) REFERENCES [ProjectScheduleTasks]([Id])
)
