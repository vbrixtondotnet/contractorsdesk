CREATE TABLE [dbo].[CompanySettings]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CompanyName] NVARCHAR(200) NOT NULL, 
    [CompanyEmail] NVARCHAR(200) NULL, 
    [GeneralContractorName] NVARCHAR(200) NULL, 
    [CompanyLogoUrl] NVARCHAR(200) NULL, 
    [CreatedBy] INT NOT NULL, 
    [UpdatedBy] INT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL, 
    [Address1] VARCHAR(MAX) NULL, 
    [Address2] VARCHAR(MAX) NULL, 
    [PhoneNumber] NVARCHAR(MAX) NULL, 
    [City] VARCHAR(200) NULL, 
    [State] NVARCHAR(MAX) NULL, 
    [Zip] VARCHAR(50) NULL
)
