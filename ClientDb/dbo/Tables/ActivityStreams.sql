CREATE TABLE [dbo].[ActivityStream]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Ref] NVARCHAR(150) NOT NULL, 
    [StepName] NVARCHAR(100) NULL, 
    [Reason] NVARCHAR(MAX) NULL, 
    [DateCreated] DATETIME2 NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [DateUpdated] DATETIME2 NULL, 
    [UpdatedBy] INT NULL
)
