CREATE PROCEDURE [dbo].[spActualCommittmentsVsActualDisbursments] 
@StartDate DATE,
@EndDate DATE,
@Fund NVARCHAR(255),
@CPSPermitted NVARCHAR(255)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END

IF 'All' IN (SELECT value FROM STRING_SPLIT(@Fund,'|'))
SET @Fund = NULL

IF 'All' IN (SELECT value FROM STRING_SPLIT(@CPSPermitted,'|'))
SET @CPSPermitted = NULL

IF @Fund = 'None'
BEGIN
	SET @CPSPermitted = ''
	SET @Fund =''
END

DECLARE @Table TABLE
(
	Account NVARCHAR(255),
	CommitmentUSD DECIMAL(21,9),
	CommitmentEUR DECIMAL(21,9),
	ExpensesUSD DECIMAL(21,9),
	ExpensesEUR DECIMAL(21,9),
	Orderby int
)

DECLARE @StatementTable TABLE
(
	Account NVARCHAR(255),
	CommitmentUSD DECIMAL(21,9),
	CommitmentEUR DECIMAL(21,9),
	ExpensesUSD DECIMAL(21,9),
	ExpensesEUR DECIMAL(21,9),
	CommitmentBalanceUSD DECIMAL(21,9),
	CommitmentBalanceEUR DECIMAL(21,9),
	[Percentage] DECIMAL(21,9),
	Orderby int
)


				
		INSERT @Table
		SELECT Account, CommitmentUSD, CommitmentEUR, ExpensesUSD, ExpensesEUR, Orderby FROM [dbo].[msf_ActualCommittmentsVsActualDisbursments](@StartDate, @EndDate, @Fund, @CPSPermitted, NULL, NULL)
		WHERE Account IS NOT NULL


		INSERT INTO @StatementTable
		SELECT Account, ISNULL(CommitmentUSD,0), ISNULL(CommitmentEUR,0), ISNULL(ExpensesUSD,0), ISNULL(ExpensesEUR,0), ISNULL(CommitmentUSD,0) - ISNULL(ExpensesUSD,0) AS CommitmentBalanceUSD, ISNULL(CommitmentEUR,0) - ISNULL(ExpensesEUR,0) AS CommitmentBalanceEUR, dbo.getPercentageACAD(ISNULL(ExpensesUSD,0),ISNULL(CommitmentUSD,0)) AS [Percentage], Orderby
		FROM @Table


		SELECT Account, CommitmentUSD, CommitmentEUR, ExpensesUSD, ExpensesEUR, CommitmentBalanceUSD, CommitmentBalanceEUR, [Percentage], Orderby, SortBy 
		FROM (
				SELECT Account, CommitmentUSD, CommitmentEUR, ExpensesUSD, ExpensesEUR, CommitmentBalanceUSD, CommitmentBalanceEUR, [Percentage], Orderby, 1 AS SortBy 
				FROM @StatementTable
				UNION ALL
				SELECT 'Total' AS Account, SUM(CommitmentUSD), SUM(CommitmentEUR), SUM(ExpensesUSD), SUM(ExpensesEUR), SUM(CommitmentBalanceUSD), SUM(CommitmentBalanceEUR), dbo.getPercentageACAD(SUM(ExpensesUSD),SUM(CommitmentUSD)) AS [Percentage], MAX(Orderby)+1, 2 AS SortBy 
				FROM @StatementTable
				WHERE Account LIKE 'Total%'  
			)a
		ORDER BY SortBy, Orderby

END