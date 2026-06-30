CREATE PROCEDURE [dbo].[spJobBankReconciliationReport]
(
    @AccountCategory NVARCHAR(255)
)
AS
BEGIN
	IF 'All' IN (SELECT value FROM STRING_SPLIT(@AccountCategory,'|'))
	SET @AccountCategory = NULL

    CREATE TABLE #CategoryDetails
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

    CREATE TABLE #BankingAccount
    (
        Date Date,
        TxnID NVARCHAR(255),
        RefNo NVARCHAR(255),
        Payee NVARCHAR(255),
	    [Division/Location] NVARCHAR(255),
        BankingAccount NVARCHAR(255)
    );

    INSERT INTO #BankingAccount
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
		   t.Location,
           a.FullyQualifiedName AS BankingAccount
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND a.FullyQualifiedName IN ('CHA Construction 0090', 'CHA Const 9100') 
	AND a.FullyQualifiedName IN (SELECT CASE WHEN @AccountCategory IS NULL THEN a.FullyQualifiedName ELSE value END FROM STRING_SPLIT(ISNULL(@AccountCategory, ''),'|'))

    INSERT INTO #CategoryDetails (Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], Class, [Division/Location], Amount)
    SELECT t.TransactionDate AS Date,
           t.TxnType AS Type,
           t.TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
           t.Memo,
           a.FullyQualifiedName AS 'Account/Category',
           c.FullyQualifiedName AS Class,
           t.Location AS 'Division/Location',
           t.Amount 
    FROM [dbo].[QBTransactions] t
    INNER JOIN QBAccounts a ON a.ID = t.AccountID
    INNER JOIN QBClasses c ON c.ID = t.ClassID
    WHERE c.AllowedForJobReports = 1
        AND EXISTS (
            SELECT 1
            FROM #BankingAccount tr
            WHERE tr.TxnID = t.TxnID AND tr.RefNo = t.TxnNumber AND t.Name = tr.Payee AND tr.[Division/Location] = t.Location
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
				COALESCE(t.[Division/Location], b.[Division/Location]) AS [Division/Location],
				Amount
			FROM #CategoryDetails t
			INNER JOIN #BankingAccount b ON b.TxnID = t.txnID AND b.RefNo = t.RefNo AND b.Payee = t.Payee AND t.Date = b.Date 
		) r
		ORDER BY Date, LEN(TxnID), TxnID, LEN(RefNo), RefNo, Memo, Payee, [Account/Category];


    -- Drop temp tables
    DROP TABLE #CategoryDetails;
    DROP TABLE #BankingAccount;
END