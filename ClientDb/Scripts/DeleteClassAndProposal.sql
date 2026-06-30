
DECLARE @ClassId uniqueidentifier, @ProposalId uniqueidentifier;
SET @ClassId = 'bcbd893b-cfda-46e4-93e8-60b83c4cc4dc';
SET @ProposalId = (Select Id from Proposals where QBClassId = @ClassId);

delete from ProposalSupervisors where ProposalId = @ProposalId;
delete from Proposals where id = @ProposalId
delete from ProjectScheduleTasks where ProjectScheduleId = (Select Id from ProjectSchedules where ProjectId = @ClassId)
delete from ProjectSchedules where ProjectId = @ClassId
delete from ProjectSupervisors where ProjectId = @ClassId
delete from ProjectJournal where ProjectId = @ClassId
delete from JobBalances where JobId = @ClassId
delete from ProjectThresholds where ProjectId = @ClassId
delete from QBTransactions where ClassID = @ClassId
delete from QBClasses where id = @ClassId