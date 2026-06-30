CREATE TABLE [dbo].[ClientDocuments]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()), 
    [FolderId] UNIQUEIDENTIFIER NOT NULL, 
    [ClientId] UNIQUEIDENTIFIER NOT NULL, 
    [FileName] NVARCHAR(150) NULL, 
    [FileExtension] NVARCHAR(50) NULL, 
    [Version] INT NOT NULL DEFAULT(1),
    [Url] NVARCHAR(250) NOT NULL, 
    [SubcontractorId] UNIQUEIDENTIFIER NULL,
    [DateCreated] DATETIME NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [DateUpdated] DATETIME NULL, 
    [UpdatedBy] INT NULL, 
    CONSTRAINT [FK_ClientDocuments_Subcontractors] FOREIGN KEY ([SubcontractorId]) REFERENCES [SubContractors]([Id]), 
    CONSTRAINT [FK_ClientDocuments_SysFolders] FOREIGN KEY ([FolderId]) REFERENCES [SysFolders]([Id])
)
