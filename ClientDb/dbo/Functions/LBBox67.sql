CREATE FUNCTION [dbo].[LBBox67](
				@Date DATE,
				@DocNumber NVARCHAR(500),
				@VendorName NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
			
		--[67] Totali i TVSH-së së zbritshme me 18% dhe 8%: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat(K1+K2).
		SELECT @Result = SUM(ISNULL(Box67,0)) 
		FROM (
				SELECT dbo.LBBoxK1(@Date, @DocNumber, @VendorName) as Box67
				UNION ALL
				SELECT dbo.LBBoxK2(@Date, @DocNumber, @VendorName) as Box67
			 ) box67

		RETURN @Result
        END;