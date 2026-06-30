CREATE VIEW [dbo].[vwActiveJobs]
AS
	SELECT 
	c.ID,
	c.FullyQualifiedName as Name,
	ISNULL(jb.Balance,0) as JobBalance
	from QBClasses c
	left join JobBalances jb
	on jb.JobId = c.ID
	where (c.ActiveJobs = 1 or c.ActiveSpecJobs = 1)
	and c.IsDeleted = 0
