CREATE   PROC spDeleteProposal
(
	@ProposalId uniqueIdentifier
)
AS 
BEGIN
DECLARE @ProjectId UNIQUEIDENTIFIER;
SET @ProjectId = (SELECT QbClassId from Proposals where Id = @ProposalId);

delete from ProposalLines where proposalId = @ProposalId
delete from ProposalLinesHistory where ProposalID = @ProposalId
delete from ProjectSupervisors where ProjectId = @ProjectId
delete from Proposals where id = @ProposalId

END