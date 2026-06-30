CREATE PROCEDURE [dbo].[spQuarterlyCashBudgetVsExpenses] 
				@CurrentYear NVARCHAR(4),
				@Quarters NVARCHAR(10)
AS
BEGIN	

SET NOCOUNT ON;
     IF 1=0 BEGIN
       SET FMTONLY OFF
     END

IF 'All' IN (SELECT value FROM STRING_SPLIT(@Quarters,','))
SET @Quarters = NULL

DECLARE @Table TABLE
(
	Account NVARCHAR(255),
	QDRPCash DECIMAL(21,9),
	ExpensesUSD DECIMAL(21,9),
	Orderby int
)

DECLARE @StatementTable TABLE
(
	Account NVARCHAR(255),
	QDRPCash DECIMAL(21,9),
	ExpensesUSD DECIMAL(21,9),
	Balance DECIMAL(21,9),
	Orderby int
)


				
		INSERT @Table
		SELECT Account, QDRPCash, ExpensesUSD, Orderby FROM [dbo].[msf_QuarterlyCashBudgetVsExpenses](@CurrentYear, @Quarters, NULL, NULL)


		INSERT INTO @StatementTable
		SELECT Account, QDRPCash, ExpensesUSD, ISNULL(QDRPCash, 0)-ISNULL(ExpensesUSD, 0) AS Balance, Orderby FROM @Table
		WHERE Account IS NOT NULL

		SELECT Account, QDRPCash, ExpensesUSD, Balance, Orderby FROM @StatementTable
		ORDER BY Orderby

END