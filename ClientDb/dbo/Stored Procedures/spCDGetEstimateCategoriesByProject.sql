CREATE PROCEDURE [dbo].[spCDGetEstimateCategoriesByProject]
	@ProjectId uniqueidentifier
AS
BEGIN
	select 
		pl.EstimateCategoryID,
		pl.Name,
		ISNULL((SELECT TOP 1 Amount from ProposalLinesHistory ph     
		   WHERE ph.ProposalID = p.ID and ph.EstimateCategoryID = pl.EstimateCategoryID     
		   AND ph.ChangeType = 'Updated'    
		   ORDER BY ChangeDate DESC),pl.Amount
		) as CurrentAmount
	from ProposalLines pl
	inner join Proposals p
	on p.ID = pl.ProposalID
	inner join QBClasses c
	on c.ID = p.QBClassId
	where c.ID = @ProjectId
	and pl.ParentEstimateCategoryID IS NOT NULL
	order by pl.Name
END
