CREATE PROCEDURE [dbo].[spUpdateOpenedAndClosedDate] 
AS
BEGIN	
    SET NOCOUNT ON;

    IF 1=0 BEGIN
       SET FMTONLY OFF
    END


    CREATE TABLE #Query
    (
        ClassID NVARCHAR(255),
        OpenedDate datetime2,
        ClosedDate datetime2
    );

    CREATE TABLE #Results
    (
        ClassID NVARCHAR(255),
        OpenedDate datetime2,
        ClosedDate datetime2
    );


    -- Call dbo.GetHighestPercentageOfTransactions() function to update AccountID
	UPDATE c
	SET c.QBAccountID = h.BankAccount
	FROM QBClasses c
	INNER JOIN dbo.GetHighestPercentageOfTransactions() h ON h.ClassID = c.ID
	WHERE c.QBAccountID IS NULL;



    INSERT INTO #Query
    SELECT c.ID AS Class,
           MIN(TransactionDate) AS OpenedDate,
           MAX(TransactionDate) AS ClosedDate
    FROM QBClasses c
    FULL JOIN QBTransactions t ON t.ClassID = c.ID
	WHERE C.ID IS NOT NULL
    GROUP BY c.ID
    ORDER BY c.ID

    -- Select final results
    INSERT INTO #Results
    SELECT ClassID, OpenedDate, ClosedDate 
    FROM #Query r

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Update the opened and closed dates
        UPDATE c
        SET c.OpenedDate = r.OpenedDate, c.ClosedDate = r.ClosedDate
        FROM QBClasses c
        INNER JOIN #Results r ON r.ClassID = c.ID 


        DROP TABLE #Query;
        DROP TABLE #Results;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0  
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 'Something went wrong.'
        END
    END CATCH

    IF @@TRANCOUNT > 0  
    BEGIN
        COMMIT TRANSACTION;
        SELECT 'Changes saved successfully.'
    END
END