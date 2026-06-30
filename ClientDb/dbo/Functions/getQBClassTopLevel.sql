CREATE FUNCTION [dbo].[getQBClassTopLevel](
				@Name NVARCHAR(255)
				)
                RETURNS NVARCHAR(255)
                AS
                BEGIN

				DECLARE @Result NVARCHAR(255)
				DECLARE @ParentName NVARCHAR(255)
				DECLARE @ParentID NVARCHAR(255)
				
				SET @ParentID = (SELECT ParentID FROM QBClasses WHERE FullyQualifiedName = @Name)

				SET @ParentName = (SELECT FullyQualifiedName FROM QBClasses WHERE ListID = @ParentID)

				IF @ParentID IS NOT NULL AND @Name IS NOT NULL
				BEGIN
					
					SET @Result = (SELECT dbo.getQBClassTopLevel(@ParentName))
					
				END ELSE
				BEGIN
					
					SET @Result = (SELECT FullyQualifiedName FROM dbo.QBClasses WHERE FullyQualifiedName = @Name)
				END

			RETURN @Result
            END;