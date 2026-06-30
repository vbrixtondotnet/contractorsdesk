CREATE   PROCEDURE [dbo].[spMonthlyBudgetReport_Test_2]   
    @StartDate DATE,  
    @EndDate DATE,  
    @Class NVARCHAR(MAX)  
AS  
BEGIN     
  
    IF 'All' IN (SELECT value FROM STRING_SPLIT(@Class,'|'))  
    SET @Class = NULL  
  
 DECLARE @InitialDeposit DECIMAL(21,9);  
 DECLARE @CurrentJobBalance DECIMAL(21,9);  
 DECLARE @RequestedAmount DECIMAL(21,9);  
 DECLARE @Actual TABLE  
    (  
        ItemId uniqueidentifier, 
		Name nvarchar(200),
        TotalToDate DECIMAL(21,9)  
    )  
  
DECLARE @Estimate TABLE  
(  
    ID uniqueidentifier,
    Estimate DECIMAL(21,9)  
)  
  
DECLARE @RevisedEstimate TABLE  
(  
    ID uniqueidentifier,  
    RevisedEstimate DECIMAL(21,9)  
)  
  
DECLARE @EarliestHistoryAmount TABLE  
(  
    EstimateCategoryID NVARCHAR(255),  
    ParentEstimateCategoryID NVARCHAR(255),  
    ProposalID NVARCHAR(255),  
    ProposalLineID NVARCHAR(255),  
EarliestChangeDate DATETIME2,  
    Estimate DECIMAL(21,9)  
)  
  
DECLARE @Merge TABLE  
(  
	ID NVARCHAR(255),
    Name NVARCHAR(255),   
    TotalToDate DECIMAL(21,9),  
	Estimate DECIMAL(21,9),  
	RevisedEstimate DECIMAL(21,9)
)  
  
DECLARE @QBClass TABLE  
(  
    QBClass NVARCHAR(255),  
	QBAccountID NVARCHAR(255),  
    TotalToDate DECIMAL(21,9)  
);   
  
 DECLARE @OpenJobs TABLE  
 (  
  AccountType NVARCHAR(50),  
  JobBalance DECIMAL(21,2)  
 )  
  
 DECLARE @GrossProfit TABLE  
 (  
  JobBalance DECIMAL(21,2)  
 )  
  
 DECLARE @NetOperatingIncome TABLE  
 (  
  JobBalance DECIMAL(21,2)  
 )  
  
 DECLARE @NetIncome TABLE  
 (  
  JobBalance DECIMAL(21,2)  
 )  
  
 DECLARE @OwnerOposit TABLE  
 (  
  Amount DECIMAL(21,2)  
 )  
    
 INSERT @OpenJobs  
  SELECT AccountType, JobBalance  
  FROM(  
    SELECT c.FullyQualifiedName AS OpenConstructionJobs, a.AccountType, SUM(Amount) AS JobBalance  
    FROM QBAccounts a  
    FULL JOIN QBTransactions t ON a.ID = t.AccountID  
    INNER JOIN QBClasses c ON c.ID = t.ClassID  
    WHERE AccountType IN ('Income','CostofGoodsSold','Expense','OtherIncome', 'OtherExpense') AND c.OpenJob = 1  
    AND t.TransactionDate BETWEEN @StartDate AND @EndDate  
    GROUP BY c.FullyQualifiedName, a.AccountType  
   )j   
  WHERE OpenConstructionJobs IN (SELECT CASE WHEN @Class IS NULL THEN OpenConstructionJobs  ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))  
  ORDER BY OpenConstructionJobs   
  
 INSERT @GrossProfit  
  SELECT SUM(JobBalance) AS JobBalance  
  FROM(  
    SELECT JobBalance  
    FROM @OpenJobs  
    WHERE AccountType = 'Income'  
    UNION ALL  
    SELECT -JobBalance  
    FROM @OpenJobs  
    WHERE AccountType = 'CostofGoodsSold'  
   ) gp  
  
 INSERT @NetOperatingIncome  
  SELECT SUM(JobBalance) AS JobBalance  
  FROM(    
    SELECT JobBalance  
    FROM @GrossProfit  
    UNION ALL  
    SELECT -JobBalance  
    FROM @OpenJobs  
    WHERE AccountType = 'Expense'  
   ) noi  
  
 INSERT @NetIncome  
  SELECT SUM(JobBalance) AS JobBalance  
  FROM(    
    SELECT JobBalance  
    FROM @NetOperatingIncome  
    UNION ALL  
    SELECT JobBalance  
    FROM @OpenJobs  
    WHERE AccountType = 'OtherIncome'  
    UNION ALL  
    SELECT -JobBalance  
    FROM @OpenJobs  
    WHERE AccountType = 'OtherExpense'  
   ) ni  
  
 INSERT INTO @OwnerOposit  
  SELECT Amount FROM QBAccounts a  
  INNER JOIN QBTransactions t on t.AccountID = a.ID  
  INNER JOIN QBClasses c on c.id = t.ClassID  
 -- WHERE a.FullyQualifiedName like 'Owner Deposit'   
        WHERE a.AccountType = 'Income'  
    --    OR a.FullyQualifiedName LIKE 'Owner Deposit'  
    --      OR a.FullyQualifiedName LIKE 'CH Holding loan'  
    --      OR a.FullyQualifiedName LIKE 'CHA Loan - Income'  
    --    OR a.FullyQualifiedName LIKE 'Construction Loan - Trent'  
  AND c.FullyQualifiedName IN (SELECT CASE WHEN @Class IS NULL THEN c.FullyQualifiedName  ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))  
  AND t.TransactionDate BETWEEN @StartDate AND @EndDate  
  
 SELECT TOP 1 @InitialDeposit = Amount  
 FROM @OwnerOposit  
 WHERE Amount % 5000 = 0;  
  
 -- Get the Current Job Balance  
 SELECT @CurrentJobBalance = JobBalance FROM @NetIncome;  
  
 -- Additional logic for RequestedAmount  
 -- IF @InitialDeposit * 0.2 > @CurrentJobBalance  
 --  SET @RequestedAmount = 0;  
 -- ELSE  
 --  SET @RequestedAmount = @InitialDeposit;  
  
    If @CurrentJobBalance > @InitialDeposit * 0.2 -- N.B. E kam ndryshuar ne 0.2 nga 2  
     SET @RequestedAmount = 0;  
    ELSE  
        SET @RequestedAmount = @InitialDeposit;  
 
  
 INSERT INTO  @QBClass  
    SELECT QBClass, AccountID, TotalToDate  
 FROM(  
   SELECT C.FullyQualifiedName AS QBClass, t.AccountID,   
   SUM(Amount) AS TotalToDate  
   FROM QBAccounts a  
   INNER JOIN QBTransactions t ON t.AccountID = a.ID   
   INNER JOIN QBClasses c ON c.ID = t.ClassID  
   --INNER JOIN #SourceBankAccount tr ON tr.TxnID = t.TxnID AND t.Name = tr.Name AND tr.Date = t.TransactionDate  
   WHERE  TransactionDate between @StartDate AND @EndDate AND c.AllowedForBudgetReports = 1  
   GROUP BY C.FullyQualifiedName, t.AccountID  
  ) t  
 WHERE QBClass IN (SELECT CASE WHEN @Class IS NULL THEN QBClass ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))  
  
 INSERT @Actual  
  SELECT 
	ItemId,   
	 T.Name as Name,
     SUM(TotalToDate) AS TotalToDate  
     FROM (  
    SELECT EM.ID as ItemId, EM.Name as Name, c.TotalToDate
    FROM EstimateMappings E  
    INNER JOIN EstimateCategories EM ON EM.ID = E.EstimateSubCategoryID  
    INNER JOIN @QBClass c ON c.QBAccountID = E.QBAccountID  
    WHERE   
     (EM.Name <> 'Other' OR (EM.Name = 'Other' AND E.AccountType IN ('Expenses', 'Other Expense')))  
       )t  
  GROUP BY ItemId, T.Name
  
 INSERT @EarliestHistoryAmount  
    SELECT   
        PLH.EstimateCategoryID,  
        PLH.ParentEstimateCategoryID,  
        PLH.ProposalID,  
  PLH.ProposalLineID,  
        MIN(PLH.ChangeDate) AS EarliestChangeDate,  
  Amount  
    FROM   
        ProposalLinesHistory PLH  
 WHERE ChangeType <> 'DocStatusChanged(Accepted)'  
 AND ChangeDate = (SELECT MIN(ChangeDate)  
        FROM ProposalLinesHistory  
        WHERE EstimateCategoryID = PLH.EstimateCategoryID  
       AND ParentEstimateCategoryID = PLH.ParentEstimateCategoryID   
       AND ProposalID = PLH.ProposalID)  
    GROUP BY   
        PLH.EstimateCategoryID,  
        PLH.ParentEstimateCategoryID,  
        PLH.ProposalID,  
  PLH.ProposalLineID,  
  Amount  
  
 INSERT @Estimate  
        SELECT ID, SUM(Estimate) AS Amount  
  FROM(  
    SELECT 
		EC.ID, 
		CASE   
			WHEN P.DocStatus = 'Accepted' AND EH.ProposalLineID = pl.ID THEN EH.Estimate  
			ELSE PL.Amount  
		END AS Estimate  
    FROM Proposals P  
    INNER JOIN ProposalLines PL ON PL.ProposalID = P.ID  
    INNER JOIN EstimateCategories EC ON EC.ID = PL.EstimateCategoryID   
     AND EC.ParentEstimateCategoryID = PL.ParentEstimateCategoryID  
    LEFT JOIN @EarliestHistoryAmount EH ON EH.EstimateCategoryID = EC.ID  
     AND EH.ParentEstimateCategoryID = EC.ParentEstimateCategoryID  
     AND EH.ProposalID = PL.ProposalID   
     AND EH.ProposalLineID = PL.ID  
    INNER JOIN QBClasses C ON C.ID = P.QBClassID   
    WHERE c.AllowedForBudgetReports = 1 AND Amount <> 0  
    AND C.FullyQualifiedName IN (SELECT CASE WHEN @Class IS NULL THEN C.FullyQualifiedName ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))      
  ) t  
  GROUP BY ID  
  
 INSERT @RevisedEstimate  
        SELECT EC.ID, SUM(PE.Amount) AS Estimate  
        FROM Proposals P  
  INNER JOIN ProposalLines PE ON PE.ProposalID = P.ID  
  INNER JOIN EstimateCategories EC ON EC.ID = PE.EstimateCategoryID  
  INNER JOIN QBClasses C ON C.ID = P.QBClassId   
  WHERE c.AllowedForBudgetReports = 1 AND Amount <> 0  
  AND C.FullyQualifiedName IN (SELECT CASE WHEN @Class IS NULL THEN C.FullyQualifiedName ELSE value END FROM STRING_SPLIT(ISNULL(@Class, ''),'|'))  
  GROUP BY EC.ID
  
    
 INSERT INTO @Merge (ID, Name, TotalToDate, Estimate, RevisedEstimate)  
 SELECT   
  E.ID,  
  T.Name,
  ISNULL(T.TotalToDate, 0) AS TotalToDate,   
  ISNULL(T.Estimate, 0) AS Estimate,  
  ISNULL(T.RevisedEstimate, 0) AS RevisedEstimate
 FROM (  
  SELECT   
   COALESCE(A.ItemId, E.ID, RE.ID) AS ID,   
   A.Name,
   SUM(ISNULL(A.TotalToDate, 0)) AS TotalToDate,   
   SUM(ISNULL(E.Estimate, 0)) AS Estimate,  
   SUM(ISNULL(RE.RevisedEstimate, 0)) AS RevisedEstimate  
  FROM   
   @Actual A  
   FULL JOIN @Estimate E ON E.ID = A.ItemId
   FULL JOIN @RevisedEstimate RE ON RE.ID = A.ItemId
  GROUP BY   
   COALESCE(A.ItemId, E.ID, RE.ID),
   A.Name
 ) T  
 INNER JOIN EstimateCategories E ON E.ID = T.ID

	select * from @Merge
	UNION ALL
	SELECT '', 'OWNER DEPOSITS', SUM(Amount), NULL, NULL FROM @OwnerOposit  
	UNION ALL
	SELECT '', 'JOB BALANCE', SUM(JobBalance), NULL, NULL FROM @NetIncome  
	UNION ALL
	SELECT '', 'REQUESTED AMOUNT', @RequestedAmount, NULL, NULL

END