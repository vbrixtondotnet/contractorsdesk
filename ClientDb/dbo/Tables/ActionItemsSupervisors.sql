CREATE TABLE [dbo].[ActionItemsSupervisors] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [ActionItemId] INT              NOT NULL,
    [SupervisorId] INT              NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [UpdatedBy]    INT              NULL,
    [DateCreated]  DATETIME         NOT NULL,
    [DateUpdated]  DATETIME         NULL,
    CONSTRAINT [PK_ActionItemsSupervisors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActionItemsSupervisors_ActionItemsSupervisors] FOREIGN KEY ([ActionItemId]) REFERENCES [dbo].[ActionItems] ([Id]), 
    CONSTRAINT [FK_ActionItemsSupervisors_Users] FOREIGN KEY ([SupervisorId]) REFERENCES [Users]([Id])
);

