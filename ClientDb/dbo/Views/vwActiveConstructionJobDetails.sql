CREATE VIEW
vwActiveConstructionJobDetails
AS
SELECT tc.ID as ID, 
   CASE 
	WHEN a.Name = 'Clean Up/Trash' THEN 'Clean-up' 
	WHEN a.Name = 'Exterior Railing' THEN 'Exterior and Interior Railing' 
	WHEN a.Name = 'Finish Carpentry Contractor' THEN 'Finish Carpentry' 
	WHEN a.Name = 'Flooring install' THEN 'Flooring Contractor' 
	WHEN a.Name = 'Foundation/Concrete Contractor' THEN 'Foundation Contractor' 
	WHEN a.Name = 'Granite Contractor' THEN 'Flooring Material' 
	WHEN a.Name = 'Heating and Air Contractor' THEN 'Heating & Air Contractor' 
	WHEN a.Name = 'Lumber' THEN 'Lumber/Hardware' 
	WHEN a.Name = 'Painting/Stain Contractor' THEN 'Painting Contractor' 
	WHEN a.Name = 'Permits' THEN 'Building permits' 
	WHEN a.Name = 'Roof Contractor' THEN 'Roofing Contractor' 
	WHEN a.Name = 'Sheet Metal Contractor' THEN 'Sheet metal' 
	WHEN a.Name = 'Shower Enclosures Contractor' THEN 'Shower Enclosure/Mirror Contractor' 
	WHEN a.Name = 'Steel Contractor' THEN 'Structural Steel' 
	WHEN a.Name = 'Supervision - GC Fee' THEN 'General Contractor Fee' 
	WHEN a.Name = 'Supervision - On Site' THEN 'On Site Supervision' 
	WHEN a.Name = 'Trash/Hauling' THEN 'Trash Hauling' 
	WHEN a.Name = 'Wallpaper' THEN 'Drywall Contractor' 
	WHEN a.Name = 'Waterproof' THEN 'Waterproofing' 
	WHEN a.Name = 'Window/Door Install' THEN 'Window and Door Install' 
	WHEN a.Name = 'Windows/Sliders' THEN 'Interior doors/Windows' 
	WHEN a.Name = 'Wood Floor Material' THEN 'Flooring Material' 
	WHEN a.Name = 'Closet Contractor' THEN 'Closets' 
	WHEN a.Name = 'Drainage' THEN 'Site Drainage' 
	WHEN a.Name = 'Garage Door Contractor' THEN 'Garage Doors Contractor' 
	WHEN a.Name = 'Landscape Contractor' THEN 'Landscape' 
	WHEN a.Name = 'Railing' THEN 'Internal Railings Contractor' 
	WHEN a.Name = 'Rough Hardware' THEN 'Lumber/Hardware' 
	WHEN a.Name = 'Stairs/Railing Contractor' THEN 'Stairs Contractor' 
	WHEN a.Name = 'Wood Floor Contractor' THEN 'Flooring Material' 
   ELSE a.Name
   END as Name,
   b.Name as Parent,
   CASE WHEN 
	 a.AccountType = 'CostofGoodsSold' THEN 'Cost Of Goods Sold' 
	WHEN a.AccountType = 'OtherIncome' THEN 'Other Income' 
	WHEN a.AccountType = 'OtherExpense' THEN 'Other Expense' 
	ELSE a.AccountType END AS AccountType,
	tc.Amount as Amount, 
	qb.FullyQualifiedName AS ProjectName
   FROM QBAccounts a  
   INNER JOIN QBTransactions tc ON a.ID = tc.AccountID  
   INNER JOIN QBClasses qb on qb.ID = tc.ClassID  
   LEFT JOIN QBAccounts b on a.ParentID = b.ListID
   WHERE tc.TransactionDate BETWEEN ISNULL('2012-05-01', tc.TransactionDate) AND ISNULL('2024-11-20', tc.TransactionDate)
   AND a.AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND qb.ActiveJobs = 1