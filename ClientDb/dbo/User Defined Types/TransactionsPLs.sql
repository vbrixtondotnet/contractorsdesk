CREATE TYPE [dbo].[TransactionsPLs] AS TABLE (
    [AccountID]     NVARCHAR (255)  NULL,
    [CommitmentUSD] DECIMAL (21, 9) NULL,
    [CommitmentEUR] DECIMAL (21, 9) NULL,
    [ExpensesUSD]   DECIMAL (21, 9) NULL,
    [ExpensesEUR]   DECIMAL (21, 9) NULL);

