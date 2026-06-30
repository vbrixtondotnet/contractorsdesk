CREATE TABLE [dbo].[Vendors]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Email] NVARCHAR(100) NULL, 
    [Phone] NVARCHAR(100) NULL, 
    [Address] NVARCHAR(200) NULL, 
    [City] NVARCHAR(50) NULL, 
    [State] NVARCHAR(50) NULL, 
    [IsActive] BIT NOT NULL , 
    [CreatedBy] INT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [UpdatedBy] INT NULL, 
    [DateUpdated] DATETIME NULL, 
    [Zip] NVARCHAR(50) NULL, 
    [Category] NVARCHAR(150) NULL, 
    [Company] NVARCHAR(100) NULL
)
