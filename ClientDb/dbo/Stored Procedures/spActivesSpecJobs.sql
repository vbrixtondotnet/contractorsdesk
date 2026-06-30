CREATE PROCEDURE [dbo].[spActivesSpecJobs] 
@StartDate NVARCHAR(255),
@EndDate NVARCHAR(255),
@QBClass NVARCHAR(255)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END


	IF 'All' IN (SELECT value FROM STRING_SPLIT(@QBClass,'|'))
	SET @QBClass = NULL

	DECLARE @ActiveJobs TABLE
	(
		ActiveSpecJobs NVARCHAR(MAX),
		AccountType NVARCHAR(50),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @GrossProfit TABLE
	(
		ActiveSpecJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @NetOperatingIncome TABLE
	(
		ActiveSpecJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @NetIncome TABLE
	(
		ActiveSpecJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	DECLARE @QBCHA9112 TABLE
	(
		ActiveSpecJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2)
	)

	INSERT @QBCHA9112
		SELECT 'QB CHA 9112 Acct Balance:' AS ActiveSpecJobs, SUM(Amount) AS JobBalance
		FROM QBTransactions T
		INNER JOIN QBAccounts A ON A.ID = T.AccountID
		WHERE A.FullyQualifiedName = 'CHA and Associates 9112' AND TransactionDate <= @EndDate	

	INSERT @ActiveJobs
		SELECT ActiveSpecJobs, AccountType, JobBalance
		FROM(
				SELECT c.FullyQualifiedName AS ActiveSpecJobs, a.AccountType, SUM(Amount) AS JobBalance
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON a.ID = t.AccountID
				INNER JOIN QBClasses c ON c.ID = t.ClassID 
				WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND c.ActiveSpecJobs = 1
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
				GROUP BY c.FullyQualifiedName, a.AccountType
			)j	
		WHERE ActiveSpecJobs IN (SELECT CASE WHEN @QBClass IS NULL THEN ActiveSpecJobs  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))
		ORDER BY ActiveSpecJobs	

	INSERT @GrossProfit
		SELECT ActiveSpecJobs, SUM(JobBalance) AS JobBalance
		FROM(
				SELECT ActiveSpecJobs, JobBalance
				FROM @ActiveJobs
				WHERE AccountType = 'Income'
				UNION ALL
				SELECT ActiveSpecJobs, -JobBalance
				FROM @ActiveJobs
				WHERE AccountType = 'CostofGoodsSold'
			) gp
		GROUP BY ActiveSpecJobs

	INSERT @NetOperatingIncome
		SELECT ActiveSpecJobs, SUM(JobBalance) AS JobBalance
		FROM(		
				SELECT ActiveSpecJobs, JobBalance
				FROM @GrossProfit
				UNION ALL
				SELECT ActiveSpecJobs, -JobBalance
				FROM @ActiveJobs
				WHERE AccountType = 'Expense'
			) noi
		GROUP BY ActiveSpecJobs


	INSERT @NetIncome
		SELECT ActiveSpecJobs, SUM(JobBalance) AS JobBalance
		FROM(		
				SELECT ActiveSpecJobs, JobBalance
				FROM @NetOperatingIncome
				UNION ALL
				SELECT ActiveSpecJobs, JobBalance
				FROM @ActiveJobs
				WHERE AccountType = 'OtherIncome'
				UNION ALL
				SELECT ActiveSpecJobs, -JobBalance
				FROM @ActiveJobs
				WHERE AccountType = 'OtherExpense'
			) ni
		GROUP BY ActiveSpecJobs
		

	SELECT 	ActiveSpecJobs, JobBalance, Orderby
	FROM(
			SELECT ActiveSpecJobs, JobBalance, 1 AS Orderby
			FROM @NetIncome
			UNION ALL
			SELECT 'Total Job Balance:' AS ActiveSpecJobs, SUM(JobBalance), 2 AS Orderby
			FROM @NetIncome
			UNION ALL
			SELECT ActiveSpecJobs, JobBalance, 3 AS OrderBy FROM @QBCHA9112
			UNION ALL
			SELECT 'Difference:' AS ActiveSpecJobs, SUM(JobBalance), 4 AS Orderby
			FROM (
					SELECT ActiveSpecJobs, JobBalance, 3 AS OrderBy FROM @QBCHA9112
					UNION ALL
					SELECT 'Total Job Balance' AS ActiveSpecJobs, SUM(-JobBalance), 2 AS Orderby
					FROM @NetIncome
				)d
		)T
	ORDER BY Orderby, ActiveSpecJobs


END