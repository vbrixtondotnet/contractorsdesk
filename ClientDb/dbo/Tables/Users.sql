CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(150) NOT NULL, 
    [Phone] NVARCHAR(150) NULL, 
    [AvatarUrl] NVARCHAR(200) NULL, 
    [Password] NVARCHAR(250) NOT NULL, 
    [RoleId] INT NOT NULL, 
    [RequireLogin] BIT NOT NULL DEFAULT(0),
    [CreatedBy] INT NOT NULL, 
    [UpdatedBy] INT NULL, 
    [DateCreated] DATETIME2 NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT(0), 
    [Status] INT NOT NULL DEFAULT (0), 
    [ConfirmationCode] VARCHAR(50) NULL, 
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
)
