CREATE TABLE [dbo].[Proposals] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Number]       INT              NOT NULL,
    [TemplateId]   UNIQUEIDENTIFIER NULL,
    [QBClassId]    UNIQUEIDENTIFIER NULL,
    [QBCustomerId]    UNIQUEIDENTIFIER NULL,
    [ProposalProjectId] uniqueidentifier NULL,
    [ClientId] uniqueidentifier NULL,
    [Date]         DATETIME2 (7)    NOT NULL,
    [TotalAmount]  DECIMAL (18, 2)  NOT NULL,
    [DocStatus]    NVARCHAR (MAX)   CONSTRAINT [DF__Proposals__DocSt__45BE5BA9] DEFAULT (N'') NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT ((0)), 
    [IsArchived] BIT NOT NULL DEFAULT ((0)), 
    [IncludeLinesWithZeroAmount] BIT NOT NULL DEFAULT ((0)), 
    [DateCreated]  DATETIME2 (7)    NOT NULL DEFAULT((GETDATE())),
    [CreatedBy]    INT              NOT NULL DEFAULT((1)),
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL
    CONSTRAINT [PK_Proposals] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Proposals_ProposalTemplates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[ProposalTemplates] ([Id]),
    CONSTRAINT [FK_Proposals_QBClasses] FOREIGN KEY ([QBClassId]) REFERENCES [dbo].[QBClasses] ([ID]), 
    CONSTRAINT [FK_Proposals_Clients] FOREIGN KEY ([ClientId]) REFERENCES [Clients]([Id]), 
    CONSTRAINT [FK_Proposals_ProposalProjects] FOREIGN KEY ([ProposalProjectId]) REFERENCES [ProposalProjects]([Id]),
    CONSTRAINT [FK_Proposals_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Proposals_Users_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Proposals_QBClassID]
    ON [dbo].[Proposals]([QBClassId] ASC);


GO
ALTER INDEX [IX_Proposals_QBClassID]
    ON [dbo].[Proposals] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_Proposals_QBCustomerID]
    ON [dbo].[Proposals]([ClientId] ASC);


GO
ALTER INDEX [IX_Proposals_QBCustomerID]
    ON [dbo].[Proposals] DISABLE;

