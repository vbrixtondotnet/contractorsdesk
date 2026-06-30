CREATE TABLE [dbo].[Emails]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY , 
    [ProjectId] UNIQUEIDENTIFIER NULL,
    [SenderId] int NULL,
    [MessageId] nvarchar(250) NOT NULL,
    [ReplyToMessageId] nvarchar(250) NULL,
    [Subject] VARCHAR(MAX) NOT NULL, 
    [Body] VARCHAR(MAX) NULL, 
    [Cc] VARCHAR(MAX) NULL, 
    [Bcc] VARCHAR(MAX) NULL, 
    [DateCreated] DATETIME2 NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [IsRead] BIT NOT NULL DEFAULT 0, 
    [From] NVARCHAR(150) NULL, 
    [To] NVARCHAR(150) NULL, 
    [EmailType] INT NULL, 
    [PostMarkReferences] NVARCHAR(MAX) NULL, 
    [IsArchived] BIT NOT NULL DEFAULT (0), 
    [IsDeleted] BIT NOT NULL DEFAULT (0)
)
