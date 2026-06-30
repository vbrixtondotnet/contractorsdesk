CREATE FUNCTION [dbo].[msf_QDRPCommittmentsVsActualCommittments](
				@CurrentYear NVARCHAR(4),
				@Quarters NVARCHAR(10),
				@Fund NVARCHAR(255),
				@ID NVARCHAR(255),
				@Level int
				)
                RETURNS @Result TABLE
				(
					AccountID NVARCHAR(255),
					Account NVARCHAR(255),
					CommitmentBudget DECIMAL(21,9),
					CommitmentUSD DECIMAL(21,9),
					Orderby int
				)
                AS
                BEGIN

					IF @Level IS NULL
					SET @Level = 1

					DECLARE @StartDate DATE, @EndDate DATE
					DECLARE @Quarter NVARCHAR(10)

					IF @Quarters IS NOT NULL
					BEGIN
						SET @StartDate = (SELECT MIN(DATEFROMPARTS(@CurrentYear, (value-1)*3+1, 1)) FROM string_split(@Quarters,','))
						SET @EndDate = (SELECT MAX(DATEADD(day, -1, DATEADD(month, value*3, DATEFROMPARTS(@CurrentYear, 1, 1)))) FROM string_split(@Quarters,','))
					END
					ELSE 
					BEGIN
						SET @StartDate = (SELECT CONCAT(1, '/' , 1 , '/' , @CurrentYear))
						SET @EndDate = (SELECT CONCAT(12, '/' , 31 , '/' , @CurrentYear))
					END
					DECLARE @AccountTable AccountExpense

					DECLARE @Transactions TABLE(
					AccountID NVARCHAR(255),
					CommitmentBudget DECIMAL(21,9),
					CommitmentUSD DECIMAL(21,9)
					)

					DECLARE @Contracts TABLE(
					ListID NVARCHAR(255),
					CommitmentUSD DECIMAL(21,9)
					)

	
 				--	DECLARE db_cursor CURSOR FOR 
					--SELECT value FROM STRING_SPLIT(@Quarters, ',')
					--ORDER BY value;
					--OPEN db_cursor;  
					--FETCH NEXT FROM db_cursor INTO @Quarter

					--	WHILE @@FETCH_STATUS = 0  
					--	BEGIN
					--			SET @StartDate = DATEFROMPARTS(@CurrentYear, 3 * @Quarter - 2, 1)
					--			SET @EndDate = DATEADD(DAY, -1, DATEADD(MONTH, 3 * @Quarter, DATEFROMPARTS(@CurrentYear, 1, 1)))

					--			INSERT @Contracts
					--			SELECT ListID, SUM(CommitmentUSD) AS CommitmentUSD
					--			FROM(
					--					SELECT a.ListID, SumUSD AS CommitmentUSD
					--					FROM QBAccounts a
					--					INNER JOIN Contracts c ON CASE WHEN AccountNumber IS NULL THEN FullyQualifiedName ELSE '('+AccountNumber+') '+ FullyQualifiedName END = c.Activity
					--					WHERE a.AccountType = 'Expense' AND DATEPART(qq, c.EndDate) = @Quarter AND Year(c.EndDate) = @CurrentYear
					--					--AND CONVERT(varchar(10),c.EndDate, 101) <= @EndDate AND c.StartDate >= @StartDate AND 
					--				)t2
					--			GROUP BY ListID

					--		FETCH NEXT FROM db_cursor INTO @Quarter
					--	END
					--	CLOSE db_cursor;
					--	DEALLOCATE db_cursor;



					INSERT @Contracts
					SELECT ListID, SUM(CommitmentUSD) AS CommitmentUSD
					FROM(
							SELECT a.ListID, CASE WHEN c.Fund1 IS NULL OR c.Fund1 = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass, Value1USD AS CommitmentUSD
							FROM Contracts c
							INNER JOIN QBAccounts a ON c.Activity = a.ID
							INNER JOIN QBClasses qbc on qbc.ID = c.Fund1
							WHERE a.AccountType = 'Expense' 
							AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
							UNION ALL
							SELECT a.ListID, CASE WHEN c.Fund2 IS NULL OR c.Fund2 = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass, Value2USD AS CommitmentUSD
							FROM Contracts c
							INNER JOIN QBAccounts a ON c.Activity = a.ID
							INNER JOIN QBClasses qbc on qbc.ID = c.Fund2
							WHERE a.AccountType = 'Expense' 
							AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
							UNION ALL
							SELECT a.ListID, CASE WHEN c.Fund3 IS NULL OR c.Fund3 = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass, Value3USD AS CommitmentUSD
							FROM Contracts c
							INNER JOIN QBAccounts a ON c.Activity = a.ID
							INNER JOIN QBClasses qbc on qbc.ID = c.Fund3
							WHERE a.AccountType = 'Expense' 
							AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
							UNION ALL
							SELECT a.ListID, CASE WHEN c.Fund4 IS NULL OR c.Fund4 = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass, Value4USD AS CommitmentUSD
							FROM Contracts c
							INNER JOIN QBAccounts a ON c.Activity = a.ID
							INNER JOIN QBClasses qbc on qbc.ID = c.Fund4
							WHERE a.AccountType = 'Expense' 
							AND CAST(StartDate AS Date)BETWEEN @StartDate AND @EndDate
						)t2
					WHERE TopClass IN (SELECT CASE WHEN @Fund IS NULL THEN TopClass  ELSE value END FROM STRING_SPLIT(ISNULL(@Fund, ''),'|'))
					GROUP BY ListID
					
					INSERT @Transactions
					SELECT ISNULL(c.ListID, a.ListID) AS ListID, CommitmentBudget, CommitmentUSD
					FROM @Contracts c
					FULL JOIN (
								SELECT ListID, SUM(CommitmentBudget) AS CommitmentBudget
								FROM(
										SELECT a.ListID, Q1 AS CommitmentBudget, '1' AS Quarter, CASE WHEN b.Fund IS NULL OR b.Fund = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass
										FROM QBAccounts a
										INNER JOIN Budgets b ON B.Activity = a.ID
										INNER JOIN QBClasses qbc on qbc.ID = b.Fund
										WHERE a.AccountType = 'Expense' AND b.Year = @CurrentYear
										UNION ALL
										SELECT a.ListID, Q2 AS  CommitmentBudget,'2' AS Quarter, CASE WHEN b.Fund IS NULL OR b.Fund = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass
										FROM QBAccounts a
										INNER JOIN Budgets b ON B.Activity = a.ID
										INNER JOIN QBClasses qbc on qbc.ID = b.Fund
										WHERE a.AccountType = 'Expense' AND b.Year = @CurrentYear
										UNION ALL
										SELECT a.ListID, Q3  AS CommitmentBudget, '3' AS Quarter, CASE WHEN b.Fund IS NULL OR b.Fund = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass
										FROM QBAccounts a
										INNER JOIN Budgets b ON B.Activity = a.ID
										INNER JOIN QBClasses qbc on qbc.ID = b.Fund
										WHERE a.AccountType = 'Expense' AND b.Year = @CurrentYear
										UNION ALL
										SELECT a.ListID, Q4 AS CommitmentBudget, '4' AS Quarter, CASE WHEN b.Fund IS NULL OR b.Fund = '' THEN 'None' ELSE (SELECT dbo.getQBClassTopLevel(qbc.FullyQualifiedName)) END AS TopClass
										FROM QBAccounts a
										INNER JOIN Budgets b ON B.Activity = a.ID
										INNER JOIN QBClasses qbc on qbc.ID = b.Fund
										WHERE a.AccountType = 'Expense' AND b.Year = @CurrentYear
									)t
								WHERE Quarter IN (SELECT CASE WHEN @Quarters IS NULL THEN Quarter ELSE value END FROM STRING_SPLIT(ISNULL(@Quarters, ''),','))
								AND TopClass IN (SELECT CASE WHEN @Fund IS NULL THEN TopClass  ELSE value END FROM STRING_SPLIT(ISNULL(@Fund, ''),'|'))
							GROUP BY ListID
					) a on a.ListID = c.ListID

					INSERT @AccountTable
					SELECT ListID, ParentID, Account, Children
					FROM(
						SELECT DISTINCT a.ListID, ParentID, ISNULL(a.AccountNumber,'')+' - '+a.Name as Account, 
						(SELECT COUNT(*) FROM QBAccounts WHERE ParentID = a.ListID) Children
						FROM QBAccounts a
						LEFT JOIN Contracts c ON c.Activity = a.ID
						LEFT JOIN Budgets b ON b.Activity = a.ID
						WHERE a.AccountType = 'Expense'
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
						ID,
						ParentID,
						Account,
						0 AS AccountLevel
					FROM @AccountTable
					WHERE ParentID IS NULL or ParentID = ''

					UNION ALL

					SELECT 
						a.ID,
						a.ParentID,
						a.Account,
						t.AccountLevel + 1 AS AccountLevel
					FROM @AccountTable AS a
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
								(SELECT SUM(CommitmentBudget) FROM @Transactions t WHERE AccountID = @AccountID) AS CommitmentBudget, 
								(SELECT SUM(CommitmentUSD) FROM @Transactions t WHERE AccountID = @AccountID) AS CommitmentUSD, @Orderby
								FROM @Transactions 
								UNION ALL
								
								SELECT AccountID, Account, CommitmentBudget, CommitmentUSD, Orderby+@Orderby FROM [dbo].[msf_QDRPCommittmentsVsActualCommittments](@CurrentYear, @Quarters, @Fund, @AccountID, @Level+1)

								SELECT @Orderby = MAX(Orderby) FROM @Result

								INSERT INTO @Result
								SELECT DISTINCT @AccountID, REPLICATE('   ', @Level-1)+'Total '+@Account,
								SUM(CommitmentBudget) AS CommitmentBudget, SUM(CommitmentUSD) AS CommitmentUSD, @Orderby+1
								FROM @Transactions
								WHERE AccountID IN (SELECT ListID FROM [dbo].[getQBAccountChildrenID](@AccountID))

								SET @Orderby = @Orderby+2
							END
							IF @hasChild = 0
							BEGIN
								INSERT INTO @Result
								SELECT DISTINCT @AccountID, REPLICATE('   ', @Level-1)+@Account,
								SUM(CommitmentBudget) AS CommitmentBudget, SUM(CommitmentUSD) AS CommitmentUSD, @Orderby+1
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