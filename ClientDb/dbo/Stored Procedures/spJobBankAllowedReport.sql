CREATE PROCEDURE [dbo].[spJobBankAllowedReport]
(
    @AccountCategory NVARCHAR(MAX)
)
AS
BEGIN
	IF 'All' IN (SELECT value FROM STRING_SPLIT(@AccountCategory,'|'))
	SET @AccountCategory = NULL

    CREATE TABLE #CategoryDetails1
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

    CREATE TABLE #BankingAccount1
    (
        Date Date,
        TxnID NVARCHAR(255),
        RefNo NVARCHAR(255),
        Payee NVARCHAR(255),
        BankingAccount NVARCHAR(255)
    );

    INSERT INTO #BankingAccount1
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
           a.FullyQualifiedName AS BankingAccount
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND a.FullyQualifiedName NOT IN ('CHA Construction 0090', 'CHA Const 9100') 
	AND a.FullyQualifiedName IN (SELECT CASE WHEN @AccountCategory IS NULL THEN a.FullyQualifiedName ELSE value END FROM STRING_SPLIT(ISNULL(@AccountCategory, ''),'|'))

    INSERT INTO #CategoryDetails1 (Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], Class, [Division/Location], Amount)
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
            FROM #BankingAccount1 tr
            WHERE tr.TxnID = t.TxnID AND tr.RefNo = t.TxnNumber AND t.Name = tr.Payee
        );

	SELECT Date, TxnID, RefNo, Type, Payee, Memo, [Account/Category], BankingAccount, Class, [Division/Location], Amount
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
		FROM #CategoryDetails1 t
		INNER JOIN #BankingAccount1 b ON b.TxnID = t.txnID AND b.RefNo = t.RefNo AND b.Payee = t.Payee AND t.Date = b.Date
	) r
	ORDER BY Date, LEN(TxnID), TxnID, LEN(RefNo), RefNo, Memo, Payee, [Account/Category];


    -- Drop temp tables
    DROP TABLE #CategoryDetails1;
    DROP TABLE #BankingAccount1;
END