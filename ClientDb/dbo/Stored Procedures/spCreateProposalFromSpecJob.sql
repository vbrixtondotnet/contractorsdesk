CREATE   PROC spCreateProposalFromSpecJob
(@ProjectName nvarchar(100))
AS 
BEGIN
DECLARE @QbClassId as nvarchar(200);
DECLARE @Number as int;
DECLARE @ProposalId nvarchar(200);

SET @QbClassId = (SELECT TOP 1 Id from QBClasses where Name = @ProjectName)
SET @Number = (SELECT MAX(Number) FROM Proposals) + 1;
SET @ProposalId = NEWID();

--INSERT INTO [dbo].[Proposals]
--           ([ID]
--           ,[Number]
--           ,[TemplateId]
--           ,[QBClassId]
--           ,[Date]
--           ,[ExpiredDate]
--           ,[TotalAmount]
--           ,[Created]
--           ,[DocStatus])
--     VALUES
--           (@ProposalId
--           ,@Number
--           ,'D1103B14-2196-48C9-BB93-DC6DE65962C2'
--           ,@QbClassId
--           ,GETDATE()
--           ,GETDATE() + 30
--           ,0
--		   ,GETDATE()
--           ,'Accepted')

--INSERT INTO [dbo].[ProposalProjectDetails]
--           ([Id]
--           ,[ProposalId]
--           ,[Name])
--     VALUES
--           (NEWID()
--           ,@ProposalId
--           ,@ProjectName)

--INSERT INTO [dbo].[ProposalLines]
--           ([ID]
--           ,[Name]
--           ,[Amount]
--           ,[ProposalID]
--           ,[EstimateCategoryID]
--           ,[ParentEstimateCategoryID]
--           ,[Sequence])

   SELECT 
    NEWID(),
	e.Name,
	0,
	@ProposalId,	
	e.ID,
	e.ParentEstimateCategoryID,
	e.Sequence
   FROM EstimateCategories e
   INNER JOIN EstimateMappings m on m.EstimateSubCategoryID = e.ID
   INNER JOIN QBAccounts a on a.ID = m.QBAccountID
   INNER JOIN QBTransactions t on t.AccountID = a.ID
   INNER JOIN QBClasses c on c.ID = t.ClassID
   Where c.Name = @ProjectName
   GROUP BY e.ID,e.Name, e.ParentEstimateCategoryID, c.Name,c.FullyQualifiedName,e.Sequence
END

--exec spCreateProposalFromSpecJob N'109 Panorama'

--exec spDeleteProposal '33c1ac9f-5ebf-478c-8cc8-3de818992ec9'