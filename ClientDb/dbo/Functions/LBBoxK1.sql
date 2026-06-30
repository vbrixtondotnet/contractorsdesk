CREATE FUNCTION [dbo].[LBBoxK1](
				@Date DATE,
				@DocNumber NVARCHAR(500),
				@VendorName NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
		
		--[K1] TVSH e zbritshme me 18%: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat
		--(35+39+43+47+53+57+61+65) dhe i shumëzon me normën standarde të TVSH-së, 18%.

		SELECT @Result = SUM(K1)*18/100  
		FROM (
				SELECT ISNULL(Box35,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box39,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box43,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box47,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box53,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box57,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box61,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
				UNION ALL
				SELECT ISNULL(Box65,0) AS K1
				FROM PurchaseBooks
				WHERE TxnDate = ISNULL(@Date, TxnDate)  
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND VendorName = ISNULL(@VendorName, VendorName)
			 ) K1

		RETURN @Result
        END;