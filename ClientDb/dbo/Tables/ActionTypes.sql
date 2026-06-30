CREATE TABLE [dbo].[ActionTypes] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (MAX) NOT NULL,
    [DateCreated] DATETIME2 (7)  NOT NULL,
    [CreatedBy]   INT            NULL,
    [DateUpdated] DATETIME2 (7)  NULL,
    [UpdatedBy]   INT            NULL,
    [IsDeleted]   BIT            NOT NULL,
    CONSTRAINT [PK_ActionTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

