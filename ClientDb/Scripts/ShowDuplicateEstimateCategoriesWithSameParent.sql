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
