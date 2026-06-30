CREATE PROCEDURE [dbo].[spMonthlyBudgetReport] 
    @StartDate DATE,
    @EndDate DATE,
    @Class NVARCHAR(MAX)
AS
BEGIN   

    IF 'All' IN (SELECT value FROM STRING_SPLIT(@Class,'|'))
    SET @Class = NULL

	DECLARE @InitialDeposit DECIMAL(21,9);
	DECLARE @CurrentJobBalance DECIMAL(21,9);
	DECLARE @RequestedAmount DECIMAL(21,9);

    DECLARE @Results TABLE
    (
        Item NVARCHAR(255),
		Level INT,
        TotalToDate DECIMAL(21,9),
        Estimate DECIMAL(21,9),
		RevisedEstimate DECIMAL(21,9),
        Balance DECIMAL(21,9),
        Percentage DECIMAL(21,9),
		SortBy INT
    )

    DECLARE @MonthlyBudgetReport TABLE
    (
        Item NVARCHAR(255),
		Level INT,
        TotalToDate DECIMAL(21,9),
        Estimate DECIMAL(21,9),
		RevisedEstimate DECIMAL(21,9),
		SortBy INT
    )

    DECLARE @Actual TABLE
    (
        Item NVARCHAR(255),
        SubItem NVARCHAR(255),
        TotalToDate DECIMAL(21,9)
    )

    DECLARE @Estimate TABLE
    (
        Item NVARCHAR(255),
        SubItem NVARCHAR(255),
        Estimate DECIMAL(21,9)
    )

    DECLARE @RevisedEstimate TABLE
    (
        Item NVARCHAR(255),
        SubItem NVARCHAR(255),
        RevisedEstimate DECIMAL(21,9)
    )

    DECLARE @EarliestHistoryAmount TABLE
    (
        EstimateCategoryID NVARCHAR(255),
        ParentEstimateCategoryID NVARCHAR(255),
        ProposalID NVARCHAR(255),
        ProposalLineID NVARCHAR(255),
		EarliestChangeDate DATETIME2,
        Estimate DECIMAL(21,9)
    )

    DECLARE @Merge TABLE
    (
        Item NVARCHAR(255),
        SubItem NVARCHAR(255),
		Level INT,
        TotalToDate DECIMAL(21,9),
		Estimate DECIMAL(21,9),
		RevisedEstimate DECIMAL(21,9)
    )


	CREATE TABLE #SourceBankAccount
    (
        AccountID NVARCHAR(255),
		Date Date,
        TxnID NVARCHAR(255),
        Name NVARCHAR(255)
    );

    CREATE TABLE #Class
    (
        QBClass NVARCHAR(255),
		QBAccountID NVARCHAR(255),
        TotalToDate DECIMAL(21,9)
    );

	CREATE TABLE #EstimateCategories
	(
		ID NVARCHAR(255),
		SubCategory NVARCHAR(255),
		ParentSubCategoryID NVARCHAR(255),
		Category NVARCHAR(255),
		Level INT
	);

	DECLARE @OpenJobs TABLE
	(
		AccountType NVARCHAR(50),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @GrossProfit TABLE
	(
		JobBalance DECIMAL(21,2)
	)

	DECLARE @NetOperatingIncome TABLE
	(
		JobBalance DECIMAL(21,2)
	)

	DECLARE @NetIncome TABLE
	(
		JobBalance DECIMAL(21,2)
	)

	DECLARE @OwnerOposit TABLE
	(
		Amount DECIMAL(21,2)
	)


	INSERT @OpenJobs
		SELECT AccountType, JobBalance
		FROM(
				SELECT c.FullyQualifiedName AS OpenConstructionJobs, a.AccountType, SUM(Amount) AS JobBalance
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON a.ID = t.AccountID
				INNER JOIN QBClasses c ON c.ID = t.ClassID
				WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND c.OpenJob = 1
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
				GROUP BY c.FullyQualifiedName, a.AccountType
			)j	
		WHERE OpenConstructionJobs IN (SELECT CASE WHEN @Class IS NULL THEN OpenConstructionJobs  ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))
		ORDER BY OpenConstructionJobs	

	INSERT @GrossProfit
		SELECT SUM(JobBalance) AS JobBalance
		FROM(
				SELECT JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'Income'
				UNION ALL
				SELECT -JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'CostofGoodsSold'
			) gp

	INSERT @NetOperatingIncome
		SELECT SUM(JobBalance) AS JobBalance
		FROM(		
				SELECT JobBalance
				FROM @GrossProfit
				UNION ALL
				SELECT -JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'Expense'
			) noi

	INSERT @NetIncome
		SELECT SUM(JobBalance) AS JobBalance
		FROM(		
				SELECT JobBalance
				FROM @NetOperatingIncome
				UNION ALL
				SELECT JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'OtherIncome'
				UNION ALL
				SELECT -JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'OtherExpense'
			) ni

	INSERT INTO @OwnerOposit
		SELECT Amount FROM QBAccounts a
		INNER JOIN QBTransactions t on t.AccountID = a.ID
		INNER JOIN QBClasses c on c.id = t.ClassID
	--	WHERE a.FullyQualifiedName like 'Owner Deposit' 
        WHERE a.AccountType = 'Income'
    --    OR a.FullyQualifiedName LIKE 'Owner Deposit'
    --      OR a.FullyQualifiedName LIKE 'CH Holding loan'
    --      OR a.FullyQualifiedName LIKE 'CHA Loan - Income'
    --    OR a.FullyQualifiedName LIKE 'Construction Loan - Trent'
		AND c.FullyQualifiedName IN (SELECT CASE WHEN @Class IS NULL THEN c.FullyQualifiedName  ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))
		AND t.TransactionDate BETWEEN @StartDate AND @EndDate

	SELECT TOP 1 @InitialDeposit = Amount
	FROM @OwnerOposit
	WHERE Amount % 5000 = 0;

	-- Get the Current Job Balance
	SELECT @CurrentJobBalance = JobBalance FROM @NetIncome;

	-- Additional logic for RequestedAmount
	-- IF @InitialDeposit * 0.2 > @CurrentJobBalance
	-- 	SET @RequestedAmount = 0;
	-- ELSE
	-- 	SET @RequestedAmount = @InitialDeposit;

    If @CurrentJobBalance > @InitialDeposit * 0.2 -- N.B. E kam ndryshuar ne 0.2 nga 2
     SET @RequestedAmount = 0;
    ELSE
        SET @RequestedAmount = @InitialDeposit;

    INSERT INTO #SourceBankAccount
    SELECT a.ID,
		   t.TransactionDate AS Date,
           TxnID,
           t.Name AS Name
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND t.TransactionDate  between @StartDate AND @EndDate

	INSERT INTO  #Class
    SELECT QBClass, AccountID, TotalToDate
	FROM(
			SELECT C.FullyQualifiedName AS QBClass, t.AccountID, 
			SUM(Amount) AS TotalToDate
			FROM QBAccounts a
			INNER JOIN QBTransactions t ON t.AccountID = a.ID 
			INNER JOIN QBClasses c ON c.ID = t.ClassID
			INNER JOIN #SourceBankAccount tr ON tr.TxnID = t.TxnID AND t.Name = tr.Name AND tr.Date = t.TransactionDate
			WHERE  TransactionDate between @StartDate AND @EndDate AND c.AllowedForBudgetReports = 1
			GROUP BY C.FullyQualifiedName, t.AccountID
		) t
	WHERE QBClass IN (SELECT CASE WHEN @Class IS NULL THEN QBClass ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))

	;WITH CategoryHierarchy AS (
		SELECT 
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
		ID,
		SubCategory,
		ParentEstimateCategoryID,
		Category,
		Level
	FROM 
		CategoryHierarchy
	--WHERE Level = 0
	ORDER BY 
		Category;

    INSERT @Actual
		SELECT Category, SubCategory, 
	    SUM(TotalToDate) AS TotalToDate
	    FROM (
				SELECT EM.Category, EM.SubCategory, c.TotalToDate
				FROM EstimateMappings E
				INNER JOIN #EstimateCategories EM ON EM.ID = E.EstimateSubCategoryID
				INNER JOIN #Class c ON c.QBAccountID = E.QBAccountID
				WHERE 
					(EM.SubCategory <> 'Other' OR 
					(EM.SubCategory = 'Other' AND E.AccountType IN ('Expenses', 'Other Expense')))
		     )t
		GROUP BY Category, SubCategory

	INSERT @EarliestHistoryAmount
    SELECT 
        PLH.EstimateCategoryID,
        PLH.ParentEstimateCategoryID,
        PLH.ProposalID,
		PLH.ProposalLineID,
        MIN(PLH.ChangeDate) AS EarliestChangeDate,
		Amount
    FROM 
        ProposalLinesHistory PLH
	WHERE ChangeType <> 'DocStatusChanged(Accepted)'
	AND ChangeDate = (SELECT MIN(ChangeDate)
        FROM ProposalLinesHistory
        WHERE EstimateCategoryID = PLH.EstimateCategoryID
							AND ParentEstimateCategoryID = PLH.ParentEstimateCategoryID 
							AND ProposalID = PLH.ProposalID)
    GROUP BY 
        PLH.EstimateCategoryID,
        PLH.ParentEstimateCategoryID,
        PLH.ProposalID,
		PLH.ProposalLineID,
		Amount

    INSERT @Estimate
        SELECT Category, SubCategory, SUM(Estimate) AS Amount
		FROM(
				SELECT EC.Category, EC.SubCategory, CASE 
				WHEN P.DocStatus = 'Accepted' AND EH.ProposalLineID = pl.ID THEN EH.Estimate
				ELSE PL.Amount
				 END AS Estimate
				FROM Proposals P
				INNER JOIN ProposalLines PL ON PL.ProposalID = P.ID
				INNER JOIN #EstimateCategories EC ON EC.ID = PL.EstimateCategoryID 
					AND EC.ParentSubCategoryID = PL.ParentEstimateCategoryID
				LEFT JOIN @EarliestHistoryAmount EH ON EH.EstimateCategoryID = EC.ID
					AND EH.ParentEstimateCategoryID = EC.ParentSubCategoryID
					AND EH.ProposalID = PL.ProposalID 
					AND EH.ProposalLineID = PL.ID
				INNER JOIN QBClasses C ON C.ID = P.QBClassID 
				WHERE c.AllowedForBudgetReports = 1 AND Amount <> 0
				AND C.FullyQualifiedName IN (SELECT CASE WHEN @Class IS NULL THEN C.FullyQualifiedName ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))				
		) t
		GROUP BY Category, SubCategory
		ORDER BY Category, SubCategory	

    INSERT @RevisedEstimate
        SELECT EC.Category, EC.SubCategory, SUM(PE.Amount) AS Estimate
        FROM Proposals P
		INNER JOIN ProposalLines PE ON PE.ProposalID = P.ID
		INNER JOIN #EstimateCategories EC ON EC.ID = PE.EstimateCategoryID
		INNER JOIN QBClasses C ON C.ID = P.QBClassId 
		WHERE c.AllowedForBudgetReports = 1 AND Amount <> 0
		AND C.FullyQualifiedName IN (SELECT CASE WHEN @Class IS NULL THEN C.FullyQualifiedName ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))
		GROUP BY EC.Category, EC.SubCategory
		ORDER BY EC.Category, EC.SubCategory

		
	INSERT INTO @Merge (Item, SubItem, Level, TotalToDate, Estimate, RevisedEstimate)
	SELECT 
		T.Item, 
		T.SubItem, 
		E.Level, 
		ISNULL(T.TotalToDate, 0) AS TotalToDate, 
		ISNULL(T.Estimate, 0) AS Estimate,
		ISNULL(T.RevisedEstimate, 0) AS RevisedEstimate
	FROM (
		SELECT 
			COALESCE(A.Item, E.Item, RE.Item) AS Item, 
			COALESCE(A.SubItem, E.SubItem, RE.SubItem) AS SubItem, 
			SUM(ISNULL(A.TotalToDate, 0)) AS TotalToDate, 
			SUM(ISNULL(E.Estimate, 0)) AS Estimate,
			SUM(ISNULL(RE.RevisedEstimate, 0)) AS RevisedEstimate
		FROM 
			@Actual A
			FULL JOIN @Estimate E ON E.Item = A.Item AND E.SubItem = A.SubItem
			FULL JOIN @RevisedEstimate RE ON RE.Item = A.Item AND RE.SubItem = A.SubItem
		GROUP BY 
			COALESCE(A.Item, E.Item, RE.Item), 
			COALESCE(A.SubItem, E.SubItem, RE.SubItem)
	) T
	INNER JOIN #EstimateCategories E ON E.Category = T.Item AND E.SubCategory = T.SubItem
	ORDER BY 
		E.Level;



    -- Cursor to fetch Category and EstimateSubCategory
    DECLARE @Category NVARCHAR(255)
    DECLARE @SortBy INT = 0

    DECLARE category_cursor CURSOR FOR
	SELECT Item 
	FROM(
			SELECT DISTINCT Item
			FROM @Merge
		)T
	ORDER BY 
	  CASE 
		WHEN Item = 'Preparation' THEN 1 
		WHEN Item = 'Material' THEN 2 
		WHEN Item = 'Sub Contractors' THEN 3 
		WHEN Item = 'Site Work' THEN 4 
		ELSE 5 
  END;
    OPEN category_cursor;
    FETCH NEXT FROM category_cursor INTO @Category

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Insert Category
        INSERT INTO @MonthlyBudgetReport 
        SELECT UPPER(@Category), NULL, NULL, NULL, NULL, @SortBy
        INSERT INTO @MonthlyBudgetReport 
        SELECT 
            '      '+SubItem AS Item, Level, TotalToDate, Estimate, RevisedEstimate, @SortBy+1
        FROM @Merge
        WHERE Item = @Category

		INSERT INTO @MonthlyBudgetReport
        SELECT 'TOTAL '+UPPER(@Category) AS Item, NULL, ISNULL(SUM(TotalToDate),0), ISNULL(SUM(Estimate),0), ISNULL(SUM(RevisedEstimate),0), @SortBy+2
        FROM @Merge
        WHERE Item = @Category
		

		SET @SortBy = @SortBy+3

        FETCH NEXT FROM category_cursor INTO @Category;
    END

    CLOSE category_cursor;
    DEALLOCATE category_cursor;

	-- INSERT @Results
	-- 	SELECT Item, Level, TotalToDate, Estimate, RevisedEstimate, (RevisedEstimate - TotalToDate) AS Balance, (SELECT [dbo].[getPercentage] (ISNULL(TotalToDate,0), ISNULL(RevisedEstimate,0))) AS Percentage, SortBy
	-- 	FROM @MonthlyBudgetReport

INSERT INTO @Results
SELECT 
    Item, 
    Level, 
    TotalToDate, 
    Estimate, 
    RevisedEstimate, 
    -- Calculate Balance: use RevisedEstimate if it's different than 0, otherwise use Estimate
    (CASE 
        WHEN RevisedEstimate <> 0 
            THEN (RevisedEstimate - TotalToDate) 
        ELSE 
            (Estimate - TotalToDate) 
    END) AS Balance,
    -- Calculate Percentage: use RevisedEstimate if it's different than 0, otherwise use Estimate
    (SELECT [dbo].[getPercentage] (
        ISNULL(TotalToDate, 0), 
        CASE 
            WHEN RevisedEstimate <> 0 
                THEN RevisedEstimate 
            ELSE 
                Estimate 
        END
    )) AS Percentage, 
    SortBy
FROM 
    @MonthlyBudgetReport


    SELECT Item, Level, TotalToDate, Estimate, RevisedEstimate, Balance, Percentage, SortBy
	FROM (
    SELECT Item, Level, TotalToDate, Estimate, RevisedEstimate, Balance, Percentage, SortBy
    FROM @Results 
	UNION ALL
    SELECT 'PROJECT TOTALS', MAX(Level+1) AS Level, SUM(TotalToDate), SUM(Estimate), SUM(RevisedEstimate) AS RevisedEstimate, (SUM(Estimate) - SUM(TotalToDate)) AS Balance, (SELECT [dbo].[getPercentage] (ISNULL(SUM(TotalToDate),0), ISNULL(SUM(Estimate),0))) AS Percentage, MAX(SortBy)+1 AS SortBy
    FROM @Results
	WHERE Item LIKE 'TOTAL%'
	UNION ALL
    SELECT 'OWNER DEPOSITS', 1000 AS Level, SUM(Amount), NULL, NULL, NULL, NULL, 1000 AS SortBy
    FROM @OwnerOposit
	UNION ALL
    SELECT 'JOB BALANCE', 2000 AS Level, SUM(JobBalance), NULL, NULL, NULL, NULL, 2000 AS SortBy
    FROM @NetIncome
	UNION ALL
    SELECT 'REQUESTED AMOUNT', 3000, @RequestedAmount, NULL, NULL, NULL, NULL, 30000 AS SortBy
	) T
	ORDER BY SortBy, Level




     DROP TABLE #EstimateCategories 
     DROP TABLE #SourceBankAccount 
	 DROP TABLE #Class


END