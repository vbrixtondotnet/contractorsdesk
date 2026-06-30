CREATE TABLE [dbo].[AudioUploads]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] INT NOT NULL, 
    [ProjectId] UNIQUEIDENTIFIER NULL, 
    [FileUrl] NVARCHAR(250) NOT NULL, 
    [StorageStatus] NVARCHAR(50) NULL, 
    [AIStatus] NVARCHAR(50) NULL, 
    [Transcript] NVARCHAR(MAX) NULL, 
    [DateCreated] DATETIME NOT NULL, 
    CONSTRAINT [FK_AudioUploads_QbClasses] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id])
)
