CREATE PROC spCDPopulateProposalLinesParentCategories
(
	@ProposalId uniqueidentifier
)
AS 
BEGIN

DECLARE @ParentCategories as TABLE(
	EstimateCategoryId uniqueidentifier,
	Name nvarchar(100),
	Sequence int
)

INSERT INTO @ParentCategories
select DISTINCT
	ec.ID,
	ec.Name,
	ec.Sequence
from ProposalLines pl
inner join EstimateCategories ec
on ec.ID = pl.ParentEstimateCategoryID
where pl.ProposalID = @ProposalId
and ec.ID <> 'a5da1985-1858-49e8-abb8-558efc382aac'

INSERT INTO ProposalLines (ID,Name,ProposalID,EstimateCategoryID,Amount,Sequence,Updated)
select 
	NEWID(),
	Name,
	@ProposalId,
	EstimateCategoryId,
	0,
	Sequence,
	GETDATE()
from @ParentCategories

END
