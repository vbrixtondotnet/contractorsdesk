CREATE PROC spCDUpdateProjectTotals (    
 @QBClassId uniqueidentifier    
)    
AS    
BEGIN    
BEGIN TRY
	DECLARE @ProposalId uniqueidentifier;    
    SET @ProposalId = (SELECT Id from Proposals where QBClassId = @QBClassId);    
    
    DECLARE @InitialDeposit DECIMAL(21,2);     
    DECLARE @RequestedAmount DECIMAL(21,2);      
    DECLARE @CurrentJobBalance DECIMAL(21,2);    
    DECLARE @TotalCostToDate DECIMAL(21,2);     
    DECLARE @JobBalance DECIMAL(21,2);     
    DECLARE @OwnerDeposits DECIMAL(21,2);     
    DECLARE @RevisedEstimates TABLE      
    (      
     Name nvarchar(150),      
     EstimateCategoryID uniqueidentifier null,      
     ParentEstimateCategoryID uniqueidentifier null,      
     ParentEstimateCategory nvarchar(150) null,      
     OriginalAmount decimal(21,2) null,      
     RevisedAmount decimal(21,2) null,     
     CostToDate decimal(21,2) null,   
     Balance decimal(21,2) null,      
     ParentSequence int null,      
     Percentage decimal(21,2) null,      
     Sequence int null     
    )   
  
    DECLARE @Transactions TABLE(  
     ID uniqueidentifier,  
     AccountId uniqueidentifier,  
     FullyQualifiedName nvarchar(250),  
     AccountType nvarchar(100),  
     Amount decimal(21,2),  
     TransactionDate datetime,  
     EstimateCategoryID uniqueidentifier null,  
     EstimateCategory nvarchar(250) null  
    )  
  
    INSERT INTO @Transactions  
    select   
     t.ID,  
     a.ID,  
     a.FullyQualifiedName,  
     a.AccountType,  
     t.Amount,  
     t.TransactionDate,  
     ec.ID,  
     ec.Name as EstimateCategory  
    from QBTransactions t  
    inner join QBAccounts a  
    on a.ID = t.AccountID  
    left join EstimateMappings em  
    on em.QBAccountID = a.ID  
    left join EstimateCategories ec  
    on ec.ID = em.EstimateSubCategoryID  
    where t.ClassID = @QbClassId  
    order by em.EstimateSubCategoryID  
     
    INSERT INTO @RevisedEstimates  
    SELECT     
     Name,    
     EstimateCategoryID,    
     ParentEstimateCategoryID,    
     ParentEstimateCategory,    
     OriginalAmount,    
     RevisedAmount,    
     CostToDate,    
     (RevisedAmount - CostToDate) as Balance,    
     ParentSequence,    
     dbo.getCDPercentage(CostToDate,RevisedAmount) as Percentage,    
     Sequence    
    FROM (    
     SELECT     
      ISNULL(pl.Name,ec.Name) as Name,      
      pl.EstimateCategoryID,    
      pl.ParentEstimateCategoryID,     
      (Select TOP 1 Name from ProposalLines where ProposalId = @ProposalId and EstimateCategoryId = pl.ParentEstimateCategoryID) as ParentEstimateCategory,    
      pl.Amount as OriginalAmount,    
      ISNULL((SELECT TOP 1 Amount from ProposalLinesHistory ph     
       WHERE ph.ProposalID = @ProposalId and ph.EstimateCategoryID = pl.EstimateCategoryID     
       AND ph.ChangeType = 'Updated'    
       ORDER BY ChangeDate DESC),pl.Amount) as RevisedAmount,    
      (SELECT CONVERT(DECIMAL(18,2), SUM(Amount))     
       FROM @Transactions t     
       WHERE t.EstimateCategoryId = pl.EstimateCategoryID and AccountType <> 'Income') as CostToDate,    
     (Select TOP 1 Sequence from ProposalLines where ProposalId = @ProposalId and EstimateCategoryId = pl.ParentEstimateCategoryID) as ParentSequence,    
      pl.Sequence as Sequence    
    
     from Proposals p    
     inner join ProposalLines pl    
     on p.ID = pl.ProposalID    
     LEFT join EstimateCategories ec    
     on ec.ID = pl.EstimateCategoryID    
     where p.ID = @ProposalId   
     and pl.ParentEstimateCategoryID is not null
     ) sq1  

    SET @TotalCostToDate = (SELECT SUM(ISNULL(CostToDate,0)) from @RevisedEstimates)  
    SET @OwnerDeposits = (SELECT ISNULL(SUM(ISNULL(AMOUNT,0)),0) from @Transactions WHERE AccountType in ('Income','CostofGoodsSold'))  
    SET @JobBalance = @OwnerDeposits - @TotalCostToDate  
    SELECT TOP 1 @InitialDeposit = ISNULL(Amount,0)
	    FROM  @Transactions WHERE AccountType = 'Income'
	    and Amount % 5000 = 0;

    If @CurrentJobBalance > @InitialDeposit
        SET @RequestedAmount = 0;
    ELSE
        SET @RequestedAmount = @InitialDeposit;

    DELETE FROM ProjectTotals where ProjectId = @QBClassId;
    INSERT INTO ProjectTotals
    SELECT NEWID(), @QbClassId, ISNULL(@RequestedAmount,0), ISNULL(@TotalCostToDate,0), ISNULL(@OwnerDeposits,0), GETDATE()
END TRY
BEGIN CATCH
	PRINT CONCAT('ERROR ON ', @QbClassId)
END CATCH
END