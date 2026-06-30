CREATE PROC spGetTotalCost
(@QbClassId uniqueidentifier)
AS 
BEGIN
SELECT dbo.getTotalCost(@QbClassId) as TotalCost
END