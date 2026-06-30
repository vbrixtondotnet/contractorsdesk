CREATE   VIEW vwActiveSpecJobDetails
AS
SELECT 
	a.ID as AccountID,
	a.ParentID as ParentID,
	a.Name as OriginalName,
	CASE 
		WHEN a.Name = 'Electrical Material' THEN 'Electrical Contractor'
		WHEN a.Name = 'Material/Lumber' THEN 'Finish Material'
		WHEN a.Name = 'Mortgage expense' THEN 'Interest Expense'
		WHEN a.Name = 'Utilities' THEN 'Temp Utilities'
		WHEN a.Name = 'Permits' THEN 'Building permits'
		WHEN a.Name = 'Clean Up/Trash' THEN 'Trash Hauling'
		WHEN a.Name = 'Fireplace Contractor' THEN 'Fireplace  Contractor'
		WHEN a.Name = 'Foundation' THEN 'Foundation Contractor'
		WHEN a.Name = 'Siding Material' THEN 'Other'
		WHEN a.Name = 'Gas' THEN 'Other'
		WHEN a.Name = 'Engineering' THEN 'Other'
		WHEN a.Name = 'Lumber' THEN 'Lumber/Hardware'
		WHEN a.Name = 'Plans' THEN 'Plans/survey'
		WHEN a.Name = 'Water' THEN 'Waterproofing'
		WHEN a.Name = 'Roofing Material' THEN 'Roofing Contractor'
		WHEN a.Name = 'Window/Door Install' THEN 'Window and Door Install'
		WHEN a.Name = 'Landscape Contractor' THEN 'Landscape'
		WHEN a.Name = 'Trash/Hauling' THEN 'Trash Hauling'
		WHEN a.Name = 'Recording Fees' THEN 'Other'
		WHEN a.Name = 'Rock Veneer, Brick, Stone Contractor' THEN 'Other'
		WHEN a.Name = 'Stucco Contractor' THEN 'Other'
		WHEN a.Name = 'Sheet Metal Contractor' THEN 'Sheet metal'
		WHEN a.Name = 'Waterproof' THEN 'Waterproofing'
		WHEN a.Name = 'Foundation/Concrete Contractor' THEN 'Foundation Contractor'
		WHEN a.Name = 'Heating and Air Contractor' THEN 'Heating & Air Contractor'
		WHEN a.Name = 'Steel Contractor' THEN 'Structural Steel'
		ELSE a.Name 
	END AS MappingName, 
	CASE WHEN AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' 
		WHEN AccountType = 'OtherIncome' THEN 'Other Income' 
		WHEN AccountType = 'OtherExpense' THEN 'Other Expense' 
		ELSE AccountType 
	END AS AccountType,
 SUM(tc.Amount) as Amount, qb.FullyQualifiedName AS ProjectName  
   FROM QBAccounts a  
   INNER JOIN QBTransactions tc ON a.ID = tc.AccountID  
   INNER JOIN QBClasses qb on qb.ID = tc.ClassID  
   WHERE tc.TransactionDate BETWEEN ISNULL('5/1/2012', tc.TransactionDate) 
   AND ISNULL(GETDATE(), tc.TransactionDate)   
   AND AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.ActiveSpecJobs = 1  
   GROUP BY a.ID,a.ParentID,a.Name,AccountType,qb.FullyQualifiedName