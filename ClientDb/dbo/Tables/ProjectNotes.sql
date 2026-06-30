CREATE TABLE [dbo].[ProjectNotes]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectId] UNIQUEIDENTIFIER NULL, 
    [Notes] NVARCHAR(MAX) NULL,
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL, 
    CONSTRAINT [FK_ProjectNotes_QbClasses] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id]),
    CONSTRAINT [FK_ProjectNotes_CreatedByUser] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_ProjectNotes_UpdatedByUser] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id])
)
