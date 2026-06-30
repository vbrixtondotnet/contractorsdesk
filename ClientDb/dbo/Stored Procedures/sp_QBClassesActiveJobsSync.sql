/*

Name: SP_QBClassesActiveJobsSync
Description: Execute this SP after daily data sync to create proposal records from QuickBooks Classes with active jobs.

*/

CREATE PROCEDURE [dbo].[sp_QBClassesActiveJobsSync]
AS
BEGIN

	BEGIN TRY

		DECLARE @QbClassId uniqueidentifier
		DECLARE @ActiveProjects TABLE (    
			QbClassId uniqueidentifier,
			[Name] NVARCHAR(MAX)
		)
		
		TRUNCATE TABLE ProjectTotals
		
		INSERT @ActiveProjects    
			SELECT c.ID, c.[Name]
			FROM QBClasses c
			WHERE c.IsDeleted = 0
			AND (c.ActiveJobs = 1  OR c.ActiveSpecJobs = 1)
		
		DECLARE @Counter INT
		DECLARE @CurrentQbClassId uniqueidentifier
		
		SET @Counter = (SELECT COUNT(QbClassId) from @ActiveProjects)
		
		WHILE @Counter > 0
		BEGIN
			SET @CurrentQbClassId = (SELECT TOP 1 QbClassId FROM @ActiveProjects)
			
			EXEC [dbo].spCreateProposalFromJob @CurrentQbClassId
			EXEC [dbo].spCDUpdateProjectTotals @CurrentQbClassId
			
			DELETE FROM @ActiveProjects where QbClassId = @CurrentQbClassId
			SET @Counter = (SELECT COUNT(QbClassId) from @ActiveProjects)

		END

	END TRY
    BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE()
		EXEC [audit].[spLogSpError]
			'[dbo].[SP_QBClassesActiveJobsSync]',
			@ErrorMessage
        PRINT @ErrorMessage;
    END CATCH;

	SET NOCOUNT OFF;

END
