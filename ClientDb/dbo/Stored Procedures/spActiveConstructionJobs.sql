CREATE PROCEDURE [dbo].[spActiveConstructionJobs]     
@StartDate NVARCHAR(255),    
@EndDate NVARCHAR(255),    
@QBClass NVARCHAR(255)    
AS    
BEGIN     
    
SET NOCOUNT ON;    
     IF 1=0 BEGIN    
       SET FMTONLY OFF    
     END    
    
    
 IF 'All' IN (SELECT value FROM STRING_SPLIT(@QBClass,'|'))    
 SET @QBClass = NULL    
    
 DECLARE @ActiveJobs TABLE    
 (    
  ActiveConstructionJobs NVARCHAR(MAX),    
  AccountType NVARCHAR(50),    
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @GrossProfit TABLE    
 (    
  ActiveConstructionJobs NVARCHAR(MAX),    
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @NetOperatingIncome TABLE    
 (    
  ActiveConstructionJobs NVARCHAR(MAX),    
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @NetIncome TABLE    
 (    
  ActiveConstructionJobs NVARCHAR(MAX),    
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @QBCHA9100 TABLE    
 (    
  ActiveConstructionJobs NVARCHAR(MAX),    
  JobBalance DECIMAL(21,2)    
 )    
    
 INSERT @QBCHA9100    
  SELECT 'QB CHA 9100 Acct Balance:' AS ActiveConstructionJobs, SUM(Amount) AS JobBalance    
  FROM QBTransactions T    
  INNER JOIN QBAccounts A ON A.ID = T.AccountID    
  WHERE A.FullyQualifiedName = 'CHA Const 9100' AND TransactionDate <= @EndDate     
    
 INSERT @ActiveJobs    
  SELECT ActiveConstructionJobs, AccountType, JobBalance    
  FROM(    
    SELECT c.FullyQualifiedName AS ActiveConstructionJobs, a.AccountType, SUM(Amount) AS JobBalance    
    FROM QBAccounts a    
    FULL JOIN QBTransactions t ON a.ID = t.AccountID    
    INNER JOIN QBClasses c ON c.ID = t.ClassID     
    WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense')   
 AND (c.ActiveJobs = 1 and ISNULL(c.IsArchived,0) = 0 and ISNULL(c.IsDeleted,0) = 0 and ISNULL(c.ActiveSpecJobs,0) = 0)  
    AND t.TransactionDate BETWEEN @StartDate AND @EndDate    
    GROUP BY c.FullyQualifiedName, a.AccountType    
   )j     
  WHERE ActiveConstructionJobs IN (SELECT CASE WHEN @QBClass IS NULL THEN ActiveConstructionJobs  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))    
  ORDER BY ActiveConstructionJobs     
    
 INSERT @GrossProfit    
  SELECT ActiveConstructionJobs, SUM(JobBalance) AS JobBalance    
  FROM(    
    SELECT ActiveConstructionJobs, JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'Income'    
    UNION ALL    
    SELECT ActiveConstructionJobs, -JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'CostofGoodsSold'    
   ) gp    
  GROUP BY ActiveConstructionJobs    
    
 INSERT @NetOperatingIncome    
  SELECT ActiveConstructionJobs, SUM(JobBalance) AS JobBalance    
  FROM(      
    SELECT ActiveConstructionJobs, JobBalance    
    FROM @GrossProfit    
    UNION ALL    
    SELECT ActiveConstructionJobs, -JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'Expense'    
   ) noi    
  GROUP BY ActiveConstructionJobs    
    
    
 INSERT @NetIncome    
  SELECT ActiveConstructionJobs, SUM(JobBalance) AS JobBalance    
  FROM(      
    SELECT ActiveConstructionJobs, JobBalance    
    FROM @NetOperatingIncome    
    UNION ALL    
    SELECT ActiveConstructionJobs, JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'OtherIncome'    
    UNION ALL    
    SELECT ActiveConstructionJobs, -JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'OtherExpense'    
   ) ni    
  GROUP BY ActiveConstructionJobs    
      
    
 SELECT  ActiveConstructionJobs, JobBalance, Orderby    
 FROM(    
   SELECT ActiveConstructionJobs, JobBalance, 1 AS Orderby    
   FROM @NetIncome    
   UNION ALL    
   SELECT 'Total Job Balance:' AS ActiveConstructionJobs, SUM(JobBalance), 2 AS Orderby    
   FROM @NetIncome    
   UNION ALL    
   SELECT ActiveConstructionJobs, JobBalance, 3 AS OrderBy FROM @QBCHA9100    
   UNION ALL    
   SELECT 'Difference:' AS ActiveConstructionJobs, SUM(JobBalance), 4 AS Orderby    
   FROM (    
     SELECT ActiveConstructionJobs, JobBalance, 3 AS OrderBy FROM @QBCHA9100    
     UNION ALL    
     SELECT 'Total Job Balance' AS ActiveConstructionJobs, SUM(-JobBalance), 2 AS Orderby    
     FROM @NetIncome    
    )d    
  )T    
 ORDER BY Orderby, ActiveConstructionJobs    
    
    
END