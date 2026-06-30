CREATE PROCEDURE [dbo].[spReverseJobBankReconciliationReport]
(
    @AccountCategory NVARCHAR(255)
)
AS
BEGIN
    IF 'All' IN (SELECT value FROM STRING_SPLIT(@AccountCategory, '|'))
        SET @AccountCategory = NULL

    CREATE TABLE #ReverseCategoryDetails
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

    CREATE TABLE #ReverseBankingAccount
    (
        Date Date,
        TxnID NVARCHAR(255),
        RefNo NVARCHAR(255),
        Payee NVARCHAR(255),
        ReverseBankingAccount NVARCHAR(255)
    );

    INSERT INTO #ReverseBankingAccount
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
           a.FullyQualifiedName AS ReverseBankingAccount
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.FullyQualifiedName NOT IN ('CHA Construction 0090', 'CHA Const 9100') 
        AND ( @AccountCategory IS NULL OR a.FullyQualifiedName IN (SELECT value FROM STRING_SPLIT(@AccountCategory, '|')));

    INSERT INTO #ReverseCategoryDetails (Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], Class, [Division/Location], Amount)
    SELECT t.TransactionDate AS Date,
           t.TxnType AS Type,
           t.TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
           t.Memo,
           a.FullyQualifiedName AS 'Account/Category',
           c.FullyQualifiedName AS Class,
           '' AS 'Division/Location',
           t.Amount 
    FROM [dbo].[QBTransactions] t
    INNER JOIN QBAccounts a ON a.ID = t.AccountID
    INNER JOIN QBClasses c ON c.ID = t.ClassID
    WHERE c.AllowedForJobReports = 1
        AND EXISTS (
            SELECT 1
            FROM #ReverseBankingAccount tr
            WHERE tr.TxnID = t.TxnID AND tr.RefNo = t.TxnNumber AND t.Name = tr.Payee
        );

		WITH SortedResults AS (
			SELECT
				t.Date,
				Type,
				COALESCE(t.TxnID, b.TxnID) AS TxnID,
				COALESCE(t.RefNo, b.RefNo) AS RefNo,
				COALESCE(t.Payee, b.Payee) AS Payee,
				Memo,
				[Account/Category],
				ReverseBankingAccount,
				Class,
				[Division/Location],
				Amount
			FROM #ReverseCategoryDetails t
			INNER JOIN #ReverseBankingAccount b ON b.TxnID = t.txnID AND b.RefNo = t.RefNo AND b.Payee = t.Payee AND t.Date = b.Date
		)
		SELECT CONVERT(varchar(10), Date) AS Date, TxnID, RefNo, Payee, Memo, [Account/Category], ReverseBankingAccount, Class, [Division/Location], Amount
		FROM SortedResults
		ORDER BY Date, LEN(TxnID), TxnID, LEN(RefNo), RefNo, Memo, Payee, [Account/Category];


    -- Drop temp tables
    DROP TABLE #ReverseCategoryDetails;
    DROP TABLE #ReverseBankingAccount;
END