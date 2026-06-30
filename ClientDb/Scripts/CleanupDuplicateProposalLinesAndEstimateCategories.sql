DECLARE @ProposalId uniqueidentifier;

-- Declare the cursor
DECLARE proposal_cursor CURSOR FOR
SELECT ID FROM Proposals;

-- Open the cursor
OPEN proposal_cursor;

-- Fetch the first row
FETCH NEXT FROM proposal_cursor INTO @ProposalId;

-- Loop through all rows
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Call the stored procedure for each row
    EXEC spCleanUpDuplicateProposalLinesAndEstimateCategories @ProposalId;

    -- Fetch the next row
    FETCH NEXT FROM proposal_cursor INTO @ProposalId;
END;

-- Close and deallocate the cursor
CLOSE proposal_cursor;
DEALLOCATE proposal_cursor;