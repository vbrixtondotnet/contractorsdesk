CREATE PROC spCreateProposalFromJob
(@QbClassId nvarchar(100))
AS 
BEGIN
DECLARE @Number as int;
DECLARE @ProposalId uniqueidentifier;
DECLARE @CompanyUserId int;


IF EXISTS (SELECT TOP 1 * FROM Proposals WHERE QBClassId = @QbClassId)
RETURN

-- check if a proposal already exists for this class based on name
DECLARE @ClassName nvarchar(250);
DECLARE @ExistingProposalId uniqueidentifier;
DECLARE @ClassIdFromExistingProposal uniqueidentifier;
DECLARE @Address nvarchar(250);
DECLARE @City nvarchar(250);
DECLARE @State nvarchar(250);
DECLARE @ExistingProposalName nvarchar(250);

SELECT @ClassName = Name FROM QBClasses WHERE ID = @QbClassId;
SELECT @Address = Address FROM QBClasses WHERE ID = @QbClassId;
SELECT @City = City FROM QBClasses WHERE ID = @QbClassId;
SELECT @State = State FROM QBClasses WHERE ID = @QbClassId;
SELECT @ClassIdFromExistingProposal = ID from QBClasses where ID <> @QbClassId and LOWER(REPLACE(RTRIM(LTRIM(Name)), ' ', ''))
 = LOWER(REPLACE(RTRIM(LTRIM(@ClassName)), ' ', ''))

IF @ClassIdFromExistingProposal IS NOT NULL
BEGIN
    -- a proposal with a dummy qbclass 
    SELECT @ExistingProposalId = ID FROM Proposals WHERE QBClassId = @ClassIdFromExistingProposal
	IF @ExistingProposalId IS NOT NULL
	BEGIN
        -- assign the existing proposal to this class
		UPDATE Proposals
		SET QBClassId = @QbClassId, 
            DocStatus = 'Accepted'
		WHERE ID = @ExistingProposalId

        --deactivate the previous class for this proposal
        UPDATE QBClasses
        SET IsActive = 0, IsDeleted = 1
		WHERE ID = @ClassIdFromExistingProposal

	END
	RETURN
END
ELSE
    SET @ExistingProposalId = (SELECT p.ID from proposals p 
        inner join proposalProjects pp on p.ProposalProjectId = pp.Id 
        where
            p.QBClassId is null and
            LOWER(REPLACE(RTRIM(LTRIM(pp.Name)), ' ', '')) =  LOWER(REPLACE(RTRIM(LTRIM(@ClassName)), ' ', '')));

    IF @ExistingProposalId IS NOT NULL-- a proposal without a dummy qb class
    BEGIN
       UPDATE Proposals
       SET QBClassId = @QbClassId,
		   DocStatus = 'Accepted'
	   WHERE ID = @ExistingProposalId
    END
    ELSE
        BEGIN -- a newly downloaded class with no proposal
            SET @Number = (SELECT ISNULL(MAX(NUMBER),0) FROM Proposals) + 1; 
            SET @ProposalId = NEWID();
            SET @CompanyUserId = (select top 1 Id from Users where RoleId = 4);

            INSERT INTO [dbo].[Proposals]
                       ([ID]
                       ,[Number]
                       ,[TemplateId]
                       ,[QBClassId]
                       ,[Date]
                       ,[TotalAmount]
                       ,[DocStatus])
                 VALUES
                       (@ProposalId
                       ,@Number
                       ,'D1103B14-2196-48C9-BB93-DC6DE65962C2'
                       ,@QbClassId
                       ,GETDATE()
                       ,0
                       ,'Accepted')

            -- Insert proposal lines
            INSERT INTO [dbo].[ProposalLines]
                       ([ID]
                       ,[Name]
                       ,[Amount]
                       ,[ProposalID]
                       ,[EstimateCategoryID]
                       ,[ParentEstimateCategoryID]
                       ,[Sequence])

               SELECT 
                NEWID(),
	            e.Name,
	            0,
	            @ProposalId,	
	            e.ID,
	            e.ParentEstimateCategoryID,
	            e.Sequence
               FROM EstimateCategories e
               INNER JOIN EstimateMappings m on m.EstimateSubCategoryID = e.ID
               INNER JOIN QBAccounts a on a.ID = m.QBAccountID
               INNER JOIN QBTransactions t on t.AccountID = a.ID
               INNER JOIN QBClasses c on c.ID = t.ClassID
               Where c.ID = @QbClassId
               and a.AccountType in ('Expense', 'OtherExpense')
               GROUP BY e.ID,e.Name, e.ParentEstimateCategoryID, c.Name,c.FullyQualifiedName,e.Sequence

			-- INSERT PROPOSAL LINE ITEMS FROM GROUND UP TEMPLATE
			INSERT INTO [dbo].[ProposalLines]
                       ([ID]
                       ,[Name]
                       ,[Amount]
                       ,[ProposalID]
                       ,[EstimateCategoryID]
                       ,[ParentEstimateCategoryID])
			SELECT 
				NEWID(),
				ptli.Name,
				0,
				@ProposalId,
				ptli.EstimateCategoryId,
				ptli.ParentId
			from ProposalTemplatesLineItems ptli
			inner join ProposalTemplates pt
			on pt.Id = ptli.ProposalTemplateId
			where pt.Name = 'Ground-Up (Default)'
			and ptli.EstimateCategoryId <> 'A5DA1985-1858-49E8-ABB8-558EFC382AAC'
			and ptli.ParentId <> 'A5DA1985-1858-49E8-ABB8-558EFC382AAC'
			and ptli.EstimateCategoryId not in
			(
				select EstimateCategoryId from ProposalLines where ProposalID = @ProposalId
			)
			order by ptli.Sequence
    
            IF NOT EXISTS (SELECT TOP 1 * FROM ProposalLines WHERE ProposalID = @ProposalId and EstimateCategoryID = 'a5da1985-1858-49e8-abb8-558efc382aac')
            BEGIN
            -- INSERT THE OVERHEAD CATEGORY
            INSERT INTO [dbo].[ProposalLines]
                       ([ID]
                       ,[Name]
                       ,[Amount]
                       ,[ProposalID]
                       ,[EstimateCategoryID]
                       ,[ParentEstimateCategoryID]
                       ,[Sequence])
            VALUES
            (
	            NEWID(),
	            'Overhead',
	            0,
	            @ProposalId,
	            'a5da1985-1858-49e8-abb8-558efc382aac',
	            NULL,
	            100
            )
            -- Line items for the overhead category
            INSERT INTO [dbo].[ProposalLines]
                       ([ID]
                       ,[Name]
                       ,[Amount]
                       ,[ProposalID]
                       ,[EstimateCategoryID]
                       ,[ParentEstimateCategoryID]
                       ,[Sequence])
            SELECT 
                NEWID() AS Id,
                pli.Name,
                0 AS Amount,
                @ProposalId AS ProposalId,
                pli.EstimateCategoryId,
                e.ParentEstimateCategoryID,
                ROW_NUMBER() OVER (ORDER BY pli.Name) AS Sequence
            FROM ProposalTemplatesLineItems pli
            LEFT JOIN EstimateCategories e
                ON e.ID = pli.EstimateCategoryId
            WHERE pli.ProposalTemplateId = 'D1103B14-2196-48C9-BB93-DC6DE65962C2'
              AND pli.ParentId = 'A5DA1985-1858-49E8-ABB8-558EFC382AAC';
            END

            INSERT INTO [dbo].[ProposalLinesHistory]
                       ([HistoryID]
                       ,[ProposalLineID]
                       ,[ProposalID]
                       ,[ChangeType]
                       ,[ChangeDate]
                       ,[Amount]
                       ,[Description]
                       ,[EstimateCategoryID]
                       ,[ParentEstimateCategoryID]
                       ,[Created]
                       ,[CreatedBy]
                       ,[Percentage])
            SELECT
	            NEWID(),
	            ID,
	            ProposalID,
	            'Updated',
	            GETDATE(),
	            Amount,
	            Description,
	            EstimateCategoryID,
	            ParentEstimateCategoryID,
	            GETDATE(),
	            1,
	            Percentage
            from ProposalLines where proposalid = @ProposalId

            EXEC spCDPopulateProposalLinesParentCategories @ProposalId;

            -- create proposal client
            DECLARE @ClientId uniqueidentifier;
            SET @ClientId = NEWID();
            INSERT INTO [dbo].[Clients]
                   ([Id]
                   ,[Name]
                   ,[Address]
                   ,[City]
                   ,[State]
                   ,[CompanyName]
                   ,[EmailAddress]
                   ,[Phone]
                   ,[DateCreated]
                   ,[CreatedBy])
             VALUES
                   (@ClientId
                   ,''
                   ,''
                   ,''
                   ,''
                   ,''
                   ,''
                   ,''
                   ,GETDATE()
                   ,@CompanyUserId)

            UPDATE Proposals SET ClientId = @ClientId WHERE ID = @ProposalId;

            -- PROPOSAL PROJECT DETAILS
            DECLARE @ProposalProjectId uniqueidentifier;
            SET @ProposalProjectId = NEWID();
            INSERT INTO [dbo].[ProposalProjects]
                       ([Id]
                       ,[Name]
                       ,[Address]
                       ,[City]
                       ,[State]
                       ,[DateCreated]
                       ,[CreatedBy])
                 VALUES
                       (@ProposalProjectId
                       ,@ClassName
                       ,@Address
                       ,@City
                       ,@State
                       ,GETDATE()
                       ,@CompanyUserId)

            UPDATE Proposals SET ProposalProjectId = @ProposalProjectId WHERE ID = @ProposalId;

            -- PROPOSAL SUPERVISOR
            INSERT INTO [dbo].[ProposalSupervisors]
                   ([Id]
                   ,[ProposalId]
                   ,[UserId]
                   ,[DateCreated])
             VALUES
                   (NEWID()
                   ,@ProposalId
                   ,@CompanyUserId
                   ,GETDATE())
        END
END
