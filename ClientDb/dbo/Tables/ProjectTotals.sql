CREATE TABLE [dbo].[ProjectTotals]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ProjectId] UNIQUEIDENTIFIER NULL, 
    [MinimumRequestedAmount] DECIMAL(18, 2) NULL, 
    [CostToDate] DECIMAL(18, 2) NOT NULL,
    [OwnerDeposits] DECIMAL(18, 2) NOT NULL,
    [DateUpdated] DATETIME2 NOT NULL,

)
