CREATE PROCEDURE [dbo].[spTransactionsReport1]
(
    @StartDate DATE,
	@EndDate DATE,
	@FullNameEntity NVARCHAR(MAX),
	@ClearStatus NVARCHAR(100)
)
AS
BEGIN

	IF 'All' IN (SELECT value FROM STRING_SPLIT(@ClearStatus,'|'))
	SET @ClearStatus = NULL

	DECLARE @FullName NVARCHAR(MAX);
	DECLARE @Entity NVARCHAR(MAX);

	SET @FullName = LEFT(@FullNameEntity, CHARINDEX('|', @FullNameEntity) - 1);
	SET @Entity = RIGHT(@FullNameEntity, LEN(@FullNameEntity) - CHARINDEX('|', @FullNameEntity));

	-- Merging @FullName and @Entity
	SET @FullNameEntity = CONCAT(@FullName, ' ', @Entity);

-- Create a temporary table to hold the ordered data
	CREATE TABLE #TempResult (
      Date Date,
      Type NVARCHAR(255),
      Num NVARCHAR(255),
      Name NVARCHAR(600),
      Memo NVARCHAR(600),
      Category NVARCHAR(600),
      SourceAccount NVARCHAR(255),
      Customer NVARCHAR(255),
      Vendor NVARCHAR(255),
      Class NVARCHAR(255),
      [Division/Location] NVARCHAR(255),
	  [Clear Status] NVARCHAR(255),
      Amount DECIMAL(21,2)
    );
    
    CREATE TABLE #SourceAccount
    (
        Date Date,
        TxnID NVARCHAR(255),
        Num NVARCHAR(255),
        Name NVARCHAR(255),
        SourceAccount NVARCHAR(255),
		[Clear Status] NVARCHAR(255)
    );

    INSERT INTO #SourceAccount
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS Num,
           t.Name AS Name,
           a.FullyQualifiedName AS SourceAccount,
		   t.IsCleared AS [Clear Status]
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND t.TransactionDate BETWEEN @StartDate AND @EndDate --AND TxnType <> 'Journal Entry'

	IF @Entity = 'Customer'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount)
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Customer = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Customer = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE IF @Entity = 'Vendor'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount)
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Vendor = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Vendor = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE IF @Entity = 'Class'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount)
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Class = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Class = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount)
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE (Category = @FullName
		OR SourceAccount = @FullName) AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Customer, Vendor, Class, [Division/Location], [Clear Status], Amount
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   '' AS 'Division/Location',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
				AND t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE (Category = @FullName
		OR SourceAccount = @FullName) AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END



	SELECT CONVERT(nvarchar(10), Date, 101) AS Date, Type, CASE WHEN [Clear Status] = 'Missing' THEN '' ELSE [Clear Status] END [Clear Status], Num, Name, [Division/Location], CASE WHEN Customer = 'Missing' THEN '' ELSE Customer END Customer, CASE WHEN Vendor = 'Missing' THEN '' ELSE Vendor END Vendor, CASE WHEN Class = 'Missing' THEN '' ELSE Class END Class, Memo, SourceAccount, Category, Amount,
		SUM(Amount) OVER (ORDER BY Date, Memo, LEN(Num), Num, Name, Category rows between unbounded preceding and current row) AS Balance
	FROM #TempResult t1


	-- Drop the temporary table
    DROP TABLE #SourceAccount   
    DROP TABLE #TempResult
END