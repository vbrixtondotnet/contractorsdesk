CREATE VIEW [dbo].[Test1] AS
SELECT a.FullyQualifiedName AS AccountName, t.Amount, t.TxnNumber, t.Memo, c.FullyQualifiedName AS Class, v.DisplayName AS Vendor 
	FROM [dbo].[QBTransactions]  t
	FULL JOIN QBAccounts a ON a.ID = t.AccountID
	FULL JOIN QBClasses c ON c.ID = t.ClassID
	FULL JOIN QBVendors v ON v.ID = t.VendorID
	WHERE a.FullyQualifiedName = 'CHA Construction 0090' AND c.AllowedForJobReports = 0