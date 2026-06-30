  
CREATE   PROCEDURE [dbo].[spCDMonthlyBudgetReportDetail]   
    @Class NVARCHAR(MAX),  
    @Item NVARCHAR(MAX)  
AS  
BEGIN           
    IF 'All' IN (SELECT value FROM STRING_SPLIT(@Class, '|'))        
        SET @Class = NULL;        
        
    DECLARE @ClassDetail TABLE         
    (        
        Date DATE,        
        Type NVARCHAR(255),        
        Num NVARCHAR(255),        
        Name NVARCHAR(600),        
        AccountID NVARCHAR(255),        
        Payee NVARCHAR(255),        
        Memo NVARCHAR(600),        
        Category NVARCHAR(200),        
        Class NVARCHAR(255),        
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
        EstimateSubCategoryId uniqueidentifier,         
        Date DATE,        
        Type NVARCHAR(255),        
        Num NVARCHAR(255),        
  Payee NVARCHAR(255),      
        Memo NVARCHAR(600),          
        Amount DECIMAL(21,2),      
  CategorySequence int,      
  ItemSequence int      
    );        
        
    INSERT INTO @ClassDetail        
    SELECT Date, Type, Num, Name, AccountID, Name, Memo, Category, Class, Amount        
    FROM(        
            SELECT t.TransactionDate AS Date,        
                   t.TxnType AS Type,        
                   t.TxnNumber AS Num,        
                   t.AccountID,        
                   t.Name,        
                   t.Memo,        
                   a.FullyQualifiedName AS 'Category',        
                   CASE WHEN t.ClassID IS NULL THEN 'Missing' ELSE c.FullyQualifiedName END AS Class,        
       t.Amount        
            FROM QBAccounts a        
            INNER JOIN QBTransactions t ON t.AccountID = a.ID         
            INNER JOIN QBClasses c ON c.ID = t.ClassID        
            WHERE c.AllowedForBudgetReports = 1        
        ) t        
    WHERE Class IN (SELECT CASE WHEN @Class IS NULL THEN Class ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''), '|'));        
           
    INSERT INTO @Results        
    SELECT ECP.Name, EC.Name, ec.ID, c.Date, Type, Num, c.Name, Memo,  Amount, ECP.Sequence, EC.Sequence      
       
    FROM EstimateMappings E        
    INNER JOIN EstimateCategories EC ON EC.ID = E.EstimateSubCategoryID        
    LEFT JOIN EstimateCategories ECP ON ECP.ID = EC.ParentEstimateCategoryID        
    INNER JOIN @ClassDetail c ON c.AccountID = E.QBAccountID        
    WHERE EC.Name <> 'Other'         
    UNION ALL        
    SELECT ECP.Name, EC.Name, ec.ID, c.Date, Type, Num, c.Name, Memo,  Amount, ECP.Sequence, EC.Sequence      
    FROM EstimateMappings E        
    INNER JOIN EstimateCategories EC ON EC.ID = E.EstimateSubCategoryID        
    LEFT JOIN EstimateCategories ECP ON ECP.ID = EC.ParentEstimateCategoryID       
    INNER JOIN @ClassDetail c ON c.AccountID = E.QBAccountID        
    WHERE EC.Name = 'Other' AND E.AccountType IN ('Expenses', 'Other Expense')      
            
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
        SELECT ECP.Name, EC.Name, EC.ID, c.Date, Type, Num, c.Name, Memo, Amount, ECP.Sequence, EC.Sequence        
        FROM EstimateMappings E        
        INNER JOIN EstimateCategories EC ON EC.ID = E.EstimateSubCategoryID        
  LEFT JOIN EstimateCategories ECP ON ECP.ID = EC.ParentEstimateCategoryID       
        INNER JOIN @ClassDetail c ON c.AccountID = E.QBAccountID        
        WHERE EC.Name = 'Other' AND E.AccountType = 'Income'        
      
  SELECT * FROM @Results;      
  --select * from @Results      
        --INSERT INTO @ActualDetail        
        --SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount        
        --FROM @Results        
    END        
    ELSE IF @Item LIKE ('%PROJECT TOTALS')        
    BEGIN        
        --SET @Item = LOWER(SUBSTRING(LTRIM(@Item), 7, LEN(LTRIM(@Item)) - 5));        
        
        --INSERT INTO @ActualDetail        
        --SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount        
        --FROM @Results;        
  SELECT * FROM @Results ORDER BY CategorySequence, ItemSequence;      
    END        
    ELSE IF @Item LIKE ('TOTAL%')        
    BEGIN        
        SET @Item = LOWER(SUBSTRING(LTRIM(@Item), 7, LEN(LTRIM(@Item)) - 5));        
        
        --INSERT INTO @ActualDetail        
        --SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount        
        --FROM @Results         
  SELECT * FROM @Results      
        WHERE EstimateCategory = @Item      
  ORDER BY CategorySequence, ItemSequence;      
    END        
    ELSE        
    BEGIN        
        --INSERT INTO @ActualDetail        
        --SELECT Date, Type, Num, Name, Memo, Category, SourceAccount, Class, [Division/Location], [Clear Status], Amount        
        --FROM @Results        
  SELECT * FROM @Results      
        WHERE EstimateSubCategory = @Item      
  ORDER BY CategorySequence, ItemSequence, Date;      
    END;        
        
    --SELECT CONVERT(NVARCHAR(10), Date, 101) AS Date, Type,         
    --       CASE WHEN [Clear Status] = 'Missing' THEN '' ELSE [Clear Status] END AS [Clear Status],         
    --       Num, Name, [Division/Location],         
    --       CASE WHEN Class = 'Missing' THEN '' ELSE Class END AS Class,         
    --       Memo, SourceAccount, Category, Amount,        
    --       SUM(Amount) OVER (ORDER BY Date, Memo, LEN(Num), Num, Name, Category         
    --                         ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Balance         
    --FROM @ActualDetail;        
      
 --select * from @Results      
 --GROUP BY       
         
END;