CREATE   PROC
spExtractTransactionsAndCreateMissingProposal
(@ProjectName as nvarchar(100))
AS
BEGIN

DECLARE @QbClassId as nvarchar(200);
DECLARE @Number as int;
DECLARE @ProposalId nvarchar(200);

SET @QbClassId = (SELECT TOP 1 Id from QBClasses where Name = @ProjectName)
SET @Number = (SELECT MAX(Number) FROM Proposals) + 1;
SET @ProposalId = NEWID();

--SELECT 
--	DISTINCT a.Name,
--	b.ID as EstimateCategoryId,
--	b.Name as Item,
--	c.Name as Category,
--	c.Id as ParentEstimateCategoryId,
--	b.Sequence
--FROM vwActiveConstructionJobDetails a
--LEFT JOIN EstimateCategories b on a.Name = b.Name
--LEFT JOIN EstimateCategories c on b.ParentEstimateCategoryID = c.Id
--where a.ProjectName = @ProjectName
--and b.Name IS NOT NULL

INSERT INTO [dbo].[Proposals]
           ([ID]
           ,[Number]
           ,[TemplateId]
           ,[QBClassId]
           ,[Date]
           ,[TotalAmount]
           ,[DocStatus])
     VALUES
           (@ProposalId
           ,@Number
           ,'D1103B14-2196-48C9-BB93-DC6DE65962C2'
           ,@QbClassId
           ,GETDATE()
		   ,0
           ,'Accepted')


INSERT INTO [dbo].[ProposalProjectDetails]
           ([Id]
           ,[ProposalId]
           ,[Name])
     VALUES
           (NEWID()
           ,@ProposalId
           ,@ProjectName)

INSERT INTO [dbo].[ProposalLines]
           ([ID]
           ,[Name]
           ,[Amount]
           ,[ProposalID]
           ,[EstimateCategoryID]
           ,[ParentEstimateCategoryID]
           ,[Sequence])

SELECT NEWID(), Name, Amount,ProposalId,EstimateCategoryId,ParentEstimateCategoryId,Sequence
FROM (
SELECT 
	b.Name as Name,
	0 as Amount,
	@ProposalId as ProposalId,
	b.ID as EstimateCategoryId,
	c.Id as ParentEstimateCategoryId,
	b.Sequence
FROM vwActiveConstructionJobDetails a
LEFT JOIN EstimateCategories b on a.Name = b.Name
LEFT JOIN EstimateCategories c on b.ParentEstimateCategoryID = c.Id
where a.ProjectName = @ProjectName
and b.Name IS NOT NULL
GROUP BY b.Name,b.ID,c.Id,b.Sequence
) TBL 


--SELECT * FROM Proposals where QBClassId = '9C3532C9-E6CE-482A-560A-08DC17A9A6B7' order by Number desc
--SELECT @QbClassId, @ProjectName, @Number

--select * from ProposalLines where ProposalID = '69AA5BE2-6A12-40B6-A5F1-9DE570B2B425'
--DELETE FROM PROPOSALLINES WHERE PROPOSALID = '69AA5BE2-6A12-40B6-A5F1-9DE570B2B425'


--SELECT * FROM ProposalTemplates


--select * from EstimateCategories where name like '%Floor%'

--select * from QBAccounts where name like 'Painting%'


--select * from QBAccounts where ListID = 160

--select * from QBClasses where Name like '%110 La Placentia%'

--select * from QBAccounts where id = 'B9631B83-CE8B-4AF3-13A8-08DC17A99CDF'

--select top 10 * from QBTransactions where CustomerID = '0FC7198A-0D16-4F35-D065-08DC733DF3C7'

--select * from QBCustomers where id = '65325956-F4E1-4798-D21F-08DC733DF3C7'

--select * from QBCustomers where FullName like '%110%'

--select * from Proposals order by Created desc

--select newid()

--exec spDeleteProposal 'BF0DA240-96E8-4FEE-B246-C7DE77CFCE59'






END