CREATE PROCEDURE [dbo].[spVendorComulativeReport] 
@StartDate DATE,
@EndDate DATE
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END

	DECLARE @VendorComulative TABLE
	(
		Vendor NVARCHAR(255),
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
		ExpensesUSD DECIMAL(21,9),
		ExpensesEUR DECIMAL(21,9)
	)

	DECLARE @Commitments TABLE
	(
		VendorName NVARCHAR(255),
		CommittmentsUSD DECIMAL(21,9),
		CommittmentsEUR DECIMAL(21,9)
	)

	INSERT @Expenses
		SELECT DisplayName, SUM(ExpensesUSD) AS ExpensesUSD, SUM(ExpensesEUR) AS ExpensesEUR
		FROM(	
				SELECT v.DisplayName, CASE WHEN t.Currency ='USD' THEN SUM(ISNULL(t.Amount,0)) ELSE SUM(ISNULL(t.Amount,0)*t.ExchangeRate) END AS ExpensesUSD, CASE WHEN t.Currency ='EUR' THEN SUM(ISNULL(t.Amount,0)) ELSE SUM(ISNULL(t.Amount,0)*0.94) END AS ExpensesEUR
				FROM QBTransactions t
				INNER JOIN QBAccounts a on a.ID = t.AccountID
				INNER JOIN QBVendors v on v.ID = t.VendorID
				INNER JOIN QBClasses c on c.ID = t.ClassID
				AND AccountType = 'Expense' 
				AND CAST(TransactionDate AS Date) BETWEEN @StartDate AND @EndDate
				GROUP BY v.DisplayName, t.Currency
			)t
		GROUP BY DisplayName

	INSERT @Commitments
		SELECT v.DisplayName AS Vendor, SUM(c.SumUSD) AS CommittmentsUSD, SUM(c.SumEUR) AS CommittmentsEUR
		FROM Contracts c
		INNER JOIN QBVendors v ON v.ID = c.Vendor
		INNER JOIN QBAccounts a ON c.Activity = a.ID
		WHERE a.AccountType = 'Expense' --AND CAST(StartDate AS Date) >= @StartDate AND CAST(EndDate AS Date) <= @EndDate 
		AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
		--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
		AND AccountNumber IS NOT NULL
		GROUP BY v.DisplayName

	INSERT @VendorComulative
		SELECT VendorName, SUM(CommittmentsUSD) AS CommittmentsUSD, SUM(ExpensesUSD) AS ExpensesUSD, SUM(BalanceUSD) AS BalanceUSD, SUM(CommittmentsEUR) AS CommittmentsEUR, SUM(ExpensesEUR) AS ExpensesEUR, SUM(BalanceEUR) AS BalanceEUR, 1 AS OrderBy
		FROM(
				SELECT VendorName, CommittmentsUSD, ExpensesUSD, ISNULL(CommittmentsUSD, 0) - ISNULL(ExpensesUSD, 0) AS BalanceUSD, CommittmentsEUR, ExpensesEUR, ISNULL(CommittmentsEUR, 0) - ISNULL(ExpensesEUR, 0) AS BalanceEUR
				FROM(
						SELECT ISNULL(c.VendorName, e.VendorName) AS VendorName, c.CommittmentsUSD, e.ExpensesUSD, c.CommittmentsEUR, e.ExpensesEUR
						FROM @Commitments c
						FULL JOIN @Expenses e ON e.VendorName = c.VendorName
					) t
			)e
		GROUP BY VendorName

	SELECT 	Vendor, CommittmentsUSD, ExpensesUSD, BalanceUSD, CommittmentsEUR, ExpensesEUR, BalanceEUR, Orderby
	FROM(
			SELECT Vendor, CommittmentsUSD, ExpensesUSD, BalanceUSD, CommittmentsEUR, ExpensesEUR, BalanceEUR, Orderby
			FROM @VendorComulative
			UNION ALL
			SELECT 'Total' AS Vendor, SUM(CommittmentsUSD) AS CommittmentsUSD, SUM(ExpensesUSD) AS ExpensesUSD, SUM(BalanceUSD) AS BalanceUSD, SUM(CommittmentsEUR) AS CommittmentsEUR, SUM(ExpensesEUR) AS ExpensesEUR, SUM(BalanceEUR) AS BalanceEUR, MAX(Orderby)+1
			FROM @VendorComulative
		)T
	ORDER BY Orderby, Vendor

END