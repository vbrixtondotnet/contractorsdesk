CREATE FUNCTION [dbo].[getAccountChildrenID](
					@ID NVARCHAR(255)
					)
					RETURNS 
							@Table_OBSID TABLE 
							(
								ID NVARCHAR(255)
							)
					AS
					BEGIN


					DECLARE @ChildID NVARCHAR(255)
					DECLARE @hasChild INT
					DECLARE db_cursor_children CURSOR FOR 
					SELECT ListID FROM dbo.QBAccounts WHERE ParentID = @ID
					OPEN db_cursor_children;
					FETCH NEXT FROM db_cursor_children INTO @ChildID

					SET @hasChild = (SELECT COUNT(*) FROM QBAccounts WHERE ParentID = @ID)

					INSERT @Table_OBSID
					SELECT ListID FROM dbo.QBAccounts WHERE ListID = ISNULL(@ID, ListID)

					IF @hasChild > 0 AND @ID IS NOT NULL
					BEGIN

						 WHILE @@FETCH_STATUS = 0  

							 BEGIN
								 INSERT @Table_OBSID
								 SELECT ID FROM [dbo].[getAccountChildrenID](@ChildID)
								 FETCH NEXT FROM db_cursor_children INTO @ChildID
							 END

							 CLOSE db_cursor_children;
							 DEALLOCATE db_cursor_children; 

					END

				RETURN 
				END;