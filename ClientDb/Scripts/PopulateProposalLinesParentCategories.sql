DECLARE @ProposalId UNIQUEIDENTIFIER;

-- Declare a cursor to iterate through all IDs in the Proposals table
DECLARE ProposalCursor CURSOR FOR
SELECT ID FROM Proposals;

-- Open the cursor
OPEN ProposalCursor;

-- Fetch the first ID into the @ProposalId variable
FETCH NEXT FROM ProposalCursor INTO @ProposalId;

-- Loop through each ID
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Execute the stored procedure for the current ProposalId
    EXEC spCDPopulateProposalLinesParentCategories @ProposalId;

    -- Fetch the next ID
    FETCH NEXT FROM ProposalCursor INTO @ProposalId;
END;

-- Close and deallocate the cursor
CLOSE ProposalCursor;
DEALLOCATE ProposalCursor;
