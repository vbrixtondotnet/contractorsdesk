CREATE TABLE [dbo].[ProposalLinesHistory] (
    [HistoryID]                UNIQUEIDENTIFIER NOT NULL,
    [ProposalLineID]           UNIQUEIDENTIFIER NOT NULL,
    [ProposalID]               UNIQUEIDENTIFIER NOT NULL,
    [ChangeType]               NVARCHAR (MAX)   NOT NULL,
    [ChangeDate]               DATETIME2 (7)    NOT NULL,
    [Amount]                   DECIMAL (18, 2)  NOT NULL,
    [Description]              NVARCHAR (MAX)   NULL,
    [EstimateCategoryID]       UNIQUEIDENTIFIER NOT NULL,
    [ParentEstimateCategoryID] UNIQUEIDENTIFIER NULL,
    [Created]                  DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (MAX)   NULL,
    [Updated]                  DATETIME2 (7)    NULL,
    [UpdatedBy]                NVARCHAR (MAX)   NULL,
    [Percentage]               FLOAT (53)       NULL,
    CONSTRAINT [PK_ProposalLinesHistory] PRIMARY KEY CLUSTERED ([HistoryID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ProposalLinesHistory_ProposalLineID]
    ON [dbo].[ProposalLinesHistory]([ProposalLineID] ASC);


GO
ALTER INDEX [IX_ProposalLinesHistory_ProposalLineID]
    ON [dbo].[ProposalLinesHistory] DISABLE;

