CREATE TABLE [dbo].[ProjectThresholds]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectId] UNIQUEIDENTIFIER NOT NULL, 
    [Threshold] DECIMAL(18, 2) NOT NULL, 
    [DateUpdated] DATETIME NOT NULL, 
    CONSTRAINT [FK_ProjectThresholds_QbClass] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id])
)
