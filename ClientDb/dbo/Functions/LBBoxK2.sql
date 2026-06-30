CREATE FUNCTION [dbo].[LBBoxK2](
				@Date DATE,
				@DocNumber NVARCHAR(500),
				@VendorName NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
			
		--[K2] TVSH e zbritshme me 8%: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat 
		-- (37+41+45+49+51+55+59+63) dhe i shumëzon me normën e reduktuar të TVSH-së, 8%.	

		SELECT @Result = SUM(ISNULL(K2,0))*8/100  
		FROM (
				SELECT Box37 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box41 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box45 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box49 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box51 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box55 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box59 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT Box63 AS K2
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
			 ) K2

		RETURN @Result
        END;