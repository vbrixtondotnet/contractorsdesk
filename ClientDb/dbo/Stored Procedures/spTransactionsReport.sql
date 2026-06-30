CREATE PROCEDURE [dbo].[spTransactionsReport]
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
	  [Clear Status] NVARCHAR(255),
	  Amount DECIMAL(21,2),
	  Class NVARCHAR(255),
	  Division NVARCHAR(255),
      Category NVARCHAR(600),
	  Memo NVARCHAR(600),
	  [Customer:Payee] NVARCHAR(255)
    );
    
    CREATE TABLE #SourceAccount
    (
        Date Date,
        TxnID NVARCHAR(255),
        Num NVARCHAR(255),
        Name NVARCHAR(255),
        SourceAccount NVARCHAR(255),
        AccountType NVARCHAR(255),
		Division NVARCHAR(255),
		[Clear Status] NVARCHAR(255)
    );

    INSERT INTO #SourceAccount
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS Num,
           t.Name AS Name,
           a.FullyQualifiedName AS SourceAccount,
		   a.AccountType,
		   t.Location AS Division,
		   t.IsCleared AS [Clear Status]
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND t.TransactionDate BETWEEN @StartDate AND @EndDate --AND TxnType <> 'Journal Entry'

	IF @Entity = 'Customer'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, [Customer:Payee])
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, Customer AS [Customer:Payee]
		FROM(
				SELECT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Customer = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, Customer AS [Customer:Payee]
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Customer = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE IF @Entity = 'Vendor'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, [Customer:Payee])
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, Vendor AS [Customer:Payee]
		FROM(
				SELECT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Vendor = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, Vendor AS [Customer:Payee]
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   a.FullyQualifiedName AS 'Category',
					   tr.SourceAccount,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Vendor = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE IF @Entity = 'Class'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, [Customer:Payee])
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, CASE WHEN Customer ='Missing' AND Vendor <> 'Missing' THEN Vendor WHEN Customer <> 'Missing' AND Vendor = 'Missing' THEN Customer WHEN Vendor <> 'Missing' AND Customer <> 'Missing' THEN Customer+'/'+Vendor ELSE '' END AS [Customer:Payee]
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
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Class = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, CASE WHEN Customer ='Missing' AND Vendor <> 'Missing' THEN Vendor WHEN Customer <> 'Missing' AND Vendor = 'Missing' THEN Customer WHEN Vendor <> 'Missing' AND Customer <> 'Missing' THEN Customer+'/'+Vendor ELSE '' END AS [Customer:Payee]
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
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate
			) t
		WHERE Class = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE IF @Entity = 'Bank'
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, [Customer:Payee])
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, CASE WHEN Customer ='Missing' AND Vendor <> 'Missing' THEN Vendor WHEN Customer <> 'Missing' AND Vendor = 'Missing' THEN Customer WHEN Vendor <> 'Missing' AND Customer <> 'Missing' THEN Customer+'/'+Vendor ELSE '' END AS [Customer:Payee]
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
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate AND tr.AccountType = @Entity AND A.AccountType <> 'Bank'
			) t
		WHERE (SourceAccount = @FullName) AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, CASE WHEN Customer ='Missing' AND Vendor <> 'Missing' THEN Vendor WHEN Customer <> 'Missing' AND Vendor = 'Missing' THEN Customer WHEN Vendor <> 'Missing' AND Customer <> 'Missing' THEN Customer+'/'+Vendor ELSE '' END AS [Customer:Payee]
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   tr.SourceAccount,
					   a.FullyQualifiedName AS 'Category',
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN t.IsCleared IS NULL OR t.IsCleared ='' THEN 'Missing' ELSE t.IsCleared END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate AND tr.AccountType = @Entity AND A.AccountType <> 'Bank'
			) t
		WHERE Category = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END
	ELSE 
	BEGIN
		INSERT INTO #TempResult (Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, [Customer:Payee])
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, CASE WHEN Customer ='Missing' AND Vendor <> 'Missing' THEN Vendor WHEN Customer <> 'Missing' AND Vendor = 'Missing' THEN Customer WHEN Vendor <> 'Missing' AND Customer <> 'Missing' THEN Customer+'/'+Vendor ELSE '' END AS [Customer:Payee]
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
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN tr.[Clear Status] IS NULL OR tr.[Clear Status] ='' THEN 'Missing' ELSE tr.[Clear Status] END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate AND a.AccountType = @Entity AND a.AccountType <>'Bank'
			) t
		WHERE (SourceAccount = @FullName) AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 
		AND Type <> 'Journal Entry'
		UNION ALL
		SELECT Date, Type, Num, [Clear Status] ,Amount, Class, Division, Category, Memo, CASE WHEN Customer ='Missing' AND Vendor <> 'Missing' THEN Vendor WHEN Customer <> 'Missing' AND Vendor = 'Missing' THEN Customer WHEN Vendor <> 'Missing' AND Customer <> 'Missing' THEN Customer+'/'+Vendor ELSE '' END AS [Customer:Payee]
		FROM(
				SELECT DISTINCT t.TransactionDate AS Date,
					   t.TxnType AS Type,
					   t.TxnNumber AS Num,
					   t.Name,
					   t.Memo,
					   tr.SourceAccount,
					   a.FullyQualifiedName AS 'Category',
					   CASE WHEN t.CustomerID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(cu.FullName AS NVARCHAR(255)) END AS Customer,
					   CASE WHEN t.VendorID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(v.DisplayName AS NVARCHAR(255)) END AS Vendor,
					   CASE WHEN t.ClassID IS NULL  THEN CAST('Missing' AS NVARCHAR(100)) ELSE CAST(c.FullyQualifiedName AS NVARCHAR(255)) END AS Class,
					   COALESCE(tr.Division, t.Location) AS 'Division',
					   CASE WHEN t.IsCleared IS NULL OR t.IsCleared ='' THEN 'Missing' ELSE t.IsCleared END AS [Clear Status],
					   CASE WHEN (a.AccountType IN ('CostofGoodsSold','Expense','OtherExpense')) THEN -t.Amount ELSE t.Amount END Amount 
				FROM QBAccounts a
				FULL JOIN QBTransactions t ON t.AccountID = a.ID 
				FULL JOIN QBClasses c ON c.ID = t.ClassID
				FULL JOIN QBCustomers cu ON cu.ID = t.CustomerID
				FULL JOIN QBVendors v ON v.ID = t.VendorID
				FULL JOIN #SourceAccount tr ON tr.TxnID = t.TxnID AND tr.Num = t.TxnNumber AND t.Name = tr.Name AND t.Location = tr.Division
				WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate AND a.AccountType = @Entity AND a.AccountType <>'Bank'
			) t
		WHERE Category = @FullName AND Date IS NOT NULL
		AND [Clear Status] IN (SELECT CASE WHEN @ClearStatus IS NULL THEN [Clear Status] ELSE value END FROM STRING_SPLIT(ISNULL(@ClearStatus, ''),'|')) 	
		AND Type = 'Journal Entry'
	END


	SELECT CONVERT(nvarchar(10), Date, 101) AS Date, Type, Num, CASE WHEN [Clear Status] = 'Missing' THEN '' ELSE [Clear Status] END [Clear Status], 
	Amount, Class, Division, Category, Memo, [Customer:Payee],
		SUM(Amount) OVER (ORDER BY Date, Memo, LEN(Num), Num, Category, [Customer:Payee] rows between unbounded preceding and current row) AS Balance
	FROM #TempResult t1


	-- Drop the temporary table
    DROP TABLE #SourceAccount   
    DROP TABLE #TempResult
END