CREATE PROCEDURE [dbo].[spEnsureUniqueProposalLines]
	@ProposalId uniqueidentifier
AS
BEGIN
	DELETE FROM ProposalLines WHERE ID IN (
	select ID from (
		select *, ROW_NUMBER() over (partition by ProposalId, EstimateCategoryId order by Name) as RowNumber from ProposalLines 
		where ProposalId = @ProposalId
	) sq1
	where RowNumber > 1
	)
	and ProposalId = @ProposalId

	EXEC spCleanUpDuplicateProposalLinesAndEstimateCategories @ProposalId;
END