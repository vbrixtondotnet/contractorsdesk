CREATE FUNCTION [dbo].[LSHBoxK2](
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

		--[K2] Totali i TVSH-së së llogaritur me normën 8%: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat
        --(14+18+22+26) dhe i shumëzon me normën e reduktuar të TVSH-së, 8%.
				
		SELECT @Result = SUM(ISNULL(K2,0))*8/100
		FROM (
				SELECT Box14 AS K2
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box18 AS K2
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box22 AS K2
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
				UNION ALL
				SELECT Box26 AS K2
				FROM SalesBooks
				WHERE TxnDate BETWEEN ISNULL(@FromDate, TxnDate)  AND ISNULL(@ToDate, TxnDate)
				AND DocNumber = ISNULL(@DocNumber, DocNumber) AND CustomerName = ISNULL(@CustomerName, CustomerName)
				AND FiscalNumber = ISNULL(@FiscalNumber, FiscalNumber) AND VATNumber = ISNULL(@VATNumber, VATNumber)
			 ) K2

		RETURN @Result
        END;