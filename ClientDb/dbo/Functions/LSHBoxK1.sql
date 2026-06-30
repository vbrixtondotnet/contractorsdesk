CREATE FUNCTION [dbo].[LSHBoxK1](
				@FromDate DATE,
				@ToDate DATE,
				@DocNumber NVARCHAR(500),
				@CustomerName NVARCHAR(500),
				@FiscalNumber NVARCHAR(500),
				@VATNumber NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
			
		--[K1] Totali i TVSH-së së llogaritur me normën 18%: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat
        -- (12+16+20+24+28) dhe i shumëzon me normën standarde të TVSH-së, 18%.	

		SELECT @Result = SUM(ISNULL(K1,0))*18/100
		FROM (
				SELECT Box12 AS K1
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box16 AS K1
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box20 AS K1
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box24 AS K1
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box28 AS K1
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
			 ) K1

		RETURN @Result
        END;