CREATE TABLE [dbo].[ProposalTemplatesLineItems] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [ProposalTemplateId] UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (250)   NOT NULL,
    [Description]        NVARCHAR (MAX)   NULL,
    [Amount]             DECIMAL (18, 2)  NULL,
    [Percentage]         FLOAT (53)       NULL,
    [Sequence]           INT              NOT NULL,
    [ParentId]           UNIQUEIDENTIFIER NULL,
    [EstimateCategoryId]           UNIQUEIDENTIFIER NULL,
    [IsDeleted]          BIT              CONSTRAINT [DF_ProposalTemplatesLineItems_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedBy]          INT              NOT NULL,
    [UpdatedBy]          INT              NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [DateUpdated]        DATETIME         NULL,
    CONSTRAINT [PK_ProposalTemplatesLineItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProposalTemplatesLineItems_ProposalTemplates] FOREIGN KEY ([ProposalTemplateId]) REFERENCES [dbo].[ProposalTemplates] ([Id]),
    CONSTRAINT [FK_ProposalTemplatesLineItems_ProposalTemplatesLineItems] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ProposalTemplatesLineItems] ([Id])
);

