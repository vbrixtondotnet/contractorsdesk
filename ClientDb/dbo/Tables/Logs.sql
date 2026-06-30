CREATE TABLE [dbo].[Logs] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [Timestamp]  NVARCHAR (100) NOT NULL,
    [Level]      NVARCHAR (15)  NOT NULL,
    [Message]    NVARCHAR (MAX) NOT NULL,
    [Exception]  NVARCHAR (MAX) NOT NULL,
    [Properties] NVARCHAR (MAX) NOT NULL,
    [_ts]        DATETIME2 (7)  NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([id] ASC)
);

