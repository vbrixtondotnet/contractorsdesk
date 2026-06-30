
DECLARE @ProposalTemplateLineItems AS TABLE(
	Id uniqueidentifier,
	Name nvarchar(250),
	EstimateCategoryId uniqueidentifier null,
	CorrectEstimateCategoryId uniqueidentifier null,
	ParentId uniqueidentifier null,
	Parent nvarchar(250) null,
	ParentEstimateCategoryId uniqueidentifier null,
	EstimateCategory nvarchar(250) null,
	ParentEstimateCategory nvarchar(250) null
);

INSERT INTO @ProposalTemplateLineItems
select
	ptl.Id,
	ptl.Name,
	ptl.EstimateCategoryId,
	(SELECT TOP 1 ID FROM EstimateCategories where LOWER(Name) = LOWER(ptl.Name)) as CorrectEstimateCategoryId,
	ptl.ParentId,
	ptlParent.Name as Parent,
	ptlParent.EstimateCategoryId as ParentEstimateCategoryId,
	ec.Name as EstimateCategory,
	ecp.Name as ParentEstimateCategory
from ProposalTemplatesLineItems ptl
left join ProposalTemplatesLineItems ptlParent
on ptl.ParentId = ptlParent.Id
left join EstimateCategories ec
on ptl.EstimateCategoryId = ec.ID
left join EstimateCategories ecp
on ptlParent.EstimateCategoryId = ecp.ID

--FIX CATEGORIES
UPDATE ptli
SET 
	ptli.EstimateCategoryID = t2.CorrectEstimateCategoryId
FROM ProposalTemplatesLineItems ptli
INNER JOIN @ProposalTemplateLineItems t2
on ptli.Id = t2.Id

DELETE FROM @ProposalTemplateLineItems
INSERT INTO @ProposalTemplateLineItems
select
	ptl.Id,
	ptl.Name,
	ptl.EstimateCategoryId,
	(SELECT TOP 1 ID FROM EstimateCategories where LOWER(Name) = LOWER(ptl.Name)) as CorrectEstimateCategoryId,
	ptl.ParentId,
	ptlParent.Name as Parent,
	ptlParent.EstimateCategoryId as ParentEstimateCategoryId,
	ec.Name as EstimateCategory,
	ecp.Name as ParentEstimateCategory
from ProposalTemplatesLineItems ptl
left join ProposalTemplatesLineItems ptlParent
on ptl.ParentId = ptlParent.Id
left join EstimateCategories ec
on ptl.EstimateCategoryId = ec.ID
left join EstimateCategories ecp
on ptlParent.EstimateCategoryId = ecp.ID

--process parents
INSERT INTO [dbo].[EstimateCategories]
           ([ID]
           ,[Name]
           ,[Sequence]
           ,[Created]
           ,[CreatedBy])

SELECT NEWID(), Name, 1,GETDATE(),1
FROM (
	SELECT DISTINCT Name from @ProposalTemplateLineItems
	where ParentId IS NULL and CorrectEstimateCategoryId IS NULL
) sq1

DELETE FROM @ProposalTemplateLineItems
INSERT INTO @ProposalTemplateLineItems
select
	ptl.Id,
	ptl.Name,
	ptl.EstimateCategoryId,
	(SELECT TOP 1 ID FROM EstimateCategories where LOWER(Name) = LOWER(ptl.Name)) as CorrectEstimateCategoryId,
	ptl.ParentId,
	ptlParent.Name as Parent,
	ptlParent.EstimateCategoryId as ParentEstimateCategoryId,
	ec.Name as EstimateCategory,
	ecp.Name as ParentEstimateCategory
from ProposalTemplatesLineItems ptl
left join ProposalTemplatesLineItems ptlParent
on ptl.ParentId = ptlParent.Id
left join EstimateCategories ec
on ptl.EstimateCategoryId = ec.ID
left join EstimateCategories ecp
on ptlParent.EstimateCategoryId = ecp.ID
 
-- process items
INSERT INTO [dbo].[EstimateCategories]
           ([ID]
           ,[Name]
           ,[Sequence]
           ,[Created]
           ,[CreatedBy])

SELECT NEWID(), Name, 1,GETDATE(),1
FROM (
	SELECT DISTINCT Name from @ProposalTemplateLineItems
	where EstimateCategoryId IS NULL and CorrectEstimateCategoryId IS NULL
) sq1

UPDATE ptli
SET 
	ptli.EstimateCategoryID = t2.CorrectEstimateCategoryId
FROM ProposalTemplatesLineItems ptli
INNER JOIN @ProposalTemplateLineItems t2
on ptli.Id = t2.Id

DELETE FROM @ProposalTemplateLineItems
INSERT INTO @ProposalTemplateLineItems
select
	ptl.Id,
	ptl.Name,
	ptl.EstimateCategoryId,
	(SELECT TOP 1 ID FROM EstimateCategories where LOWER(Name) = LOWER(ptl.Name)) as CorrectEstimateCategoryId,
	ptl.ParentId,
	ptlParent.Name as Parent,
	ptlParent.EstimateCategoryId as ParentEstimateCategoryId,
	ec.Name as EstimateCategory,
	ecp.Name as ParentEstimateCategory
from ProposalTemplatesLineItems ptl
left join ProposalTemplatesLineItems ptlParent
on ptl.ParentId = ptlParent.Id
left join EstimateCategories ec
on ptl.EstimateCategoryId = ec.ID
left join EstimateCategories ecp
on ptlParent.EstimateCategoryId = ecp.ID

SELECT * from @ProposalTemplateLineItems order by EstimateCategoryId
