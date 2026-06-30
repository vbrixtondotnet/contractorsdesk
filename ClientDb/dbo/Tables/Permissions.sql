CREATE TABLE [dbo].[Permissions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PermissionType] INT NULL, 
    [Description] NVARCHAR(250) NULL 
)
