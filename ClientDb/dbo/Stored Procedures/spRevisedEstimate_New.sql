CREATE   PROCEDURE [dbo].[spRevisedEstimate_New]   
    @ClassID NVARCHAR(255)  
AS  
BEGIN     
  
  
 DECLARE @InitialDeposit DECIMAL(21,9);  
 DECLARE @CurrentJobBalance DECIMAL(21,9);  
 DECLARE @RequestedAmount DECIMAL(21,9);  
  
    DECLARE @Results TABLE  
    (  
        Sequence INT,  
  Item NVARCHAR(255),  
        ParentEstimateCategoryID NVARCHAR(255),  
  EstimateCategoryID NVARCHAR(255),  
  ProposalLineID NVARCHAR(255),  
  Level INT,  
        TotalToDate DECIMAL(21,9),  
        Estimate DECIMAL(21,9),  
  RevisedEstimate DECIMAL(21,9),  
        Balance DECIMAL(21,9),  
        Percentage DECIMAL(21,9),  
  SortBy INT  
    )  
  
    DECLARE @MonthlyBudgetReport TABLE  
    (  
        Sequence INT,  
  Item NVARCHAR(255),  
        ParentEstimateCategoryID NVARCHAR(255),  
  EstimateCategoryID NVARCHAR(255),  
  ProposalLineID NVARCHAR(255),  
  Level INT,  
        TotalToDate DECIMAL(21,9),  
        Estimate DECIMAL(21,9),  
  RevisedEstimate DECIMAL(21,9),  
  SortBy INT  
    )  
  
    DECLARE @Actual TABLE  
    (  
        EstimateCategoryID NVARCHAR(255),  
        ParentEstimateCategoryID NVARCHAR(255),  
        TotalToDate DECIMAL(21,9)  
    )  
  
    DECLARE @Estimate TABLE  
    (  
        ProposalLineID NVARCHAR(255),  
  EstimateCategoryID NVARCHAR(255),  
        ParentEstimateCategoryID NVARCHAR(255),  
        Estimate DECIMAL(21,9)  
    )  
  
    DECLARE @RevisedEstimate TABLE  
    (  
        ProposalLineID NVARCHAR(255),  
  EstimateCategoryID NVARCHAR(255),  
        ParentEstimateCategoryID NVARCHAR(255),  
        RevisedEstimate DECIMAL(21,9)  
    )  
  
    DECLARE @Merge TABLE  
    (  
        Sequence INT,  
  EstimateCategoryID NVARCHAR(255),  
        ParentEstimateCategoryID NVARCHAR(255),  
  EstimateCategory NVARCHAR(255),  
  SubCategory NVARCHAR(255),  
  ProposalLineID NVARCHAR(255),  
  Level INT,  
        TotalToDate DECIMAL(21,9),  
  Estimate DECIMAL(21,9),  
  RevisedEstimate DECIMAL(21,9)  
    )  
  
 
  
    CREATE TABLE #Class  
    (  
        QBClass NVARCHAR(255),  
  QBAccountID NVARCHAR(255),  
        TotalToDate DECIMAL(21,9)  
    );  
  
 CREATE TABLE #EstimateCategories  
 (  
        Sequence INT,  
  EstimateCategoryID NVARCHAR(255),  
  SubCategory NVARCHAR(255),  
  ParentEstimateCategoryID NVARCHAR(255),  
  Category NVARCHAR(255),  
  Level INT  
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
  
 DECLARE @OwnerOposit TABLE  
 (  
  Amount DECIMAL(21,2)  
 )  
  
  
 INSERT @OpenJobs  
  SELECT AccountType, JobBalance  
  FROM(  
    SELECT C.ID AS QBClassID, a.AccountType, SUM(ISNULL(Amount, 0)) AS JobBalance  
    FROM QBAccounts a  
    FULL JOIN QBTransactions t ON a.ID = t.AccountID  
    INNER JOIN QBClasses c ON c.ID = t.ClassID  
    WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND c.OpenJob = 1   
    AND C.ID = @ClassID  
    GROUP BY C.ID, a.AccountType  
   )j   
    
  
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
  
 INSERT INTO @OwnerOposit  
  SELECT Amount FROM QBAccounts a  
  INNER JOIN QBTransactions t on t.AccountID = a.ID  
  INNER JOIN QBClasses c on c.id = t.ClassID  
  WHERE a.FullyQualifiedName like 'Owner Deposit'   
  AND C.ID = @ClassID  
    
  
 SELECT TOP 1 @InitialDeposit = Amount  
 FROM @OwnerOposit  
 WHERE Amount % 5000 = 0;  
  
 -- Get the Current Job Balance  
 SELECT @CurrentJobBalance = JobBalance FROM @NetIncome;  
  
 -- Additional logic for RequestedAmount  
 -- IF @InitialDeposit * 0.2 > @CurrentJobBalance  
 --  SET @RequestedAmount = 0;  
 -- ELSE  
 --  SET @RequestedAmount = @InitialDeposit;  
  
    If @CurrentJobBalance > @InitialDeposit * 2  
     SET @RequestedAmount = 0;  
    ELSE  
        SET @RequestedAmount = @InitialDeposit;  
  

  
 INSERT INTO  #Class  
    SELECT ClassID, AccountID, TotalToDate  
 FROM(  
   SELECT C.ID AS ClassID, t.AccountID,   
   SUM(Amount) AS TotalToDate  
   FROM QBAccounts a  
   INNER JOIN QBTransactions t ON t.AccountID = a.ID   
   INNER JOIN QBClasses c ON c.ID = t.ClassID  
  
   GROUP BY C.ID, t.AccountID  
  ) t  
 WHERE ClassID= @ClassID  
  
 ;WITH CategoryHierarchy AS (  
  SELECT   
   Sequence,  
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
   ec.Sequence,  
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
  Sequence,   
  ID AS EstimateCategoryID,  
  SubCategory,  
  ParentEstimateCategoryID,  
  Category,  
  Level  
 FROM   
  CategoryHierarchy  
 --WHERE Level = 0  
 ORDER BY   
  Category;  
  
    INSERT @Actual  
  SELECT EstimateCategoryID, ParentEstimateCategoryID,   
     SUM(TotalToDate) AS TotalToDate  
     FROM (  
    SELECT EM.EstimateCategoryID, EM.ParentEstimateCategoryID, c.TotalToDate  
    FROM EstimateMappings E  
    INNER JOIN #EstimateCategories EM ON EM.EstimateCategoryID = E.EstimateSubCategoryID  
    INNER JOIN #Class c ON c.QBAccountID = E.QBAccountID  
    WHERE   
     (EM.SubCategory <> 'Other' OR   
     (EM.SubCategory = 'Other' AND E.AccountType IN ('Expenses', 'Other Expense')))  
       )t  
  GROUP BY EstimateCategoryID, ParentEstimateCategoryID  
  
  
  INSERT INTO @Estimate  
  SELECT PH.ProposalLineID, EC.EstimateCategoryID, EC.ParentEstimateCategoryID, 0 AS Estimate  
  FROM   
   Proposals P  
   INNER JOIN ProposalLinesHistory PH ON PH.ProposalID = P.ID  
   INNER JOIN #EstimateCategories EC ON EC.EstimateCategoryID = PH.EstimateCategoryID AND EC.ParentEstimateCategoryID = PH.ParentEstimateCategoryID  
   INNER JOIN QBClasses C ON C.ID = P.QBClassID  
   -- Join with the earliest record subquery  
   INNER JOIN (  
    SELECT ProposalLineID, EstimateCategoryID, ParentEstimateCategoryID, Amount  
    FROM ProposalLinesHistory PLH  
    WHERE   
     ChangeDate = (  
      SELECT MIN(ChangeDate)  
      FROM ProposalLinesHistory  
      WHERE   
       ProposalLineID = PLH.ProposalLineID   
       AND EstimateCategoryID = PLH.EstimateCategoryID  
       AND ParentEstimateCategoryID = PLH.ParentEstimateCategoryID   
       AND ProposalID = PLH.ProposalID  
     )  
   ) EarliestPH ON EarliestPH.ProposalLineID = PH.ProposalLineID AND EarliestPH.EstimateCategoryID = PH.EstimateCategoryID  
        AND EarliestPH.ParentEstimateCategoryID = PH.ParentEstimateCategoryID   
  WHERE C.ID = @ClassID   
   AND ChangeType <> 'DocStatusChanged(Accepted)'  
  --GROUP BY EC.EstimateCategoryID, EC.ParentEstimateCategoryID, PH.ProposalLineID  
  ORDER BY EC.EstimateCategoryID, EC.ParentEstimateCategoryID;  
  
  
    INSERT @RevisedEstimate  
        SELECT PE.ID, EC.EstimateCategoryID, EC.ParentEstimateCategoryID, SUM(PE.Amount) AS Estimate  
        FROM Proposals P  
  INNER JOIN ProposalLines PE ON PE.ProposalID = P.ID  
  INNER JOIN #EstimateCategories EC ON EC.EstimateCategoryID = PE.EstimateCategoryID AND EC.ParentEstimateCategoryID = PE.ParentEstimateCategoryID  
  INNER JOIN QBClasses C ON C.ID = P.QBClassId   
  WHERE C.ID = @ClassID  
  GROUP BY EC.EstimateCategoryID, EC.ParentEstimateCategoryID, PE.ID  
  ORDER BY EC.EstimateCategoryID, EC.ParentEstimateCategoryID  
  
    
 INSERT INTO @Merge (Sequence, EstimateCategoryID, ParentEstimateCategoryID, EstimateCategory, SubCategory, ProposalLineID, Level, TotalToDate, Estimate, RevisedEstimate)  
 SELECT   
  E.Sequence,  
  T.EstimateCategoryID,   
  T.ParentEstimateCategoryID,   
  E.Category,  
  E.SubCategory,  
  T.ProposalLineID,  
  E.Level,   
  ISNULL(T.TotalToDate, 0) AS TotalToDate,   
  ISNULL(T.Estimate, 0) AS Estimate,  
  ISNULL(T.RevisedEstimate, 0) AS RevisedEstimate  
 FROM (  
  SELECT   
   COALESCE(A.EstimateCategoryID, E.EstimateCategoryID, RE.EstimateCategoryID) AS EstimateCategoryID,   
   COALESCE(A.ParentEstimateCategoryID, E.ParentEstimateCategoryID, RE.ParentEstimateCategoryID) AS ParentEstimateCategoryID,   
   ISNULL(E.ProposalLineID, RE.ProposalLineID) AS ProposalLineID,  
   SUM(ISNULL(A.TotalToDate, 0)) AS TotalToDate,   
   SUM(ISNULL(E.Estimate, 0)) AS Estimate,  
   SUM(ISNULL(RE.RevisedEstimate, 0)) AS RevisedEstimate  
  FROM   
   @Actual A  
   FULL JOIN @Estimate E ON E.EstimateCategoryID = A.EstimateCategoryID AND E.ParentEstimateCategoryID = A.ParentEstimateCategoryID  
   FULL JOIN @RevisedEstimate RE ON RE.EstimateCategoryID = A.EstimateCategoryID AND RE.ParentEstimateCategoryID = A.ParentEstimateCategoryID AND RE.ProposalLineID = E.ProposalLineID  
  GROUP BY   
   COALESCE(A.EstimateCategoryID, E.EstimateCategoryID, RE.EstimateCategoryID),   
   COALESCE(A.ParentEstimateCategoryID, E.ParentEstimateCategoryID, RE.ParentEstimateCategoryID),  
   ISNULL(E.ProposalLineID, RE.ProposalLineID)  
 ) T  
 INNER JOIN #EstimateCategories E ON E.EstimateCategoryID = T.EstimateCategoryID AND E.ParentEstimateCategoryID = T.ParentEstimateCategoryID  
 ORDER BY   
  E.Level;  
  
  
  
    -- Cursor to fetch Category and EstimateSubCategory  
    DECLARE @Category NVARCHAR(255)  
    DECLARE @CategoryID NVARCHAR(255)  
  
    DECLARE @SortBy INT = 0  
  
    DECLARE category_cursor CURSOR FOR  
 SELECT EstimateCategory, ParentEstimateCategoryID   
 FROM(  
   SELECT DISTINCT ParentEstimateCategoryID, EstimateCategory  
   FROM @Merge  
  )T  
 ORDER BY   
   CASE   
  WHEN EstimateCategory = 'Preparation' THEN 1   
  WHEN EstimateCategory = 'Material' THEN 2   
  WHEN EstimateCategory = 'Sub Contractors' THEN 3   
  WHEN EstimateCategory = 'Site Work' THEN 4   
  ELSE 5   
  END;  
    OPEN category_cursor;  
    FETCH NEXT FROM category_cursor INTO @Category, @CategoryID  
  
    WHILE @@FETCH_STATUS = 0  
    BEGIN  
        -- Insert Category  
        INSERT INTO @MonthlyBudgetReport   
        SELECT NULL, UPPER(@Category), @CategoryID, NULL, NULL, NULL, NULL, NULL, NULL, @SortBy  
        INSERT INTO @MonthlyBudgetReport   
        SELECT DISTINCT  
            Sequence, '      '+SubCategory, @CategoryID, EstimateCategoryID, ProposalLineID, Level, TotalToDate, Estimate, RevisedEstimate, @SortBy+1  
        FROM @Merge  
        WHERE ParentEstimateCategoryID = @CategoryID AND EstimateCategory = @Category  
  
  --INSERT INTO @MonthlyBudgetReport  
  --      SELECT NULL, 'TOTAL '+UPPER(@Category) AS Item, @CategoryID, NULL, NULL, ISNULL(SUM(TotalToDate),0), ISNULL(SUM(Estimate),0), ISNULL(SUM(RevisedEstimate),0), @SortBy+2  
  --      FROM @Merge  
  --      WHERE ParentEstimateCategoryID = @CategoryID AND EstimateCategory = @Category  
    
  
  SET @SortBy = @SortBy+3  
  
        FETCH NEXT FROM category_cursor INTO @Category, @CategoryID;  
    END  
  
    CLOSE category_cursor;  
    DEALLOCATE category_cursor;  
  
 -- INSERT @Results  
 --  SELECT EstimateCategoryID, Level, TotalToDate, Estimate, RevisedEstimate, (RevisedEstimate - TotalToDate) AS Balance, (SELECT [dbo].[getPercentage] (ISNULL(TotalToDate,0), ISNULL(RevisedEstimate,0))) AS Percentage, SortBy  
 --  FROM @MonthlyBudgetReport  
  
INSERT INTO @Results  
SELECT   
    Sequence,  
 Item,  
 ParentEstimateCategoryID,  
 EstimateCategoryID,   
 ProposalLineID,  
    Level,   
    TotalToDate,   
    Estimate,   
    RevisedEstimate,   
    -- Calculate Balance: use RevisedEstimate if it's different than 0, otherwise use Estimate  
    (CASE   
        WHEN RevisedEstimate <> 0   
            THEN (RevisedEstimate - TotalToDate)   
        ELSE   
            (Estimate - TotalToDate)   
    END) AS Balance,  
    -- Calculate Percentage: use RevisedEstimate if it's different than 0, otherwise use Estimate  
    (SELECT [dbo].[getPercentage] (  
        ISNULL(TotalToDate, 0),   
        CASE   
            WHEN RevisedEstimate <> 0   
                THEN RevisedEstimate   
            ELSE   
                Estimate   
        END  
    )) AS Percentage,   
    SortBy  
FROM   
    @MonthlyBudgetReport  
  
  
    SELECT Sequence, Item, ParentEstimateCategoryID, EstimateCategoryID, ProposalLineID, Level, TotalToDate, Estimate, RevisedEstimate, Balance, Percentage, SortBy  
 FROM (  
    SELECT Sequence, Item, ParentEstimateCategoryID, EstimateCategoryID, ProposalLineID, Level, TotalToDate, Estimate, RevisedEstimate, Balance, Percentage, SortBy  
    FROM @Results   
 UNION ALL  
 --   SELECT NULL, 'PROJECT TOTALS', NULL, NULL, MAX(Level+1) AS Level, SUM(TotalToDate), SUM(Estimate), SUM(RevisedEstimate) AS RevisedEstimate, (SUM(Estimate) - SUM(TotalToDate)) AS Balance, (SELECT [dbo].[getPercentage] (ISNULL(SUM(TotalToDate),0), ISNULL(SUM(Estimate),0))) AS Percentage, MAX(SortBy)+1 AS SortBy  
 --   FROM @Results  
 --WHERE Item LIKE 'TOTAL%'  
 --UNION ALL  
    SELECT NULL, 'OWNER DEPOSITS', NULL, NULL, NULL, 1000 AS Level, SUM(Amount), NULL, NULL, NULL, NULL, 1000 AS SortBy  
    FROM @OwnerOposit  
 UNION ALL  
    SELECT NULL, 'JOB BALANCE', NULL, NULL, NULL, 2000 AS Level, SUM(JobBalance), NULL, NULL, NULL, NULL, 2000 AS SortBy  
    FROM @NetIncome  
 UNION ALL  
    SELECT NULL, 'REQUESTED AMOUNT', NULL, NULL, NULL, 3000, @RequestedAmount, NULL, NULL, NULL, NULL, 30000 AS SortBy  
 ) T  
 ORDER BY SortBy, Level, Sequence  
  
  
  
  
     DROP TABLE #EstimateCategories     
  DROP TABLE #Class  
  
  
END