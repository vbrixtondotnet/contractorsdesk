CREATE TABLE [dbo].[CostRevisions]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[ProjectId] uniqueidentifier NOT NULL,
	[ActionItemId] INT NULL,
	[RevisionNumber] int NOT NULL,
	[RevisionDate] datetime2(7) NOT NULL,
	[StatusId] int NOT NULL DEFAULT(1),
	[DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL, 
    CONSTRAINT [FK_CostRevisions_QbClasses] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id])
)
