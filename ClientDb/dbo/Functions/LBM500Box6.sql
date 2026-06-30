CREATE FUNCTION [dbo].[LBM500Box6](
				@Year NVARCHAR(500),
				@VendorName NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
		
		--[6] Totali: Formati është numër, është rubrikë kalkuluese, e cila mbledh rubrikat
		--(7+8+9).

		SELECT @Result = SUM(ISNULL(Box6,0)) 
		FROM (
				--Box 7
				SELECT dbo.LBM500Box7(@Year, @VendorName, 0) as Box6
				UNION ALL
				--Box 8
				SELECT dbo.LBM500Box8(@Year, @VendorName, 0) as Box6
				UNION ALL
				--Box 9 
				SELECT dbo.LBM500Box9(@Year, @VendorName) as Box6
			 ) box67

		RETURN @Result
        END;