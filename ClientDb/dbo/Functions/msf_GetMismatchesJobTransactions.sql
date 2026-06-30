CREATE FUNCTION [dbo].[msf_GetMismatchesJobTransactions]()
RETURNS @Result TABLE (
    BankingAccountAPP NVARCHAR(255),
    BankingAccountQBO NVARCHAR(255),
    [Account/Category] NVARCHAR(255),
    Class NVARCHAR(255),
    Date DATE,
    TxnID NVARCHAR(255),
    RefNo NVARCHAR(255),
    Type NVARCHAR(255),
    Payee NVARCHAR(255),
    Memo NVARCHAR(MAX),
    [Division/Location] NVARCHAR(255),
    Amount DECIMAL(21, 2)
)
AS
BEGIN
    DECLARE @BankingAccountQ1 dbo.BankingAccountType;
    DECLARE @CategoryDetailsQ1 dbo.CategoryDetailsType;
    DECLARE @CategoryDetailsQ2 dbo.CategoryDetailsType;
    DECLARE @BankingAccountClass dbo.BankingAccountClassType;
	DECLARE @TempResultQ1 dbo.TempResultQ1Type;


    INSERT INTO @BankingAccountQ1
    SELECT t.TransactionDate AS Date,
           TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
           a.FullyQualifiedName AS BankingAccount
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND a.Detailtype <> 'CashOnHand';

    INSERT INTO @BankingAccountClass
    SELECT c.FullyQualifiedName AS Class,
           a.FullyQualifiedName AS BankingAccount
    FROM QBAccounts a 
    INNER JOIN QBClasses c ON c.QBAccountID = a.ID
    WHERE a.AccountType = 'Bank';

    INSERT INTO @CategoryDetailsQ1 (Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], Class, [Division/Location], Amount)
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
    FROM QBAccounts a
    INNER JOIN QBTransactions t ON t.AccountID = a.ID 
    INNER JOIN QBClasses c ON c.ID = t.ClassID
    WHERE t.TxnType NOT IN ('Transfer', 'Journal Entry') AND EXISTS (
            SELECT 1
            FROM @BankingAccountQ1 tr
            WHERE tr.TxnID = t.TxnID AND tr.RefNo = t.TxnNumber AND t.Name = tr.Payee
        );

    INSERT INTO @CategoryDetailsQ2 (Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], Class, [Division/Location], Amount)
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
    FROM QBAccounts a
    INNER JOIN QBTransactions t ON t.AccountID = a.ID 
    INNER JOIN QBClasses c ON c.ID = t.ClassID
    WHERE t.TxnType = 'Journal Entry' AND a.AccountType = 'Bank' AND a.Detailtype <> 'CashOnHand'

    INSERT INTO @TempResultQ1
        SELECT Date, Type, TxnID, RefNo, Payee, Memo, [Account/Category], BankingAccount, Class, [Division/Location], Amount
        FROM (
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
            FROM @CategoryDetailsQ1 t
            INNER JOIN @BankingAccountQ1 b ON b.TxnID = t.txnID AND b.RefNo = t.RefNo AND b.Payee = t.Payee AND t.Date = b.Date
        ) r;

    INSERT @Result
	SELECT BankingAccountAPP, BankingAccountQBO, [Account/Category], Class, Date, TxnID, RefNo, Type, Payee, Memo, [Division/Location], Amount
	FROM(
			SELECT c.BankingAccount AS BankingAccountAPP, t.BankingAccount AS BankingAccountQBO, [Account/Category], COALESCE(t.Class, c.Class) AS Class, Date, TxnID, RefNo, Type, Payee, Memo, [Division/Location], Amount
			FROM @TempResultQ1 t
			INNER JOIN @BankingAccountClass c ON c.Class = t.Class 
			WHERE c.BankingAccount <> t.BankingAccount AND (c.BankingAccount NOT IN ('CHA Const 9100','CHA Construction 0090') OR t.BankingAccount NOT IN ('CHA Const 9100','CHA Construction 0090'))  
			UNION ALL
			SELECT c.BankingAccount AS BankingAccountAPP, '' AS BankingAccountQBO, [Account/Category], COALESCE(t.Class, c.Class) AS Class, Date, TxnID, RefNo, Type, Payee, Memo, [Division/Location], Amount
			FROM @CategoryDetailsQ2 t
			INNER JOIN @BankingAccountClass c ON c.Class = t.Class
			WHERE c.BankingAccount <> t.[Account/Category] AND (c.BankingAccount NOT IN ('CHA Const 9100','CHA Construction 0090') OR t.[Account/Category] NOT IN ('CHA Const 9100','CHA Construction 0090'))  
		)t
    ORDER BY t.Class, BankingAccountAPP, BankingAccountQBO, Date, LEN(TxnID), TxnID, LEN(RefNo), RefNo, Memo, Payee, [Account/Category];

RETURN
END