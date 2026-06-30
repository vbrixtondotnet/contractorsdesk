CREATE TABLE [dbo].[ActionItemCostChange]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ActionItemId] INT NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [EstimateCategoryId] UNIQUEIDENTIFIER NULL, 
    [RequiresClientApproval] BIT NULL, 
    CONSTRAINT [FK_ActionItemCostChange_ActionItem] FOREIGN KEY (ActionItemId) REFERENCES [ActionItems]([Id])
)
