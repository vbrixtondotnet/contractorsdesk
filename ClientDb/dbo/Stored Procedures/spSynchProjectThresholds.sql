CREATE PROC spSynchProjectThresholds
(@ProjectId uniqueidentifier)
AS 
BEGIN

DECLARE @MinimumRequestedAmount decimal(18,2);

SET @MinimumRequestedAmount = (SELECT MinimumRequestedAmount from QBClasses where Id = @ProjectId);

INSERT INTO ProjectThresholds
		   (ID
		   ,ProjectId
		   ,Threshold
		   ,DateUpdated
		   )

SELECT 
	NEWID(),
	@ProjectId,
	(SELECT ROUND(ISNULL(.20 * ISNULL((@MinimumRequestedAmount),(SELECT TOP 1 qt.Amount
	FROM  QbTransactions qt INNER JOIN QbAccounts qba on qba.ID = qt.AccountID
	WHERE qba.AccountType = 'Income'
	and ClassID = @ProjectId
	and Amount % 5000 = 0
	order by TransactionDate
	)), 0),2)),
	GETDATE()
END
