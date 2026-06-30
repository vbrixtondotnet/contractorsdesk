CREATE TABLE [dbo].[ProposalSupervisors]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProposalId] UNIQUEIDENTIFIER NOT NULL, 
    [UserId] INT NOT NULL,
    [SupervisorTypeId] INT NULL, 
    [DateCreated]  DATETIME2 (7)    NOT NULL, 
    CONSTRAINT [FK_ProposalSupervisors_Proposals] FOREIGN KEY ([ProposalId]) REFERENCES [Proposals]([Id]), 
    CONSTRAINT [FK_ProposalSupervisors_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
)
