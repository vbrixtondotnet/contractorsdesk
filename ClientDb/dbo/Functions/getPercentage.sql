CREATE FUNCTION [dbo].[getPercentage]
                        (
						   @Actual AS DECIMAL(21,9),
						   @Estimate AS DECIMAL(21,9)
                        )
                        RETURNS DECIMAL(21,9)
                        AS
                        BEGIN
						RETURN CASE WHEN @Estimate<0 AND @Actual=@Estimate THEN -@Actual/NULLIF(@Estimate,0)
									WHEN @Estimate = 0 AND @Actual !=0 THEN -1
									WHEN @Estimate<0 AND @Actual<0 THEN -@Actual/NULLIF(@Estimate,0)
						ELSE @Actual/NULLIF(@Estimate,0) END;
						END;