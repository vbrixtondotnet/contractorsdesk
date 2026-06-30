-- TO DO: Fix Proposal Lines with the correct estimate categoryid
DECLARE @ProposalLineItems AS TABLE(
	ID uniqueidentifier,
	Name nvarchar(150),
	ProposalID uniqueidentifier,
	EstimateCategoryId uniqueidentifier,
	CorrectEstimateCategoryId uniqueidentifier null,
	ParentEstimateCategoryId uniqueidentifier,
	CorrectParentEstimateCategoryId uniqueidentifier null
)
INSERT INTO @ProposalLineItems
select
	pl.ID,
	pl.Name,
	pl.ProposalID,
	pl.EstimateCategoryID,
	(SELECT TOP 1 ID FROM EstimateCategories where LOWER(Name) = LOWER(pl.Name)) as CorrectEstimateCategoryId,
	pl.ParentEstimateCategoryID,
	(SELECT TOP 1 ID FROM EstimateCategories 
		where Id = (SELECT TOP 1 ParentEstimateCategoryID FROM EstimateCategories where LOWER(Name) = LOWER(pl.Name))) as CorrectParentEstimateCategoryID
from ProposalLines pl
left join EstimateCategories ec
on ec.ID = pl.EstimateCategoryID
left join EstimateCategories ecp
on ecp.ID = pl.ParentEstimateCategoryID
where ec.ID IS NULL

UPDATE pl
SET 
	pl.EstimateCategoryID = pl2.CorrectEstimateCategoryId,
	pl.ParentEstimateCategoryID = pl2.CorrectParentEstimateCategoryId
FROM ProposalLines pl
INNER JOIN @ProposalLineItems pl2
on pl.Id = pl2.Id
where pl2.CorrectEstimateCategoryId IS NOT NULL

DELETE FROM @ProposalLineItems
INSERT INTO @ProposalLineItems
select
	pl.ID,
	pl.Name,
	pl.ProposalID,
	pl.EstimateCategoryID,
	(SELECT TOP 1 ID FROM EstimateCategories where LOWER(Name) = LOWER(pl.Name)) as CorrectEstimateCategoryId,
	pl.ParentEstimateCategoryID,
	(SELECT TOP 1 ID FROM EstimateCategories 
		where Id = (SELECT TOP 1 ParentEstimateCategoryID FROM EstimateCategories where LOWER(Name) = LOWER(pl.Name))) as CorrectParentEstimateCategoryID
from ProposalLines pl
left join EstimateCategories ec
on ec.ID = pl.EstimateCategoryID
left join EstimateCategories ecp
on ecp.ID = pl.ParentEstimateCategoryID
where ec.ID IS NULL

--SELECT * FROM @ProposalLineItems 
----where ProposalID = '4061d79b-5b6a-44d2-a32d-08dc5df3a28a'
--order by CorrectEstimateCategoryId

DECLARE @ParentId UNIQUEIDENTIFIER;
IF NOT EXISTS (SELECT ID FROM EstimateCategories WHERE NAME = 'UNCATEGORIZED' AND ParentEstimateCategoryID is null)
BEGIN
	SET @ParentId = NEWID();
	INSERT INTO [dbo].[EstimateCategories]([ID],[Name],[Sequence],[Created],[CreatedBy])
	VALUES (@ParentId,'UNCATEGORIZED',290,GETDATE(),1)
END
ELSE
BEGIN
	SET @ParentId = (SELECT ID FROM EstimateCategories WHERE NAME = 'UNCATEGORIZED' AND ParentEstimateCategoryID is null)
END

DECLARE @MissingCorrectEstimateCategories AS TABLE
(
	ID uniqueidentifier,
	Name nvarchar(150),
	EstimateCategoryId uniqueidentifier,
	CorrectEstimateCategoryId uniqueidentifier null,
	ParentEstimateCategoryId uniqueidentifier,
	CorrectParentEstimateCategoryId uniqueidentifier null
)

DECLARE @TempNewEstimateCategories AS TABLE(
	ID uniqueidentifier,
	Name nvarchar(150),
	ParentEstimateCategoryID uniqueidentifier,
	Sequence int,
	Created datetime,
	CreatedBy int
)

INSERT INTO @MissingCorrectEstimateCategories
SELECT ID, Name,EstimateCategoryId,CorrectEstimateCategoryId,ParentEstimateCategoryId,CorrectParentEstimateCategoryId
from @ProposalLineItems where CorrectEstimateCategoryId IS NULL

INSERT INTO @TempNewEstimateCategories
SELECT NEWID(), Name, @ParentId, 1,GETDATE(),1
FROM (
	SELECT DISTINCT Name from @MissingCorrectEstimateCategories
) sq1

INSERT INTO [dbo].[EstimateCategories]
           ([ID]
           ,[Name]
		   ,[ParentEstimateCategoryID]
           ,[Sequence]
           ,[Created]
           ,[CreatedBy])
SELECT ID,Name,ParentEstimateCategoryID,Sequence,Created,CreatedBy 
FROM @TempNewEstimateCategories

UPDATE t1
SET 
	t1.CorrectEstimateCategoryId = t2.ID,
	t1.CorrectParentEstimateCategoryId = t2.ParentEstimateCategoryID
FROM @MissingCorrectEstimateCategories t1
INNER JOIN @TempNewEstimateCategories t2
on t1.Name = t2.Name

UPDATE t1
SET 
	t1.EstimateCategoryId = t2.CorrectEstimateCategoryId,
	t1.ParentEstimateCategoryId = t2.CorrectParentEstimateCategoryId
FROM ProposalLines t1
INNER JOIN @MissingCorrectEstimateCategories t2
on t1.ID = t2.ID

select
	pl.ID,
	pl.Name,
	pl.ProposalID,
	pl.EstimateCategoryID,
	ec.ID as CorrectEstimateCategoryId,
	pl.ParentEstimateCategoryID,
	ecp.ID as CorrectParentEstimateCategoryId
from ProposalLines pl
left join EstimateCategories ec
on ec.ID = pl.EstimateCategoryID
left join EstimateCategories ecp
on ecp.ID = pl.ParentEstimateCategoryID
where ec.ID IS NULL
order by Name

