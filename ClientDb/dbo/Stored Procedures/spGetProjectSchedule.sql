CREATE    PROC spGetProjectSchedule  
(  
 @ProjectId uniqueidentifier  
)  
AS  
BEGIN  
SELECT 1 
 --SELECT   
 ---- ps.Id,  
 ---- ISNULL(ps.ProjectId, @ProjectId) as ProjectId,  
 ---- ISNULL(ps.ConstructionTaskId,ct.ID) as ConstructionTaskId,  
 ---- ISNULL(ps.Name,ct.Name) as Name,  
 ---- ISNULL(ps.Sequence,ct.Sequence) as Sequence,  
 ---- ISNULL(ps.Duration,ct.Duration) as Duration,  
 ---- ps.StartDate,  
 ---- ps.EndDate,  
 ---- ISNULL(ps.Pred1,ct.ParentTaskID) as Pred1,  
 ---- ISNULL(ps.Lag1,ct.Pred1Lag) as Lag1,  
 ---- ps.Pred2,  
 ---- ISNULL(ps.Lag2,ct.Pred2Lag) as Lag2,  
 ---- ps.Pred3,  
 ---- ISNULL(ps.Lag3,ct.Pred3Lag) as Lag3  
 ----FROM ProjectSchedules ps  
 ----RIGHT JOIN ConstructionTasks ct  
 ----on ps.ConstructionTaskId = ct.ID AND ps.ProjectId = @ProjectId  
 ------order by ISNULL(ps.Sequence,ct.Sequence)  
END
