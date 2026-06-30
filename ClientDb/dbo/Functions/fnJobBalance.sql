
CREATE FUNCTION fnJobBalance ()
RETURNS @JobBalance TABLE
(
    QbClassId uniqueidentifier,
    JobBalance DECIMAL(18,2)
)
AS
BEGIN
DECLARE @ActiveJobs TABLE    
 (    
  QbClassId uniqueidentifier,    
  AccountType NVARCHAR(50),    
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @GrossProfit TABLE    
 (    
  QbClassId uniqueidentifier,  
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @NetOperatingIncome TABLE    
 (    
  QbClassId uniqueidentifier,    
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @NetIncome TABLE    
 (    
  QbClassId uniqueidentifier,  
  JobBalance DECIMAL(21,2)    
 )    
    
 DECLARE @QBCHA9100 TABLE    
 (    
  QbClassId uniqueidentifier,  
  JobBalance DECIMAL(21,2)    
 )    
    
 --INSERT @QBCHA9100    
 -- SELECT 'Acct Balance:' AS ActiveConstructionJobs, SUM(Amount) AS JobBalance    
 -- FROM QBTransactions T    
 -- INNER JOIN QBAccounts A ON A.ID = T.AccountID    
    
 INSERT @ActiveJobs    
  SELECT ID, AccountType, JobBalance    
  FROM(    
    SELECT c.ID, a.AccountType, SUM(Amount) AS JobBalance    
    FROM QBAccounts a    
    FULL JOIN QBTransactions t ON a.ID = t.AccountID    
    INNER JOIN QBClasses c ON c.ID = t.ClassID     
    WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense')   
  AND (c.ActiveJobs = 1  OR c.ActiveSpecJobs = 1)  
    GROUP BY c.ID, a.AccountType   
   )j        
    
 INSERT @GrossProfit    
  SELECT QbClassId, SUM(JobBalance) AS JobBalance    
  FROM(    
    SELECT QbClassId, JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'Income'    
    UNION ALL    
    SELECT QbClassId, -JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'CostofGoodsSold'    
   ) gp    
  GROUP BY QbClassId    
    
 INSERT @NetOperatingIncome    
  SELECT QbClassId, SUM(JobBalance) AS JobBalance    
  FROM(      
    SELECT QbClassId, JobBalance    
    FROM @GrossProfit    
    UNION ALL    
    SELECT QbClassId, -JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'Expense'    
   ) noi    
  GROUP BY QbClassId    
    
 INSERT @NetIncome    
  SELECT QbClassId, SUM(JobBalance) AS JobBalance    
  FROM(      
    SELECT QbClassId, JobBalance    
    FROM @NetOperatingIncome    
    UNION ALL    
    SELECT QbClassId, JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'OtherIncome'    
    UNION ALL    
    SELECT QbClassId, -JobBalance    
    FROM @ActiveJobs    
    WHERE AccountType = 'OtherExpense'    
   ) ni    
  GROUP BY QbClassId    
      
 INSERT INTO @JobBalance   
 SELECT QbClassId, JobBalance FROM @NetIncome 

 RETURN;
END
