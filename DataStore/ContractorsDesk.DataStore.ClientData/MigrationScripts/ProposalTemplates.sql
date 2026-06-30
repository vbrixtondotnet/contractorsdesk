DROP TABLE IF EXISTS ProposalTemplatesLineItems

CREATE TABLE [dbo].[ProposalTemplatesLineItems](
	[Id] [uniqueidentifier] NOT NULL,
	[ProposalTemplateId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Amount] [decimal](18, 2) NULL,
	[Sequence] [int] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedBy] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_ProposalTemplatesLineItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProposalTemplatesLineItems]  WITH CHECK ADD  CONSTRAINT [FK_ProposalTemplatesLineItems_ProposalTemplatesLineItems] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ProposalTemplatesLineItems] ([Id])
GO

ALTER TABLE [dbo].[ProposalTemplatesLineItems] CHECK CONSTRAINT [FK_ProposalTemplatesLineItems_ProposalTemplatesLineItems]
GO

DROP TABLE IF EXISTS ProposalTemplates;

CREATE TABLE [dbo].[ProposalTemplates](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedBy] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_ProposalTemplates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProposalTemplates] ADD  CONSTRAINT [DF_ProposalTemplates_Default]  DEFAULT ((0)) FOR [IsDefault]
GO

ALTER TABLE [dbo].[ProposalTemplatesLineItems]  WITH CHECK ADD  CONSTRAINT [FK_ProposalTemplatesLineItems_ProposalTemplates] FOREIGN KEY([ProposalTemplateId])
REFERENCES [dbo].[ProposalTemplates] ([Id])
GO

ALTER TABLE [dbo].[ProposalTemplatesLineItems] CHECK CONSTRAINT [FK_ProposalTemplatesLineItems_ProposalTemplates]
GO

DECLARE @TemplateId NVARCHAR(100);
SET @TemplateId = 'D1103B14-2196-48C9-BB93-DC6DE65962C2';

INSERT INTO ProposalTemplates (Id,Name,IsDefault,CreatedBy,DateCreated)
VALUES (@TemplateId,'Ground-Up (Default)',1, 1, GETDATE())

INSERT INTO ProposalTemplatesLineItems (Id,ProposalTemplateId,Name,Sequence,CreatedBy,DateCreated)
SELECT DISTINCT CategoryId,@TemplateId, CategoryName, Sequence,1,GETDATE() from
(
SELECT 
    ec.Id AS CategoryId,
    ec.Sequence AS Sequence,
    ec.Name AS CategoryName,
    ISNULL(child.Id, '00000000-0000-0000-0000-000000000000') AS ItemId,
    child.Sequence AS ItemSequence,
    ISNULL(child.Name, '') AS ItemName,
    ISNULL(child.Description, '') AS Description,
    0 AS Amount
FROM EstimateCategories ec
LEFT JOIN EstimateCategories child
    ON ec.Id = child.ParentEstimateCategoryId
WHERE ec.ParentEstimateCategoryId IS NULL
) a
Order by Sequence

INSERT INTO ProposalTemplatesLineItems
SELECT
    ISNULL(child.Id, '00000000-0000-0000-0000-000000000000') AS Id,
	@TemplateId as ProposalTemplateId,
    ISNULL(child.Name, '') AS Name,
    ISNULL(child.Description, '') AS Description,
    0 AS Amount,
    child.Sequence AS Sequence,
    ec.Id AS ParentId,
    1,
    null,
	GetDate(),
	null
FROM EstimateCategories ec
LEFT JOIN EstimateCategories child
    ON ec.Id = child.ParentEstimateCategoryId
WHERE ec.ParentEstimateCategoryId IS NULL
ORDER BY 
    ec.Sequence,
    ec.Id,
    child.Sequence;
