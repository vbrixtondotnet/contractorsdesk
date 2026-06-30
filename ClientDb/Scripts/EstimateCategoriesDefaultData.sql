DECLARE @ProposalTemplates TABLE
(
 [Id]          UNIQUEIDENTIFIER NOT NULL,
 [Name]        NVARCHAR (100)   NULL,
 [IsDefault]   BIT,
 [CreatedBy]   INT              NOT NULL,
 [DateCreated] DATETIME         NOT NULL
)

DECLARE @ProposalTemplateLineItems TABLE
(
	[Id]                 UNIQUEIDENTIFIER NOT NULL,
	[ProposalTemplateId] UNIQUEIDENTIFIER NOT NULL,
	[Name]               NVARCHAR (250)   NOT NULL,
	[Description]        NVARCHAR (MAX)   NULL,
	[Sequence]           INT              NOT NULL,
	[ParentId]           UNIQUEIDENTIFIER NULL,
	[EstimateCategoryId] UNIQUEIDENTIFIER NULL,
	[CreatedBy]          INT              NOT NULL,
	[DateCreated]        DATETIME         NOT NULL
)

DECLARE @DefaultEstimateCategories TABLE
(
	Id uniqueidentifier,
	Name nvarchar(150),
	Description nvarchar(max) null,
	Sequence int null,
	ParentEstimateCategoryId uniqueidentifier null,
	ParentName nvarchar(150),
	ParentDescription nvarchar(max) null,
	ParentSequence int null
)

DECLARE @DefaultParentEstimateCategories TABLE
(
	Id uniqueidentifier,
	Name nvarchar(150),
	Sequence int null
)

--Proposal Template
INSERT INTO @ProposalTemplates
select 
ID,
Name,
1,
1,
GETDATE()
from ProposalTemplates where id = 'D1103B14-2196-48C9-BB93-DC6DE65962C2'

--Proposal Template Line Items
INSERT INTO @ProposalTemplateLineItems
select 
pli.Id,
'D1103B14-2196-48C9-BB93-DC6DE65962C2',
pli.Name,
pli.Description,
pli.Sequence,
pli.ParentId,
pli.EstimateCategoryId,
1,
GETDATE()
from ProposalTemplatesLineItems pli
where ProposalTemplateId = 'D1103B14-2196-48C9-BB93-DC6DE65962C2'

--Estimate Categories
INSERT INTO @DefaultEstimateCategories
select 
pli.EstimateCategoryId,
e.Name,
e.Description,
e.Sequence,
e.ParentEstimateCategoryID,
ep.Name as ParentName,
ep.Description as ParentDescription,
ep.Sequence as ParentSequence
from @ProposalTemplateLineItems pli
inner join EstimateCategories e
on e.ID = pli.EstimateCategoryId
inner join EstimateCategories ep
on ep.ID = e.ParentEstimateCategoryID
where ProposalTemplateId = 'D1103B14-2196-48C9-BB93-DC6DE65962C2'

-- Parent Categories
INSERT INTO @DefaultParentEstimateCategories
select 
ParentEstimateCategoryId,
ParentName,
ParentSequence
from @DefaultEstimateCategories
Group By ParentEstimateCategoryId,ParentName,ParentSequence

TRUNCATE TABLE ProposalTemplatesTemp
INSERT INTO ProposalTemplatesTemp (Id,Name,IsDefault,CreatedBy,DateCreated)
SELECT * FROM @ProposalTemplates

TRUNCATE TABLE ProposalTemplatesLineItemsTemp
INSERT INTO ProposalTemplatesLineItemsTemp (Id,ProposalTemplateId,Name,Description,Sequence,ParentId,EstimateCategoryId,IsDeleted,CreatedBy,DateCreated)
SELECT Id,'D1103B14-2196-48C9-BB93-DC6DE65962C2',Name,Description,Sequence,ParentId,EstimateCategoryId,0,1,GetDate() from @ProposalTemplateLineItems

TRUNCATE TABLE EstimateCategoriesTemp
INSERT INTO EstimateCategoriesTemp (Id,Name,Sequence)
SELECT Id,Name,Sequence from @DefaultParentEstimateCategories
Order by Sequence

INSERT INTO EstimateCategoriesTemp  (Id,Name,Description,Sequence,ParentEstimateCategoryID)
SELECT Id,Name,Description,Sequence,ParentEstimateCategoryID from @DefaultEstimateCategories 
order by ParentSequence, Sequence



INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'825800a8-00cd-4ac2-5865-08dce19eb8a0', N'Waterproofing', 71, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c27bde33-8f87-4a7b-5866-08dce19eb8a0', N'Hardscape', 74, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'107c1dd9-9c5e-4e82-5867-08dce19eb8a0', N'Scaffolding', 57, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ba366bd1-6213-4034-5868-08dce19eb8a0', N'Exterior and Interior Railing', 34, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Per Plans')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'467b86dd-a9ef-4965-5869-08dce19eb8a0', N'Shower Pan', 60, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Shower Waterproofing')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e7f27724-e523-4c3b-586a-08dce19eb8a0', N'Garage Floor', 43, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Level and Epoxy Floor')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'0997663d-ed47-4eab-586b-08dce19eb8a0', N'Window and Door Install', 70, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Per Plans')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1b99337e-a152-477d-37dd-08dcefc578ae', N'Water Heater', 75, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Upgrade')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'fd41b3fa-df36-4de1-37de-08dcefc578ae', N'Sauna', 76, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'43f6c83e-44fc-4d9f-37df-08dcefc578ae', N'Epoxy Garage Floor', 77, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a09f00bb-5e94-463b-37e0-08dcefc578ae', N'Backfill', 40, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'5671cd57-d2ad-40a6-37e1-08dcefc578ae', N'Panel upgrade', 99, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7cd6334b-653d-4537-37e2-08dcefc578ae', N'Excavation', 25, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'585030d5-0300-421e-37e3-08dcefc578ae', N'Stain Contractor', 77, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'11fb7a0e-d6af-4952-37e4-08dcefc578ae', N'Electrical Panel Upgrade', 81, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'86abef50-ad0b-4448-7441-08dcf1e6bcdd', N'On Site Supervision', 12, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'676d782f-201a-4a5f-a15f-1341599130d1', N'Hot Mop Contractor', 46, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c2682aa6-a285-413a-a081-13eb66499965', N'Interior Design', 48, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1f86bc77-e947-44dc-a4ed-1591f87626ee', N'Countertop Fabricator', 27, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'Site Work', 4, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7eeadbe1-d848-4e07-9aec-2f4e7d3979bd', N'Interest Expense', 82, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e1903443-4536-4d24-8507-32d8fbfc5903', N'Fire Sprinklers Contractor', 37, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'b24bacd8-edc7-4878-8aa4-37c5724237f2', N'Deck Contractor', 28, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'cdd4afab-20d3-40b2-b53f-3e31773f729e', N'Tile Material', 35, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd31868a9-eb0a-4202-a7d3-4396f5e0b741', N'Plumbing Fixtures', 34, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'980d37ec-9ede-45fa-8a9d-451d218a4182', N'Curbs & Gutter Contractor', 66, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'b0b192e0-0cc8-42ac-96cc-4a77adca09e5', N'Interior doors/Windows', 10, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'daad3d7b-4d42-4f22-ae1a-5514d88b2600', N'Plans/survey', 1, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a5da1985-1858-49e8-abb8-558efc382aac', N'Overhead', 301, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'45a886d9-f473-41c2-9359-55f9b968bddc', N'Trash Hauling', 68, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e8c89fac-0014-438e-89ba-56c89026aeb4', N'Stairs Contractor', 65, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ab4c749c-4a10-4eb9-9779-575a60307f1e', N'Asbestos Removal', 8, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'df7acf8b-9156-4cf8-a3d2-589a7953466e', N'Interior doors/Hardware', 32, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'3c32d1de-a305-4f38-b22b-5b5fe0c3ea04', N'Finish Hardware', 27, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide: Cabinet Hardware')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'28be9922-a0d4-44ca-8393-5e603cd0610f', N'Wine Storage Contractor', 69, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'6504c427-be01-44e4-ab3c-5ea2a6dbb7f1', N'Other', 83, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ff3c03b9-d3bf-4b56-b2be-6326bc1d8459', N'Solar Contractor', 63, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1594908e-cb37-428d-b3d8-640a683960b9', N'General Contractor Fee', 78, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'117ebb33-6930-4e8c-8230-686c8da61688', N'Painting Contractor', 53, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'536b2c08-b239-463c-8f46-69dfa835ddb2', N'Low Voltage Contractor', 51, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Priced for Prewire. Automated shades?, Security? Cable and Internet, speakers. ')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ba5c7b3a-b441-4958-857d-6a0d940cf118', N'Bath accessories', 19, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide: Towel Bars, Mirrors, Toilet paper holders, Hardware')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'Preparation', 1, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'b325dd9d-d055-40a6-9139-75eb7b19c7db', N'Lumber/Hardware', 33, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd3bd6589-552b-464b-ae4d-76aeb609de11', N'External Railings Contractor', 35, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7bace765-f941-4c33-a797-79de17303864', N'Exterior Doors/Hardware', 32, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd51655e7-3060-454d-930a-87f47b064312', N'Cabinet Contractor', 25, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a5533127-aea0-48de-82af-8886827fc1c5', N'Site Drainage', 71, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'65421aa4-89fe-412d-8d42-8e9ac05c1420', N'Sheet metal', 58, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'740aa1d7-7113-43ed-ab01-902eb8e9e603', N'Precast Contractor', 55, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'0115d846-31a3-405e-8c23-92906df2ef77', N'Elevator Contractor', 31, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'47269347-037d-41db-bf70-93d49c36a275', N'Internal Railings Contractor', 50, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'92883fec-dfdf-4bf3-a033-94f129c42272', N'Countertop Material', 21, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide. Supplier Moda in San Clemente, Deniz- 949-244-4839')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'175a03b2-565e-4eb3-a5d7-9728befbe82e', N'Pool/Spa', 69, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'265a03b2-565e-4eb3-a5d7-9728befbe82e', N'Retaining Wall', 70, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'9b2bba0c-90bc-4892-a064-98d8d5838182', N'Flooring Material', 30, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'86c74224-0170-400c-bd74-99a4146229af', N'Landscape', 68, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'063d6c29-2430-40de-bb2b-9a83941120cd', N'Material Delivery', 52, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a1b01fd2-872e-4a6b-a834-9f230342a1c1', N'Insulation Contractor', 47, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd2c47096-9af5-44d1-8cd2-a0ecee5a8c42', N'Structural Steel', 66, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ae56314b-7ac7-49fe-a2e0-a53adbfc6332', N'Finish Material', 29, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'0a5e9f89-2d1d-40b9-8244-a65f94cc02a3', N'Temp Utilities', 7, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c5ff414e-8f3d-498b-bfc3-a9fc0bb7bd4b', N'Electrical Fixtures', 23, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide: Sconces, Pendants, Fans, Chandeliers')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'Sub Contractors', 300, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'cee94ee2-f3d6-4f63-8b63-ae0ff14313eb', N'Shower Doors Contractor', 59, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a1c3e88f-e7ec-46d1-adf8-b38997770c31', N'Foundation Contractor', 40, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'62022341-ec48-4087-aa56-b4d120100ca6', N'Demolition', 6, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'01282e22-a193-48be-8508-b6e4055459a2', N'General Contractor Contingency', 77, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'746abd5e-956d-459c-9a49-b73ba7d596a8', N'Flooring Contractor', 39, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'25b467dd-97f0-4816-b0f6-ba193a7acb51', N'Closets', 26, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1bf7e623-8741-4bfb-a9f2-bd009d658ffc', N'Electrical Contractor', 30, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'82599839-ac72-4e9e-82ca-bdb53085c253', N'Drywall Contractor', 29, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'81fcb8c4-ce90-47e4-bb9b-c01643f4b80f', N'Special Inspections', 64, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7b180310-468e-4b59-9337-c2d84b23cb14', N'Building permits', 3, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7023d8e9-6716-4ee1-9c51-c4041baaf865', N'Site Drainage Contractor', 62, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'edd0ee18-a505-4a74-8adf-c57517fae7f1', N'Front Door', 31, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'bae8de79-c74a-4be5-b3eb-cca2d535c2bf', N'BBQ Grill/Firepit', 64, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e3ad07e4-1f16-4d2d-b305-d04fd0b1cbe7', N'Tile Contractor', 67, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'271aa5bc-f5b2-4f56-94fb-d37d7305da08', N'Appliances', 17, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'5b3a00a9-4fc6-42fc-9b77-d73fd784700a', N'Property Tax', 81, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a44dbb0d-fc97-43d1-b12c-d744428a4df8', N'Grading', 4, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'af245b53-809c-4531-9f8b-dc7114b0f4b4', N'Garage Doors Contractor', 42, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'cfe6afff-4a8e-47ba-b24a-dff9e6b43e3e', N'Exterior Doors/Windows', 25, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'"Budget Freindly windows Andersen 100 or Milgard.  Doors- La Cantina higher end and Windor or Milgard for budget friendly."')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'Material', 2, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'986cbdc3-be93-406a-babc-e2d14ba882db', N'Fireplace  Contractor', 38, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'68b3fffd-d37f-481b-8303-e43a00a0799e', N'Roofing Contractor', 56, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'416fdc24-339a-499d-9bde-e4be5ef19ed9', N'Heating & Air Contractor', 45, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'075f528a-c636-4b6e-9cc7-e729d0c0ee9e', N'Framing Contractor', 41, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c207c3cd-494c-462f-903c-e75f8c8a9707', N'Finish Carpentry', 36, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Install: Finish Material, bath accessories, interior doors')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'01a4dd1e-36bf-427e-8f28-e803351e48fe', N'Exterior Finish Contractor', 33, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'2929c384-45bd-494e-9e98-f0e140c99aa0', N'Temp Services', 72, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'6ecf6dbd-171f-4978-857e-f8545d997164', N'Shower Enclosure/Mirror Contractor', 61, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'6e525d26-883e-402d-94a4-f87d96b7cb52', N'Insurance', 5, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'9a0da22f-afd4-4813-b7a5-f94a5d37416b', N'Driveway', 67, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'21578f57-31ed-420f-bfc1-fd77098a2bb2', N'Gutters', 44, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'f3832da8-ea6f-436e-9c99-ff65cd5b061e', N'Land', 80, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[ProposalTemplates] ([Id], [Name], [IsDefault], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Ground-Up (Default)', 1, 1, NULL, CAST(N'2025-03-30T01:25:47.820' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Preparation', NULL, NULL, NULL, 1, NULL, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Material', NULL, NULL, NULL, 2, NULL, N'c00df57f-c09b-4967-bc93-e2330e2f303f', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Sub Contractors', NULL, NULL, NULL, 3, NULL, N'851efb5a-de4f-4380-a264-ab31bb3acecd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Site Work', NULL, NULL, NULL, 4, NULL, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a5da1985-1858-49e8-abb8-558efc382aac', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Overhead', NULL, NULL, NULL, 5, NULL, N'a5da1985-1858-49e8-abb8-558efc382aac', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'825800a8-00cd-4ac2-5865-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Waterproofing', N'', NULL, NULL, 71, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'825800a8-00cd-4ac2-5865-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c27bde33-8f87-4a7b-5866-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Hardscape', N'', NULL, NULL, 74, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'c27bde33-8f87-4a7b-5866-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'107c1dd9-9c5e-4e82-5867-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Scaffolding', N'', NULL, NULL, 57, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'107c1dd9-9c5e-4e82-5867-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ba366bd1-6213-4034-5868-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior and Interior Railing', N'Per Plans', NULL, NULL, 34, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'ba366bd1-6213-4034-5868-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'467b86dd-a9ef-4965-5869-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Shower Pan', N'Shower Waterproofing', NULL, NULL, 60, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'467b86dd-a9ef-4965-5869-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e7f27724-e523-4c3b-586a-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Garage Floor', N'Level and Epoxy Floor', NULL, NULL, 43, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e7f27724-e523-4c3b-586a-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0997663d-ed47-4eab-586b-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Window and Door Install', N'Per Plans', NULL, NULL, 70, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'0997663d-ed47-4eab-586b-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1b99337e-a152-477d-37dd-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Water Heater', N'Upgrade', NULL, NULL, 75, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'1b99337e-a152-477d-37dd-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'fd41b3fa-df36-4de1-37de-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Sauna', N'', NULL, NULL, 76, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'fd41b3fa-df36-4de1-37de-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'43f6c83e-44fc-4d9f-37df-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Epoxy Garage Floor', N'', NULL, NULL, 77, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'43f6c83e-44fc-4d9f-37df-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a09f00bb-5e94-463b-37e0-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Backfill', N'', NULL, NULL, 40, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'a09f00bb-5e94-463b-37e0-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'5671cd57-d2ad-40a6-37e1-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Panel upgrade', N'', NULL, NULL, 99, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'5671cd57-d2ad-40a6-37e1-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7cd6334b-653d-4537-37e2-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Excavation', N'', NULL, NULL, 25, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'7cd6334b-653d-4537-37e2-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'585030d5-0300-421e-37e3-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Stain Contractor', N'', NULL, NULL, 77, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'585030d5-0300-421e-37e3-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'11fb7a0e-d6af-4952-37e4-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Electrical Panel Upgrade', N'', NULL, NULL, 81, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'11fb7a0e-d6af-4952-37e4-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'86abef50-ad0b-4448-7441-08dcf1e6bcdd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'On Site Supervision', N'', NULL, NULL, 12, N'a5da1985-1858-49e8-abb8-558efc382aac', N'86abef50-ad0b-4448-7441-08dcf1e6bcdd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'676d782f-201a-4a5f-a15f-1341599130d1', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Hot Mop Contractor', N'', NULL, NULL, 46, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'676d782f-201a-4a5f-a15f-1341599130d1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c2682aa6-a285-413a-a081-13eb66499965', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interior Design', N'', NULL, NULL, 48, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'c2682aa6-a285-413a-a081-13eb66499965', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1f86bc77-e947-44dc-a4ed-1591f87626ee', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Countertop Fabricator', N'', NULL, NULL, 27, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'1f86bc77-e947-44dc-a4ed-1591f87626ee', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'b68851c1-9bd1-4234-9bda-28490931a31f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Other', N'', NULL, NULL, 88, N'a5da1985-1858-49e8-abb8-558efc382aac', N'6504c427-be01-44e4-ab3c-5ea2a6dbb7f1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7eeadbe1-d848-4e07-9aec-2f4e7d3979bd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interest Expense', N'', NULL, NULL, 82, N'a5da1985-1858-49e8-abb8-558efc382aac', N'7eeadbe1-d848-4e07-9aec-2f4e7d3979bd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e1903443-4536-4d24-8507-32d8fbfc5903', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Fire Sprinklers Contractor', N'', NULL, NULL, 37, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e1903443-4536-4d24-8507-32d8fbfc5903', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'b24bacd8-edc7-4878-8aa4-37c5724237f2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Deck Contractor', N'', NULL, NULL, 28, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'b24bacd8-edc7-4878-8aa4-37c5724237f2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'cdd4afab-20d3-40b2-b53f-3e31773f729e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Tile Material', N'', NULL, NULL, 35, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'cdd4afab-20d3-40b2-b53f-3e31773f729e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0cb8b9c0-7e57-4ea1-a8da-429ace1c6107', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interior Doors/Hardware', N'', NULL, NULL, 49, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'df7acf8b-9156-4cf8-a3d2-589a7953466e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd31868a9-eb0a-4202-a7d3-4396f5e0b741', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Plumbing Fixtures', N'', NULL, NULL, 34, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'd31868a9-eb0a-4202-a7d3-4396f5e0b741', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'980d37ec-9ede-45fa-8a9d-451d218a4182', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Curbs & Gutter Contractor', N'', NULL, NULL, 66, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'980d37ec-9ede-45fa-8a9d-451d218a4182', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'daad3d7b-4d42-4f22-ae1a-5514d88b2600', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Plans/survey', N'', NULL, NULL, 1, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'daad3d7b-4d42-4f22-ae1a-5514d88b2600', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'45a886d9-f473-41c2-9359-55f9b968bddc', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Trash Hauling', N'', NULL, NULL, 68, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'45a886d9-f473-41c2-9359-55f9b968bddc', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e8c89fac-0014-438e-89ba-56c89026aeb4', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Stairs Contractor', N'', NULL, NULL, 65, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e8c89fac-0014-438e-89ba-56c89026aeb4', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ab4c749c-4a10-4eb9-9779-575a60307f1e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Asbestos Removal', N'', NULL, NULL, 8, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'ab4c749c-4a10-4eb9-9779-575a60307f1e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'df7acf8b-9156-4cf8-a3d2-589a7953466e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interior doors/Windows', N'', NULL, NULL, 32, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'b0b192e0-0cc8-42ac-96cc-4a77adca09e5', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'3c32d1de-a305-4f38-b22b-5b5fe0c3ea04', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Finish Hardware', N'Owner to Provide: Cabinet Hardware', NULL, NULL, 27, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'3c32d1de-a305-4f38-b22b-5b5fe0c3ea04', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'28be9922-a0d4-44ca-8393-5e603cd0610f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Wine Storage Contractor', N'', NULL, NULL, 69, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'28be9922-a0d4-44ca-8393-5e603cd0610f', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ff3c03b9-d3bf-4b56-b2be-6326bc1d8459', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Solar Contractor', N'', NULL, NULL, 63, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'ff3c03b9-d3bf-4b56-b2be-6326bc1d8459', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1594908e-cb37-428d-b3d8-640a683960b9', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'General Contractor Fee', N'', NULL, NULL, 78, N'a5da1985-1858-49e8-abb8-558efc382aac', N'1594908e-cb37-428d-b3d8-640a683960b9', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'117ebb33-6930-4e8c-8230-686c8da61688', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Painting Contractor', N'', NULL, NULL, 53, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'117ebb33-6930-4e8c-8230-686c8da61688', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'536b2c08-b239-463c-8f46-69dfa835ddb2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Low Voltage Contractor', N'Priced for Prewire. Automated shades?, Security? Cable and Internet, speakers. ', NULL, NULL, 51, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'536b2c08-b239-463c-8f46-69dfa835ddb2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ba5c7b3a-b441-4958-857d-6a0d940cf118', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Bath accessories', N'Owner to Provide: Towel Bars, Mirrors, Toilet paper holders, Hardware', NULL, NULL, 19, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'ba5c7b3a-b441-4958-857d-6a0d940cf118', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'b325dd9d-d055-40a6-9139-75eb7b19c7db', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Lumber/Hardware', N'', NULL, NULL, 33, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'b325dd9d-d055-40a6-9139-75eb7b19c7db', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd3bd6589-552b-464b-ae4d-76aeb609de11', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'External Railings Contractor', N'', NULL, NULL, 35, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd3bd6589-552b-464b-ae4d-76aeb609de11', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7bace765-f941-4c33-a797-79de17303864', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior Doors/Hardware', N'', NULL, NULL, 32, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'7bace765-f941-4c33-a797-79de17303864', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd51655e7-3060-454d-930a-87f47b064312', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Cabinet Contractor', N'', NULL, NULL, 25, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd51655e7-3060-454d-930a-87f47b064312', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a5533127-aea0-48de-82af-8886827fc1c5', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Site Drainage', N'', NULL, NULL, 71, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'a5533127-aea0-48de-82af-8886827fc1c5', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'65421aa4-89fe-412d-8d42-8e9ac05c1420', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Sheet metal', N'', NULL, NULL, 58, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'65421aa4-89fe-412d-8d42-8e9ac05c1420', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'740aa1d7-7113-43ed-ab01-902eb8e9e603', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Precast Contractor', N'', NULL, NULL, 55, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'740aa1d7-7113-43ed-ab01-902eb8e9e603', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0115d846-31a3-405e-8c23-92906df2ef77', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Elevator Contractor', N'', NULL, NULL, 31, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'0115d846-31a3-405e-8c23-92906df2ef77', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'47269347-037d-41db-bf70-93d49c36a275', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Internal Railings Contractor', N'', NULL, NULL, 50, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'47269347-037d-41db-bf70-93d49c36a275', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'92883fec-dfdf-4bf3-a033-94f129c42272', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Countertop Material', N'Owner to Provide. Supplier Moda in San Clemente, Deniz- 949-244-4839', NULL, NULL, 21, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'92883fec-dfdf-4bf3-a033-94f129c42272', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'175a03b2-565e-4eb3-a5d7-9728befbe82e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Pool/Spa', N'', NULL, NULL, 69, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'175a03b2-565e-4eb3-a5d7-9728befbe82e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'265a03b2-565e-4eb3-a5d7-9728befbe82e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Retaining Wall', N'', NULL, NULL, 70, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'265a03b2-565e-4eb3-a5d7-9728befbe82e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'9b2bba0c-90bc-4892-a064-98d8d5838182', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Flooring Material', N'', NULL, NULL, 30, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'9b2bba0c-90bc-4892-a064-98d8d5838182', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'86c74224-0170-400c-bd74-99a4146229af', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Landscape', N'', NULL, NULL, 68, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'86c74224-0170-400c-bd74-99a4146229af', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'063d6c29-2430-40de-bb2b-9a83941120cd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Material Delivery', N'', NULL, NULL, 52, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'063d6c29-2430-40de-bb2b-9a83941120cd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a1b01fd2-872e-4a6b-a834-9f230342a1c1', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Insulation Contractor', N'', NULL, NULL, 47, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'a1b01fd2-872e-4a6b-a834-9f230342a1c1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd2c47096-9af5-44d1-8cd2-a0ecee5a8c42', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Structural Steel', N'', NULL, NULL, 66, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd2c47096-9af5-44d1-8cd2-a0ecee5a8c42', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ae56314b-7ac7-49fe-a2e0-a53adbfc6332', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Finish Material', N'', NULL, NULL, 29, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'ae56314b-7ac7-49fe-a2e0-a53adbfc6332', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0a5e9f89-2d1d-40b9-8244-a65f94cc02a3', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Temp Utilities', N'', NULL, NULL, 7, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'0a5e9f89-2d1d-40b9-8244-a65f94cc02a3', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c5ff414e-8f3d-498b-bfc3-a9fc0bb7bd4b', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Electrical Fixtures', N'Owner to Provide: Sconces, Pendants, Fans, Chandeliers', NULL, NULL, 23, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'c5ff414e-8f3d-498b-bfc3-a9fc0bb7bd4b', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)


INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'cee94ee2-f3d6-4f63-8b63-ae0ff14313eb', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Shower Doors Contractor', N'', NULL, NULL, 59, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'cee94ee2-f3d6-4f63-8b63-ae0ff14313eb', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a1c3e88f-e7ec-46d1-adf8-b38997770c31', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Foundation Contractor', N'', NULL, NULL, 40, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'a1c3e88f-e7ec-46d1-adf8-b38997770c31', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'62022341-ec48-4087-aa56-b4d120100ca6', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Demolition', N'', NULL, NULL, 6, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'62022341-ec48-4087-aa56-b4d120100ca6', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'01282e22-a193-48be-8508-b6e4055459a2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'General Contractor Contingency', N'', NULL, NULL, 77, N'a5da1985-1858-49e8-abb8-558efc382aac', N'01282e22-a193-48be-8508-b6e4055459a2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'746abd5e-956d-459c-9a49-b73ba7d596a8', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Flooring Contractor', N'', NULL, NULL, 39, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'746abd5e-956d-459c-9a49-b73ba7d596a8', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'25b467dd-97f0-4816-b0f6-ba193a7acb51', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Closets', N'', NULL, NULL, 26, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'25b467dd-97f0-4816-b0f6-ba193a7acb51', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1bf7e623-8741-4bfb-a9f2-bd009d658ffc', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Electrical Contractor', N'', NULL, NULL, 30, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'1bf7e623-8741-4bfb-a9f2-bd009d658ffc', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'82599839-ac72-4e9e-82ca-bdb53085c253', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Drywall Contractor', N'', NULL, NULL, 29, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'82599839-ac72-4e9e-82ca-bdb53085c253', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'81fcb8c4-ce90-47e4-bb9b-c01643f4b80f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Special Inspections', N'', NULL, NULL, 64, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'81fcb8c4-ce90-47e4-bb9b-c01643f4b80f', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7b180310-468e-4b59-9337-c2d84b23cb14', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Building permits', N'', NULL, NULL, 3, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'7b180310-468e-4b59-9337-c2d84b23cb14', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7023d8e9-6716-4ee1-9c51-c4041baaf865', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Site Drainage Contractor', N'', NULL, NULL, 62, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'7023d8e9-6716-4ee1-9c51-c4041baaf865', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'edd0ee18-a505-4a74-8adf-c57517fae7f1', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Front Door', N'', NULL, NULL, 31, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'edd0ee18-a505-4a74-8adf-c57517fae7f1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'bae8de79-c74a-4be5-b3eb-cca2d535c2bf', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'BBQ Grill/Firepit', N'', NULL, NULL, 64, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'bae8de79-c74a-4be5-b3eb-cca2d535c2bf', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e3ad07e4-1f16-4d2d-b305-d04fd0b1cbe7', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Tile Contractor', N'', NULL, NULL, 67, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e3ad07e4-1f16-4d2d-b305-d04fd0b1cbe7', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'271aa5bc-f5b2-4f56-94fb-d37d7305da08', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Appliances', N'Owner to Provide', NULL, NULL, 17, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'271aa5bc-f5b2-4f56-94fb-d37d7305da08', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'5b3a00a9-4fc6-42fc-9b77-d73fd784700a', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Property Tax', N'', NULL, NULL, 81, N'a5da1985-1858-49e8-abb8-558efc382aac', N'5b3a00a9-4fc6-42fc-9b77-d73fd784700a', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a44dbb0d-fc97-43d1-b12c-d744428a4df8', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Grading', N'', NULL, NULL, 4, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'a44dbb0d-fc97-43d1-b12c-d744428a4df8', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'af245b53-809c-4531-9f8b-dc7114b0f4b4', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Garage Doors Contractor', N'', NULL, NULL, 42, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'af245b53-809c-4531-9f8b-dc7114b0f4b4', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'cfe6afff-4a8e-47ba-b24a-dff9e6b43e3e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior Doors/Windows', N'"Budget Freindly windows Andersen 100 or Milgard.  Doors- La Cantina higher end and Windor or Milgard for budget friendly."', NULL, NULL, 25, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'cfe6afff-4a8e-47ba-b24a-dff9e6b43e3e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'986cbdc3-be93-406a-babc-e2d14ba882db', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Fireplace  Contractor', N'', NULL, NULL, 38, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'986cbdc3-be93-406a-babc-e2d14ba882db', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'68b3fffd-d37f-481b-8303-e43a00a0799e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Roofing Contractor', N'', NULL, NULL, 56, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'68b3fffd-d37f-481b-8303-e43a00a0799e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'416fdc24-339a-499d-9bde-e4be5ef19ed9', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Heating & Air Contractor', N'', NULL, NULL, 45, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'416fdc24-339a-499d-9bde-e4be5ef19ed9', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'075f528a-c636-4b6e-9cc7-e729d0c0ee9e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Framing Contractor', N'', NULL, NULL, 41, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'075f528a-c636-4b6e-9cc7-e729d0c0ee9e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c207c3cd-494c-462f-903c-e75f8c8a9707', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Finish Carpentry', N'Install: Finish Material, bath accessories, interior doors', NULL, NULL, 36, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'c207c3cd-494c-462f-903c-e75f8c8a9707', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'01a4dd1e-36bf-427e-8f28-e803351e48fe', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior Finish Contractor', N'', NULL, NULL, 33, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'01a4dd1e-36bf-427e-8f28-e803351e48fe', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'2929c384-45bd-494e-9e98-f0e140c99aa0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Temp Services', N'', NULL, NULL, 72, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'2929c384-45bd-494e-9e98-f0e140c99aa0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'6ecf6dbd-171f-4978-857e-f8545d997164', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Shower Enclosure/Mirror Contractor', N'', NULL, NULL, 61, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'6ecf6dbd-171f-4978-857e-f8545d997164', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'6e525d26-883e-402d-94a4-f87d96b7cb52', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Insurance', N'', NULL, NULL, 5, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'6e525d26-883e-402d-94a4-f87d96b7cb52', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'9a0da22f-afd4-4813-b7a5-f94a5d37416b', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Driveway', N'', NULL, NULL, 67, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'9a0da22f-afd4-4813-b7a5-f94a5d37416b', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'21578f57-31ed-420f-bfc1-fd77098a2bb2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Gutters', N'', NULL, NULL, 44, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'21578f57-31ed-420f-bfc1-fd77098a2bb2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'f3832da8-ea6f-436e-9c99-ff65cd5b061e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Land', N'', NULL, NULL, 80, N'a5da1985-1858-49e8-abb8-558efc382aac', N'f3832da8-ea6f-436e-9c99-ff65cd5b061e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)



