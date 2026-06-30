CREATE TABLE [dbo].[ActionItemScheduleChange]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ActionItemId] INT NULL, 
    [NoOfDays] INT NULL, 
    [ConstructionTaskId] UNIQUEIDENTIFIER NULL, 
    [RequiresClientApproval] BIT NULL, 
    CONSTRAINT [FK_ActionItemScheduleChange_ActionItems] FOREIGN KEY ([ActionItemId]) REFERENCES [ActionItems]([Id]) 
)
