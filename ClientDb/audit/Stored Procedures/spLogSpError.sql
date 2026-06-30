/*

Description: Log error from sp execution to [audit].[SpErrorLogs]

Info: 
RAISERROR(message_string, severity, state)
Severity level (common levels):
10 = Informational
16 = General errors
20-25 = Fatal errors (closes the connection)

State value:
1 = state (can be used to identify where the error occurred)
*/

CREATE PROCEDURE [audit].[spLogSpError]
(
    @SpName NVARCHAR(300),
	@ErrorMessage NVARCHAR(MAX)
)
AS
BEGIN
	IF @SpName IS NULL OR LTRIM(RTRIM(@SpName)) = ''
    BEGIN
        RAISERROR('Error: @spName cannot be NULL or empty.', 16, 1)
    END

	IF @ErrorMessage IS NULL OR LTRIM(RTRIM(@SpName)) = ''
    BEGIN
        RAISERROR('Error: @ErrorMessage cannot be NULL or empty.', 16, 1)
    END

    INSERT INTO [audit].[SpErrorLogs]
           ([SP_Name]
           ,[ErrorMessage]
           ,[DateCreated])
     VALUES
           (@SpName
           ,@ErrorMessage
           ,GETDATE())
END
