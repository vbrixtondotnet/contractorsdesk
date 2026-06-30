CREATE TABLE [dbo].[QBClassesExcel] (
    [FullyQualifiedName]   NVARCHAR (MAX)   NULL,
    [QBAccountID]          UNIQUEIDENTIFIER NULL,
    [AllowedForJobReports] BIT              NULL,
    [OpenJob]              BIT              NULL,
    [ClosedDate]           DATETIME2 (7)    NULL
);

