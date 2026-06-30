CREATE PROCEDURE [dbo].[spCHAOpenJobsReport] 
@StartDate DATE,
@EndDate DATE,
@QBClass NVARCHAR(255)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END


	IF 'All' IN (SELECT value FROM STRING_SPLIT(@QBClass,'|'))
	SET @QBClass = NULL

	DECLARE @OpenJobs TABLE
	(
		OpenConstructionJobs NVARCHAR(MAX),
		AccountType NVARCHAR(50),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @GrossProfit TABLE
	(
		OpenConstructionJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @NetOperatingIncome TABLE
	(
		OpenConstructionJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @NetIncome TABLE
	(
		OpenConstructionJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @QBCHA9100 TABLE
	(
		OpenConstructionJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	INSERT @QBCHA9100
		SELECT 'QB CHA 9100 Acct Balance:' AS OpenConstructionJobs, SUM(Amount) AS JobBalance
		FROM QBTransactions T
		INNER JOIN QBAccounts A ON A.ID = T.AccountID
		WHERE A.FullyQualifiedName = 'CHA Const 9100' AND TransactionDate <= @EndDate	

	INSERT @OpenJobs
		SELECT OpenConstructionJobs, AccountType, JobBalance
		FROM(
				SELECT c.FullyQualifiedName AS OpenConstructionJobs, a.AccountType, SUM(Amount) AS JobBalance
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON a.ID = t.AccountID
				INNER JOIN QBClasses c ON c.ID = t.ClassID 
				WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND c.OpenJob = 1
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
				GROUP BY c.FullyQualifiedName, a.AccountType
			)j	
		WHERE OpenConstructionJobs IN (SELECT CASE WHEN @QBClass IS NULL THEN OpenConstructionJobs  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))
		ORDER BY OpenConstructionJobs	

	INSERT @GrossProfit
		SELECT OpenConstructionJobs, SUM(JobBalance) AS JobBalance
		FROM(
				SELECT OpenConstructionJobs, JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'Income'
				UNION ALL
				SELECT OpenConstructionJobs, -JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'CostofGoodsSold'
			) gp
		GROUP BY OpenConstructionJobs

	INSERT @NetOperatingIncome
		SELECT OpenConstructionJobs, SUM(JobBalance) AS JobBalance
		FROM(		
				SELECT OpenConstructionJobs, JobBalance
				FROM @GrossProfit
				UNION ALL
				SELECT OpenConstructionJobs, -JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'Expense'
			) noi
		GROUP BY OpenConstructionJobs


	INSERT @NetIncome
		SELECT OpenConstructionJobs, SUM(JobBalance) AS JobBalance
		FROM(		
				SELECT OpenConstructionJobs, JobBalance
				FROM @NetOperatingIncome
				UNION ALL
				SELECT OpenConstructionJobs, JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'OtherIncome'
				UNION ALL
				SELECT OpenConstructionJobs, -JobBalance
				FROM @OpenJobs
				WHERE AccountType = 'OtherExpense'
			) ni
		GROUP BY OpenConstructionJobs
		

	SELECT 	OpenConstructionJobs, JobBalance, Orderby
	FROM(
			SELECT OpenConstructionJobs, JobBalance, 1 AS Orderby
			FROM @NetIncome
			UNION ALL
			SELECT 'Total Job Balance:' AS OpenConstructionJobs, SUM(JobBalance), 2 AS Orderby
			FROM @NetIncome
			UNION ALL
			SELECT OpenConstructionJobs, JobBalance, 3 AS OrderBy FROM @QBCHA9100
			UNION ALL
			SELECT 'Difference:' AS OpenConstructionJobs, SUM(JobBalance), 4 AS Orderby
			FROM (
					SELECT OpenConstructionJobs, JobBalance, 3 AS OrderBy FROM @QBCHA9100
					UNION ALL
					SELECT 'Total Job Balance' AS OpenConstructionJobs, SUM(-JobBalance), 2 AS Orderby
					FROM @NetIncome
				)d
		)T
	ORDER BY Orderby, OpenConstructionJobs


END