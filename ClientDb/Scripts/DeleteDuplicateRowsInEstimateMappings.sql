-- Step 1: Identify duplicates using a CTE and ROW_NUMBER()
WITH DuplicateMappings AS (
    SELECT 
        Id,
        QbAccountId, 
        EstimateSubCategoryId,
        ROW_NUMBER() OVER (PARTITION BY QbAccountId, EstimateSubCategoryId ORDER BY Id) AS RowNum
    FROM 
        EstimateMappings
)

-- Step 2: Delete the duplicate records, retaining only the first one
DELETE FROM EstimateMappings
WHERE Id IN (
    SELECT Id
    FROM DuplicateMappings
    WHERE RowNum > 1
);
