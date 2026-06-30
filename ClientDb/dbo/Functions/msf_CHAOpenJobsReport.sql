CREATE FUNCTION [dbo].[msf_CHAOpenJobsReport](
				@StartDate DATE,
				@EndDate DATE
				)
                RETURNS @Result TABLE
				(
					OpenConstructionJobs NVARCHAR(255),
					JobBalance DECIMAL(21,9)
				)
                AS
                BEGIN
						DECLARE @CategoryDetails1 TABLE
						(
							Date Date,
							Type NVARCHAR(255),
							TxnID NVARCHAR(255),
							RefNo NVARCHAR(255),
							Payee NVARCHAR(255),
							Memo NVARCHAR(MAX),
							[Account/Category] NVARCHAR(255),
							Class NVARCHAR(255),
							[Division/Location] NVARCHAR(255),
							Amount DECIMAL(21,2)
						);

						DECLARE @BankingAccount1 TABLE
						(
							Date Date,
							TxnID NVARCHAR(255),
							RefNo NVARCHAR(255),
							Payee NVARCHAR(255),
							BankingAccount NVARCHAR(255)
						);

						INSERT INTO @BankingAccount1
						SELECT t.TransactionDate AS Date,
							   TxnID,
							   t.TxnNumber AS RefNo,
							   t.Name AS Payee,
							   a.FullyQualifiedName AS BankingAccount
						FROM QBAccounts a 
						INNER JOIN QBTransactions t ON t.AccountID = a.ID
						WHERE a.AccountType = 'Bank' AND a.FullyQualifiedName = 'CHA Const 9100'

						INSERT INTO @CategoryDetails1 (Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], Class, [Division/Location], Amount)
						SELECT t.TransactionDate AS Date,
							   t.TxnType AS Type,
							   t.TxnID,
							   t.TxnNumber AS RefNo,
							   t.Name AS Payee,
							   t.Memo,
							   a.AccountType AS 'Account/Category',
							   c.FullyQualifiedName AS Class,
							   '' AS 'Division/Location',
							   t.Amount 
						FROM [dbo].[QBTransactions] t
						INNER JOIN QBAccounts a ON a.ID = t.AccountID
						INNER JOIN QBClasses c ON c.ID = t.ClassID
						WHERE c.OpenJob = 1 AND t.TransactionDate BETWEEN @StartDate AND @EndDate
						AND AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') 
							AND EXISTS (
								SELECT 1
								FROM @BankingAccount1 tr
								WHERE tr.TxnID = t.TxnID AND tr.RefNo = t.TxnNumber AND t.Name = tr.Payee
							);

						DECLARE @OpenJobs TABLE
						(
							AccountType NVARCHAR(50),
							JobBalance DECIMAL(21,2)
						)

						DECLARE @GrossProfit TABLE
						(
							JobBalance DECIMAL(21,2)
						)

						DECLARE @NetOperatingIncome TABLE
						(
							JobBalance DECIMAL(21,2)
						)

						DECLARE @NetIncome TABLE
						(
							JobBalance DECIMAL(21,2)
						)



						INSERT @OpenJobs
						SELECT [Account/Category], SUM(Amount) AS JobBalance
						FROM(
								SELECT
								t.Date,
								Type,
								COALESCE(t.TxnID, b.TxnID) AS TxnID,
								COALESCE(t.RefNo, b.RefNo) AS RefNo,
								COALESCE(t.Payee, b.Payee) AS Payee,
								Memo,
								[Account/Category],
								BankingAccount,
								Class,
								[Division/Location],
								Amount
							FROM @CategoryDetails1 t
							INNER JOIN @BankingAccount1 b ON b.TxnID = t.txnID AND b.RefNo = t.RefNo AND b.Payee = t.Payee AND t.Date = b.Date
						) r
						GROUP BY [Account/Category]

						INSERT @GrossProfit
							SELECT SUM(JobBalance) AS JobBalance
							FROM(
									SELECT JobBalance
									FROM @OpenJobs
									WHERE AccountType = 'Income'
									UNION ALL
									SELECT -JobBalance
									FROM @OpenJobs
									WHERE AccountType = 'CostofGoodsSold'
								) gp

						INSERT @NetOperatingIncome
							SELECT SUM(JobBalance) AS JobBalance
							FROM(		
									SELECT JobBalance
									FROM @GrossProfit
									UNION ALL
									SELECT -JobBalance
									FROM @OpenJobs
									WHERE AccountType = 'Expense'
								) noi


						INSERT @NetIncome
							SELECT SUM(JobBalance) AS JobBalance
							FROM(		
									SELECT JobBalance
									FROM @NetOperatingIncome
									UNION ALL
									SELECT JobBalance
									FROM @OpenJobs
									WHERE AccountType = 'OtherIncome'
									UNION ALL
									SELECT -JobBalance
									FROM @OpenJobs
									WHERE AccountType = 'OtherExpense'
								) ni

							INSERT @Result
							SELECT 'QB CHA 9100 Acct Balance:', JobBalance
							FROM @NetIncome
					RETURN
                END;