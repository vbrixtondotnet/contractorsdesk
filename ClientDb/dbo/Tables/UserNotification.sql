CREATE TABLE [dbo].[UserNotification]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(300) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [IsGlobal] BIT NULL, 
    [UserId] INT NULL, 
    [RoleId] INT NULL, 
    [IsRead] BIT NULL, 
    [CreatedById] INT NULL,
    [DateCreated] DATETIME NOT NULL, 
    [RelatedUrl] NVARCHAR(MAX) NULL, 
    [EmailId] uniqueidentifier NULL, 
    [NextActionEnum] NVARCHAR(200) NULL, 
    CONSTRAINT [PK_UserNotification] PRIMARY KEY CLUSTERED ([Id] ASC)
)
