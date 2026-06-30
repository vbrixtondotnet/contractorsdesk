CREATE TABLE [dbo].[SubContractorCategories]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [CreatedBy] INT NULL, 
    [DateCreated] DATETIME NULL, 
    [DateUpdated] DATETIME NULL, 
    [UpdatedBy] INT NULL, 
    [IsActive] BIT NOT NULL
)
