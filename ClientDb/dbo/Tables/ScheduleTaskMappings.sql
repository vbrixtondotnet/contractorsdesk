CREATE TABLE [dbo].[ScheduleTaskMappings](
	[Id] [uniqueidentifier] NOT NULL,
	[EstimateCategoryId] [uniqueidentifier] NOT NULL,
	[ConstructionTaskId] [uniqueidentifier] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedBy] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ScheduleTaskMappings] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ScheduleTaskMappings]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTaskMappings_ConstructionTasks] FOREIGN KEY([ConstructionTaskId])
REFERENCES [dbo].[ConstructionTasks] ([ID])
GO
ALTER TABLE [dbo].[ScheduleTaskMappings] CHECK CONSTRAINT [FK_ScheduleTaskMappings_ConstructionTasks]
GO
ALTER TABLE [dbo].[ScheduleTaskMappings]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTaskMappings_EstimateCategories] FOREIGN KEY([EstimateCategoryId])
REFERENCES [dbo].[EstimateCategories] ([ID])
GO
ALTER TABLE [dbo].[ScheduleTaskMappings] CHECK CONSTRAINT [FK_ScheduleTaskMappings_EstimateCategories]
GO