CREATE FUNCTION [dbo].[LBM500Box7](
				@Year NVARCHAR(500),
				@VendorName NVARCHAR(500),
				@Renttxn INT
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
		
		--[7] Vlera e blerjeve pa TVSH: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat
		--(35+39+43+47+53+57+61+65+37+41+45+49+51+55+59+63).

		SELECT @Result = SUM(ISNULL(B7,0)) 
		FROM (
				SELECT ISNULL(Box35,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,','))
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box39,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box43,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box47,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box53,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box57,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box61,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box65,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box37,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box41,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box45,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box49,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box51,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box55,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box59,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box63,0) AS B7
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
			 ) B7

		RETURN @Result
        END;