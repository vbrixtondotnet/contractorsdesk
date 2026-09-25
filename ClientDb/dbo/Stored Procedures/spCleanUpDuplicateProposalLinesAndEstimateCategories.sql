CREATE PROC spCleanUpDuplicateProposalLinesAndEstimateCategories
(@ProposalId uniqueidentifier)
AS
BEGIN

DECLARE @DuplicateProposalLines TABLE
(
	ID uniqueidentifier,
	EstimateCategoryId uniqueidentifier,
	Name nvarchar(150),
	ParentEstimateCategoryId uniqueidentifier,
	Existence int,
	MappingId uniqueidentifier null,
	ScheduleMappingId uniqueidentifier null
)
DECLARE @UnMappedEstimateCategories TABLE
(
	ID uniqueidentifier,
	EstimateCategoryId uniqueidentifier
)
INSERT INTO @DuplicateProposalLines
SELECT 
    pl.ID,
    pl.EstimateCategoryId,
	pl.Name,
	pl.ParentEstimateCategoryID,
    Existence = COUNT(*) OVER (
		PARTITION BY
			pl.ParentEstimateCategoryID,
			LOWER(LTRIM(RTRIM(pl.Name)))
	),
	(Select TOP 1 ID from EstimateMappings where EstimateSubCategoryID = pl.EstimateCategoryID),
	(Select TOP 1 ID from ScheduleTaskMappings where EstimateCategoryID = pl.EstimateCategoryID)
FROM 
    ProposalLines pl
WHERE 
    ProposalID = @ProposalId
	AND pl.ParentEstimateCategoryID IS NOT NULL
	AND pl.Name IS NOT NULL
	AND LTRIM(RTRIM(pl.Name)) <> ''
ORDER BY 
    pl.Name
INSERT INTO @UnMappedEstimateCategories
SELECT ID,EstimateCategoryId 
from @DuplicateProposalLines where Existence > 1 and MappingId IS NULL and ScheduleMappingId IS NULL

DELETE from EstimateCategories where id in 
(select EstimateCategoryId from @UnMappedEstimateCategories)

DELETE from ProposalLines where ID in
(select ID from @UnMappedEstimateCategories)
and ProposalID = @ProposalId
END
