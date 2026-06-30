CREATE FUNCTION [dbo].[GetHighestPercentageOfTransactions]()
RETURNS @ResultsTable TABLE
(
    ClassID NVARCHAR(255),
    BankAccount NVARCHAR(255),
    TotalTransactions INT,
    PercentageOfTransactions DECIMAL(21,2)
)
AS
BEGIN
    DECLARE @BankingAccountQ1 TABLE
    (
        Date Date,
        TxnID NVARCHAR(255),
        RefNo NVARCHAR(255),
        Payee NVARCHAR(255),
        BankingAccountID NVARCHAR(255)
    );

    DECLARE @Query TABLE
    (
        ClassID NVARCHAR(255),
        BankAccount NVARCHAR(255),
        TotalTransactions INT,
        PercentageOfTransactions DECIMAL(21,3)
    );

    DECLARE @ClassName NVARCHAR(255);
    DECLARE @Sort INT = 1;

    -- Insert data into @BankingAccountQ1
    INSERT INTO @BankingAccountQ1
    SELECT t.TransactionDate AS Date,
           t.TxnID,
           t.TxnNumber AS RefNo,
           t.Name AS Payee,
           a.ID AS BankingAccountID
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank';

    -- Insert data into @Query
    INSERT INTO @Query
	SELECT c.ID AS ClassID,
		   tr.BankingAccountID,
		   COUNT(*) AS TotalTransactions,
		   CAST(ROUND(100.0 * COUNT(*) / SUM(COUNT(*)) OVER (PARTITION BY c.ID), 4) AS DECIMAL(21,2)) AS PercentageOfTransactions
	FROM QBClasses c
	INNER JOIN QBTransactions t ON t.ClassID = c.ID
	INNER JOIN QBAccounts a ON a.ID = t.AccountID
	INNER JOIN @BankingAccountQ1 tr ON tr.TxnID = t.TxnID AND tr.RefNo = t.TxnNumber AND t.Name = tr.Payee
	WHERE c.QBAccountID IS NULL 
	GROUP BY c.ID, tr.BankingAccountID
	ORDER BY c.ID, tr.BankingAccountID;

    -- Declare a cursor to iterate through each class
    DECLARE class_cursor CURSOR FOR
    SELECT DISTINCT ClassID 
    FROM @Query
    ORDER BY ClassID;

    OPEN class_cursor;
    FETCH NEXT FROM class_cursor INTO @ClassName;

    -- Loop through each class
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Insert the highest percentage of transactions for each class into the results table variable
        INSERT INTO @ResultsTable
        SELECT TOP 1 
            @ClassName as ClassID, 
            BankAccount, 
            TotalTransactions, 
            CAST(PercentageOfTransactions AS DECIMAL(21,2)) AS PercentageOfTransactions
        FROM @Query
        WHERE ClassID = @ClassName
        ORDER BY PercentageOfTransactions DESC, BankAccount;

        FETCH NEXT FROM class_cursor INTO @ClassName;
    END

    CLOSE class_cursor;
    DEALLOCATE class_cursor;

    RETURN;
END;