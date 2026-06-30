CREATE FUNCTION [dbo].[msf_QuarterlyCashBudgetVsExpenses](
				@CurrentYear NVARCHAR(4),
				@Quarters NVARCHAR(10),
				@ID NVARCHAR(255),
				@Level int
				)
                RETURNS @Result TABLE
				(
					AccountID NVARCHAR(255),
					Account NVARCHAR(255),
					QDRPCash DECIMAL(21,9),
					ExpensesUSD DECIMAL(21,9),
					Orderby int
				)
                AS
                BEGIN

					IF @Level IS NULL
					SET @Level = 1

					DECLARE @StartDate DATE, @EndDate DATE
					DECLARE @Quarter NVARCHAR(10)

					DECLARE @AccountTable AccountExpense

					DECLARE @Transactions TABLE(
					AccountID NVARCHAR(255),
					QDRPCash DECIMAL(21,9),
					ExpensesUSD DECIMAL(21,9)
					)
						
					INSERT @Transactions
						SELECT a.ListID, CASE WHEN AccountType = 'OtherCurrentAsset' THEN Amount END AS QDRPCash, CASE WHEN AccountType = 'Expense' THEN Amount END AS ExpensesUSD
						FROM QBTransactions t
						INNER JOIN QBAccounts a on a.ID = t.AccountID
						WHERE AccountType IN ('OtherCurrentAsset', 'Expense') AND YEAR(TransactionDate) = @CurrentYear
						AND DATEPART(QUARTER, TransactionDate) IN (SELECT CASE WHEN @Quarters IS NULL THEN DATEPART(QUARTER, TransactionDate)  ELSE value END FROM STRING_SPLIT(ISNULL(@Quarters, ''),','))

					INSERT @AccountTable
						SELECT ListID, ParentID, Account, Children
						FROM(
							SELECT DISTINCT a.ListID, ParentID, ISNULL(a.AccountNumber,'')+' - '+a.Name as Account, 
							(SELECT COUNT(*) FROM QBAccounts WHERE ParentID = a.ListID) Children
							FROM QBAccounts a
							LEFT JOIN Contracts c ON CASE WHEN AccountNumber IS NULL THEN FullyQualifiedName ELSE '('+AccountNumber+') '+ FullyQualifiedName END = c.Activity
							LEFT JOIN Budgets b ON CASE WHEN AccountNumber IS NULL THEN FullyQualifiedName ELSE '('+AccountNumber+') '+ FullyQualifiedName END = b.Activity
							WHERE a.AccountType IN ('OtherCurrentAsset', 'Expense')
						)tbl			
					
					DECLARE @CursorTable TABLE
					(
						ID NVARCHAR(255),
						ParentID NVARCHAR(255),
						Account NVARCHAR(255),
						AccountLevel NVARCHAR(255),
						Children int
					)


					;WITH Tree (ListID, ParentID, Account, AccountLevel)
					AS (
					SELECT
						ListID,
						ParentID,
						ISNULL(AccountNumber,'')+' - '+Name AS Account,
						0 AS AccountLevel
					FROM QBAccounts
					WHERE ParentID IS NULL or ParentID = ''

					UNION ALL

					SELECT 
						a.ListID,
						a.ParentID,
						ISNULL(AccountNumber,'')+' - '+Name AS Account,
						t.AccountLevel + 1 AS AccountLevel
					FROM QBAccounts AS a
						JOIN Tree t ON t.ListID = a.ParentID    
					)
					
					INSERT INTO @CursorTable
					SELECT DISTINCT ListID, ParentID, Account, AccountLevel, 0 Children 
					FROM Tree t 
					WHERE t.AccountLevel = @Level AND (ListID IS NOT NULL OR ListID <>'') 
					ORDER BY AccountLevel, Account

					DECLARE @AccountID NVARCHAR(255)
					DECLARE @Account NVARCHAR(255)
					DECLARE @hasChild INT
					DECLARE @Orderby INT = 1
					DECLARE @Section NVARCHAR(255)
					DECLARE db_cursor_account CURSOR FOR 
					SELECT ID, Account FROM @CursorTable
					WHERE ParentID = ISNULL(@ID, ParentID)
					ORDER BY AccountLevel, Account
					OPEN db_cursor_account;
					FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account

					
					WHILE @@FETCH_STATUS = 0  
						BEGIN
							IF (SELECT COUNT(AccountID) FROM @Transactions WHERE AccountID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))) IS NULL
							BEGIN
							FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account
							CONTINUE;
							END

							SET @hasChild = (SELECT COUNT(*) FROM QBAccounts a WHERE ParentID = @AccountID)
							IF @hasChild > 0 
							BEGIN
								INSERT INTO @Result
								SELECT DISTINCT @AccountID, REPLICATE('   ', @Level-1)+@Account+'|' ,
								(SELECT SUM(QDRPCash) FROM @Transactions t WHERE AccountID = @AccountID) AS QDRPCash, 
								(SELECT SUM(ExpensesUSD) FROM @Transactions t WHERE AccountID = @AccountID) AS ExpensesUSD, @Orderby
								FROM @Transactions 
								UNION ALL
								
								SELECT AccountID, Account, QDRPCash, ExpensesUSD, Orderby+@Orderby FROM [dbo].[msf_QuarterlyCashBudgetVsExpenses](@CurrentYear, @Quarters, @AccountID, @Level+1)

								SELECT @Orderby = MAX(Orderby) FROM @Result

								INSERT INTO @Result
								SELECT DISTINCT @AccountID, REPLICATE('   ', @Level-1)+'Total '+@Account,
								SUM(QDRPCash) AS QDRPCash, SUM(ExpensesUSD) AS ExpensesUSD, @Orderby+1
								FROM @Transactions
								WHERE AccountID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))

								SET @Orderby = @Orderby+2
							END
							IF @hasChild = 0
							BEGIN
								INSERT INTO @Result
								SELECT DISTINCT @AccountID, REPLICATE('   ', @Level-1)+@Account,
								SUM(QDRPCash) AS QDRPCash, SUM(ExpensesUSD) AS ExpensesUSD, @Orderby+1
								FROM @Transactions
								WHERE AccountID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))
								

								SET @Orderby = @Orderby+1
							END
							FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account
						END

					CLOSE db_cursor_account;
					DEALLOCATE db_cursor_account;

					RETURN
                END;