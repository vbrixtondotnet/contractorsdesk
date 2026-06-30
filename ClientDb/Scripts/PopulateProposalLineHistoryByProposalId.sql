
INSERT INTO [dbo].[ProposalLinesHistory]
           ([HistoryID]
           ,[ProposalLineID]
           ,[ProposalID]
           ,[ChangeType]
           ,[ChangeDate]
           ,[Amount]
           ,[Description]
           ,[EstimateCategoryID]
           ,[ParentEstimateCategoryID]
           ,[Created]
           ,[CreatedBy]
           ,[Percentage])
SELECT
	NEWID(),
	ID,
	ProposalID,
	'Updated',
	GETDATE(),
	Amount,
	Description,
	EstimateCategoryID,
	ParentEstimateCategoryID,
	GETDATE(),
	1,
	Percentage
from ProposalLines where proposalid = 'd2d24b59-7b64-4b27-adfa-e3600efae188'