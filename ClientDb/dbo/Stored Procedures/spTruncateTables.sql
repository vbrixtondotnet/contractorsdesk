CREATE PROCEDURE [dbo].[spTruncateTables]
AS
BEGIN
			DECLARE @TABLES NVARCHAR(MAX) = 'SalesBooks|PurchaseBooks|QBJournalEntries|QBBills|QBCreditMemos|QBInvoices|QBSalesReceipts|QBVendorCredits|QBVendors|QBCustomers|QBItems|QBAccounts|Token'
			DECLARE @TableName NVARCHAR(50)
			DECLARE db_cursor CURSOR FOR 
			SELECT value FROM STRING_SPLIT(@TABLES, '|');
			OPEN db_cursor;  
			FETCH NEXT FROM db_cursor INTO @TableName

				WHILE @@FETCH_STATUS = 0  
				BEGIN
					DECLARE @SQL nvarchar(MAX) = (
						SELECT N'TRUNCATE TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(object_id)) + N'.' + QUOTENAME(name) +N';'
						FROM sys.tables
						WHERE  name = @TableName
						FOR XML PATH(''), TYPE).value('.', 'nvarchar(MAX)');
						EXECUTE sp_executesql @SQL;

						--PRINT @SQL 
					FETCH NEXT FROM db_cursor INTO @TableName
				END
				CLOSE db_cursor;
				DEALLOCATE db_cursor;
END;