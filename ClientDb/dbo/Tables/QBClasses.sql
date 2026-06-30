CREATE TABLE [dbo].[QBClasses] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [ListID]                  NVARCHAR (MAX)   NULL,
    [Name]                    NVARCHAR (MAX)   NULL,
    [FullyQualifiedName]      NVARCHAR (MAX)   NULL,
    [Address]                 NVARCHAR (250)   NULL,
    [SubClass]                BIT              NULL,
    [ParentID]                NVARCHAR (MAX)   NULL,
    [TimeCreated]             DATETIME2 (7)    NULL,
    [TimeModified]            DATETIME2 (7)    NULL,
    [CreatedBy]               NVARCHAR (MAX)   NULL,
    [UpdatedBy]               NVARCHAR (MAX)   NULL,
    [AllowedForJobReports]    BIT              NULL,
    [ClosedDate]              DATETIME2 (7)    NULL,
    [Notes]                   NVARCHAR (MAX)   NULL,
    [OpenJob]                 BIT              NULL,
    [QBAccountID]             UNIQUEIDENTIFIER NULL,
    [OpenedDate]              DATETIME2 (7)    NULL,
    [Ownership]               NVARCHAR (MAX)   NULL,
    [ActiveJobs]              BIT              NULL,
    [ActiveSpecJobs]          BIT              NULL,
    [AllowedForBudgetReports] BIT              NULL,
    [Description]             NVARCHAR (MAX)   NULL,
    [State] NVARCHAR(MAX) NULL, 
    [City] NVARCHAR(MAX) NULL, 
    [IsArchived] BIT NOT NULL DEFAULT(0), 
    [IsDeleted] BIT NOT NULL DEFAULT(0), 
    [IsActive] BIT NULL, 
    [MinimumRequestedAmount] DECIMAL(18, 2) NULL, 
    [IsCompleted] BIT NOT NULL DEFAULT(0), 
    CONSTRAINT [PK_QBClasses] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_QBClasses_QBAccounts_QBAccountID] FOREIGN KEY ([QBAccountID]) REFERENCES [dbo].[QBAccounts] ([ID])
);


GO
ALTER TABLE [dbo].[QBClasses] NOCHECK CONSTRAINT [FK_QBClasses_QBAccounts_QBAccountID];


GO
CREATE NONCLUSTERED INDEX [IX_QBClasses_QBAccountID]
    ON [dbo].[QBClasses]([QBAccountID] ASC);


GO
ALTER INDEX [IX_QBClasses_QBAccountID]
    ON [dbo].[QBClasses] DISABLE;

