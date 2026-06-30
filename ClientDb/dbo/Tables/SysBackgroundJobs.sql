CREATE TABLE [dbo].[SysBackgroundJobs]
(
	[SysJobsId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[JobName] NVARCHAR(100) NOT NULL,
	[DateOfExecution] DATETIME NOT NULL DEFAULT GETDATE(),
	[LastStatus] NVARCHAR(50) NOT NULL,
	[ApiEndpoint] NVARCHAR(150) NULL,
	[Development] bit  NULL,
	[Staging] bit  NULL,
	[Production] bit  NULL
)
