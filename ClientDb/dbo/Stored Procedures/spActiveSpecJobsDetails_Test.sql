CREATE   PROCEDURE [dbo].[spActiveSpecJobsDetails_Test]   
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
 
 --SELECT Account, AccountType, SUM(Amount) AS Amount, QuickBooksClassShort  
 --FROM(  
 --  SELECT a.FullyQualifiedName AS Account, CASE WHEN AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' WHEN AccountType = 'OtherIncome' THEN 'Other Income' WHEN AccountType = 'OtherExpense' THEN 'Other Expense' ELSE AccountType END AS AccountType,
 --tc.Amount, qb.FullyQualifiedName AS QuickBooksClassShort  
 --  FROM QBAccounts a  
 --  INNER JOIN QBTransactions tc ON a.ID = tc.AccountID  
 --  INNER JOIN QBClasses qb on qb.ID = tc.ClassID  
 --  WHERE tc.TransactionDate BETWEEN ISNULL(@StartDate, tc.TransactionDate) AND ISNULL(@EndDate, tc.TransactionDate)   
 --  AND AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.ActiveSpecJobs = 1  
 -- )tbl  
 --WHERE QuickBooksClassShort IN (SELECT CASE WHEN @QBClass IS NULL THEN QuickBooksClassShort  ELSE value END FROM STRING_SPLIT(ISNULL(@QBClass, ''),'|'))  
 --GROUP BY Account, AccountType, QuickBooksClassShort  
   
  SELECT 
	a.ID as AccountID,
	a.ParentID as ParentID,
	a.Name AS ItemName, 
	CASE WHEN AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' 
		WHEN AccountType = 'OtherIncome' THEN 'Other Income' 
		WHEN AccountType = 'OtherExpense' THEN 'Other Expense' 
		ELSE AccountType 
	END AS AccountType,
 SUM(tc.Amount), qb.FullyQualifiedName AS QuickBooksClassShort  
   FROM QBAccounts a  
   INNER JOIN QBTransactions tc ON a.ID = tc.AccountID  
   INNER JOIN QBClasses qb on qb.ID = tc.ClassID  
   WHERE tc.TransactionDate BETWEEN ISNULL(@StartDate, tc.TransactionDate) 
   AND ISNULL(@EndDate, tc.TransactionDate)   
   AND qb.FullyQualifiedName = @QBClass
   AND AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.ActiveSpecJobs = 1  
   GROUP BY a.ID,a.ParentID,a.Name,AccountType,qb.FullyQualifiedName
END