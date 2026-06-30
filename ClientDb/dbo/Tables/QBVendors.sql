CREATE TABLE [dbo].[QBVendors] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [ListID]       NVARCHAR (MAX)   NOT NULL,
    [DisplayName]  NVARCHAR (MAX)   NULL,
    [CompanyName]  NVARCHAR (MAX)   NULL,
    [Address]      NVARCHAR (MAX)   NULL,
    [NACE]         NVARCHAR (MAX)   NULL,
    [VATNumber]    NVARCHAR (MAX)   NULL,
    [NUINumber]    NVARCHAR (MAX)   NULL,
    [TimeCreated]  DATETIME2 (7)    NULL,
    [TimeModified] DATETIME2 (7)    NULL,
    [CreatedBy]    NVARCHAR (MAX)   NULL,
    [UpdatedBy]    NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_QBVendors] PRIMARY KEY CLUSTERED ([ID] ASC)
);

