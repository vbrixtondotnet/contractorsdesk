CREATE TABLE [dbo].[UserLogs]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] int NOT NULL, 
    [Url] nvarchar(150) NOT NULL,
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL
)
