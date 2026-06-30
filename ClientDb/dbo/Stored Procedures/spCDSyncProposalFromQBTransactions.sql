CREATE PROC spCDSyncProposalFromQBTransactions
(@ProposalId uniqueidentifier)
AS
BEGIN
DECLARE @QBClassId UNIQUEIDENTIFIER;
SET @QBClassId = (SELECT QBClassId from Proposals where ID = @ProposalId);

DECLARE @Transactions TABLE
(
	AccountID uniqueidentifier,
	AccountName nvarchar(100),
	AccountType nvarchar(100),
	EstimateSubCategoryID uniqueidentifier,
	ParentEstimateCategoryID uniqueidentifier,
	Amount decimal(18,2),
	Sequence int
)
DECLARE @NewEstimateCategories TABLE
(
	ID uniqueidentifier,
	Name nvarchar(100),
	Sequence int,
	ParentEstimateCategoryID uniqueidentifier,
	AccountType nvarchar(100),
	DetailType nvarchar(100),
	AccountId uniqueidentifier
)
DECLARE @EstimateMappings TABLE(
	ID uniqueidentifier,
	AccountType nvarchar(100),
	AccountSubType nvarchar(100),
	Description nvarchar(100),
	QBAccountID uniqueidentifier,
	EstimateSubCategoryID uniqueidentifier
)

INSERT INTO @Transactions
SELECT 
E.AccountID,
E.AccountName,
E.AccountType, 
E.EstimateSubCategoryID, 
E.ParentEstimateCategoryID, 
CONVERT(DECIMAL(18,2), SUM(Amount)),
E.Sequence 
FROM (
	select 
	t.AccountID,
	a.Name as AccountName,
	a.AccountType, 
	em.EstimateSubCategoryID,
	e.ParentEstimateCategoryID,
	t.Amount,
	ISNULL(e.Sequence,(SELECT MAX(Sequence) + 1 FROM ESTIMATECATEGORIES )) as Sequence
	from QBTransactions t
	inner join QBAccounts a
	on a.ID = t.AccountID
	left join EstimateMappings em
	on em.QBAccountID = a.ID
	left join EstimateCategories e
	on e.ID = em.EstimateSubCategoryID
	where t.ClassID = @QBClassId
	and a.AccountType <> 'Income'
) E
GROUP BY E.AccountID,E.AccountType,E.AccountName, E.EstimateSubCategoryID, E.ParentEstimateCategoryID, E.Sequence
ORDER BY E.AccountName

INSERT INTO @NewEstimateCategories
SELECT 
	NEWID(),AccountName,Sequence,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD',AccountType,AccountName,
	AccountID
FROM @Transactions WHERE EstimateSubCategoryId IS NULL

UPDATE t
SET t.EstimateSubCategoryID = ne.ID, t.ParentEstimateCategoryID = ne.ParentEstimateCategoryID
FROM @Transactions t
INNER JOIN @NewEstimateCategories ne on ne.Name = t.AccountName

INSERT INTO EstimateCategories (ID,Name,Sequence,ParentEstimateCategoryID) 
SELECT ID,Name,Sequence,ParentEstimateCategoryID
FROM @NewEstimateCategories

--POPULATE ESTIMATE MAPPINGS
INSERT INTO @EstimateMappings (ID,AccountType,AccountSubType,Description,QBAccountID,EstimateSubCategoryID)
SELECT NEWID(),AccountType,DetailType,Name, AccountId, ID FROM @NewEstimateCategories

INSERT INTO EstimateMappings (ID,AccountType,AccountSubType,Description,QBAccountID,EstimateSubCategoryID)
SELECT ID,AccountType,AccountSubType,Description,QBAccountID,EstimateSubCategoryID FROM @EstimateMappings

-- POPULATE PROPOSAL LINES
DELETE FROM ProposalLines WHERE ProposalID =@ProposalId
INSERT INTO [dbo].[ProposalLines]
           ([ID]
           ,[Name]
           ,[Amount]
           ,[ProposalID]
           ,[EstimateCategoryID]
           ,[ParentEstimateCategoryID]
           ,[Sequence])
SELECT NEWID(),AccountName,Amount,@ProposalId,EstimateSubCategoryID,ParentEstimateCategoryID,Sequence FROM @Transactions

--REMOVE DUPLICATES
DELETE FROM ProposalLines WHERE ID in (
SELECT ID from (
select 
	ROW_NUMBER() over (partition by EstimateCategoryId order by Name ) as RowNumber,
* from ProposalLines where ProposalID = @ProposalId 
) sq1
WHERE RowNumber != 1)

SELECT * FROM @NewEstimateCategories;
SELECT * FROM @EstimateMappings;
SELECT * FROM @Transactions ORDER BY AccountName;
END
