DECLARE @DuplicateEstimateCategories as TABLE
(
	ID uniqueidentifier,
	Name nvarchar(150),
	ParentEstimateCategoryId uniqueidentifier
)

INSERT INTO @DuplicateEstimateCategories
SELECT 
    ec1.ID, 
    ec1.Name, 
    ec1.ParentEstimateCategoryId
FROM 
    EstimateCategories ec1
JOIN 
    (SELECT 
        Name, 
        ParentEstimateCategoryId
     FROM 
        EstimateCategories
     GROUP BY 
        Name, 
        ParentEstimateCategoryId
     HAVING 
        COUNT(ID) > 1) ec2
ON 
    ec1.Name = ec2.Name 
    AND ec1.ParentEstimateCategoryId = ec2.ParentEstimateCategoryId
ORDER BY 
    ec1.Name, 
    ec1.ParentEstimateCategoryId;

SELECT 
	d.ID,
	d.Name,
	d.ParentEstimateCategoryId,
	c.Name
FROM @DuplicateEstimateCategories d
left join ProposalLines pl
on d.ID = pl.EstimateCategoryID
left join Proposals p
on p.ID = pl.ProposalID
left join QBClasses c
on c.ID = p.QBClassId
where c.ActiveJobs = 1 or c.ActiveSpecJobs = 1
order by c.Name
