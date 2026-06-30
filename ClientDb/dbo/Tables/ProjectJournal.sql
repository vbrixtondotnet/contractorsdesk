CREATE TABLE [dbo].[ProjectJournal]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectId] UNIQUEIDENTIFIER NOT NULL, 
    [CurrentWeek] INT NULL, 
    [Journal] NVARCHAR(MAX) NOT NULL,
    [DateCreated] DATETIME2 NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UpdatedBy] INT NULL, 
    CONSTRAINT [FK_ProjectJournal_QbClasses] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id])
)
