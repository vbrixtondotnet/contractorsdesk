


-- PROPOSAL SUPERVISORS
INSERT INTO ProposalSupervisors (ID, ProposalId,UserId,SupervisorTypeId,DateCreated)
select 
	NEWID(),
	p.ID as ProposalId,
	ps.SupervisorId as UserId,
	ps.SupervisorTypeId,
	GETDATE()
from ProjectSupervisors ps
inner join QBClasses q
on q.ID = ps.ProjectId
inner join Proposals p
on q.ID = p.QBClassId
where p.IsDeleted <> 1
order by ps.ProjectId
-- END PROPOSAL SUPERVISORS

-- Proposal Projects
INSERT INTO ProposalProjects (ID,Name,Address,City,State,Description,DateCreated,CreatedBy)
select 
	NEWID(),
	q.Name,
	q.Address,
	q.City,
	q.State,
	q.Description,
	GETDATE(),
	1
from Proposals p
inner join QBClasses q
on q.ID = p.QBClassId
where p.IsDeleted <> 1


UPDATE p
SET p.ProposalProjectId = s.ProposalProjectId
FROM Proposals p
JOIN (
   Select 
	pp.ID as ProposalProjectId,
	p.ID as ProposalId
from QBClasses q
inner join ProposalProjects pp
on pp.Name = q.Name
inner join Proposals p
on p.QBClassId = q.ID
) s ON p.Id = s.ProposalId;
-- End Proposal Projects

-- Clients

DELETE FROM Clients;
INSERT INTO [dbo].[Clients]
           ([Id]
           ,[Name]
           ,[Address]
           ,[City]
           ,[State]
           ,[CompanyName]
           ,[EmailAddress]
           ,[Phone]
		   ,[SecondaryEmailAddress]
           ,[DateCreated]
           ,[CreatedBy])
SELECT 
	ID,Name,Address,City,State,CompanyName,Email,Phone,SecondaryEmail,DateCreated,CreatedBy
FROM (
select
	c.ID,
	ISNULL(c.Name,'') as Name,
	c.Address,
	c.City,
	c.State,
	c.CompanyName,
	ISNULL(c.Email,'') as Email,
	c.Phone,
	c.SecondaryEmail,
	GETDATE() as DateCreated,
	1 as CreatedBy,
	ROW_NUMBER() over (partition by c.Id order by c.Id) as RowNum
from Proposals p
inner join QBCustomers c
on p.QBCustomerId = c.ID
inner join QBClasses q
on q.ID = p.QBClassId
where p.IsDeleted <> 1
and p.IsArchived <> 1
and (q.IsActive = 1 OR q.ActiveJobs = 1 or q.ActiveSpecJobs = 1 and (q.IsDeleted <> 1 and q.IsArchived <> 1))
) cdd
where cdd.RowNum = 1


UPDATE p
SET p.ClientId = s.ClientId
FROM Proposals p
JOIN (
  SELECT ProposalId,ClientId FROM (
select 
	p.Id as ProposalId,
	c.Id as ClientId,
	ROW_NUMBER() over (partition by c.Id order by c.Id) as RowNumber 
from Proposals p
inner join Clients c
on c.Id = p.QBCustomerId
) c
where c.RowNumber = 1
) s ON p.Id = s.ProposalId;

-- END CLIENTS

-- Date Created and Created By on Proposals

UPDATE p
SET 
	p.CreatedBy = s.UserId,
	p.DateCreated = p.Date
FROM Proposals p
JOIN (
	SELECT 
		UserId,
		ProposalId
	FROM (Select 
		UserId,
		ProposalId,
		ROW_NUMBER() over (partition by ProposalId order by UserId) as RowNumber
	from ProposalSupervisors
	) a
where a.RowNumber = 1
) s ON p.Id = s.ProposalId;

UPDATE Proposals set CreatedBy = 1 where CreatedBy IS NULL

-- End Date Created and Created By on Proposals