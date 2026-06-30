CREATE FUNCTION [dbo].[getTotalCost]
(
	@QbClassId uniqueidentifier
)
RETURNS DECIMAL(18,2)
AS
BEGIN
	DECLARE @TotalCost decimal (18,2);
	SET @TotalCost = (
	SELECT     
	 SUM(t.Amount)
	from QBTransactions t    
	inner join QBAccounts a    
	on a.ID = t.AccountID    
	left join EstimateMappings em    
	on em.QBAccountID = a.ID    
	left join EstimateCategories ec    
	on ec.ID = em.EstimateSubCategoryID    
	where t.ClassID = @QbClassId   
	and a.AccountType  <> 'Income')
	
	RETURN ISNULL(@TotalCost,0);
END;