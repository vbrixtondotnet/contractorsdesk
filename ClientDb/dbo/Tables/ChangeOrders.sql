CREATE TABLE [dbo].[ChangeOrders]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ActionItemId] INT NOT NULL, 
    [CostChangeName] NVARCHAR(100) NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [CurrentAmount] DECIMAL(18, 2) NULL, 
    [NewAmount] DECIMAL(18, 2) NULL, 
    [ScheduleChangeItem] NVARCHAR(100) NULL, 
    [NoOfDays] INT NULL, 
    [ChangeOrderNumber] INT NULL, 
    CONSTRAINT [FK_ChangeOrders_ActionItems] FOREIGN KEY ([ActionItemId]) REFERENCES [ActionItems]([Id])
)
