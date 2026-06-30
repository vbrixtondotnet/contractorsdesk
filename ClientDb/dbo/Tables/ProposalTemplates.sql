CREATE TABLE [dbo].[ProposalTemplates] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (100)   NULL,
    [IsDefault]   BIT              CONSTRAINT [DF_ProposalTemplates_Default] DEFAULT ((0)) NOT NULL,
    [IsActive]   BIT               DEFAULT ((1)) NOT NULL,
    [CreatedBy]   INT              NOT NULL,
    [UpdatedBy]   INT              NULL,
    [DateCreated] DATETIME         NOT NULL,
    [DateUpdated] DATETIME         NULL,
    CONSTRAINT [PK_ProposalTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
);

