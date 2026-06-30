CREATE PROC spCDGetRevisedEstimate
(
 @ProposalId uniqueidentifier
)
AS
BEGIN
    DECLARE @QbClassId uniqueidentifier;
    SET @QbClassId = (
        SELECT QbClassId
        FROM Proposals
        WHERE ID = @ProposalId
    );
    
    DECLARE @InitialDeposit DECIMAL(21,2);
    DECLARE @RequestedAmount DECIMAL(21,2);
    DECLARE @CurrentJobBalance DECIMAL(21,2);
    DECLARE @TotalCostToDate DECIMAL(21,2);
    DECLARE @JobBalance DECIMAL(21,2);
    DECLARE @OwnerDeposits DECIMAL(21,2);
    DECLARE @MinimumRequestedAmount DECIMAL(21,2);

    DECLARE @RevisedEstimates TABLE (
        Name nvarchar(150),
        EstimateCategoryID uniqueidentifier NULL,
        ParentEstimateCategoryID uniqueidentifier NULL,
        ParentEstimateCategory nvarchar(150) NULL,
        OriginalAmount decimal(21,2) NULL,
        RevisedAmount decimal(21,2) NULL,
        CostToDate decimal(21,2) NULL,
        Balance decimal(21,2) NULL,
        ParentSequence int NULL,
        Percentage decimal(21,2) NULL,
        Sequence int NULL
    );

    DECLARE @Transactions TABLE (
        ID uniqueidentifier,
        AccountId uniqueidentifier,
        FullyQualifiedName nvarchar(250),
        AccountType nvarchar(100),
        Amount decimal(21,2),
        TransactionDate datetime,
        EstimateCategoryID uniqueidentifier NULL,
        EstimateCategory nvarchar(250) NULL
    );

    -- ===================================
    -- GET TRANSACTIONS
    -- ===================================
    INSERT INTO @Transactions
    SELECT
        t.ID,
        a.ID,
        a.FullyQualifiedName,
        a.AccountType,
        t.Amount,
        t.TransactionDate,
        ec.ID,
        ec.Name AS EstimateCategory
    FROM QBTransactions t
    INNER JOIN QBAccounts a
        ON a.ID = t.AccountID
    LEFT JOIN EstimateMappings em
        ON em.QBAccountID = a.ID
    LEFT JOIN EstimateCategories ec
        ON ec.ID = em.EstimateSubCategoryID
    WHERE t.ClassID = @QbClassId
        AND ( t.[Status] IS NULL OR t.[Status] NOT LIKE '%Voided%' )
    ORDER BY em.EstimateSubCategoryID

    -- ===================================
    -- GET REVISED ESTIMATE
    -- ===================================
    INSERT INTO @RevisedEstimates
    SELECT
        Name,
        EstimateCategoryID,
        ParentEstimateCategoryID,
        ParentEstimateCategory,
        OriginalAmount,
        RevisedAmount,
        CostToDate,
        (RevisedAmount - CostToDate) AS Balance,
        ParentSequence,
        dbo.getCDPercentage(CostToDate,RevisedAmount) AS Percentage,
        Sequence
    FROM (
        SELECT
            Name,
            EstimateCategoryID,
            ParentEstimateCategoryID,
            ParentEstimateCategory,
            OriginalAmount,
            --RevisedAmount,
            CASE WHEN RevisedAmount = -1 THEN OriginalAmount ELSE RevisedAmount END AS RevisedAmount,
            CostToDate,
            0 AS Balance,
            ParentSequence,
            dbo.getCDPercentage(CostToDate,RevisedAmount) AS Percentage,
            Sequence
        FROM (
            SELECT
                ISNULL(pl.Name,ec.Name) AS Name,
                pl.EstimateCategoryID,
                pl.ParentEstimateCategoryID,
                (SELECT TOP 1 Name FROM ProposalLines WHERE ProposalId = @ProposalId AND EstimateCategoryId = pl.ParentEstimateCategoryID) AS ParentEstimateCategory,
                pl.Amount AS OriginalAmount,
                ISNULL((SELECT TOP 1 Amount FROM ProposalLinesHistory ph
                        WHERE ph.ProposalID = @ProposalId AND ph.EstimateCategoryID = pl.EstimateCategoryID
                        AND ph.ChangeType = 'Updated'
                        ORDER BY ChangeDate DESC),-1) AS RevisedAmount,
                ISNULL((SELECT CONVERT(DECIMAL(18,2), SUM(Amount))
                        FROM @Transactions t
                        WHERE t.EstimateCategoryId = pl.EstimateCategoryID AND AccountType <> 'Income'),0) AS CostToDate,
                (SELECT TOP 1 Sequence FROM ProposalLines WHERE ProposalId = @ProposalId AND EstimateCategoryId = pl.ParentEstimateCategoryID) AS ParentSequence,
                pl.Sequence AS Sequence
            FROM Proposals p
            INNER JOIN ProposalLines pl
                ON p.ID = pl.ProposalID
            LEFT JOIN EstimateCategories ec
                ON ec.ID = pl.EstimateCategoryID
            WHERE p.ID = @ProposalId
              AND pl.ParentEstimateCategoryID IS NOT NULL
        ) sq1
    ) sq2;

    -- ===================================
    -- SET Total Cost To Date
    -- SET Owner Deposits
    -- SET Job Balance
    -- SET Minimum Requested Amount
    -- ===================================
    SET @TotalCostToDate = (SELECT SUM(ISNULL(CostToDate,0)) FROM @RevisedEstimates);
    SET @OwnerDeposits = (SELECT ISNULL(SUM(ISNULL(AMOUNT,0)),0) FROM @Transactions WHERE AccountType = 'Income');
    --SET @JobBalance = @OwnerDeposits - @TotalCostToDate;
    SET @JobBalance = (SELECT ISNULL(Balance,0) FROM JobBalances WHERE JobId = @QbClassId);
    SET @MinimumRequestedAmount = (SELECT MinimumRequestedAmount FROM QBClasses WHERE ID = @QbClassId);

    -- ===================================
    -- GET Initial Deposit
    -- ===================================
    SELECT TOP 1 @InitialDeposit = ISNULL(Amount,0)
    FROM @Transactions
    WHERE AccountType = 'Income'
      AND Amount % 5000 = 0
    ORDER BY TransactionDate;

    -- ===================================
    -- SET Requested Amount
    -- ===================================
    /*
    If @CurrentJobBalance > @InitialDeposit
        SET @RequestedAmount = 0;
    ELSE
        SET @RequestedAmount = @InitialDeposit;

    SET @RequestedAmount = ROUND((@InitialDeposit * .20),2);
    */
    SET @RequestedAmount = ISNULL(@MinimumRequestedAmount,@InitialDeposit);

    -- ===================================
    -- OUTPUT
    -- ===================================
    SELECT
        Name,
        NULL AS AccountId,
        EstimateCategoryID,
        ParentEstimateCategoryID,
        ParentEstimateCategory,
        OriginalAmount,
        RevisedAmount,
        CostToDate,
        (RevisedAmount - CostToDate) AS Balance,
        ParentSequence,
        dbo.getCDPercentage(CostToDate,RevisedAmount) AS Percentage,
        Sequence
    FROM @RevisedEstimates re
    
    UNION ALL
    
    SELECT
        FullyQualifiedName, AccountId, NULL,NULL,NULL,NULL,NULL,SUM(Amount) AS CostToDate,NULL,1000,NULL,NULL
    FROM @Transactions
    WHERE EstimateCategoryID IS NULL AND AccountType <> 'Income'
    GROUP BY FullyQualifiedName, AccountId
    
    UNION ALL
    
    SELECT 'MINIMUM REQUESTED AMOUNT', NULL, NEWID(),NULL,NULL,NULL,NULL,@RequestedAmount AS CostToDate,NULL,1000,NULL,3
    
    UNION ALL
    
    SELECT 'TOTAL COST TO DATE', NULL, NEWID(),NULL,NULL,NULL,NULL,@TotalCostToDate AS CostToDate,NULL,1000,NULL,1
    
    UNION ALL
    
    SELECT 'OWNER DEPOSITS', NULL, NEWID(),NULL,NULL,NULL,NULL,@OwnerDeposits AS CostToDate,NULL,1000,NULL,1
    
    UNION ALL
    
    SELECT 'JOB BALANCE', NULL, NEWID(),NULL,NULL,NULL,NULL,@JobBalance AS CostToDate,NULL,1000,NULL,2
    ORDER BY re.ParentSequence, re.Sequence;
END
