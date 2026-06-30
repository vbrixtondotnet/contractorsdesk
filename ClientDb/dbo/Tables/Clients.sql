CREATE TABLE [dbo].[Clients]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[UserId] INT NULL, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Address] NCHAR(200) NULL,
    [City] NVARCHAR(100) NULL, 
    [State] NVARCHAR(10) NULL, 
    [CompanyName] NVARCHAR(100) NULL, 
    [EmailAddress] NVARCHAR(100) NOT NULL, 
    [SecondaryEmailAddress] NVARCHAR(250) NULL, 
    [Phone] NVARCHAR(50) NULL,
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL, 
    CONSTRAINT [FK_Clients_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
)
