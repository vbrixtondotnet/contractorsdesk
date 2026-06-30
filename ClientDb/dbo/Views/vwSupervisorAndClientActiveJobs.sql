CREATE VIEW [dbo].[vwSupervisorAndClientActiveJobs]
AS
SELECT 
	c.ID,
	c.Name as Name,
	ISNULL(jb.Balance,0) as JobBalance,
	ISNULL(pt.Threshold,0) as Threshold,
	ISNULL(c.IsArchived,0) as IsArchived,
	ps.UserId as SupervisorId,
	u.FirstName as SupervisorFirstName,
	u.LastName as SupervisorLastName,
	cp.UserId as ClientId,
	uc.FirstName as ClientFirstName,
	uc.LastName as ClientLastName,
	cust.Name as ClientFullName,
	cust.EmailAddress as ClientEmailAddress,
	pr.ID as ProposalId,
	pr.DocStatus,
	c.IsCompleted,
	ppr.PhotoUrl
	from QBClasses c
	left join JobBalances jb
	on jb.JobId = c.ID
	left join Proposals pr
	on pr.QBClassId = c.ID
	left join ProposalSupervisors ps
	on ps.ProposalId = pr.ID
	left join Users u
	on u.Id = ps.UserId
	left join ClientProjects cp
	on cp.ProjectId = c.ID
	left join Users uc
	on uc.Id = cp.UserId
	left join ProjectThresholds pt
	on pt.ProjectId = c.ID
	left join Clients cust
	on cust.ID = pr.ClientId
	left join ProposalProjects ppr
	on pr.ProposalProjectId = ppr.Id

	WHERE (c.IsActive = 1) 
	and (c.ActiveJobs = 1 or c.ActiveSpecJobs = 1)
	and ISNULL(c.IsDeleted,0) = 0
	and (pr.IsDeleted != 1)

