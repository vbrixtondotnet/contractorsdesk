CREATE FUNCTION [dbo].[msf_ActualCommittmentsVsActualDisbursments](
				@StartDate DATE,
				@EndDate DATE,
				@Fund NVARCHAR(255),
				@CPSPermitted NVARCHAR(255),
				@ID NVARCHAR(255),
				@Level int
				)
                RETURNS @Result TABLE
				(
					AccountID NVARCHAR(255),
					Account NVARCHAR(255),
					CommitmentUSD DECIMAL(21,9),
					CommitmentEUR DECIMAL(21,9),
					ExpensesUSD DECIMAL(21,9),
					ExpensesEUR DECIMAL(21,9),
					Orderby int
				)
                AS
                BEGIN

					IF @Level IS NULL
					SET @Level = 0

					DECLARE @Transactions TransactionsACAD
					DECLARE @AccountTable AccountExpense
	
					--DELETE FROM @Transactions
					--DELETE FROM @AccountTable
					DECLARE @Expenses TABLE
					(
						AccountID NVARCHAR(255),
						ExpensesUSD DECIMAL(21,9),
						ExpensesEUR DECIMAL(21,9)
					)

					DECLARE @Commitments TABLE
					(
						AccountID NVARCHAR(255),
						CommittmentsUSD DECIMAL(21,9),
						CommittmentsEUR DECIMAL(21,9)
					)

					INSERT @Expenses
						SELECT AccountID, SUM(ExpensesUSD) AS ExpensesUSD, SUM(ExpensesEUR) AS ExpensesEUR
						FROM(
								SELECT a.ListID AS AccountID,
								(SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) as TopClass, qbc.FullyQualifiedName AS Fund
								, t.Amount * CASE WHEN t.Currency ='USD' THEN 1 ELSE t.ExchangeRate END AS ExpensesUSD
								, CASE WHEN t.Currency ='EUR' THEN ISNULL(t.Amount,0) ELSE ISNULL(t.Amount,0)*0.94 END AS ExpensesEUR
								FROM QBTransactions T
								INNER JOIN QBAccounts A ON A.ID = T.AccountID 
								INNER JOIN QBClasses qbc on qbc.id = t.ClassID
								WHERE AccountType = 'Expense' AND CAST(TransactionDate AS Date) BETWEEN @StartDate AND @EndDate AND AccountNumber IS NOT NULL	
							)e
						WHERE TopClass IN (SELECT CASE WHEN @Fund IS NULL THEN TopClass  ELSE value END FROM STRING_SPLIT(ISNULL(@Fund, ''),'|'))
						AND Fund IN (SELECT CASE WHEN @CPSPermitted IS NULL THEN Fund  ELSE value END FROM STRING_SPLIT(ISNULL(@CPSPermitted, ''),'|'))
						GROUP BY AccountID

					INSERT @Commitments
					SELECT ID, SUM(CommitmentUSD) AS CommitmentUSD, SUM(CommitmentEUR) AS CommitmentEUR
					FROM(
							SELECT ID, Fund, TopClass, ISNULL(CommitmentUSD,0) as CommitmentUSD, ISNULL(CommitmentEUR,0) as CommitmentEUR
							FROM(
									SELECT a.ListID as ID, ISNULL(qbc.FullyQualifiedName,'') AS Fund, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) as TopClass, Value1USD AS CommitmentUSD, Value1EUR AS CommitmentEUR
									FROM QBAccounts a
									INNER JOIN Contracts c ON c.Activity = a.ID
									INNER JOIN QBClasses qbc on qbc.ID = c.Fund1
									WHERE a.AccountType = 'Expense' 
									AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
									--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
									AND AccountNumber IS NOT NULL
									UNION ALL
									SELECT a.ListID as ID, ISNULL(qbc.FullyQualifiedName,'') AS Fund, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) as TopClass, Value2USD AS CommitmentUSD, Value2EUR AS CommitmentEUR
									FROM QBAccounts a
									INNER JOIN Contracts c ON c.Activity = a.ID
									INNER JOIN QBClasses qbc on qbc.ID = c.Fund2
									WHERE a.AccountType = 'Expense' 
									AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
									--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
									AND AccountNumber IS NOT NULL
									UNION ALL
									SELECT a.ListID as ID, ISNULL(qbc.FullyQualifiedName,'') AS Fund, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) as TopClass, Value3USD AS CommitmentUSD, Value3EUR AS CommitmentEUR
									FROM QBAccounts a
									INNER JOIN Contracts c ON c.Activity = a.ID
									INNER JOIN QBClasses qbc on qbc.ID = c.Fund3
									WHERE a.AccountType = 'Expense' 
									AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
									--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
									AND AccountNumber IS NOT NULL
									UNION ALL
									SELECT a.ListID as ID, ISNULL(qbc.FullyQualifiedName,'') AS Fund, (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) as TopClass, Value4USD AS CommitmentUSD, Value4EUR AS CommitmentEUR
									FROM QBAccounts a
									INNER JOIN Contracts c ON c.Activity = a.ID
									INNER JOIN QBClasses qbc on qbc.ID = c.Fund4
									WHERE a.AccountType = 'Expense' 
									AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
									--AND (CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate) OR (CAST(StartDate AS Date)<= @StartDate AND CAST(EndDate AS Date)>= @EndDate)OR (CAST(EndDate AS Date) BETWEEN @StartDate AND @EndDate)
									AND AccountNumber IS NOT NULL
								)t1
								WHERE TopClass IN (SELECT CASE WHEN @Fund IS NULL THEN TopClass  ELSE value END FROM STRING_SPLIT(ISNULL(@Fund, ''),'|'))
								AND Fund IN (SELECT CASE WHEN @CPSPermitted IS NULL THEN Fund  ELSE value END FROM STRING_SPLIT(ISNULL(@CPSPermitted, ''),'|'))
							)t2
						GROUP BY ID

					INSERT @Transactions
						SELECT ISNULL(c.AccountID, e.AccountID) AS AccountID, CommittmentsUSD, CommittmentsEUR, ExpensesUSD, ExpensesEUR
						FROM @Commitments c 
						FULL JOIN @Expenses e on e.AccountID = c.AccountID 

					INSERT @AccountTable
					SELECT ID, ParentID, Account, Children
					FROM(
						SELECT a.ListID as ID, ParentID,ISNULL(a.AccountNumber,'')+' - '+a.Name as Account, 
						(SELECT COUNT(*) FROM QBAccounts WHERE ParentID = a.ListID) Children
						FROM QBAccounts a
						FULL JOIN Contracts c ON a.ID = c.Activity
						WHERE a.AccountType = 'Expense' AND AccountNumber IS NOT NULL
					)tbl			
					
					DECLARE @CursorTable TABLE
					(
						ListID NVARCHAR(255),
						ParentID NVARCHAR(255),
						Account NVARCHAR(255),
						AccountLevel NVARCHAR(255),
						Children int
					)


					;WITH Tree (ID, ParentID, AccountLevel)
					AS (
					SELECT
						ID,
						ISNULL(ParentID,''),
						0 AS AccountLevel
					FROM @AccountTable
					WHERE ParentID IS NULL or ParentID = ''

					UNION ALL

					SELECT 
						a.ID,
						a.ParentID,
						t.AccountLevel + 1 AS AccountLevel
					FROM @AccountTable AS a
						JOIN Tree t ON t.ID = a.ParentID    
					)
					
					INSERT INTO @CursorTable
					SELECT DISTINCT at.ID, ISNULL(at.ParentID,''), Account, AccountLevel, Children 
					FROM @AccountTable at
					INNER JOIN Tree t ON at.ID = t.ID
					WHERE t.AccountLevel = @Level
					ORDER BY AccountLevel, Account

					DECLARE @AccountID NVARCHAR(255)
					DECLARE @Account NVARCHAR(255)
					DECLARE @hasChild INT
					DECLARE @Orderby INT = 1
					DECLARE @Section NVARCHAR(255)
					DECLARE db_cursor_account CURSOR FOR 
					SELECT ListID, Account FROM @CursorTable
					WHERE ParentID = ISNULL(@ID, ParentID)
					ORDER BY AccountLevel, Account
					OPEN db_cursor_account;
					FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account

					
					WHILE @@FETCH_STATUS = 0  
						BEGIN
							IF (SELECT COUNT(ID) FROM @Transactions WHERE ID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))) IS NULL
							BEGIN
							FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account
							CONTINUE;
							END

							SET @hasChild = (SELECT COUNT(a.ID) FROM @AccountTable a INNER JOIN @Transactions tc ON a.ID = tc.ID WHERE ParentID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID)))
							IF @hasChild > 0
							BEGIN
								INSERT INTO @Result
								SELECT DISTINCT @AccountID, CASE WHEN @Level = 0 THEN @Account+'|' ELSE REPLICATE('     ', @Level)+@Account+'|' END,
								(SELECT SUM(CommitmentUSD) FROM @Transactions t WHERE ID = @AccountID) AS CommitmentUSD, 
								(SELECT SUM(CommitmentEUR) FROM @Transactions t WHERE ID = @AccountID) AS CommitmentEUR, 
								(SELECT SUM(ExpensesUSD) FROM @Transactions t WHERE ID = @AccountID) AS ExpensesUSD, 
								(SELECT SUM(ExpensesEUR) FROM @Transactions t WHERE ID = @AccountID) AS ExpensesEUR, @Orderby
								FROM @Transactions 
								UNION ALL
								
								SELECT AccountID, Account, CommitmentUSD, CommitmentEUR, ExpensesUSD, ExpensesEUR, Orderby+@Orderby FROM [dbo].[msf_ActualCommittmentsVsActualDisbursments](@StartDate, @EndDate, @Fund, @CPSPermitted, @AccountID, @Level+1)

								SELECT @Orderby = MAX(Orderby) FROM @Result

								IF EXISTS (SELECT 1 FROM @Result WHERE AccountID = @AccountID)
								BEGIN
									INSERT INTO @Result
									SELECT DISTINCT @AccountID, CASE WHEN @Level = 0 THEN 'Total '+@Account ELSE REPLICATE('     ', @Level)+'Total '+@Account END,
									SUM(CommitmentUSD) AS CommitmentUSD, SUM(CommitmentEUR) AS CommitmentEUR, SUM(ExpensesUSD) AS ExpensesUSD, SUM(ExpensesEUR) AS ExpensesEUR, @Orderby+1
									FROM @Transactions
									WHERE ID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))
								END
								SET @Orderby = @Orderby+2
							END
							IF @hasChild IS NULL OR @hasChild = 0
							BEGIN
								INSERT INTO @Result
								SELECT DISTINCT @AccountID, REPLICATE('     ', @Level)+@Account,
								SUM(CommitmentUSD) AS CommitmentUSD, SUM(CommitmentEUR) AS CommitmentEUR, SUM(ExpensesUSD) AS ExpensesUSD, SUM(ExpensesEUR) AS ExpensesEUR, @Orderby+1
								FROM @Transactions
								WHERE ID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))
								

								SET @Orderby = @Orderby+1
							END
							FETCH NEXT FROM db_cursor_account INTO @AccountID, @Account
						END

					CLOSE db_cursor_account;
					DEALLOCATE db_cursor_account;

					RETURN
                END;