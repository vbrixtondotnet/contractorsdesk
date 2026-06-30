CREATE FUNCTION [dbo].[getPercentageACAD]
                        (
						   @Expenses AS DECIMAL(21,9),
						   @Commitment AS DECIMAL(21,9)
                        )
                        RETURNS DECIMAL(21,9)
                        AS
                        BEGIN
						RETURN CASE WHEN @Commitment<0 AND @Commitment=@Expenses THEN -@Expenses/NULLIF(@Commitment,0)
									WHEN @Commitment = 0 AND @Expenses !=0 THEN -1
									WHEN @Commitment<0 AND @Expenses<0 THEN -@Expenses/NULLIF(@Commitment,0)
						ELSE @Expenses/NULLIF(@Commitment,0) END;
						END;