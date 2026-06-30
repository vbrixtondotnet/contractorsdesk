CREATE TABLE [dbo].[QBOLastModifiedTimestamps] (
    [ID]           NVARCHAR (450) NOT NULL,
    [RealmId]      NVARCHAR (MAX) NOT NULL,
    [LastModified] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_QBOLastModifiedTimestamps] PRIMARY KEY CLUSTERED ([ID] ASC)
);

