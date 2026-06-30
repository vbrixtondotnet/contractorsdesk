CREATE FUNCTION [dbo].[LSHBox30](
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
			
		--[30] Totali i TVSH-së së llogaritur me 18% dhe 8%: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat(K1+K2).	
		SELECT @Result = SUM(ISNULL(Box30,0)) 
		FROM (
				SELECT dbo.LSHBoxK1(@FromDate, @ToDate, @DocNumber, @CustomerName, @FiscalNumber, @VATNumber) as Box30
				UNION ALL
				SELECT dbo.LSHBoxK2(@FromDate, @ToDate, @DocNumber, @CustomerName, @FiscalNumber, @VATNumber) as Box30
			 ) box30

		RETURN @Result
        END;