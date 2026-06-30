CREATE TYPE [dbo].[AccountsISYTD] AS TABLE (
    [ID]       NVARCHAR (255) NULL,
    [ParentID] NVARCHAR (255) NULL,
    [Account]  NVARCHAR (255) NULL,
    [Children] INT            NULL);

