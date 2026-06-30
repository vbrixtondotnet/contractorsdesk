CREATE PROCEDURE [dbo].[spGetMissingAccountMappings]
AS
BEGIN
	select 
		a.Id as AccountId,
		a.Name as AccountName,
		a.AccountType,
		em.Id as MappingId,
		ec.ID as EstimateCategoryId
	from QBAccounts a
	left join EstimateMappings em
	on a.ID = em.QBAccountID
	left join EstimateCategories ec
	on ec.Id = em.EstimateSubCategoryId
	where em.ID IS NULL
	Order by AccountName asc
END