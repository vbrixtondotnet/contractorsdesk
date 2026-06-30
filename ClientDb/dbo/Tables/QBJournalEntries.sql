CREATE TABLE [dbo].[QBJournalEntries] (
    [ID]                  NVARCHAR (450)  NOT NULL,
    [TxnDate]             DATETIME2 (7)   NULL,
    [RefNumber]           NVARCHAR (MAX)  NULL,
    [DebitAccountRef]     NVARCHAR (MAX)  NULL,
    [DebitAccountAmount]  DECIMAL (18, 2) NULL,
    [DebitEntityRef]      NVARCHAR (MAX)  NULL,
    [CreditAccountRef]    NVARCHAR (MAX)  NULL,
    [CreditAccountAmount] DECIMAL (18, 2) NULL,
    [CreditEntityRef]     NVARCHAR (MAX)  NULL,
    [TimeCreated]         DATETIME2 (7)   NULL,
    [TimeModified]        DATETIME2 (7)   NULL,
    [CreatedBy]           NVARCHAR (MAX)  NULL,
    [UpdatedBy]           NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_QBJournalEntries] PRIMARY KEY CLUSTERED ([ID] ASC)
);

