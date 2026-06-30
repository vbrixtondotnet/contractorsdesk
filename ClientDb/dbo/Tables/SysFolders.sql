CREATE TABLE [dbo].[SysFolders]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()), 
    [Name] NVARCHAR(50) NOT NULL, 
    [ParentId] UNIQUEIDENTIFIER NULL, 
    [Sequence] int NOT NULL, 
    [DateCreated] DATETIME NOT NULL DEFAULT (GETDATE()), 
    [CreatedBy] INT NOT NULL DEFAULT (1), 
    [DateUpdated] DATETIME NULL, 
    [UpdatedBy] INT NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT (0)
)
