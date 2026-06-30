CREATE TABLE [dbo].[UserBookmarks]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] INT NOT NULL, 
    [Title] NVARCHAR(150) NOT NULL, 
    [Description] NVARCHAR(250) NULL, 
    [Url] NVARCHAR(150) NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [UpdatedBy] INT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
