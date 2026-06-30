CREATE TABLE [dbo].[ProjectSupervisors] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [ProjectId]    UNIQUEIDENTIFIER NOT NULL,
    [SupervisorId] INT              NOT NULL,
    [DateAssigned] DATE             NOT NULL,
    [SupervisorTypeId] INT NULL, 
    CONSTRAINT [PK_ProjectSupervisors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProjectSupervisors_QBClasses] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[QBClasses] ([ID])
);

