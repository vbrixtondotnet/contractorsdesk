CREATE TABLE [dbo].[Token] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [UserId]       NVARCHAR (MAX)   NOT NULL,
    [RealmId]      NVARCHAR (MAX)   NOT NULL,
    [AccessToken]  NVARCHAR (MAX)   NOT NULL,
    [RefreshToken] NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED ([ID] ASC)
);

