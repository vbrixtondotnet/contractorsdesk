CREATE PROCEDURE [dbo].[spCDGetProjectScheduleItems]
	@ProjectId uniqueidentifier
AS
BEGIN
	select 
		pst.Id,
		pst.Name
	from ProjectScheduleTasks pst
	inner join ProjectSchedules ps
	on ps.Id = pst.ProjectScheduleId
	where ps.ProjectId = @ProjectId
END
