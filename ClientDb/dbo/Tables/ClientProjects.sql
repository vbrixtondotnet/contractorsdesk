CREATE TABLE [dbo].[ClientProjects] (
    [Id]        INT              IDENTITY (1, 1) NOT NULL,
    [UserId]    INT              NOT NULL,
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ClientProjects] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_ClientProjects_QbClasses] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id])
);

