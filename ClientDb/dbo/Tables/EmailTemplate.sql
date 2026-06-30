CREATE TABLE [dbo].[EmailTemplate]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(300) NOT NULL,
    [EmailType] NVARCHAR(300) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [OwnerId] INT NOT NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [DateCreated] DATETIME NOT NULL,
    [CreatedById] INT NOT NULL,
    [DateModified] DATETIME NOT NULL,
    [ModifiedById] INT NOT NULL,
    CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [UQ_EmailTemplate_Name_Type] UNIQUE ([Name], [EmailType], [OwnerId])
)
