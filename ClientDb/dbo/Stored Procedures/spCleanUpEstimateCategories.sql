
CREATE PROC spCleanUpEstimateCategories
AS
BEGIN
DELETE FROM ScheduleTaskMappings
where EstimateCategoryId in (
SELECT ID from (
select e.ID, e.Name, ep.ID as ParentCategoryId
from EstimateCategories e
left join EstimateCategories ep
on ep.ID = e.ParentEstimateCategoryID
where e.ParentEstimateCategoryID IS NOT NULL
) sq1
where ParentCategoryId IS NULL
)

DELETE FROM ProposalLines where EstimateCategoryID IN (
SELECT ID from (
select e.ID, e.Name, ep.ID as ParentCategoryId
from EstimateCategories e
left join EstimateCategories ep
on ep.ID = e.ParentEstimateCategoryID
where e.ParentEstimateCategoryID IS NOT NULL
) sq1
where ParentCategoryId IS NULL
)

DELETE FROM EstimateCategories where ID IN (
SELECT ID from (
select e.ID, e.Name, ep.ID as ParentCategoryId
from EstimateCategories e
left join EstimateCategories ep
on ep.ID = e.ParentEstimateCategoryID
where e.ParentEstimateCategoryID IS NOT NULL
) sq1
where ParentCategoryId IS NULL
)

DELETE FROM EstimateMappings
WHERE NOT EXISTS (
    SELECT 1 
    FROM EstimateCategories 
    WHERE EstimateCategories.ID = EstimateMappings.EstimateSubCategoryID
);
END