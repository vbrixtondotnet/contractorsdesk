
CREATE TABLE [dbo].[ProposalClients](
	[Id] [uniqueidentifier] NOT NULL,
	[ProposalId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[Address] [nvarchar](250) NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
 CONSTRAINT [PK_ProposalClients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProposalClients]  WITH CHECK ADD  CONSTRAINT [FK_ProposalClients_Proposals] FOREIGN KEY([ProposalId])
REFERENCES [dbo].[Proposals] ([ID])
GO

ALTER TABLE [dbo].[ProposalClients] CHECK CONSTRAINT [FK_ProposalClients_Proposals]
GO


CREATE TABLE [dbo].[ProposalProjectDetails](
	[Id] [uniqueidentifier] NOT NULL,
	[ProposalId] [uniqueidentifier] NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](250) NULL,
	[Desciption] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProposalProjectDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProposalProjectDetails]  WITH CHECK ADD  CONSTRAINT [FK_ProposalProjectDetails_Proposals] FOREIGN KEY([ProposalId])
REFERENCES [dbo].[Proposals] ([ID])
GO

ALTER TABLE [dbo].[ProposalProjectDetails] CHECK CONSTRAINT [FK_ProposalProjectDetails_Proposals]
GO

