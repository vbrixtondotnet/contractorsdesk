CREATE PROCEDURE [dbo].[spClassTransactionsReport]
(
    @StartDate DATE,
	@EndDate DATE,
	@Class NVARCHAR(MAX),
	@ClearStatus NVARCHAR(100)
)
AS
BEGIN
	IF 'All' IN (SELECT value FROM STRING_SPLIT(@Class,'|'))
	SET @Class = NULL

	IF 'All' IN (SELECT value FROM STRING_SPLIT(@ClearStatus,'|'))
	SET @ClearStatus = NULL

-- Create a temporary table to hold the ordered data
	CREATE TABLE #TempResultQ1 (
      Date Date,
      Type NVARCHAR(255),
      Num NVARCHAR(255),
      Name NVARCHAR(600),
      Memo NVARCHAR(600),
      Category NVARCHAR(600),
      SourceAccount NVARCHAR(255),
      Class NVARCHAR(255),
      [Division/Location] NVARCHAR(255),
	  [Clear Status] NVARCHAR(255),
      Amount DECIMAL(21,2)
    );
    
    CREATE TABLE #SourceAccountQ1
    (
        Date Date,
        TxnID NVARCHAR(255),
        Num NVARCHAR(255),
        Name NVARCHAR(255),
        SourceAccount NVARCHAR(255),
		[Division/Location] NVARCHAR(255),
		[Clear Status] NVARCHAR(255)
    );

    INSERT INTO #SourceAccountQ1
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS Num,
           t.Name AS Name,
           a.FullyQualifiedName AS SourceAccount,
		   t.Location AS [Division/Location],
		   t.IsCleared AS [Clear Status]
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND t.TransactionDate BETWEEN @StartDate AND @EndDate--AND TxnType <> 'Journal Entry'

    INSERT INTO #TempResultQ1 (Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount)
    SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
	FROM(
			SELECT t.TransactionDate AS Date,
				   t.TxnType AS Type,
				   t.TxnNumber AS Num,
				   t.Name,
				   t.Memo,
				   a.FullyQualifiedName AS 'Category',
				   tr.SourceAccount,
				   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
				   t.Location AS'Division/Location',
				   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
				   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
			FROM QBAccounts a
			FULL JOIN QBTransactions t ON t.AccountID = a.ID 
			FULL JOIN QBClasses c ON c.ID = t.ClassID
			FULL JOIN #SourceAccountQ1 tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND tr.[Division/Location] = t.Location
			WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
		) t
	WHERE Class IN (SELECT CASE WHEN @Class IS NULL THEN Class ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|')) AND Date IS NOT NULL
	AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
	AND Type <> 'Journal Entry'
	UNION ALL
    SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
	FROM(
			SELECT DISTINCT t.TransactionDate AS Date,
				   t.TxnType AS Type,
				   t.TxnNumber AS Num,
				   t.Name,
				   t.Memo,
				   a.FullyQualifiedName AS 'Category',
				   tr.SourceAccount,
				   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
				   t.Location AS 'Division/Location',
				   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
				   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
			FROM QBAccounts a
			FULL JOIN QBTransactions t ON t.AccountID = a.ID 
			FULL JOIN QBClasses c ON c.ID = t.ClassID
			FULL JOIN #SourceAccountQ1 tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND tr.[Division/Location] = t.Location
			WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
		) t
	WHERE Class IN (SELECT CASE WHEN @Class IS NULL THEN Class ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|')) AND Date IS NOT NULL
	AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
	AND Type = 'Journal Entry'




	SELECT CONVERT(nvarchar(10), Date, 101) AS Date, Type, CASE WHEN [Clear Status] = 'Missing' THEN '' ELSE [Clear Status] END [Clear Status], Num, Name, [Division/Location], CASE WHEN Class = 'Missing' THEN '' ELSE Class END Class, Memo, SourceAccount, Category, Amount,
		SUM(Amount) OVER (ORDER BY Date, Memo, LEN(Num), Num, Name, Category rows between unbounded preceding and current row) AS Balance
	FROM #TempResultQ1 t1


	-- Drop the temporary table
    DROP TABLE #SourceAccountQ1    
    DROP TABLE #TempResultQ1
END