CREATE FUNCTION [dbo].[msf_OpenConstructionJobDetailReport](
				@StartDate DATE = NULL,
				@EndDate DATE = NULL,
				@QBClass NVARCHAR(255),
				@AccType NVARCHAR(255),
				@ID NVARCHAR(255),
				@Level int,
				@DisplayLevel int
				)
                RETURNS @Result TABLE
				(
					Account NVARCHAR(255),
					QuickBooksClass NVARCHAR(255),
					Amount DECIMAL(21,9),
					Orderby int
				)
                AS
                BEGIN

					IF @Level IS NULL
					SET @Level = 1
					
					DECLARE @Transactions TABLE
					(
						AccountID NVARCHAR(255),
						QuickBooksClass NVARCHAR(255),
						Amount DECIMAL(21,9)
					)

					DECLARE @AccTable TABLE
					(
						AccountID NVARCHAR(255),
						ParentAccountID NVARCHAR(255),
						Account NVARCHAR(255),
						Children int,
						AccountType NVARCHAR(255)
					)
					
					DECLARE @CursorTable TABLE
					(
						AccountID NVARCHAR(255),
						ParentAccountID NVARCHAR(255),
						Account NVARCHAR(255),
						AccountLevel NVARCHAR(255),
						Children int
					)

					INSERT @Transactions
					SELECT AccountID, QuickBooksClassShort, SUM(Amount) AS Amount	
					FROM(
							SELECT a.ListID AS AccountID, a.ParentID, tc.Amount, qb.FullyQualifiedName AS QuickBooksClassShort, CASE WHEN AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' WHEN AccountType = 'OtherIncome' THEN 'Other Income' WHEN AccountType = 'OtherExpense' THEN 'Other Expense' ELSE AccountType END AS AccountType
							FROM QBAccounts a
							INNER JOIN QBTransactions tc ON a.ID = tc.AccountID
							FULL JOIN QBClasses qb on qb.ID = tc.ClassID
							WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.OpenJob = 1
							AND tc.TransactionDate BETWEEN ISNULL(@StartDate, tc.TransactionDate) AND ISNULL(@EndDate, tc.TransactionDate) 
					)tbl
					WHERE AccountType = @AccType 
					AND QuickBooksClassShort IN (SELECT CASE WHEN @QBClass IS NULL THEN QuickBooksClassShort  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))
					GROUP BY AccountID, QuickBooksClassShort

					

					INSERT @AccTable
					SELECT AccountID, ParentID, Account, Children, AccountType
					FROM(
							SELECT a.ListID AS AccountID, ParentID, a.FullyQualifiedName AS Account, (SELECT COUNT(*) FROM QBAccounts WHERE ParentID = a.ListID) AS Children, FullyQualifiedName AS FullName , CASE WHEN AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' WHEN AccountType = 'OtherIncome' THEN 'Other Income' WHEN AccountType = 'OtherExpense' THEN 'Other Expense' ELSE AccountType END AS AccountType
							FROM QBAccounts a
							WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
						)tbl
					WHERE AccountType = @AccType

					;WITH Tree (ID, ParentID, AccountLevel)
					AS (
					SELECT
						AccountID,
						ISNULL(ParentAccountID,''),
						1 AS AccountLevel
					FROM @AccTable
					WHERE ParentAccountID IS NULL or ParentAccountID = ''

					UNION ALL

					SELECT 
						a.AccountID,
						a.ParentAccountID,
						t.AccountLevel + 1 AS AccountLevel
					FROM @AccTable AS a
						JOIN Tree t ON t.ID = a.ParentAccountID    
					)
					
					INSERT INTO @CursorTable
					SELECT DISTINCT at.AccountID, ISNULL(at.ParentAccountID,''), Account, AccountLevel, Children
					FROM @AccTable at
					INNER JOIN Tree t ON at.AccountID = t.ID
					WHERE t.AccountLevel = @Level 
					ORDER BY AccountLevel, Account

					DECLARE @AccountID NVARCHAR(255)
					DECLARE @Account NVARCHAR(255)
					DECLARE @hasChild INT
					DECLARE @Orderby INT = 1
					DECLARE @Section NVARCHAR(255)
					DECLARE db_cursor_account CURSOR FOR 
					SELECT AccountID, Account FROM @CursorTable
					WHERE ParentAccountID = @ID OR @ID IS NULL
					ORDER BY AccountLevel, Account
					OPEN db_cursor_account;
					FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account

					
					WHILE @@FETCH_STATUS = 0  
						BEGIN
							IF (SELECT SUM(Amount) FROM @Transactions WHERE AccountID IN (SELECT ID FROM [dbo].[getAccountChildrenID](@AccountID))) IS NULL
							BEGIN
							FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account
							CONTINUE;
							END

							SET @hasChild = (SELECT SUM(Amount) FROM @AccTable a INNER JOIN @Transactions tc ON a.AccountID = tc.AccountID WHERE ParentAccountID IN (SELECT ID FROM [dbo].[getAccountChildrenID](@AccountID)))
							IF @hasChild IS NOT NULL AND @Level < @DisplayLevel
							BEGIN
								INSERT INTO @Result
								SELECT REPLICATE('                    ', @Level)+@Account+'|', QuickBooksClass
								, (SELECT SUM(Amount) FROM @Transactions a WHERE AccountID = @AccountID AND a.QuickBooksClass = t.QuickBooksClass) as Amount, @Orderby
								FROM @Transactions t
								GROUP BY QuickBooksClass

								UNION ALL

								SELECT Account, QuickBooksClass, Amount, Orderby+@Orderby FROM [dbo].[msf_OpenConstructionJobDetailReport](@StartDate, @EndDate, @QBClass, @AccType, @AccountID, @Level+1, @DisplayLevel)

								SELECT @Orderby = MAX(Orderby) FROM @Result

								INSERT INTO @Result
								SELECT REPLICATE('                    ', @Level)+'Total '+@Account+'|' , QuickBooksClass
								, (SELECT SUM(Amount) FROM @Transactions a WHERE AccountID IN (SELECT ID FROM [dbo].[getAccountChildrenID](@AccountID)) AND a.QuickBooksClass = t.QuickBooksClass), @Orderby+1
								FROM @Transactions t
								GROUP BY QuickBooksClass

								SET @Orderby = @Orderby+2
							END
							IF @hasChild IS NULL OR @hasChild = 0
							BEGIN
								INSERT INTO @Result
								SELECT REPLICATE('               ', @Level)+@Account, QuickBooksClass
								, (SELECT SUM(Amount) FROM @Transactions a WHERE AccountID IN (SELECT ID FROM [dbo].[getAccountChildrenID](@AccountID)) AND a.QuickBooksClass = t.QuickBooksClass), @Orderby
								FROM @Transactions t
								GROUP BY QuickBooksClass

								SET @Orderby = @Orderby+1
							END
							FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account
						END
					CLOSE db_cursor_account;
					DEALLOCATE db_cursor_account;

					RETURN
                END;