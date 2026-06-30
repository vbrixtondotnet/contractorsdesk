CREATE PROCEDURE [dbo].[spVendorReport] 
@StartDate DATE,
@EndDate DATE,
@Vendor NVARCHAR(MAX)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END

IF 'All' IN (SELECT value FROM STRING_SPLIT(@Vendor,'|'))
SET @Vendor = NULL

	DECLARE @Vendors TABLE
	(
		DisplayName NVARCHAR(255),
		ContractNo NVARCHAR(255),
		Account NVARCHAR(255),
		Fund NVARCHAR(255),
		CommittmentsUSD DECIMAL(21,9),
		ExpensesUSD DECIMAL(21,9),
		BalanceUSD DECIMAL(21,9),
		CommittmentsEUR DECIMAL(21,9),
		ExpensesEUR DECIMAL(21,9),
		BalanceEUR DECIMAL(21,9),
		Orderby int
	)

	DECLARE @Expenses TABLE
	(
		VendorName NVARCHAR(255),
		Account NVARCHAR(255),
		Memo NVARCHAR(255),
		Fund NVARCHAR(255),
		ExpensesUSD DECIMAL(21,9),
		ExpensesEUR DECIMAL(21,9)
	)

	DECLARE @Commitments TABLE
	(
		VendorName NVARCHAR(255),
		ContractNo NVARCHAR(255),
		Account NVARCHAR(255),
		Fund NVARCHAR(255),
		CommittmentsUSD DECIMAL(21,9),
		CommittmentsEUR DECIMAL(21,9)
	)

	INSERT @Expenses
		SELECT DisplayName, Account, Memo, Fund, SUM(ExpensesUSD) AS ExpensesUSD, SUM(ExpensesEUR) AS ExpensesEUR
		FROM(
				SELECT v.DisplayName , ISNULL(a.AccountNumber,'')+' - '+a.FullyQualifiedName as Account, Memo, (SELECT dbo.getQBClassTopLevel(c.FullyQualifiedName)) AS Fund, CASE WHEN t.Currency ='USD' THEN SUM(ISNULL(t.Amount,0)) ELSE SUM(ISNULL(t.Amount,0)*t.ExchangeRate) END AS ExpensesUSD, CASE WHEN t.Currency ='EUR' THEN SUM(ISNULL(t.Amount,0)) ELSE SUM(ISNULL(t.Amount,0)*0.94) END AS ExpensesEUR
				FROM QBTransactions t
				INNER JOIN QBAccounts a on a.ID = t.AccountID
				INNER JOIN QBVendors v on v.ID = t.VendorID
				INNER JOIN QBClasses c on c.ID = t.ClassID
				WHERE v.DisplayName IN (SELECT CASE WHEN @Vendor IS NULL THEN v.DisplayName  ELSE value END FROM STRING_SPLIT(ISNULL(@Vendor, ''),'|'))
				AND AccountType = 'Expense' 
				AND CAST(TransactionDate AS Date) BETWEEN @StartDate AND @EndDate
				GROUP BY v.DisplayName, ISNULL(a.AccountNumber,'')+' - '+a.FullyQualifiedName, Memo, t.Currency, c.FullyQualifiedName
			)t
		GROUP BY DisplayName, Account, Memo, Fund

	INSERT @Commitments
		SELECT Vendor ,ContractNo, Account, Fund, SUM(ISNULL(CommitmentUSD,0)) as CommitmentUSD, SUM(ISNULL(CommitmentEUR,0)) as CommitmentEUR
		FROM(
				SELECT v.DisplayName AS Vendor ,c.ContractNo, ISNULL(a.AccountNumber,'')+' - '+a.FullyQualifiedName as Account, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) AS Fund, Value1USD AS CommitmentUSD, Value1EUR AS CommitmentEUR
				FROM Contracts c
				INNER JOIN QBAccounts a ON c.Activity = a.ID
				INNER JOIN QBClasses qbc ON qbc.ID = c.Fund1
				INNER JOIN QBVendors v ON v.ID = c.Vendor
				WHERE a.AccountType = 'Expense' --AND CAST(StartDate AS Date) >= @StartDate AND CAST(EndDate AS Date) <= @EndDate 
				AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
				--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
				AND AccountNumber IS NOT NULL
				UNION ALL
				SELECT v.DisplayName AS Vendor , c.ContractNo, ISNULL(a.AccountNumber,'')+' - '+a.FullyQualifiedName as Account, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) AS Fund, Value2USD AS CommitmentUSD, Value2EUR AS CommitmentEUR
				FROM Contracts c
				INNER JOIN QBAccounts a ON c.Activity = a.ID
				INNER JOIN QBClasses qbc on qbc.ID = c.Fund2
				INNER JOIN QBVendors v ON v.ID = c.Vendor
				WHERE a.AccountType = 'Expense' --AND CAST(StartDate AS Date) >= @StartDate AND CAST(EndDate AS Date) <= @EndDate 
				AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
				--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
				AND AccountNumber IS NOT NULL
				UNION ALL
				SELECT v.DisplayName AS Vendor , c.ContractNo, ISNULL(a.AccountNumber,'')+' - '+a.FullyQualifiedName as Account, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) AS Fund, Value3USD AS CommitmentUSD, Value3EUR AS CommitmentEUR
				FROM Contracts c
				INNER JOIN QBAccounts a ON c.Activity = a.ID
				INNER JOIN QBClasses qbc ON qbc.ID = c.Fund3
				INNER JOIN QBVendors v ON v.ID = c.Vendor
				WHERE a.AccountType = 'Expense' --AND CAST(StartDate AS Date) >= @StartDate AND CAST(EndDate AS Date) <= @EndDate 
				AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
				--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
				AND AccountNumber IS NOT NULL
				UNION ALL
				SELECT v.DisplayName AS Vendor , c.ContractNo, ISNULL(a.AccountNumber,'')+' - '+a.FullyQualifiedName as Account, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) AS Fund, Value4USD AS CommitmentUSD, Value4EUR AS CommitmentEUR
				FROM Contracts c
				INNER JOIN QBAccounts a ON c.Activity = a.ID
				INNER JOIN QBClasses qbc ON qbc.ID = c.Fund4
				INNER JOIN QBVendors v ON v.ID = c.Vendor
				WHERE a.AccountType = 'Expense' --AND CAST(StartDate AS Date) >= @StartDate AND CAST(EndDate AS Date) <= @EndDate 
				AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
				--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
				AND AccountNumber IS NOT NULL
			)t1
		WHERE Vendor IN (SELECT CASE WHEN @Vendor IS NULL THEN Vendor  ELSE value END FROM STRING_SPLIT(ISNULL(@Vendor, ''),'|'))
		GROUP BY Vendor ,ContractNo, Account, Fund

	INSERT @Vendors
		SELECT VendorName, ContractNo, Account, Fund, SUM(CommittmentsUSD) AS CommittmentsUSD, SUM(ExpensesUSD) AS ExpensesUSD, SUM(BalanceUSD) AS BalanceUSD, SUM(CommittmentsEUR) AS CommittmentsEUR, SUM(ExpensesEUR) AS ExpensesEUR, SUM(BalanceEUR) AS BalanceEUR, 1 AS OrderBy
		FROM(
				SELECT VendorName, ContractNo, Account, Fund, CommittmentsUSD, ExpensesUSD, ISNULL(CommittmentsUSD, 0) - ISNULL(ExpensesUSD, 0) AS BalanceUSD, CommittmentsEUR, ExpensesEUR, ISNULL(CommittmentsEUR, 0) - ISNULL(ExpensesEUR, 0) AS BalanceEUR
				FROM(
						SELECT ISNULL(c.VendorName, e.VendorName) AS VendorName, c.ContractNo, ISNULL(c.Account, e.Account) AS Account, ISNULL(c.Fund, e.Fund) AS Fund, CommittmentsUSD, ExpensesUSD, CommittmentsEUR, ExpensesEUR
						FROM @Commitments c 
						FULL JOIN @Expenses e on e.Account = c.Account AND e.VendorName = c.VendorName AND e.Fund = c.Fund AND e.Memo = c.ContractNo
					) t
			)e
		GROUP BY VendorName, ContractNo, Account, Fund	

	SELECT 	DisplayName, ContractNo, Account, Fund, CommittmentsUSD, ExpensesUSD, BalanceUSD, CommittmentsEUR, ExpensesEUR, BalanceEUR, Orderby
	FROM(
			SELECT DisplayName, ContractNo, Account, Fund, CommittmentsUSD, ExpensesUSD, BalanceUSD, CommittmentsEUR, ExpensesEUR, BalanceEUR, Orderby
			FROM @Vendors
			UNION ALL
			SELECT DisplayName, ContractNo, Account, 'Total' AS Fund, SUM(CommittmentsUSD) AS CommittmentsUSD, SUM(ExpensesUSD) AS ExpensesUSD, SUM(BalanceUSD) AS BalanceUSD, SUM(CommittmentsEUR) AS CommittmentsEUR, SUM(ExpensesEUR) AS ExpensesEUR, SUM(BalanceEUR) AS BalanceEU, Orderby
			FROM @Vendors
			GROUP BY DisplayName, ContractNo, Account, Orderby
			UNION ALL
			SELECT DisplayName, ContractNo, 'Total' AS Account, '' AS Fund, SUM(CommittmentsUSD) AS CommittmentsUSD, SUM(ExpensesUSD) AS ExpensesUSD, SUM(BalanceUSD) AS BalanceUSD, SUM(CommittmentsEUR) AS CommittmentsEUR, SUM(ExpensesEUR) AS ExpensesEUR, SUM(BalanceEUR) AS BalanceEU, Orderby
			FROM @Vendors
			GROUP BY DisplayName, ContractNo, Orderby
			UNION ALL
			SELECT 'Total' as DisplayName, '' AS ContractNo, '' AS Account, '' AS Fund, SUM(CommittmentsUSD) AS CommittmentsUSD, SUM(ExpensesUSD) AS ExpensesUSD, SUM(BalanceUSD) AS BalanceUSD, SUM(CommittmentsEUR) AS CommittmentsEUR, SUM(ExpensesEUR) AS ExpensesEUR, SUM(BalanceEUR) AS BalanceEU, MAX(Orderby)+1
			FROM @Vendors
		)T
	ORDER BY Orderby, DisplayName, CASE WHEN Account = 'Total' THEN 2 ELSE 1 END, Account, CASE WHEN Fund = 'Total' THEN 2 ELSE 1 END, Fund, ContractNo


END