CREATE TABLE [dbo].[ConnectionStrings]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [CompanyId] UNIQUEIDENTIFIER NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_ConnectionStrings_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([Id])
)
