CREATE TABLE [dbo].[CostRevisionItems]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[CostRevisionId] uniqueidentifier NOT NULL,
	[EstimateCategoryId] uniqueidentifier NOT NULL,
	[Amount] decimal(18,2) NOT NULL,
	[CurrentAmount] decimal(18,2) NOT NULL,
	[NewAmount] decimal(18,2) NOT NULL,	
    CONSTRAINT [FK_CostRevisions_CostRevisionItems] FOREIGN KEY ([CostRevisionId]) REFERENCES [CostRevisions]([Id])
)
