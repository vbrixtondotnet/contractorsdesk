CREATE FUNCTION [dbo].[LBM500Box9WithRent1](
				@Year NVARCHAR(500),
				@VendorName NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
		
		--[9] Vlera e paguar per qira e prones se palujtshme kur Renttxn=1
		--(31+32+33+34+35+39+43+47).

		SELECT @Result = SUM(ISNULL(B9,0)) 
		FROM (
				SELECT ISNULL(Box31,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,','))
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box32,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box33,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box34,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box35,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box39,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box43,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
				UNION ALL
				SELECT ISNULL(Box47,0) AS B9
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = 1
			 ) B9

		RETURN @Result
        END;