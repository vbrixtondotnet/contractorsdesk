CREATE TABLE [dbo].[QuickBooksToken]
(
	[Id] UNIQUEIDENTIFIER NOT NULL, 
    [RealmId] NVARCHAR(200) NOT NULL, 
    [AccessToken ] NVARCHAR(MAX) NOT NULL, 
    [RefreshToken] NVARCHAR(MAX) NOT NULL, 
    [ExpiryTime ] DATETIME NOT NULL,
    CONSTRAINT [PK_QuickBooksToken] PRIMARY KEY CLUSTERED ([Id] ASC)
)
