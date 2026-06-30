CREATE FUNCTION [dbo].[LBM500Box8](
				@Year NVARCHAR(500),
				@VendorName NVARCHAR(500),
				@Renttxn INT
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
		
		--[8] Vlera e blerjeve te liruar nga TVSH: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat
		--(31+32+33+34).

		SELECT @Result = SUM(ISNULL(B8,0)) 
		FROM (
				SELECT ISNULL(Box31,0) AS B8
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,','))
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box32,0) AS B8
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box33,0) AS B8
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
				UNION ALL
				SELECT ISNULL(Box34,0) AS B8
				FROM PurchaseBooks
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,',')) 
				AND VendorName = ISNULL(@VendorName, VendorName)
				AND Renttxn = @Renttxn
			 ) B8

		RETURN @Result
        END;