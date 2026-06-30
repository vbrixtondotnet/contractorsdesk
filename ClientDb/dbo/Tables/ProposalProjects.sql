CREATE TABLE [dbo].[ProposalProjects]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(100) NOT NULL,
	[Address] NVARCHAR(250) NULL,
	[City] NVARCHAR(100) NULL,
	[State] NVARCHAR(50) NULL,
	[Description] NVARCHAR(MAX) NULL,
	[SqFeet] INT NULL,
	[PhotoUrl] NVARCHAR(MAX) NULL,
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL

)
