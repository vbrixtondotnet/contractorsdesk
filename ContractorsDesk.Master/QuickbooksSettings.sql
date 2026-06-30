CREATE TABLE [dbo].[QuickbooksSettings]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [CompanyId] UNIQUEIDENTIFIER NULL,
    [RealmId] NVARCHAR(200) NOT NULL, 
    [AccessToken ] NVARCHAR(MAX) NOT NULL, 
    [RefreshToken] NVARCHAR(MAX) NOT NULL, 
    [ExpiryTime ] DATETIME NOT NULL,
    CONSTRAINT [FK_QuickBooksToken_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([Id])
)
