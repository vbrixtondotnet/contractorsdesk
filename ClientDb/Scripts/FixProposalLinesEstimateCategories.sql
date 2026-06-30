-- THIS SCRIPT IS USED TO FIX THE ESTIMATECATEGORYIDs from the proposal lines that are not found in the ESTIMATE CATEGORIES TABLE
UPDATE t1
SET 
	t1.EstimateCategoryID = t2.EstimateCategoryIDUpdate,
	t1.ParentEstimateCategoryID = t2.ParentEstimateCategoryId
FROM ProposalLines t1
INNER JOIN 
(
	SELECT 
		EstimateCategoryID,
		ISNULL(ShouldBeEstimateCategoryId,EstimateCategoryID) as EstimateCategoryIDUpdate,
		ISNULL(ParentEstimateCategoryID,ShouldBeParentEstimateCategoryID) as ParentEstimateCategoryId
	FROM (select 
	p.EstimateCategoryID,
	c2.ID as ShouldBeEstimateCategoryId,
	p.Name,
	c.ParentEstimateCategoryID,
	c2.ParentEstimateCategoryID as ShouldBeParentEstimateCategoryID
	from ProposalLines p
	left join EstimateCategories c
	on p.EstimateCategoryId = c.Id
	left join EstimateCategories c2
	on p.Name = c2.Name
	where proposalid = '5eb4ce3f-2f0e-4562-aa71-c497e23c41e3'
	) sq1
)
t2 ON t1.EstimateCategoryID = t2.EstimateCategoryID
where proposalid = '5eb4ce3f-2f0e-4562-aa71-c497e23c41e3'