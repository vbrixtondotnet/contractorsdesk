CREATE PROC spSynchTransactionsToEstimates
(@ProposalID uniqueidentifier)
AS 
BEGIN

DECLARE @ClassId uniqueidentifier;

DECLARE @TransactionsDataMapping TABLE (
	TransactionId uniqueidentifier,
	AccountId uniqueidentifier,
	AccountType nvarchar(100),
	MappingId uniqueidentifier,
	EstimateCategoryId uniqueidentifier,
	Name nvarchar(250),
	ParentEstimateCategoryId uniqueidentifier,
	Amount decimal
); 

SET @ClassId = (SELECT QBClassId from Proposals where ID = @ProposalID)

INSERT INTO @TransactionsDataMapping
SELECT
	t.ID as TransactionId,
	a.ID as AccountId,
	a.AccountType,
	em.ID as MappingId,
	ec.ID,
	ec.Name,
	ec.ParentEstimateCategoryID,
	t.Amount
from QBTransactions t
inner join QBAccounts a
on t.AccountID = a.ID
left join EstimateMappings em
on em.QBAccountID = a.ID
left join EstimateCategories ec
on em.EstimateSubCategoryID = ec.ID
where t.ClassID = @ClassId
and a.AccountType in ('Expense', 'OtherExpense')

INSERT INTO [dbo].[ProposalLines]
           ([ID]
           ,[Name]
           ,[Description]
           ,[Amount]
           ,[ProposalID]
           ,[EstimateCategoryID]
           ,[Created]
           ,[CreatedBy]
           ,[ParentEstimateCategoryID]
           ,[Sequence])
SELECT 
	NEWID(),
	Name,
	'',
	SUM(Amount) as Amount,
	@ProposalID,
	EstimateCategoryId,
	GETDATE(),
	1,
	ParentEstimateCategoryId,
	100
FROM @TransactionsDataMapping
WHERE EstimateCategoryId IS NOT NULL
and EstimateCategoryId NOT IN
(
	SELECT EstimateCategoryId FROM ProposalLines
	WHERE ProposalID = @ProposalID
)
-- Do not insert when the same Item Name already exists under the same parent category
and not exists
(
	SELECT 1
	FROM ProposalLines pl
	WHERE pl.ProposalID = @ProposalID
		AND pl.ParentEstimateCategoryID = ParentEstimateCategoryId
		AND LOWER(LTRIM(RTRIM(pl.Name))) = LOWER(LTRIM(RTRIM(Name)))
)
GROUP BY EstimateCategoryId, Name, ParentEstimateCategoryId, AccountType
ORDER BY Name

INSERT INTO ProposalLines (ID,Name,ProposalID,EstimateCategoryID,Amount,Sequence,Updated)
SELECT 
	NEWID(),
	Name,
	@ProposalID,
	ParentEstimateCategoryID,
	0,
	5,
	GETDATE()
FROM 
(
select distinct 
	pl.ParentEstimateCategoryID, 
	ec.Name
	from ProposalLines pl
	
	LEFT JOIN EstimateCategories ec on ec.Id = pl.ParentEstimateCategoryID
	where ProposalID = @ProposalID
	and pl.ParentEstimateCategoryID IS NOT NULL
	and pl.ParentEstimateCategoryID NOT IN (
		Select EstimateCategoryID from ProposalLines
		where ProposalID = @ProposalID
		and ParentEstimateCategoryID IS NULL
	)
) a

END
