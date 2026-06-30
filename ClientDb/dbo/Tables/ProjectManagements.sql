CREATE TABLE [dbo].[ProjectManagements] (
    [ProjectManagementID] UNIQUEIDENTIFIER NOT NULL,
    [QBClassID]           UNIQUEIDENTIFIER NOT NULL,
    [ConstructionTaskID]  UNIQUEIDENTIFIER NOT NULL,
    [StartDate]           DATETIME2 (7)    NULL,
    [EndDate]             DATETIME2 (7)    NULL,
    [Status]              NVARCHAR (50)    NOT NULL,
    [AssignedTo]          UNIQUEIDENTIFIER NULL,
    [Pred1ID]             UNIQUEIDENTIFIER NULL,
    [Pred1LagID]          UNIQUEIDENTIFIER NULL,
    [Pred2ID]             UNIQUEIDENTIFIER NULL,
    [Pred2LagID]          UNIQUEIDENTIFIER NULL,
    [Pred3ID]             UNIQUEIDENTIFIER NULL,
    [Pred3LagID]          UNIQUEIDENTIFIER NULL,
    [ProgressPercentage]  DECIMAL (5, 2)   NOT NULL,
    [Notes]               NVARCHAR (MAX)   NOT NULL,
    [Description]         NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_ProjectManagements] PRIMARY KEY CLUSTERED ([ProjectManagementID] ASC)
);

