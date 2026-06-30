CREATE PROCEDURE [dbo].[spPrintJobEstimate] 
    @ProposalID NVARCHAR(255)
AS
BEGIN   


	DECLARE @InitialDeposit DECIMAL(21,9);
	DECLARE @CurrentJobBalance DECIMAL(21,9);
	DECLARE @RequestedAmount DECIMAL(21,9);

    DECLARE @JobEstimate TABLE
    (
		Sequence INT,
		Item NVARCHAR(255),
		Description NVARCHAR(255),
        ParentEstimateCategoryID NVARCHAR(255),
		EstimateCategoryID NVARCHAR(255),
		ProposalLineID NVARCHAR(255),
		Amount DECIMAL(21,9),
		SortBy INT
    )

    DECLARE @RevisedEstimate TABLE
    (
		Sequence INT,
		ProposalLineID NVARCHAR(255),
		EstimateCategoryID NVARCHAR(255),
        ParentEstimateCategoryID NVARCHAR(255),
		EstimateCategory NVARCHAR(255), 
		SubCategory NVARCHAR(255),
		Description NVARCHAR(255),
        Amount DECIMAL(21,9)
    )


	CREATE TABLE #EstimateCategories
	(
        Sequence INT,
		EstimateCategoryID NVARCHAR(255),
		SubCategory NVARCHAR(255),
		ParentEstimateCategoryID NVARCHAR(255),
		Category NVARCHAR(255),
		Level INT
	);

	;WITH CategoryHierarchy AS (
		SELECT 
			Sequence,
			ID,
			Name AS SubCategory,
			ParentEstimateCategoryID,
			Name AS Category,
			0 AS Level
		FROM 
			EstimateCategories
		WHERE 
			ParentEstimateCategoryID IS NULL
    
		UNION ALL
    
		SELECT 
			ec.Sequence,
			ec.ID,
			ec.Name AS SubCategory,
			ec.ParentEstimateCategoryID,
			ch.Category,
			ch.Level + 1
		FROM 
			EstimateCategories ec
			INNER JOIN CategoryHierarchy ch ON ec.ParentEstimateCategoryID = ch.ID
	)
	INSERT INTO #EstimateCategories
	SELECT 
		Sequence, 
		ID AS EstimateCategoryID,
		SubCategory,
		ParentEstimateCategoryID,
		Category,
		Level
	FROM 
		CategoryHierarchy
	--WHERE Level = 0
	ORDER BY 
		Category;


    INSERT @RevisedEstimate
        SELECT EC.Sequence, PE.ID, EC.EstimateCategoryID, EC.ParentEstimateCategoryID, EC.Category, EC.SubCategory, PE.Description, PE.Amount
        FROM Proposals P
		INNER JOIN ProposalLines PE ON PE.ProposalID = P.ID
		INNER JOIN #EstimateCategories EC ON EC.EstimateCategoryID = PE.EstimateCategoryID AND EC.ParentEstimateCategoryID = PE.ParentEstimateCategoryID
		INNER JOIN QBClasses C ON C.ID = P.QBClassId 
		WHERE P.ID = @ProposalID AND PE.Amount > 0.00
		ORDER BY EC.EstimateCategoryID, EC.ParentEstimateCategoryID


    -- Cursor to fetch Category and EstimateSubCategory
    DECLARE @Category NVARCHAR(255)
    DECLARE @CategoryID NVARCHAR(255)

    DECLARE @SortBy INT = 0

    DECLARE category_cursor CURSOR FOR
	SELECT EstimateCategory, ParentEstimateCategoryID 
	FROM(
			SELECT DISTINCT ParentEstimateCategoryID, EstimateCategory
			FROM @RevisedEstimate
		)T
	ORDER BY 
	  CASE 
		WHEN EstimateCategory = 'Preparation' THEN 1 
		WHEN EstimateCategory = 'Material' THEN 2 
		WHEN EstimateCategory = 'Sub Contractors' THEN 3 
		WHEN EstimateCategory = 'Site Work' THEN 4 
		ELSE 5 
  END;
    OPEN category_cursor;
    FETCH NEXT FROM category_cursor INTO @Category, @CategoryID

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Insert Category
        INSERT INTO @JobEstimate 
        SELECT NULL, UPPER(@Category), NULL, @CategoryID, NULL, NULL, NULL, @SortBy
    INSERT INTO @JobEstimate 
    SELECT DISTINCT
        Sequence, '      '+SubCategory, Description, @CategoryID, EstimateCategoryID, ProposalLineID, Amount, @SortBy+1
    FROM @RevisedEstimate
    WHERE ParentEstimateCategoryID = @CategoryID AND EstimateCategory = @Category

	INSERT INTO @JobEstimate
	    SELECT NULL, 'TOTAL '+UPPER(@Category) AS Item, NULL, @CategoryID, NULL, NULL, ISNULL(SUM(Amount),0), @SortBy+2
	    FROM @RevisedEstimate
	    WHERE ParentEstimateCategoryID = @CategoryID AND EstimateCategory = @Category
		

		SET @SortBy = @SortBy+3

        FETCH NEXT FROM category_cursor INTO @Category, @CategoryID;
    END

    CLOSE category_cursor;
    DEALLOCATE category_cursor;

    SELECT Sequence, Item, Description, ParentEstimateCategoryID, EstimateCategoryID, ProposalLineID, Amount, SortBy
	FROM (
			SELECT Sequence, Item, ParentEstimateCategoryID, EstimateCategoryID, ProposalLineID, Description, Amount, SortBy
			FROM @JobEstimate 
			UNION ALL
			SELECT NULL, 'PROJECT TOTALS', NULL, NULL, NULL, NULL, SUM(Amount) AS RevisedEstimate, MAX(SortBy)+1 AS SortBy
			FROM @JobEstimate
			WHERE Item LIKE 'TOTAL%'
	) T
	ORDER BY SortBy, Sequence


    DROP TABLE #EstimateCategories 


END