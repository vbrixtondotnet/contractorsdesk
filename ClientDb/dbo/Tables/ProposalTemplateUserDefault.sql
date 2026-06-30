CREATE TABLE [dbo].[ProposalTemplateUserDefault] (
    [Id]         UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [UserId]     INT              NOT NULL,
    [TemplateId] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TemplateId] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[ProposalTemplates] ([Id])
);

