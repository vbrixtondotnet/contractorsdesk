CREATE TABLE [dbo].[EmailAttachments]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EmailId] UNIQUEIDENTIFIER NOT NULL, 
    [FileName] NVARCHAR(100) NULL, 
    [FileUrl] NVARCHAR(250) NULL, 
    CONSTRAINT [FK_EmailAttachments_Emails] FOREIGN KEY ([EmailId]) REFERENCES [Emails]([Id])
)
