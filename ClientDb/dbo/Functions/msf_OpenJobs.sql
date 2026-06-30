CREATE FUNCTION [dbo].[msf_OpenJobs](
				@StartDate DATE,
				@EndDate DATE,
				@QBClass NVARCHAR(255),
				@AccType NVARCHAR(255),
				@ID NVARCHAR(255),
				@Level int,
				@DisplayLevel int,
				@Status NVARCHAR(20) = NULL
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
						Section NVARCHAR(255),
						AccountType NVARCHAR(255)
					)
					
					DECLARE @CursorTable TABLE
					(
						AccountID NVARCHAR(255),
						ParentAccountID NVARCHAR(255),
						Account NVARCHAR(255),
						AccountLevel NVARCHAR(255)
					)

					IF @Level IS NULL
					SET @Level = 2

					INSERT @Transactions
						SELECT AccountID, QuickBooksClassShort, Amount
						FROM(
								SELECT AccountID, QBParentClass, QBClass, CASE WHEN QuickBooksClassShort IS NULL OR QuickBooksClassShort = '' THEN 'Unclassified' ELSE QuickBooksClassShort END AS QuickBooksClassShort, SUM(Amount) AS Amount
								FROM(
										SELECT AccountID, Amount, Class as QuickBooksClassShort, Class AS QBClass, Ltrim(SubString(Class, 0, Isnull(Nullif(CHARINDEX(':', Class), 0), 1000))) AS QBParentClass			
										FROM(
												SELECT a.ListID AS AccountID, a.ParentID, tc.Amount, tc.TransactionDate, qb.Name AS QBClass, (SELECT qb1.Name from QBClasses qb1 where qb1.ListID = qb.ParentID  ) as QBParentClass, CASE WHEN qb.FullyQualifiedName IS NULL OR qb.FullyQualifiedName = '' THEN 'Unclassified' else qb.FullyQualifiedName END AS Class
												FROM QBAccounts a
												INNER JOIN QBTransactions tc ON a.ID = tc.AccountID
												FULL JOIN QBClasses qb on qb.ID = tc.ClassID
												WHERE a.AccountType = @AccType AND a.AccountType IN ('Income','Cost Of Goods Sold','Expense','Other Income','Other Expense')
												AND tc.TransactionDate BETWEEN @StartDate AND @EndDate 
										)tbl
									) qbj
								WHERE QBClass IN (SELECT CASE WHEN @QBClass IS NULL THEN QBClass  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))
								GROUP BY AccountID, QBParentClass, QBClass, QuickBooksClassShort
							) t1

					

					INSERT @AccTable
					SELECT AccountID, ParentID, Account, ISNULL(Section, FullName) as Section, AccountType
					FROM(
							SELECT a.ListID AS AccountID, ParentID,ISNULL(a.AccountNumber,'')+' - '+a.Name as Account, (SELECT Name FROM Accounts WHERE ListID = a.ParentID) as Section, FullyQualifiedName AS FullName , AccountType
							FROM QBAccounts a
							WHERE a.AccountType = @AccType
						)tbl
					GROUP BY AccountID, ParentID, Account, ISNULL(Section, FullName), AccountType

					;WITH Tree (AccountID, ParentAccountID, AccountLevel)
					AS (
					SELECT
						AccountID,
						ParentAccountID,
						1 AS AccountLevel
					FROM @AccTable
					WHERE ParentAccountID IS NULL or ParentAccountID = ''

					UNION ALL

					SELECT 
						a.AccountID,
						a.ParentAccountID,
						t.AccountLevel + 1 AS AccountLevel
					FROM @AccTable AS a
						JOIN Tree t ON t.AccountID = a.ParentAccountID    
					)
					
					INSERT INTO @CursorTable
					SELECT DISTINCT at.AccountID, at.ParentAccountID, Account, AccountLevel
					FROM @AccTable at
					INNER JOIN Tree t ON at.AccountID = t.AccountID
					WHERE t.AccountLevel = @Level 
					ORDER BY AccountLevel, Account

					DECLARE @AccountID NVARCHAR(255)
					DECLARE @Account NVARCHAR(255)
					DECLARE @hasChild INT
					DECLARE @Orderby INT = 1
					DECLARE @Section NVARCHAR(255)
					DECLARE db_cursor_account CURSOR FOR 
					SELECT AccountID, Account FROM @CursorTable
					WHERE ParentAccountID = ISNULL(@ID, ParentAccountID)
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
								SELECT REPLICATE('          ', @Level)+@Account+'|', QuickBooksClass
								, (SELECT SUM(Amount) FROM @Transactions a WHERE AccountID = @AccountID AND a.QuickBooksClass = t.QuickBooksClass) as Amount, @Orderby
								FROM @Transactions t
								GROUP BY QuickBooksClass

								UNION ALL

								SELECT Account, QuickBooksClass, Amount, Orderby+@Orderby FROM [dbo].[msf_OpenJobs](@StartDate, @EndDate, @QBClass, @AccType, @AccountID, @Level+1, @DisplayLevel, @Status)

								SELECT @Orderby = MAX(Orderby) FROM @Result

								INSERT INTO @Result
								SELECT REPLICATE('          ', @Level)+'Total '+@Account, QuickBooksClass
								, (SELECT SUM(Amount) FROM @Transactions a WHERE AccountID IN (SELECT ID FROM [dbo].[getAccountChildrenID](@AccountID)) AND a.QuickBooksClass = t.QuickBooksClass), @Orderby+1
								FROM @Transactions t
								GROUP BY QuickBooksClass

								SET @Orderby = @Orderby+2
							END
							IF @hasChild IS NULL OR @Level = @DisplayLevel
							BEGIN
								INSERT INTO @Result
								SELECT REPLICATE('          ', @Level)+@Account, QuickBooksClass
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