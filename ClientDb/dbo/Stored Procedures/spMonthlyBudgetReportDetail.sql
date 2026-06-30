
CREATE PROCEDURE [dbo].[spMonthlyBudgetReportDetail] 
    @StartDate DATE,
    @EndDate DATE,
    @Class NVARCHAR(MAX),
    @Item NVARCHAR(MAX)
AS
BEGIN   
    IF 'All' IN (SELECT value FROM STRING_SPLIT(@Class, '|'))
        SET @Class = NULL;

    CREATE TABLE #EstimateCategories
    (
        ID NVARCHAR(255),
        SubCategory NVARCHAR(255),
        ParentEstimateCategoryID NVARCHAR(255),
        Category NVARCHAR(255),
        Level INT
    );

    CREATE TABLE #SourceBankAccount
    (
        AccountID NVARCHAR(255),
        Date DATE,
        TxnID NVARCHAR(255),
        Name NVARCHAR(255),
        AccountName NVARCHAR(255),
        [Clear Status] NVARCHAR(255)
    );

    CREATE TABLE #ClassDetail
    (
        Date DATE,
        Type NVARCHAR(255),
        Num NVARCHAR(255),
        Name NVARCHAR(600),
        AccountID NVARCHAR(255),
        Memo NVARCHAR(600),
        Category NVARCHAR(600),
        SourceAccount NVARCHAR(255),
        Class NVARCHAR(255),
        [Division/Location] NVARCHAR(255),
        [Clear Status] NVARCHAR(255),
        Amount DECIMAL(21,2)
    );

    DECLARE @ActualDetail TABLE
    (
        Date DATE,
        Type NVARCHAR(255),
        Num NVARCHAR(255),
        Name NVARCHAR(600),
        Memo NVARCHAR(600),
        Category NVARCHAR(600),
        SourceAccount NVARCHAR(255),
        Class NVARCHAR(255),
        [Division/Location] NVARCHAR(255),
        [Clear Status] NVARCHAR(255),
        Amount DECIMAL(21,2)
    );

    DECLARE @Results TABLE
    (
        EstimateCategory NVARCHAR(600),
        EstimateSubCategory NVARCHAR(600),
        Date DATE,
        Type NVARCHAR(255),
        Num NVARCHAR(255),
        Name NVARCHAR(600),
        Memo NVARCHAR(600),
        Category NVARCHAR(600),
        SourceAccount NVARCHAR(255),
        Class NVARCHAR(255),
        [Division/Location] NVARCHAR(255),
        [Clear Status] NVARCHAR(255),
        Amount DECIMAL(21,2)
    );

    ;WITH CategoryHierarchy AS (
        SELECT 
            ID,
            Name AS SubCategory,
            ParentEstimateCategoryID,
            Name AS Category,
            0 AS Level
        FROM 
            EstimateCategories
        WHERE 
            ParentEstimateCategoryID IS NULL
        
        UNION ALL
        
        SELECT 
            ec.ID,
            ec.Name AS SubCategory,
            ec.ParentEstimateCategoryID,
            ch.Category,
            ch.Level + 1
        FROM 
            EstimateCategories ec
            INNER JOIN CategoryHierarchy ch ON ec.ParentEstimateCategoryID = ch.ID
    )
    INSERT INTO #EstimateCategories
    SELECT 
        ID,
        SubCategory,
        ParentEstimateCategoryID,
        Category,
        Level
    FROM 
        CategoryHierarchy
    ORDER BY 
        Category;

    INSERT INTO #SourceBankAccount
    SELECT a.ID,
           t.TransactionDate AS Date,
           TxnID,
           t.Name AS Name,
           a.Name AS AccountName,
           t.IsCleared
    FROM QBAccounts a 
    INNER JOIN QBTransactions t ON t.AccountID = a.ID
    WHERE a.AccountType = 'Bank' AND t.TransactionDate BETWEEN @StartDate AND @EndDate;

    INSERT INTO #ClassDetail
    SELECT Date, Type, Num, Name, AccountID, Memo, Category, AccountName, Class, [Division/Location], [Clear Status], Amount
    FROM(
            SELECT t.TransactionDate AS Date,
                   t.TxnType AS Type,
                   t.TxnNumber AS Num,
                   t.Name,
                   t.AccountID,
                   t.Memo,
                   a.FullyQualifiedName AS 'Category',
                   tr.AccountName,
                   CASE WHEN t.ClassID IS NULL THEN 'Missing' ELSE c.FullyQualifiedName END AS Class,
                   t.Location AS 'Division/Location',
                   ISNULL(tr.[Clear Status], 'Missing') AS [Clear Status],
                   t.Amount
            FROM QBAccounts a
            INNER JOIN QBTransactions t ON t.AccountID = a.ID 
            INNER JOIN QBClasses c ON c.ID = t.ClassID
            INNER JOIN #SourceBankAccount tr ON tr.TxnID = t.TxnID AND t.Name = tr.Name AND tr.Date = t.TransactionDate
            WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate AND c.AllowedForBudgetReports = 1
        ) t
    WHERE Class IN (SELECT CASE WHEN @Class IS NULL THEN Class ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''), '|'));
   
    INSERT INTO @Results
    SELECT EM.Category, EM.SubCategory, c.Date, Type, Num, Name, Memo, c.Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
    FROM EstimateMappings E
    INNER JOIN #EstimateCategories EM ON EM.ID = E.EstimateSubCategoryID
    INNER JOIN #ClassDetail c ON c.AccountID = E.QBAccountID
    WHERE EM.SubCategory <> 'Other' 
    UNION ALL
    SELECT EM.Category, EM.SubCategory, c.Date, Type, Num, Name, Memo, c.Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
    FROM EstimateMappings E
    INNER JOIN #EstimateCategories EM ON EM.ID = E.EstimateSubCategoryID
    INNER JOIN #ClassDetail c ON c.AccountID = E.QBAccountID
    WHERE EM.SubCategory = 'Other' AND E.AccountType IN ('Expenses', 'Other Expense')
    
-----------
---te fshihet cae7ba92-c568-4847-3a4f-08dc4f40e2a6
---te mbetet  9c98f4d2-c984-43f9-1506-08dc17a99cdf

-- Select * from QBAccounts where FullyQualifiedName = 'Owner Deposit' -- dy me emer te njejte

-- Select * from QBTransactions where AccountID = '9c98f4d2-c984-43f9-1506-08dc17a99cdf' -- ka transakcione / mbetet
-- Select * from QBTransactions where AccountID = 'cae7ba92-c568-4847-3a4f-08dc4f40e2a6' -- ska transakcione / te fshihet


-- update EstimateMappings set  QBAccountID='9c98f4d2-c984-43f9-1506-08dc17a99cdf' where QBAccountID='cae7ba92-c568-4847-3a4f-08dc4f40e2a6';

-- delete from QBAccounts where id ='cae7ba92-c568-4847-3a4f-08dc4f40e2a6'

-- -----------
    -- Handle 'Owner Deposits' as Income
    IF @Item = 'OWNER DEPOSITS'
    BEGIN
        DELETE FROM @Results
        INSERT INTO @Results
        SELECT EM.Category, EM.SubCategory, c.Date, Type, Num, Name, Memo, c.Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
        FROM EstimateMappings E
        INNER JOIN #EstimateCategories EM ON EM.ID = E.EstimateSubCategoryID
        INNER JOIN #ClassDetail c ON c.AccountID = E.QBAccountID
        WHERE EM.SubCategory = 'Other' AND E.AccountType = 'Income'

        INSERT INTO @ActualDetail
        SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
        FROM @Results
    END
    ELSE IF @Item LIKE ('%PROJECT TOTALS')
    BEGIN
        SET @Item = LOWER(SUBSTRING(LTRIM(@Item), 7, LEN(LTRIM(@Item)) - 5));

        INSERT INTO @ActualDetail
        SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
        FROM @Results;
    END
    ELSE IF @Item LIKE ('TOTAL%')
    BEGIN
        SET @Item = LOWER(SUBSTRING(LTRIM(@Item), 7, LEN(LTRIM(@Item)) - 5));

        INSERT INTO @ActualDetail
        SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
        FROM @Results	
        WHERE EstimateCategory = @Item;
    END
    ELSE
    BEGIN
        INSERT INTO @ActualDetail
        SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount
        FROM @Results
        WHERE EstimateSubCategory = @Item;
    END;

    SELECT CONVERT(NVARCHAR(10), Date, 101) AS Date, Type, 
           CASE WHEN [Clear Status] = 'Missing' THEN '' ELSE [Clear Status] END AS [Clear Status], 
           Num, Name, [Division/Location], 
           CASE WHEN Class = 'Missing' THEN '' ELSE Class END AS Class, 
           Memo, SourceAccount, Category, Amount,
           SUM(Amount) OVER (ORDER BY Date, Memo, LEN(Num), Num, Name, Category 
                             ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Balance 
    FROM @ActualDetail;

    DROP TABLE #EstimateCategories;
    DROP TABLE #SourceBankAccount;
    DROP TABLE #ClassDetail;
END;