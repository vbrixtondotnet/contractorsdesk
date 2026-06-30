CREATE PROCEDURE [dbo].[spQDRPCommittmentsVsActualCommittments] 
				@CurrentYear NVARCHAR(4),
				@Quarters NVARCHAR(10),
				@Fund NVARCHAR(255)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END

IF 'All' IN (SELECT value FROM STRING_SPLIT(@Quarters,','))
SET @Quarters = NULL

IF 'All' IN (SELECT value FROM STRING_SPLIT(@Fund,'|'))
SET @Fund = NULL

DECLARE @Table TABLE
(
	Account NVARCHAR(255),
	CommitmentBudget DECIMAL(21,9),
	CommitmentUSD DECIMAL(21,9),
	Orderby int
)

DECLARE @StatementTable TABLE
(
	Account NVARCHAR(255),
	CommitmentBudget DECIMAL(21,9),
	CommitmentUSD DECIMAL(21,9),
	Balance DECIMAL(21,9),
	Orderby int
)


				
		INSERT @Table
		SELECT Account, CommitmentBudget, CommitmentUSD, Orderby FROM [dbo].[msf_QDRPCommittmentsVsActualCommittments](@CurrentYear, @Quarters, @Fund, NULL, NULL)
		WHERE Account IS NOT NULL

		INSERT INTO @StatementTable
		SELECT Account, CommitmentBudget, CommitmentUSD, ISNULL(CommitmentBudget, 0)-ISNULL(CommitmentUSD, 0) AS Balance, Orderby FROM @Table

		SELECT Account, CommitmentBudget, CommitmentUSD, Balance, Orderby, SortBy 
		FROM (
				SELECT Account, CommitmentBudget, CommitmentUSD, Balance, Orderby, 1 AS SortBy 
				FROM @StatementTable
				UNION ALL
				SELECT 'Total' AS Account, SUM(CommitmentBudget), SUM(CommitmentUSD), SUM(Balance), MAX(Orderby)+1, 2 AS SortBy 
				FROM @StatementTable
				WHERE Account LIKE 'Total%'  
			)a
		ORDER BY SortBy, Orderby

END