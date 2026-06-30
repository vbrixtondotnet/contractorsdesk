CREATE VIEW [dbo].[vwProjectShortDetails]
AS

SELECT 
	p.Id,
	pr.Id AS ProposalId,
	p.Name,
	p.FullyQualifiedName,
	CAST(psc.StartDate AS DATE) AS StartDate,
	CAST(psc_task.EndDate AS DATE) AS EndDate,
	b.Balance AS JobBalance,
	p.IsArchived,
	pr.DocStatus,
	ps.SupervisorId AS SupervisorId,
	u.FirstName AS SupervisorFirstName,
	u.LastName AS SupervisorLastName,
	u.Email AS SupervisorEmail,
	c.EmailAddress AS ClientEmailAddress,	
	c.Name AS ClientName,
	p.IsActive
FROM QBClasses p
LEFT JOIN Proposals pr ON p.ID = pr.QBClassId
LEFT JOIN JobBalances b ON b.JobId = p.ID
LEFT JOIN ProjectSupervisors ps ON ps.ProjectId = p.ID
LEFT JOIN Users u ON u.Id = ps.SupervisorId
LEFT JOIN Clients c ON c.ID = pr.ClientId
LEFT JOIN ProjectSchedules psc ON p.ID = psc.ProjectId
OUTER APPLY (
	SELECT TOP 1 psct.EndDate
	FROM ProjectScheduleTasks psct
	WHERE psct.ProjectScheduleId = psc.ID
	ORDER BY psct.EndDate DESC
) AS psc_task

