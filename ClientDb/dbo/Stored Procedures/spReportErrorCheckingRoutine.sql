CREATE PROCEDURE [dbo].[spReportErrorCheckingRoutine] 
AS
BEGIN   
	
	DECLARE @StartDate DATETIME, @EndDate DATETIME;
	--SELECT @StartDate = MIN(TransactionDate) FROM QBTransactions
	SET @StartDate = '06-01-2024'


	SET @EndDate = GETDATE();

	CREATE TABLE #ActiveJobs
	(
		ActiveJobs NVARCHAR(MAX),
		JobBalance DECIMAL(21,2),
		Orderby int
	)

	INSERT INTO #ActiveJobs
	EXEC spActiveConstructionJobs @StartDate, @EndDate, 'All'

	IF((SELECT JobBalance from #ActiveJobs where ActiveJobs = 'Difference:')<>0)
	BEGIN

		SELECT TransactionDate, t.TxnID, TxnType, TxnNumber, IsCleared, CASE WHEN a.AccountType IN ('Income', 'OtherIncome') THEN Amount ELSE -Amount END AS Amount, a.FullyQualifiedName as Account, 
			CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class, a.FullyQualifiedName AS 'Category', Memo, t.Location AS'Division/Location'
			FROM QBTransactions t
			INNER JOIN QBAccounts a on t.AccountID = a.ID
			FULL JOIN QBClasses c on t.ClassID = c.ID
			WHERE(c.ActiveJobs = 0 OR c.ActiveJobs IS NULL)
			AND TransactionDate BETWEEN @StartDate AND @EndDate
			AND c.QBAccountID = (SELECT ID FROM QBAccounts WHERE FullyQualifiedName = 'CHA Const 9100')
			ORDER BY Class
	END
	ELSE BEGIN
		SELECT 'There were no errors for Active Construction Jobs' as Result
	END

	DELETE FROM #ActiveJobs

	INSERT INTO #ActiveJobs
	EXEC spActivesSpecJobs @StartDate, @EndDate, 'All'

	IF((SELECT JobBalance from #ActiveJobs where ActiveJobs = 'Difference:')<>0)
	BEGIN

		SELECT TransactionDate, t.TxnID, TxnType, TxnNumber, IsCleared, CASE WHEN a.AccountType IN ('Income', 'OtherIncome') THEN Amount ELSE -Amount END AS Amount, a.FullyQualifiedName as Account, 
			CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class, a.FullyQualifiedName AS 'Category', Memo, t.Location AS'Division/Location'
			FROM QBTransactions t
			INNER JOIN QBAccounts a on t.AccountID = a.ID
			FULL JOIN QBClasses c on t.ClassID = c.ID
			WHERE (c.ActiveSpecJobs = 0 OR c.ActiveSpecJobs IS NULL)
			AND TransactionDate BETWEEN @StartDate AND @EndDate
			AND c.QBAccountID = (SELECT ID FROM QBAccounts WHERE FullyQualifiedName = 'CHA and Associates 9112')
			ORDER BY Class
	END
	ELSE BEGIN
		SELECT 'There were no errors for Active Spec Jobs' as Result
	END

	DROP TABLE #ActiveJobs
END