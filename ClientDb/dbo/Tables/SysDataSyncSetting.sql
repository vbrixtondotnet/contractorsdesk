CREATE TABLE [dbo].[SysDataSyncSetting]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
    [DataSyncName] NVARCHAR(400) NULL,
    [IsFirstRun] BIT NULL,
    [NumberOfDaysLookup] INT NULL,
    [DateLastRun] DATETIME2 NULL,
    [DateNextRun] DATETIME2 NULL,
    [DateCreated] DATETIME2 NOT NULL,
    [DateModified] DATETIME2 NOT NULL,
    CONSTRAINT [PK_SysDataSyncSetting] PRIMARY KEY CLUSTERED ([Id] ASC)
)
