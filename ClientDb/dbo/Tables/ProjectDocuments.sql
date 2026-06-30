CREATE TABLE [dbo].[ProjectDocuments]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectId] UNIQUEIDENTIFIER NOT NULL, 
    [DocumentTypeId] int NOT NULL,
    [FileUrl] NVARCHAR(250) NOT NULL, 
    [DateAdded] DATETIME NOT NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_ProjectDocuments_ToTable] FOREIGN KEY ([ProjectId]) REFERENCES [QBClasses]([Id])
)
