CREATE PROC spCDGetTransactionDetails
(
	@ProposalId uniqueidentifier,
	@EstimateCategoryId uniqueidentifier = null,
	@ParentEstimateCategoryId uniqueidentifier = null,
	@StartDate date null,
	@EndDate date null
)
AS
BEGIN
    DECLARE @QbClassId uniqueidentifier;

    SET @QbClassId = (
        SELECT QbClassId
        FROM Proposals
        WHERE ID = @ProposalId
    );

    DECLARE @ProjectTotalsId uniqueidentifier;
    DECLARE @OwnerDepositsId uniqueidentifier;
    DECLARE @InitialDeposit DECIMAL(21,2);
    DECLARE @RequestedAmount DECIMAL(21,2);
    DECLARE @CurrentJobBalance DECIMAL(21,2);

    SET @ProjectTotalsId = '0934406a-64db-432b-a01c-5d7573a0f3ce';
    SET @OwnerDepositsId = '36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38';

    DECLARE @Transactions TABLE
    (
        EstimateCategoryId uniqueidentifier,
        ParentEstimateCategoryId uniqueidentifier,
        EstimateCategory nvarchar(100),
        EstimateSubCategory nvarchar(100),
        AccountType nvarchar(100),
        Amount decimal(18,2),
        Date datetime,
        Type nvarchar(MAX),
        Num nvarchar(100),
        Payee nvarchar(MAX),
        Memo nvarchar(MAX),
        CategorySequence int,
        ItemSequence int
    );

    DECLARE @RevisedEstimate TABLE
    (
        EstimateCategoryId uniqueidentifier,
        RevisedEstimate decimal(18,2)
    );

    DECLARE @GrossProfit TABLE
    (
        JobBalance DECIMAL(21,2)
    );

    DECLARE @NetIncome TABLE
    (
        JobBalance DECIMAL(21,2)
    );

    DECLARE @NetOperatingIncome TABLE
    (
        JobBalance DECIMAL(21,2)
    );

	-- ===================================
    -- GET TRANSACTIONS
    -- ===================================
    IF @StartDate IS NOT NULL AND @EndDate IS NOT NULL
    BEGIN
        INSERT INTO @Transactions
        SELECT
            e.ID,
            e.ParentEstimateCategoryID,
            pe.Name,
            e.Name,
            a.AccountType,
            t.Amount,
            t.TransactionDate,
            t.TxnType,
            ISNULL(t.TxnNumber,''),
            ISNULL(t.Name,''),
            t.Memo,
            pe.Sequence,
            e.Sequence
        FROM QBTransactions t
        INNER JOIN QBAccounts a
            ON a.ID = t.AccountID
        LEFT JOIN EstimateMappings em
            ON em.QBAccountID = a.ID
        LEFT JOIN EstimateCategories e
            ON e.ID = em.EstimateSubCategoryID
        LEFT JOIN EstimateCategories pe
            ON pe.ID = e.ParentEstimateCategoryID
        WHERE t.ClassID = @QbClassId
          AND t.TransactionDate BETWEEN @StartDate AND @EndDate
          AND ( t.[Status] IS NULL OR t.[Status] NOT LIKE '%Voided%' )
        ORDER BY e.Sequence;
    END
    ELSE
    BEGIN
        INSERT INTO @Transactions
        SELECT
            e.ID,
            e.ParentEstimateCategoryID,
            pe.Name,
            e.Name,
            a.AccountType,
            t.Amount,
            t.TransactionDate,
            t.TxnType,
            ISNULL(t.TxnNumber,''),
            ISNULL(t.Name,''),
            t.Memo,
            pe.Sequence,
            e.Sequence
        FROM QBTransactions t
        INNER JOIN QBAccounts a
            ON a.ID = t.AccountID
        LEFT JOIN EstimateMappings em
            ON em.QBAccountID = a.ID
        LEFT JOIN EstimateCategories e
            ON e.ID = em.EstimateSubCategoryID
        LEFT JOIN EstimateCategories pe
            ON pe.ID = e.ParentEstimateCategoryID
        WHERE t.ClassID = @QbClassId
            AND ( t.[Status] IS NULL OR t.[Status] NOT LIKE '%Voided%' )
        ORDER BY e.Sequence;
    END
    --and a.AccountType <> 'Income'

    -- ===================================
    -- GET REVISED ESTIMATE
    -- ===================================
    INSERT INTO @RevisedEstimate
    SELECT DISTINCT
        EstimateCategoryId,
        ISNULL(
            (SELECT TOP 1 Amount
             FROM ProposalLinesHistory ph
             WHERE ph.ProposalID = @ProposalId
               AND ph.EstimateCategoryID = t.EstimateCategoryId
               AND ph.ChangeType = 'Updated'
             ORDER BY ChangeDate DESC),
            (SELECT TOP 1 Amount
             FROM ProposalLines pl
             WHERE pl.ProposalID = @ProposalId
               AND pl.EstimateCategoryID = t.EstimateCategoryId)
        ) AS RevisedAmount
    FROM @Transactions t;

    -- ===================================
    -- GROSS PROFIT
    -- ===================================
    INSERT @GrossProfit
    SELECT SUM(AMOUNT) AS JobBalance
    FROM (
        SELECT SUM(AMOUNT) as AMOUNT
        FROM @Transactions
        WHERE AccountType = 'Income'

        UNION ALL

        SELECT SUM(AMOUNT) * -1 as AMOUNT
        FROM @Transactions
        WHERE AccountType = 'CostofGoodsSold'
    ) gp;

    -- ===================================
    -- NET OPERATING INCOME
    -- ===================================
    INSERT @NetOperatingIncome
    SELECT SUM(AMOUNT) AS JobBalance
    FROM (
        SELECT JobBalance as AMOUNT
        FROM @GrossProfit

        UNION ALL

        SELECT SUM(AMOUNT) * -1 as AMOUNT
        FROM @Transactions
        WHERE AccountType = 'Expense'
    ) noi;

    -- ===================================
    -- NET INCOME
    -- ===================================
    INSERT @NetIncome
    SELECT SUM(AMOUNT) AS JobBalance
    FROM (
        SELECT JobBalance as AMOUNT
        FROM @NetOperatingIncome

        UNION ALL

        SELECT SUM(AMOUNT) as AMOUNT
        FROM @Transactions
        WHERE AccountType = 'OtherIncome'

        UNION ALL

        SELECT SUM(AMOUNT) * -1 as AMOUNT
        FROM @Transactions
        WHERE AccountType = 'OtherExpense'
    ) ni;

    -- ===================================
    -- OUTPUTS
    -- ===================================
    IF (@EstimateCategoryId IS NOT NULL)
    BEGIN
        SELECT
            t.*,
            ISNULL(re.RevisedEstimate,0) AS RevisedEstimate
        FROM @Transactions t
        INNER JOIN @RevisedEstimate re
            ON re.EstimateCategoryId = t.EstimateCategoryId
        WHERE t.EstimateCategoryId = @EstimateCategoryId
          AND t.AccountType <> 'Income'
        ORDER BY t.ItemSequence;
    END

    -- PROJECT TOTALS
    ELSE IF (@ParentEstimateCategoryId = @ProjectTotalsId)
    BEGIN
        SELECT
            t.*,
            ISNULL(re.RevisedEstimate,0) AS RevisedEstimate
        FROM @Transactions t
        INNER JOIN @RevisedEstimate re
            ON re.EstimateCategoryId = t.EstimateCategoryId
        WHERE t.EstimateCategoryId IS NOT NULL
          AND t.AccountType <> 'Income'
        ORDER BY CategorySequence, ItemSequence;
    END

    -- OWNER DEPOSITS
    ELSE IF (@ParentEstimateCategoryId = @OwnerDepositsId)
    BEGIN
        SELECT
            t.*,
            0.00 AS RevisedEstimate
        FROM @Transactions t
        WHERE t.AccountType = 'Income'
          AND EstimateCategoryId IS NOT NULL
        ORDER BY CategorySequence, ItemSequence;
    END

    -- DEFAULT
    ELSE
    BEGIN
        SELECT 
            t.*,  
            ISNULL(re.RevisedEstimate,0) AS RevisedEstimate 
        FROM @Transactions t
        INNER JOIN @RevisedEstimate re
            ON re.EstimateCategoryId = t.EstimateCategoryId
        WHERE t.ParentEstimateCategoryId = @ParentEstimateCategoryId
          AND t.AccountType <> 'Income'
        ORDER BY CategorySequence, ItemSequence;
    END
END
