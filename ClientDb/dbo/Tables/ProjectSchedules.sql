CREATE TABLE [dbo].[ProjectSchedules] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [ProjectId]   UNIQUEIDENTIFIER NULL,
    [Status]      VARCHAR (50)     NULL,
    [StartDate]   DATE             NOT NULL,
    [CreatedBy]   INT              NOT NULL,
    [UpdatedBy]   INT              NULL,
    [DateCreated] DATETIME         NOT NULL,
    [DateUpdated] DATETIME         NULL,
    CONSTRAINT [PK_ProjectSchedules] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_ProjectSchedules_QbClass] FOREIGN KEY (ProjectId) REFERENCES [QbClasses]([Id])
);

