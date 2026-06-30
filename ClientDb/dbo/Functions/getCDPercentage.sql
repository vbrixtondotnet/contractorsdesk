CREATE FUNCTION [dbo].[getCDPercentage]
(
	@Actual AS DECIMAL(21,9),
	@Estimate AS DECIMAL(21,9)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
RETURN 
	CASE 
		WHEN @Actual <= 0 THEN 0
		WHEN @Actual= @Estimate THEN 100
	ELSE (@Actual/NULLIF(@Estimate,0) * 100) END;
END;