CREATE TABLE [dbo].[AppConfiguration] (
    [Id]    UNIQUEIDENTIFIER NOT NULL,
    [Key]   NVARCHAR (MAX)   NULL,
    [Value] DATETIME2 (7)    NULL,
    CONSTRAINT [PK_AppConfiguration] PRIMARY KEY CLUSTERED ([Id] ASC)
);

