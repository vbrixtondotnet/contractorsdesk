CREATE TABLE [dbo].[ProposalLines] (
    [ID]                       UNIQUEIDENTIFIER NOT NULL,
    [Name]                     NVARCHAR (200)   NULL,
    [Description]              NVARCHAR (MAX)   NULL,
    [Amount]                   DECIMAL (18, 2)  NOT NULL,
    [ProposalID]               UNIQUEIDENTIFIER NOT NULL,
    [EstimateCategoryID]       UNIQUEIDENTIFIER NOT NULL,
    [Created]                  DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (MAX)   NULL,
    [Updated]                  DATETIME2 (7)    NULL,
    [UpdatedBy]                NVARCHAR (MAX)   NULL,
    [ParentEstimateCategoryID] UNIQUEIDENTIFIER NULL,
    [SqFoot]                   DECIMAL (18, 2)  NULL,
    [Multiplier]               DECIMAL (18, 2)  NULL,
    [Percentage]               FLOAT (53)       NULL,
    [Sequence]                 INT              NULL,
    [SqFootLocked] BIT NOT NULL DEFAULT (0), 
    CONSTRAINT [PK_ProposalLines] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ProposalLines_EstimateCategories_EstimateCategoryID] FOREIGN KEY ([EstimateCategoryID]) REFERENCES [dbo].[EstimateCategories] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProposalLines_EstimateCategories_ParentEstimateCategoryID] FOREIGN KEY ([ParentEstimateCategoryID]) REFERENCES [dbo].[EstimateCategories] ([ID]),
    CONSTRAINT [FK_ProposalLines_Proposals_ProposalID] FOREIGN KEY ([ProposalID]) REFERENCES [dbo].[Proposals] ([ID]) ON DELETE CASCADE
);


GO
ALTER TABLE [dbo].[ProposalLines] NOCHECK CONSTRAINT [FK_ProposalLines_EstimateCategories_EstimateCategoryID];


GO
ALTER TABLE [dbo].[ProposalLines] NOCHECK CONSTRAINT [FK_ProposalLines_EstimateCategories_ParentEstimateCategoryID];


GO
ALTER TABLE [dbo].[ProposalLines] NOCHECK CONSTRAINT [FK_ProposalLines_Proposals_ProposalID];


GO
CREATE NONCLUSTERED INDEX [IX_ProposalLines_EstimateCategoryID]
    ON [dbo].[ProposalLines]([EstimateCategoryID] ASC);


GO
ALTER INDEX [IX_ProposalLines_EstimateCategoryID]
    ON [dbo].[ProposalLines] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_ProposalLines_ParentEstimateCategoryID]
    ON [dbo].[ProposalLines]([ParentEstimateCategoryID] ASC);


GO
ALTER INDEX [IX_ProposalLines_ParentEstimateCategoryID]
    ON [dbo].[ProposalLines] DISABLE;


GO
CREATE NONCLUSTERED INDEX [IX_ProposalLines_ProposalID]
    ON [dbo].[ProposalLines]([ProposalID] ASC);


GO
ALTER INDEX [IX_ProposalLines_ProposalID]
    ON [dbo].[ProposalLines] DISABLE;

