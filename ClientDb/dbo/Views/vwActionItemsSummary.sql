CREATE VIEW [dbo].[vwActionItemsSummary] AS
SELECT 
    a.Id,
    a.Title,
    a.Description,
    a.DueDate,
    a.ActionTypeId,
    a.Source,
    at.Title AS ActionTypeName,
    a.Status AS StatusId,
    a.IsArchived,
    a.DateCreated,
    p.Id AS ProjectId,
    p.Name AS ProjectName,
    ac.EstimateCategoryId AS CostChangeItemId,
    plcc.Name AS CostChangeItem,
    ac.Amount,
    ISNULL(
        (
            SELECT TOP 1 ph.Amount
            FROM ProposalLinesHistory ph
            WHERE ph.ProposalID = pr.ID
              AND ph.EstimateCategoryID = plcc.EstimateCategoryID
              AND ph.ChangeType = 'Updated'
            ORDER BY ph.ChangeDate DESC
        ),
        plcc.Amount
    ) AS CurrentAmount,
    sc.ConstructionTaskId AS ScheduleChangeItemId,
    pst.Name AS ScheduleChangeItem,
    sc.NoOfDays,
    pst.Duration AS Duration,
    a.CreatedBy AS CreatedById,
    CONCAT(cu.FirstName, ' ', cu.LastName) AS CreatedBy
FROM ActionItems a
LEFT JOIN QBClasses p
    ON p.ID = a.ProjectId
LEFT JOIN ActionItemCostChange ac
    ON ac.ActionItemId = a.Id
LEFT JOIN Proposals pr 
    ON pr.QBClassId = p.ID
LEFT JOIN ProposalLines plcc
    ON plcc.ProposalID = pr.ID 
   AND plcc.EstimateCategoryID = ac.EstimateCategoryId
LEFT JOIN Users cu
    ON cu.Id = a.CreatedBy
LEFT JOIN ActionItemScheduleChange sc
    ON sc.ActionItemId = a.Id
LEFT JOIN ProjectScheduleTasks pst
    ON pst.Id = sc.ConstructionTaskId
INNER JOIN ActionTypes at
    ON at.Id = a.ActionTypeId
where ISNULL(a.IsDeleted,0) = 0;
