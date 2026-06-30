CREATE VIEW [dbo].[vwEstimateDataMapping]
AS 
SELECT 
	em.ID as MappingId,
	a.ID as AccountId,
	a.FullyQualifiedName,
	e.Id as EstimateCategoryId,
	e.Name as EstimateCategory,
	pe.Name as ParentCategory
from QBAccounts a
left join EstimateMappings em
on a.ID = em.QBAccountID
left join EstimateCategories e
on em.EstimateSubCategoryID = e.ID
left join EstimateCategories pe
on e.ParentEstimateCategoryID = pe.ID
where a.AccountType in ('Expense', 'OtherExpense')
--order by EstimateCategory
