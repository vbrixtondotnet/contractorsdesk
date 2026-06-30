CREATE FUNCTION [dbo].[fnCDJobBalance]()
RETURNS @JobBalance TABLE
(
    QbClassId uniqueidentifier,
    JobBalance DECIMAL(18,2)
)
AS
BEGIN

DECLARE @ActiveJobs TABLE (    
  QbClassId uniqueidentifier,
  ProposalId uniqueidentifier
)  

INSERT @ActiveJobs    
    SELECT c.ID, p.ID
    FROM QBClasses c
    LEFT JOIN Proposals p
    on c.ID = p.QbClassId
    WHERE c.IsDeleted = 0
    and (c.ActiveJobs = 1  OR c.ActiveSpecJobs = 1)

DECLARE @RevisedEstimates TABLE      
(         
    ClassId uniqueidentifier,
    CostToDate decimal(21,2) null,
    OwnerDeposits decimal(21,2) null
)   
  
DECLARE @Transactions TABLE(  
 ClassId uniqueidentifier,
 AccountType nvarchar(150),
 Amount decimal(21,2)
)  
  
INSERT INTO @Transactions  
select   
 j.QbClassId,
 a.AccountType,
 t.Amount
from QBTransactions t  
inner join @ActiveJobs j
on j.QbClassId = t.ClassID
inner join QBAccounts a  
on a.ID = t.AccountID  
left join EstimateMappings em  
on em.QBAccountID = a.ID  
left join EstimateCategories ec  
on ec.ID = em.EstimateSubCategoryID  
order by em.EstimateSubCategoryID  
  
    
INSERT INTO @RevisedEstimates
SELECT 
    ClassId, 
    (SELECT CONVERT(DECIMAL(18,2), SUM(Amount))     
    FROM @Transactions     
   WHERE ClassId = t.ClassId and AccountType <> 'Income'),
    (SELECT CONVERT(DECIMAL(18,2), SUM(Amount))     
    FROM @Transactions t     
   WHERE  ClassId = t.ClassId and AccountType = 'Income')
   FROM @Transactions t

      
 INSERT INTO @JobBalance   
 SELECT ClassId, ISNULL(OwnerDeposits,0) - ISNULL(CostToDate,0) FROM @RevisedEstimates 

 RETURN;
END