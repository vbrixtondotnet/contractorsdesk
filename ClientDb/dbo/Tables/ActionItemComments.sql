CREATE TABLE [dbo].[ActionItemComments]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ActionItemId] INT NULL, 
    [Comment] NVARCHAR(MAX) NULL,
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL, 
    CONSTRAINT [FK_ActionItemComments_ActionItems] FOREIGN KEY ([ActionItemId]) REFERENCES [ActionItems]([Id]),
    CONSTRAINT [FK_ActionItemComments_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id])
)
