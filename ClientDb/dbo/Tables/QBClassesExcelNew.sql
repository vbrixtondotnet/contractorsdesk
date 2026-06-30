CREATE TABLE [dbo].[QBClassesExcelNew] (
    [ClassID]              NVARCHAR (MAX)   NULL,
    [QBAccountID]          UNIQUEIDENTIFIER NULL,
    [ActiveJobs]           BIT              NULL,
    [AllowedForJobReports] BIT              NULL,
    [OpenJob]              BIT              NULL,
    [OpenedDate]           DATETIME2 (7)    NULL,
    [ClosedDate]           DATETIME2 (7)    NULL,
    [Ownership]            NVARCHAR (MAX)   NULL
);

