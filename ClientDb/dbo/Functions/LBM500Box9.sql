CREATE FUNCTION [dbo].[LBM500Box9](
				@Year NVARCHAR(500),
				@VendorName NVARCHAR(500)
				)
RETURNS DECIMAL(21,9)
AS
BEGIN
		DECLARE @Result DECIMAL(21,9);
		
		--[9] Vlera e paguar per qira e prones se palujtshme.

		SELECT @Result = SUM(ISNULL(B9,0)) 
		FROM (
				SELECT ISNULL(DebitAccountAmount,0) AS B9
				FROM QBJournalEntries
				WHERE Year(TxnDate) IN (SELECT value FROM STRING_SPLIT(@Year,','))
				AND DebitEntityRef = ISNULL(@VendorName, DebitEntityRef)
			 ) B9

		RETURN @Result
        END;