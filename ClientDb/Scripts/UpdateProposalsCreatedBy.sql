

;WITH RankedSupervisors AS (
    SELECT 
        ps.ProjectId,
        ps.SupervisorId,
        ROW_NUMBER() OVER (PARTITION BY ps.ProjectId ORDER BY ps.SupervisorId) AS rn
    FROM ProjectSupervisors ps
    INNER JOIN Users u
        ON ps.SupervisorId = u.Id
    WHERE ps.ProjectId IN (
        SELECT DISTINCT QbClassId FROM Proposals
    )
)
UPDATE p
SET 
	p.CreatedBy = rs.SupervisorId,
	p.UpdatedBy = rs.SupervisorId
FROM Proposals p
JOIN RankedSupervisors rs
    ON p.QbClassId = rs.ProjectId
WHERE rs.rn = 1;


UPDATE Proposals set CreatedBy = 1 Where CreatedBy IS NULL;

UPDATE Proposals
SET DateCreated = Created,
	DateUpdated = Created;

