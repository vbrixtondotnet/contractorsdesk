CREATE TABLE [dbo].[QBLocations] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [ListID]       NVARCHAR (MAX)   NOT NULL,
    [Name]         NVARCHAR (MAX)   NOT NULL,
    [FullName]     NVARCHAR (MAX)   NOT NULL,
    [IsActive]     BIT              NULL,
    [TimeCreated]  DATETIME2 (7)    NULL,
    [TimeModified] DATETIME2 (7)    NULL,
    CONSTRAINT [PK_QBLocations] PRIMARY KEY CLUSTERED ([ID] ASC)
);

