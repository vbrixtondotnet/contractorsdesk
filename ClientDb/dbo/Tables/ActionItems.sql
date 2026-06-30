CREATE TABLE [dbo].[ActionItems] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [Title]        NVARCHAR (MAX)   NOT NULL,
    [Description]  NVARCHAR (MAX)   NOT NULL,
    [ProjectId]    UNIQUEIDENTIFIER NULL,
    [ActionTypeId] INT              NOT NULL,
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL,
    [IsDeleted]    BIT              NOT NULL,
    [DueDate]      DATE             CONSTRAINT [DF__ActionIte__DueDa__6ABAD62E] DEFAULT ('0001-01-01') NOT NULL,
    [Status]       INT              CONSTRAINT [DF__ActionIte__Statu__6BAEFA67] DEFAULT ((1)) NOT NULL,
    [IsArchived]   BIT              NOT NULL DEFAULT 0,
    [AcceptedBy] INT NULL, 
    [Source] INT NOT NULL DEFAULT(1), 
    CONSTRAINT [PK_ActionItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ActionItems_ActionItems_QBClasses] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[QBClasses] ([ID]),
    CONSTRAINT [FK_ActionItems_ActionTypes_ActionTypeId] FOREIGN KEY ([ActionTypeId]) REFERENCES [dbo].[ActionTypes] ([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_ActionItems_Users_AcceptedBy] FOREIGN KEY ([AcceptedBy]) REFERENCES [Users]([Id]), 
    CONSTRAINT [FK_ActionItems_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id])
);

