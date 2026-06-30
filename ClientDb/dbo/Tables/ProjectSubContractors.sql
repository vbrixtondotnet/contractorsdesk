CREATE TABLE [dbo].[ProjectSubContractors]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectId] UNIQUEIDENTIFIER NOT NULL, 
    [SubContractorId] UNIQUEIDENTIFIER NOT NULL, 
    [DateCreated]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]    INT              NOT NULL,
    [DateUpdated]  DATETIME2 (7)    NULL,
    [UpdatedBy]    INT              NULL, 
    CONSTRAINT [FK_ProjectSubContractors_QbClasses] FOREIGN KEY ([ProjectId]) REFERENCES [QbClasses]([Id]),
    CONSTRAINT [FK_ProjectSubContractors_SubContractors] FOREIGN KEY ([SubContractorId]) REFERENCES [SubContractors]([Id])
)
