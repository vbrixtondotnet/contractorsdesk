	CREATE FUNCTION [dbo].[getAccountLevels]()
					RETURNS 
							@Table TABLE 
							(
								Level NVARCHAR(255),
								Display NVARCHAR(255)
							)
					AS
					BEGIN
					
					
						;WITH Tree (AccountID, ParentAccountID, AccountLevel)
						AS (
						SELECT
							ListID,
							ParentID,
							1 AS AccountLevel
						FROM QBAccounts a
						WHERE ParentID IS NULL or ParentID = ''
						UNION ALL
						SELECT 
							a.ListID,
							a.ParentID,
							t.AccountLevel + 1 AS AccountLevel
						FROM QBAccounts AS a
							JOIN Tree t ON t.AccountID = a.ParentID   
						)

						INSERT INTO @Table
						SELECT 0 as Level, 'Top Level' as Display
						UNION ALL
						SELECT DISTINCT AccountLevel, CASE WHEN AccountLevel = (SELECT MAX(AccountLevel) FROM Tree) THEN 'All Detail'
														   WHEN AccountLevel = 1                                    THEN '2nd Level'
														   WHEN AccountLevel = 2                                    THEN '3rd Level'
														   WHEN AccountLevel >= 3                                    THEN CAST(AccountLevel+1 AS NVARCHAR(3))+'th Level'END
						FROM Tree t
						GROUP BY AccountLevel

				RETURN 
				END;