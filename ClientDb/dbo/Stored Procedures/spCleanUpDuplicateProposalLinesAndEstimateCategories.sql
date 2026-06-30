CREATE PROC spCleanUpDuplicateProposalLinesAndEstimateCategories
(@ProposalId uniqueidentifier)
AS
BEGIN

DECLARE @DuplicateProposalLines TABLE
(
	ID uniqueidentifier,
	EstimateCategoryId uniqueidentifier,
	Name nvarchar(150),
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
    Existence = COUNT(*) OVER (PARTITION BY pl.Name),
	(Select TOP 1 ID from EstimateMappings where EstimateSubCategoryID = pl.EstimateCategoryID),
	(Select TOP 1 ID from ScheduleTaskMappings where EstimateCategoryID = pl.EstimateCategoryID)
FROM 
    ProposalLines pl
WHERE 
    ProposalID = @ProposalId
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
