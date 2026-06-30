CREATE PROCEDURE [dbo].[spOpenConstructionJobDetailReport] 
@StartDate DATE = NULL,
@EndDate DATE = NULL,
@QBClass NVARCHAR(255)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END


IF 'All' IN (SELECT value FROM STRING_SPLIT(@QBClass,'|'))
SET @QBClass = NULL

DECLARE @AccTable TABLE
(
	Account NVARCHAR(255),
	AccountType NVARCHAR(255),
	Amount DECIMAL(21,9),
	QuickBooksClass NVARCHAR(255)
)

DECLARE @CursorTable TABLE
(
	AccountType NVARCHAR(255),
	Orderby int,
	Level int
)

DECLARE @Table TABLE
(
	Account NVARCHAR(255),
	QuickBooksClass NVARCHAR(255),
	Amount DECIMAL(21,9),
	Orderby int,
	Sortby int
)

CREATE TABLE #StatementTable
(
	Account NVARCHAR(255),
	QuickBooksClass NVARCHAR(255),
	Amount DECIMAL(21,9),
	Total DECIMAL(21,9),
	Orderby int,
	Sortby int
)

DECLARE @DisplayLevel int = 10


INSERT INTO @AccTable
	SELECT Account, AccountType, SUM(Amount) AS Amount, QuickBooksClassShort
	FROM(
			SELECT a.FullyQualifiedName AS Account, CASE WHEN AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' WHEN AccountType = 'OtherIncome' THEN 'Other Income' WHEN AccountType = 'OtherExpense' THEN 'Other Expense' ELSE AccountType END AS AccountType, tc.Amount, qb.FullyQualifiedName AS QuickBooksClassShort
			FROM QBAccounts a
			INNER JOIN QBTransactions tc ON a.ID = tc.AccountID
			INNER JOIN QBClasses qb on qb.ID = tc.ClassID
			WHERE tc.TransactionDate BETWEEN ISNULL(@StartDate, tc.TransactionDate) AND ISNULL(@EndDate, tc.TransactionDate) 
			AND AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.OpenJob = 1
		)tbl
	WHERE QuickBooksClassShort IN (SELECT CASE WHEN @QBClass IS NULL THEN QuickBooksClassShort  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))
	GROUP BY Account, AccountType, QuickBooksClassShort
	


INSERT INTO @CursorTable
SELECT 'Ordinary Income/Expense' AS AccountType, 1 AS OrderBy, 0 AS Level
UNION ALL
SELECT 'Gross Profit' AS AccountType, 4 AS OrderBy, 1 AS Level
UNION ALL
SELECT 'Net Operating Income' AS AccountType, 6 AS OrderBy, 1 AS Level
UNION ALL
SELECT 'Other Income/Expense' AS AccountType, 7 AS OrderBy, 0 AS Level
UNION ALL
SELECT 'Net Other Income' AS AccountType, 10 AS OrderBy, 1 AS Level
UNION ALL
SELECT 'Net Income' AS AccountType, 11 AS OrderBy, 0 AS Level
UNION ALL
SELECT DISTINCT AccountType
, CASE WHEN AccountType = 'Income'			        THEN 2
	   WHEN AccountType = 'Cost Of Goods Sold'      THEN 3
	   WHEN AccountType = 'Expense'			        THEN 5
	   WHEN AccountType = 'Other Income'	        THEN 8
	   WHEN AccountType = 'Other Expense'			THEN 9 END as Orderby
, CASE WHEN AccountType = 'Income'			        THEN 1
	   WHEN AccountType = 'Cost Of Goods Sold'      THEN 1
	   WHEN AccountType = 'Expense'			        THEN 1
	   WHEN AccountType = 'Other Income'	        THEN 1
	   WHEN AccountType = 'Other Expense'			THEN 1 END as Level
FROM @AccTable

DECLARE @Level int
DECLARE @AccType NVARCHAR(255)
DECLARE @AccTypes NVARCHAR(MAX)
DECLARE @Sortby int = 0
SELECT @AccTypes = ISNULL(@AccTypes+',','')+ AccountType FROM @CursorTable
DECLARE db_cursor_acctype CURSOR FOR 
SELECT AccountType, Level FROM @CursorTable
WHERE Orderby IS NOT NULL
ORDER BY Orderby
OPEN db_cursor_acctype;
FETCH NEXT FROM db_cursor_acctype INTO @AccType, @Level
	IF @DisplayLevel > 1
	BEGIN
		WHILE @@FETCH_STATUS = 0  
			BEGIN
				INSERT @Table
				SELECT REPLICATE('          ', @Level)+@AccType, NULL, NULL, NULL, @Sortby 
				WHERE @AccType IN ('Ordinary Income/Expense', 'Income', 'Cost Of Goods Sold', 'Expense', 'Other Income/Expense', 'Other Income', 'Other Expense')
				UNION ALL
				SELECT Account, QuickBooksClass, Amount, Orderby, @Sortby+1 FROM [dbo].[msf_OpenConstructionJobDetailReport](@StartDate, @EndDate, @QBClass, @AccType, NULL, NULL, @DisplayLevel)
				UNION ALL
				SELECT REPLICATE('          ', @Level)+'Total '+@AccType, QuickBooksClass, SUM(Amount), NULL, @Sortby+2
				FROM @AccTable WHERE AccountType = @AccType 
				GROUP BY QuickBooksClass

				IF @AccType = 'Gross Profit'
				BEGIN
					INSERT @Table
					SELECT REPLICATE('          ', @Level)+@AccType, QuickBooksClass, SUM(Amount) as Amount, NULL, @Sortby+3
					FROM(
					SELECT Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Total Income'
					UNION ALL
					SELECT -Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Total Cost of Goods Sold'
					)tbl
					GROUP BY QuickBooksClass
				END

				IF @AccType = 'Net Operating Income'
				BEGIN
					INSERT @Table
					SELECT REPLICATE('          ', @Level)+@AccType, QuickBooksClass, SUM(Amount) as Amount, NULL, @Sortby+3
					FROM(
					SELECT Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Gross Profit'
					UNION ALL
					SELECT -Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Total Expense'
					)tbl
					GROUP BY QuickBooksClass

				END

				IF @AccType = 'Net Income'
				BEGIN
					INSERT @Table
					SELECT REPLICATE('          ', @Level)+@AccType, QuickBooksClass, SUM(Amount) as Amount, NULL, @Sortby+3
					FROM(
					SELECT Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Net Operating Income'
					UNION ALL
					SELECT Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Total Other Income'
					UNION ALL
					SELECT -Amount, QuickBooksClass FROM @Table WHERE LTRIM(Account) = 'Total Other Expense'
					)tbl
					GROUP BY QuickBooksClass
				END
				

				SET @Sortby = @Sortby+4
				FETCH NEXT FROM db_cursor_acctype INTO @AccType, @Level
			END
		END
		CLOSE db_cursor_acctype;
		DEALLOCATE db_cursor_acctype; 

		DECLARE @PivotColumnHeaders VARCHAR(MAX)
		DECLARE @PivotSelectColumnNames AS NVARCHAR(MAX)
		DECLARE @PivotTableSQL NVARCHAR(MAX)

		INSERT INTO #StatementTable
		SELECT Account, QuickBooksClass, Amount, SUM(Amount) OVER (PARTITION BY Account ORDER BY Account), Orderby, Sortby 
		FROM @Table        
		ORDER BY Sortby, Orderby
		
		SELECT @PivotColumnHeaders= ISNULL(@PivotColumnHeaders + '  ,  ','') 
	     + QUOTENAME(QuickBooksClass)
		FROM (
				SELECT DISTINCT QuickBooksClass AS QuickBooksClass
				FROM #StatementTable
			) AS QuickBooksClass
		

		SELECT @PivotSelectColumnNames 
			= ISNULL(@PivotSelectColumnNames + '  ,  ','')
			+ 'ISNULL(' + QUOTENAME(QuickBooksClass) + ', 0) AS '
			+ QUOTENAME(QuickBooksClass)
		FROM (
				SELECT DISTINCT QuickBooksClass AS QuickBooksClass
				FROM #StatementTable
			) AS QuickBooksClass

		--nese dojm me llogarit totalin ne Pivot
		DECLARE @ColumnForSum AS NVARCHAR(MAX)
		SELECT @ColumnForSum = REPLACE(@PivotColumnHeaders,'  ,  ','+')
		SELECT @ColumnForSum = REPLACE(@ColumnForSum,'[','ISNULL([')
		SELECT @ColumnForSum = REPLACE(@ColumnForSum,']','],0)')


		SET @PivotTableSQL = '

				SELECT  Account,  
						QuickBooksClass,
						Amount,
						Total,
						Sortby,
						OrderBy
				FROM (
						SELECT Account, Orderby, Sortby, ' + @PivotSelectColumnNames + ', Total
						FROM(
								SELECT Account, Orderby, Sortby, ' + @PivotColumnHeaders + ', Total
								FROM (
										SELECT Account, QuickBooksClass, Amount, Total, Orderby, Sortby
										FROM #StatementTable
									 ) AS PivotData

								PIVOT	(
											SUM(Amount)
											FOR QuickBooksClass IN (
																	 ' + @PivotColumnHeaders + '
																) 
										) AS PivotTbl
							) sc
						)d

				UNPIVOT
					(
							Amount
							FOR QuickBooksClass IN ('+ @PivotColumnHeaders +')
					) u
			ORDER BY SortBy, OrderBy
			
		';
			--SELECT @sql
		EXECUTE sp_ExecuteSQL  @PivotTableSQL, N'@StartDate DATE,
		@EndDate DATE,
		@QBClass NVARCHAR(255)
		', @StartDate, @EndDate, @QBClass
		
		DROP TABLE #StatementTable

END