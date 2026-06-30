CREATE   PROCEDURE [dbo].[spActiveConstructionJobsDetails_Test]   
@StartDate DATE = NULL,  
@EndDate DATE = NULL,  
@QBClass NVARCHAR(255)  
AS  
BEGIN   
  
SET NOCOUNT ON;  
     IF 1=0 BEGIN  
       SET FMTONLY OFF  
     END  
  
  
IF 'All' IN (SELECT value FROM STRING_SPLIT(@QBClass,'|'))  
SET @QBClass = NULL  

 SELECT tc.ID, a.Name AS Name, 
   a.FullyQualifiedName AS FullyQualifiedName, 
   b.Name as Parent,
   CASE WHEN 
	 a.AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' 
	WHEN a.AccountType = 'OtherIncome' THEN 'Other Income' 
	WHEN a.AccountType = 'OtherExpense' THEN 'Other Expense' 
	ELSE a.AccountType END AS AccountType,
	SUM(tc.Amount), 
	qb.FullyQualifiedName AS QuickBooksClassShort  
   FROM QBAccounts a  
   INNER JOIN QBTransactions tc ON a.ID = tc.AccountID  
   INNER JOIN QBClasses qb on qb.ID = tc.ClassID  
   LEFT JOIN QBAccounts b on a.ParentID = b.ListID
   WHERE tc.TransactionDate BETWEEN ISNULL(@StartDate, tc.TransactionDate) AND ISNULL(@EndDate, tc.TransactionDate)
   AND a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.ActiveJobs = 1 
   AND qb.FullyQualifiedName = @QBClass
   GROUP BY 
	tc.ID, 
	a.Name,
	a.FullyQualifiedName,
	b.Name,
	a.AccountType,
	qb.FullyQualifiedName

  
--DECLARE @AccTable TABLE  
--(  
-- TransactionId nvarchar(100),
-- ItemName NVARCHAR(255),  
-- FullyQualifiedName NVARCHAR(255), 
-- Parent NVARCHAR(255), 
-- AccountType NVARCHAR(255),  
-- Amount DECIMAL(21,9),  
-- QuickBooksClass NVARCHAR(255)  
--)  
  
  
--INSERT INTO @AccTable  
-- SELECT ID, Name,FullyQualifiedName, Parent, AccountType, SUM(Amount) AS Amount, QuickBooksClassShort  
-- FROM(  
--   SELECT tc.ID, a.Name AS Name, 
--   a.FullyQualifiedName AS FullyQualifiedName, 
--   b.Name as Parent,
--   CASE WHEN 
--	 a.AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' 
--	WHEN a.AccountType = 'OtherIncome' THEN 'Other Income' 
--	WHEN a.AccountType = 'OtherExpense' THEN 'Other Expense' 
--	ELSE a.AccountType END AS AccountType,
--	 tc.Amount, qb.FullyQualifiedName AS QuickBooksClassShort  
--   FROM QBAccounts a  
--   INNER JOIN QBTransactions tc ON a.ID = tc.AccountID  
--   INNER JOIN QBClasses qb on qb.ID = tc.ClassID  
--   LEFT JOIN QBAccounts b on a.ParentID = b.ListID
--   WHERE tc.TransactionDate BETWEEN ISNULL(@StartDate, tc.TransactionDate) AND ISNULL(@EndDate, tc.TransactionDate)   
--   AND a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.ActiveJobs = 1  
--  )tbl  
-- WHERE QuickBooksClassShort IN (SELECT CASE WHEN @QBClass IS NULL THEN QuickBooksClassShort  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))  
-- GROUP BY ID, Name,Parent, FullyQualifiedName, AccountType, QuickBooksClassShort  
   
  
  
  --SELECT * FROM @AccTable
    
END