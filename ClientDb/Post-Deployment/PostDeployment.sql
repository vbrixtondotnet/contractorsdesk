-- SysDataSyncSetting
IF NOT EXISTS (SELECT 1 FROM [dbo].[SysDataSyncSetting] where [DataSyncName] = 'Quickbooks Sync')
BEGIN
INSERT [dbo].[SysDataSyncSetting] ([Id], [DataSyncName], [IsFirstRun], [NumberOfDaysLookup], [DateLastRun], [DateNextRun], [DateCreated], [DateModified])
VALUES (NEWID(), N'Quickbooks Sync', 1, 5, NULL, NULL, GETDATE(), GETDATE())
END

--SYS FOLDERS
IF NOT EXISTS (SELECT 1 FROM [dbo].[SysFolders])
BEGIN

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'ccfe115c-870d-406a-8bd7-1126c9723a25', N'Job Summary', N'1315551d-10fe-49ef-ac84-58f1e6f09f89', 19)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'27ab83f1-5bcd-44a7-ae08-3987c1147094', N'Emails', NULL, 6)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'49b9472b-1274-4e50-a585-40b9803178a5', N'Miscellaneous Files', NULL, 10)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'bfee192e-4877-4e72-8c76-4cd596d47b33', N'Plans', N'51599f25-4d5c-43f9-92f3-895e436fefa8',13)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'1315551d-10fe-49ef-ac84-58f1e6f09f89', N'Reports', NULL, 9)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'40153b21-6d41-4ae8-96d1-5d01fc3eb73e', N'Complete Audio Files', N'0ac181a8-d39b-4f41-8289-a2fdc09afb64',16)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'ebb3df6b-d1b0-4dd5-a472-7d7da2173b12', N'Estimates', NULL, 2)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'9b6b815b-d081-49fd-a217-7fe1feab1841', N'Proposals', N'ebb3df6b-d1b0-4dd5-a472-7d7da2173b12',11)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'6e5d70ab-0f2d-4faa-b74a-821e1c41bb33', N'Change Orders', NULL, 4)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'51599f25-4d5c-43f9-92f3-895e436fefa8', N'Plans and Permits', NULL, 7)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'5c44fe01-a758-4ba3-b32e-947d08a72ad8', N'AI Action Items By Date', N'0ac181a8-d39b-4f41-8289-a2fdc09afb64',14)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'0ac181a8-d39b-4f41-8289-a2fdc09afb64', N'AI Action Items', NULL, 8)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'333bc95b-659c-4245-b6fe-b2c40653f99e', N'Contracts', NULL, 1)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'34657b3e-fb9c-431a-8993-c1062b87f290', N'Revised Estimates', N'ebb3df6b-d1b0-4dd5-a472-7d7da2173b12', 12)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'fa1b5f75-9625-4acf-817d-cf5d5187c1b0', N'Permits', N'51599f25-4d5c-43f9-92f3-895e436fefa8',14)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'4b5e550d-66b5-4a18-b858-cf73c22bf34e', N'Invoices', NULL, 3)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'52bed3cb-40a0-4209-9beb-d4160a7b14c9', N'Schedule', NULL, 5)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'14fb66ca-a9ed-40f8-b90f-d82e78a1bfe8', N'Complete Transcripts', N'0ac181a8-d39b-4f41-8289-a2fdc09afb64',15)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'446c52e6-9ccf-40c4-aa3a-db237a667785', N'Estimate To Actual', N'1315551d-10fe-49ef-ac84-58f1e6f09f89',18)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'5b316eb8-6ad9-4eee-9869-a583d1f5af6c', N'Client Emails', N'27ab83f1-5bcd-44a7-ae08-3987c1147094',19)

INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'9aba22b9-b130-4eb2-8d20-12dba2d5caac', N'Status Reports', N'27ab83f1-5bcd-44a7-ae08-3987c1147094',20)

END

-- NEW SYS FOLDERS
IF NOT EXISTS (SELECT 1 FROM [dbo].[SysFolders] where [Name] = 'Owner')
BEGIN
INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES ('09925665-2954-4ABA-BD61-0A22E7701721', N'Owner', NULL, 21)
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[SysFolders] where [Name] = 'Sub-Contractors')
BEGIN
INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES ('DE9CBD8A-C1BF-4A46-9B5D-D9BB4C890810', N'Sub-Contractors', NULL, 22)
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[SysFolders] where [Name] = 'Cost Revisions')
BEGIN
INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'280fa097-a537-456b-a63b-b9f18cf91591', N'Cost Revisions', NULL,23)
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[SysFolders] where [Name] = 'Schedule Revisions')
BEGIN
INSERT [dbo].[SysFolders] ([Id], [Name], [ParentId], [Sequence]) VALUES (N'26cc09bc-d9cf-4654-82f8-0e4a6090f254', N'Schedule Revisions', NULL,24)
END

--PERMISSIONS
SET IDENTITY_INSERT [dbo].[Permissions] ON; 
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage All Jobs')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (1, 2, N'Manage All Jobs')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Own Jobs')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (2, 2, N'Manage Own Jobs')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage All Estimates')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (3, 2, N'Manage All Estimates')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Own Estimates')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (4, 2, N'Manage Own Estimates')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage All Action Items')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (5, 2, N'Manage All Action Items')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Own Action Items')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (6, 2, N'Manage Own Action Items')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Company Users')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (7, 2, N'Manage Company Users')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Company Roles')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (8, 2, N'Manage Company Roles')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Schedules')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (9, 2, N'Manage Schedules')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Finance')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (10, 2, N'Manage Finance')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Client Websites')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (11, 2, N'Manage Client Websites')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Company Email')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (12, 2, N'Manage Company Email')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Company SEO')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (13, 2, N'Manage Company SEO')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Client Onboarding')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (14, 2, N'Manage Client Onboarding')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Access to Third-party Services')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (15, 2, N'Access to Third-party Services')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage System Users')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (16, 1, N'Manage System Users')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage System Roles')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (17, 1, N'Manage System Roles')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Account Owners')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (18, 1, N'Manage Account Owners')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage System Permissions')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (19, 1, N'Manage System Permissions')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Client Websites')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (20, 1, N'Manage Client Websites')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Client Onboarding')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (21, 1, N'Manage Client Onboarding')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Access to Third-party Services')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (22, 1, N'Access to Third-party Services')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Company Email')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (23, 1, N'Manage Company Email')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Company SEO')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (24, 1, N'Manage Company SEO')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Manage Clients')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (25, 1, N'Manage Clients')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Assign Jobs')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (26, 2, N'Can Assign Jobs')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Assign Estimates')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (27, 2, N'Can Assign Estimates')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Assign Action Items')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (28, 2, N'Can Assign Action Items')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Access Client Jobs')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (29, 2, N'Can Access Client Jobs')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Manage Data Mapping')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (30, 2, N'Can Manage Data Mapping')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Edit Accepted Proposals')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (32, 2, N'Can Edit Accepted Proposals')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Access Company Settings')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (33, 2, N'Can Access Company Settings')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] where Description = 'Can Access Data Sync Services')
BEGIN
INSERT [dbo].[Permissions] ([Id], [PermissionType], [Description]) VALUES (34, 2, N'Can Access Data Sync Services')
END

SET IDENTITY_INSERT [dbo].[Permissions] OFF


--ROLES
SET IDENTITY_INSERT [dbo].[Roles] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Super Admin')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name]) VALUES (1, 1, N'Super Admin')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Customer Support')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name])VALUES (2, 1, N'Customer Support')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Super IT')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name]) VALUES (3, 1, N'Super IT')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Company Owner')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name])VALUES (4, 2, N'Company Owner')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Project Manager')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name])VALUES (5, 2, N'Project Manager')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Assistant Project Manager')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name]) VALUES (6, 2, N'Assistant Project Manager')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Bookkeeper')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name]) VALUES (7, 2, N'Bookkeeper')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Company IT')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name]) VALUES (8, 2, N'Company IT')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Office Manager')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name])VALUES (9, 2, N'Office Manager')
END
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] where Name = 'Client')
BEGIN
INSERT [dbo].[Roles] ([Id], [RoleType], [Name]) VALUES (10, 2, N'Client')
END
SET IDENTITY_INSERT [dbo].[Roles] OFF

--ROLE PERMISSIONS

IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions])
BEGIN
SET IDENTITY_INSERT [dbo].[RolePermissions] ON 
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (19, 2, 6)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (20, 4, 6)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (21, 6, 6)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (22, 9, 6)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (23, 10, 7)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (24, 14, 7)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (25, 15, 7)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (31, 1, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (32, 3, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (33, 5, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (34, 7, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (35, 9, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (36, 26, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (37, 27, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (38, 28, 9)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (39, 29, 10)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (40, 11, 8)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (41, 12, 8)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (42, 13, 8)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (43, 14, 8)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (44, 15, 8)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (45, 30, 8)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (57, 1, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (58, 3, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (59, 5, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (60, 7, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (61, 8, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (62, 9, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (63, 10, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (64, 26, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (65, 27, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (66, 28, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (67, 30, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (68, 32, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (69, 2, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (70, 4, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (71, 6, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (72, 9, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (73, 10, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (74, 26, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (75, 27, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (76, 28, 5)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (77, 33, 4)
INSERT [dbo].[RolePermissions] ([Id], [PermissionId], [RoleId]) VALUES (78, 34, 4)
SET IDENTITY_INSERT [dbo].[RolePermissions] OFF
END

-- USERS
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users])
BEGIN
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [Password], [RoleId], CreatedBy, DateCreated) VALUES (1, N'System', N'Admin', 'sa@contractors-desk.com', 'AQAAAAIAAYagAAAAEPrBeJbWat5hmOzPTu5CT0Uoc03dqWDQktXIpHLT135Ri6YIi6lFdqU0jJjlzBmkcQ==', 1, 1, '03/20/2025');
INSERT [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [Password], [RoleId], CreatedBy, DateCreated) VALUES (2, N'Company', N'Owner', 'co@contractors-desk.com', 'AQAAAAIAAYagAAAAEPrBeJbWat5hmOzPTu5CT0Uoc03dqWDQktXIpHLT135Ri6YIi6lFdqU0jJjlzBmkcQ==', 4, 1, '03/20/2025');
SET IDENTITY_INSERT [dbo].[Users] OFF
END

-- ESTIMATE CATEGORIES
IF NOT EXISTS (SELECT 1 FROM [dbo].EstimateCategories)
BEGIN
INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'825800a8-00cd-4ac2-5865-08dce19eb8a0', N'Waterproofing', 71, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c27bde33-8f87-4a7b-5866-08dce19eb8a0', N'Hardscape', 74, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'107c1dd9-9c5e-4e82-5867-08dce19eb8a0', N'Scaffolding', 57, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ba366bd1-6213-4034-5868-08dce19eb8a0', N'Exterior and Interior Railing', 34, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Per Plans')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'467b86dd-a9ef-4965-5869-08dce19eb8a0', N'Shower Pan', 60, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Shower Waterproofing')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e7f27724-e523-4c3b-586a-08dce19eb8a0', N'Garage Floor', 43, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Level and Epoxy Floor')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'0997663d-ed47-4eab-586b-08dce19eb8a0', N'Window and Door Install', 70, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Per Plans')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1b99337e-a152-477d-37dd-08dcefc578ae', N'Water Heater', 75, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Upgrade')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'fd41b3fa-df36-4de1-37de-08dcefc578ae', N'Sauna', 76, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'43f6c83e-44fc-4d9f-37df-08dcefc578ae', N'Epoxy Garage Floor', 77, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a09f00bb-5e94-463b-37e0-08dcefc578ae', N'Backfill', 40, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'5671cd57-d2ad-40a6-37e1-08dcefc578ae', N'Panel upgrade', 99, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7cd6334b-653d-4537-37e2-08dcefc578ae', N'Excavation', 25, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'585030d5-0300-421e-37e3-08dcefc578ae', N'Stain Contractor', 77, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'11fb7a0e-d6af-4952-37e4-08dcefc578ae', N'Electrical Panel Upgrade', 81, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'86abef50-ad0b-4448-7441-08dcf1e6bcdd', N'On Site Supervision', 12, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'676d782f-201a-4a5f-a15f-1341599130d1', N'Hot Mop Contractor', 46, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c2682aa6-a285-413a-a081-13eb66499965', N'Interior Design', 48, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1f86bc77-e947-44dc-a4ed-1591f87626ee', N'Countertop Fabricator', 27, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'Site Work', 4, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7eeadbe1-d848-4e07-9aec-2f4e7d3979bd', N'Interest Expense', 82, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e1903443-4536-4d24-8507-32d8fbfc5903', N'Fire Sprinklers Contractor', 37, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'b24bacd8-edc7-4878-8aa4-37c5724237f2', N'Deck Contractor', 28, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'cdd4afab-20d3-40b2-b53f-3e31773f729e', N'Tile Material', 35, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd31868a9-eb0a-4202-a7d3-4396f5e0b741', N'Plumbing Fixtures', 34, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'980d37ec-9ede-45fa-8a9d-451d218a4182', N'Curbs & Gutter Contractor', 66, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'b0b192e0-0cc8-42ac-96cc-4a77adca09e5', N'Interior doors/Windows', 10, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'daad3d7b-4d42-4f22-ae1a-5514d88b2600', N'Plans/survey', 1, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a5da1985-1858-49e8-abb8-558efc382aac', N'Overhead', 301, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'45a886d9-f473-41c2-9359-55f9b968bddc', N'Trash Hauling', 68, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e8c89fac-0014-438e-89ba-56c89026aeb4', N'Stairs Contractor', 65, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ab4c749c-4a10-4eb9-9779-575a60307f1e', N'Asbestos Removal', 8, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'df7acf8b-9156-4cf8-a3d2-589a7953466e', N'Interior doors/Hardware', 32, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'3c32d1de-a305-4f38-b22b-5b5fe0c3ea04', N'Finish Hardware', 27, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide: Cabinet Hardware')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'28be9922-a0d4-44ca-8393-5e603cd0610f', N'Wine Storage Contractor', 69, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'6504c427-be01-44e4-ab3c-5ea2a6dbb7f1', N'Other', 83, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ff3c03b9-d3bf-4b56-b2be-6326bc1d8459', N'Solar Contractor', 63, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1594908e-cb37-428d-b3d8-640a683960b9', N'General Contractor Fee', 78, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'117ebb33-6930-4e8c-8230-686c8da61688', N'Painting Contractor', 53, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'536b2c08-b239-463c-8f46-69dfa835ddb2', N'Low Voltage Contractor', 51, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Priced for Prewire. Automated shades?, Security? Cable and Internet, speakers. ')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ba5c7b3a-b441-4958-857d-6a0d940cf118', N'Bath accessories', 19, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide: Towel Bars, Mirrors, Toilet paper holders, Hardware')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'Preparation', 1, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'b325dd9d-d055-40a6-9139-75eb7b19c7db', N'Lumber/Hardware', 33, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd3bd6589-552b-464b-ae4d-76aeb609de11', N'External Railings Contractor', 35, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7bace765-f941-4c33-a797-79de17303864', N'Exterior Doors/Hardware', 32, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd51655e7-3060-454d-930a-87f47b064312', N'Cabinet Contractor', 25, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a5533127-aea0-48de-82af-8886827fc1c5', N'Site Drainage', 71, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'65421aa4-89fe-412d-8d42-8e9ac05c1420', N'Sheet metal', 58, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'740aa1d7-7113-43ed-ab01-902eb8e9e603', N'Precast Contractor', 55, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'0115d846-31a3-405e-8c23-92906df2ef77', N'Elevator Contractor', 31, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'47269347-037d-41db-bf70-93d49c36a275', N'Internal Railings Contractor', 50, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'92883fec-dfdf-4bf3-a033-94f129c42272', N'Countertop Material', 21, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide. Supplier Moda in San Clemente, Deniz- 949-244-4839')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'175a03b2-565e-4eb3-a5d7-9728befbe82e', N'Pool/Spa', 69, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'265a03b2-565e-4eb3-a5d7-9728befbe82e', N'Retaining Wall', 70, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'9b2bba0c-90bc-4892-a064-98d8d5838182', N'Flooring Material', 30, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'86c74224-0170-400c-bd74-99a4146229af', N'Landscape', 68, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'063d6c29-2430-40de-bb2b-9a83941120cd', N'Material Delivery', 52, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a1b01fd2-872e-4a6b-a834-9f230342a1c1', N'Insulation Contractor', 47, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'd2c47096-9af5-44d1-8cd2-a0ecee5a8c42', N'Structural Steel', 66, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'ae56314b-7ac7-49fe-a2e0-a53adbfc6332', N'Finish Material', 29, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'0a5e9f89-2d1d-40b9-8244-a65f94cc02a3', N'Temp Utilities', 7, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c5ff414e-8f3d-498b-bfc3-a9fc0bb7bd4b', N'Electrical Fixtures', 23, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide: Sconces, Pendants, Fans, Chandeliers')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'Sub Contractors', 300, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'cee94ee2-f3d6-4f63-8b63-ae0ff14313eb', N'Shower Doors Contractor', 59, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a1c3e88f-e7ec-46d1-adf8-b38997770c31', N'Foundation Contractor', 40, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'62022341-ec48-4087-aa56-b4d120100ca6', N'Demolition', 6, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'01282e22-a193-48be-8508-b6e4055459a2', N'General Contractor Contingency', 77, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'746abd5e-956d-459c-9a49-b73ba7d596a8', N'Flooring Contractor', 39, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'25b467dd-97f0-4816-b0f6-ba193a7acb51', N'Closets', 26, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'1bf7e623-8741-4bfb-a9f2-bd009d658ffc', N'Electrical Contractor', 30, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'82599839-ac72-4e9e-82ca-bdb53085c253', N'Drywall Contractor', 29, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'81fcb8c4-ce90-47e4-bb9b-c01643f4b80f', N'Special Inspections', 64, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7b180310-468e-4b59-9337-c2d84b23cb14', N'Building permits', 3, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'7023d8e9-6716-4ee1-9c51-c4041baaf865', N'Site Drainage Contractor', 62, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'edd0ee18-a505-4a74-8adf-c57517fae7f1', N'Front Door', 31, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'bae8de79-c74a-4be5-b3eb-cca2d535c2bf', N'BBQ Grill/Firepit', 64, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'e3ad07e4-1f16-4d2d-b305-d04fd0b1cbe7', N'Tile Contractor', 67, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'271aa5bc-f5b2-4f56-94fb-d37d7305da08', N'Appliances', 17, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'Owner to Provide')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'5b3a00a9-4fc6-42fc-9b77-d73fd784700a', N'Property Tax', 81, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'a44dbb0d-fc97-43d1-b12c-d744428a4df8', N'Grading', 4, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'af245b53-809c-4531-9f8b-dc7114b0f4b4', N'Garage Doors Contractor', 42, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'cfe6afff-4a8e-47ba-b24a-dff9e6b43e3e', N'Exterior Doors/Windows', 25, N'c00df57f-c09b-4967-bc93-e2330e2f303f', NULL, NULL, NULL, NULL, N'"Budget Freindly windows Andersen 100 or Milgard.  Doors- La Cantina higher end and Windor or Milgard for budget friendly."')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'Material', 2, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'986cbdc3-be93-406a-babc-e2d14ba882db', N'Fireplace  Contractor', 38, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'68b3fffd-d37f-481b-8303-e43a00a0799e', N'Roofing Contractor', 56, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'416fdc24-339a-499d-9bde-e4be5ef19ed9', N'Heating & Air Contractor', 45, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'075f528a-c636-4b6e-9cc7-e729d0c0ee9e', N'Framing Contractor', 41, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'c207c3cd-494c-462f-903c-e75f8c8a9707', N'Finish Carpentry', 36, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, N'Install: Finish Material, bath accessories, interior doors')

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'01a4dd1e-36bf-427e-8f28-e803351e48fe', N'Exterior Finish Contractor', 33, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'2929c384-45bd-494e-9e98-f0e140c99aa0', N'Temp Services', 72, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'6ecf6dbd-171f-4978-857e-f8545d997164', N'Shower Enclosure/Mirror Contractor', 61, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'6e525d26-883e-402d-94a4-f87d96b7cb52', N'Insurance', 5, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'9a0da22f-afd4-4813-b7a5-f94a5d37416b', N'Driveway', 67, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'21578f57-31ed-420f-bfc1-fd77098a2bb2', N'Gutters', 44, N'851efb5a-de4f-4380-a264-ab31bb3acecd', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[EstimateCategories] ([ID], [Name], [Sequence], [ParentEstimateCategoryID], [Created], [CreatedBy], [Updated], [UpdatedBy], [Description]) VALUES (N'f3832da8-ea6f-436e-9c99-ff65cd5b061e', N'Land', 80, N'a5da1985-1858-49e8-abb8-558efc382aac', NULL, NULL, NULL, NULL, NULL)

END

-- PROPOSAL TEMPLATES
IF NOT EXISTS (SELECT 1 FROM ProposalTemplates)
BEGIN
INSERT [dbo].[ProposalTemplates] ([Id], [Name], [IsDefault], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Ground-Up (Default)', 1, 1, NULL, CAST(N'2025-03-30T01:25:47.820' AS DateTime), NULL)
END

-- PROPOSAL TEMPLATE LINE ITEMS
IF NOT EXISTS (SELECT 1 FROM ProposalTemplatesLineItems)
BEGIN

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Preparation', NULL, NULL, NULL, 1, NULL, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Material', NULL, NULL, NULL, 2, NULL, N'c00df57f-c09b-4967-bc93-e2330e2f303f', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Sub Contractors', NULL, NULL, NULL, 3, NULL, N'851efb5a-de4f-4380-a264-ab31bb3acecd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Site Work', NULL, NULL, NULL, 4, NULL, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a5da1985-1858-49e8-abb8-558efc382aac', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Overhead', NULL, NULL, NULL, 5, NULL, N'a5da1985-1858-49e8-abb8-558efc382aac', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'825800a8-00cd-4ac2-5865-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Waterproofing', N'', NULL, NULL, 71, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'825800a8-00cd-4ac2-5865-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c27bde33-8f87-4a7b-5866-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Hardscape', N'', NULL, NULL, 74, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'c27bde33-8f87-4a7b-5866-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'107c1dd9-9c5e-4e82-5867-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Scaffolding', N'', NULL, NULL, 57, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'107c1dd9-9c5e-4e82-5867-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ba366bd1-6213-4034-5868-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior and Interior Railing', N'Per Plans', NULL, NULL, 34, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'ba366bd1-6213-4034-5868-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'467b86dd-a9ef-4965-5869-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Shower Pan', N'Shower Waterproofing', NULL, NULL, 60, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'467b86dd-a9ef-4965-5869-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e7f27724-e523-4c3b-586a-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Garage Floor', N'Level and Epoxy Floor', NULL, NULL, 43, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e7f27724-e523-4c3b-586a-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0997663d-ed47-4eab-586b-08dce19eb8a0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Window and Door Install', N'Per Plans', NULL, NULL, 70, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'0997663d-ed47-4eab-586b-08dce19eb8a0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1b99337e-a152-477d-37dd-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Water Heater', N'Upgrade', NULL, NULL, 75, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'1b99337e-a152-477d-37dd-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'fd41b3fa-df36-4de1-37de-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Sauna', N'', NULL, NULL, 76, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'fd41b3fa-df36-4de1-37de-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'43f6c83e-44fc-4d9f-37df-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Epoxy Garage Floor', N'', NULL, NULL, 77, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'43f6c83e-44fc-4d9f-37df-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a09f00bb-5e94-463b-37e0-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Backfill', N'', NULL, NULL, 40, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'a09f00bb-5e94-463b-37e0-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'5671cd57-d2ad-40a6-37e1-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Panel upgrade', N'', NULL, NULL, 99, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'5671cd57-d2ad-40a6-37e1-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7cd6334b-653d-4537-37e2-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Excavation', N'', NULL, NULL, 25, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'7cd6334b-653d-4537-37e2-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'585030d5-0300-421e-37e3-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Stain Contractor', N'', NULL, NULL, 77, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'585030d5-0300-421e-37e3-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'11fb7a0e-d6af-4952-37e4-08dcefc578ae', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Electrical Panel Upgrade', N'', NULL, NULL, 81, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'11fb7a0e-d6af-4952-37e4-08dcefc578ae', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'86abef50-ad0b-4448-7441-08dcf1e6bcdd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'On Site Supervision', N'', NULL, NULL, 12, N'a5da1985-1858-49e8-abb8-558efc382aac', N'86abef50-ad0b-4448-7441-08dcf1e6bcdd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'676d782f-201a-4a5f-a15f-1341599130d1', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Hot Mop Contractor', N'', NULL, NULL, 46, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'676d782f-201a-4a5f-a15f-1341599130d1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c2682aa6-a285-413a-a081-13eb66499965', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interior Design', N'', NULL, NULL, 48, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'c2682aa6-a285-413a-a081-13eb66499965', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1f86bc77-e947-44dc-a4ed-1591f87626ee', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Countertop Fabricator', N'', NULL, NULL, 27, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'1f86bc77-e947-44dc-a4ed-1591f87626ee', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'b68851c1-9bd1-4234-9bda-28490931a31f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Other', N'', NULL, NULL, 88, N'a5da1985-1858-49e8-abb8-558efc382aac', N'6504c427-be01-44e4-ab3c-5ea2a6dbb7f1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7eeadbe1-d848-4e07-9aec-2f4e7d3979bd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interest Expense', N'', NULL, NULL, 82, N'a5da1985-1858-49e8-abb8-558efc382aac', N'7eeadbe1-d848-4e07-9aec-2f4e7d3979bd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e1903443-4536-4d24-8507-32d8fbfc5903', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Fire Sprinklers Contractor', N'', NULL, NULL, 37, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e1903443-4536-4d24-8507-32d8fbfc5903', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'b24bacd8-edc7-4878-8aa4-37c5724237f2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Deck Contractor', N'', NULL, NULL, 28, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'b24bacd8-edc7-4878-8aa4-37c5724237f2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'cdd4afab-20d3-40b2-b53f-3e31773f729e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Tile Material', N'', NULL, NULL, 35, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'cdd4afab-20d3-40b2-b53f-3e31773f729e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0cb8b9c0-7e57-4ea1-a8da-429ace1c6107', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interior Doors/Hardware', N'', NULL, NULL, 49, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'df7acf8b-9156-4cf8-a3d2-589a7953466e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd31868a9-eb0a-4202-a7d3-4396f5e0b741', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Plumbing Fixtures', N'', NULL, NULL, 34, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'd31868a9-eb0a-4202-a7d3-4396f5e0b741', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'980d37ec-9ede-45fa-8a9d-451d218a4182', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Curbs & Gutter Contractor', N'', NULL, NULL, 66, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'980d37ec-9ede-45fa-8a9d-451d218a4182', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'daad3d7b-4d42-4f22-ae1a-5514d88b2600', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Plans/survey', N'', NULL, NULL, 1, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'daad3d7b-4d42-4f22-ae1a-5514d88b2600', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'45a886d9-f473-41c2-9359-55f9b968bddc', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Trash Hauling', N'', NULL, NULL, 68, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'45a886d9-f473-41c2-9359-55f9b968bddc', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e8c89fac-0014-438e-89ba-56c89026aeb4', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Stairs Contractor', N'', NULL, NULL, 65, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e8c89fac-0014-438e-89ba-56c89026aeb4', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ab4c749c-4a10-4eb9-9779-575a60307f1e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Asbestos Removal', N'', NULL, NULL, 8, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'ab4c749c-4a10-4eb9-9779-575a60307f1e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'df7acf8b-9156-4cf8-a3d2-589a7953466e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Interior doors/Windows', N'', NULL, NULL, 32, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'b0b192e0-0cc8-42ac-96cc-4a77adca09e5', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'3c32d1de-a305-4f38-b22b-5b5fe0c3ea04', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Finish Hardware', N'Owner to Provide: Cabinet Hardware', NULL, NULL, 27, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'3c32d1de-a305-4f38-b22b-5b5fe0c3ea04', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'28be9922-a0d4-44ca-8393-5e603cd0610f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Wine Storage Contractor', N'', NULL, NULL, 69, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'28be9922-a0d4-44ca-8393-5e603cd0610f', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ff3c03b9-d3bf-4b56-b2be-6326bc1d8459', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Solar Contractor', N'', NULL, NULL, 63, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'ff3c03b9-d3bf-4b56-b2be-6326bc1d8459', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1594908e-cb37-428d-b3d8-640a683960b9', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'General Contractor Fee', N'', NULL, NULL, 78, N'a5da1985-1858-49e8-abb8-558efc382aac', N'1594908e-cb37-428d-b3d8-640a683960b9', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'117ebb33-6930-4e8c-8230-686c8da61688', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Painting Contractor', N'', NULL, NULL, 53, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'117ebb33-6930-4e8c-8230-686c8da61688', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'536b2c08-b239-463c-8f46-69dfa835ddb2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Low Voltage Contractor', N'Priced for Prewire. Automated shades?, Security? Cable and Internet, speakers. ', NULL, NULL, 51, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'536b2c08-b239-463c-8f46-69dfa835ddb2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ba5c7b3a-b441-4958-857d-6a0d940cf118', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Bath accessories', N'Owner to Provide: Towel Bars, Mirrors, Toilet paper holders, Hardware', NULL, NULL, 19, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'ba5c7b3a-b441-4958-857d-6a0d940cf118', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'b325dd9d-d055-40a6-9139-75eb7b19c7db', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Lumber/Hardware', N'', NULL, NULL, 33, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'b325dd9d-d055-40a6-9139-75eb7b19c7db', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd3bd6589-552b-464b-ae4d-76aeb609de11', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'External Railings Contractor', N'', NULL, NULL, 35, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd3bd6589-552b-464b-ae4d-76aeb609de11', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7bace765-f941-4c33-a797-79de17303864', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior Doors/Hardware', N'', NULL, NULL, 32, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'7bace765-f941-4c33-a797-79de17303864', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd51655e7-3060-454d-930a-87f47b064312', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Cabinet Contractor', N'', NULL, NULL, 25, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd51655e7-3060-454d-930a-87f47b064312', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a5533127-aea0-48de-82af-8886827fc1c5', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Site Drainage', N'', NULL, NULL, 71, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'a5533127-aea0-48de-82af-8886827fc1c5', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'65421aa4-89fe-412d-8d42-8e9ac05c1420', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Sheet metal', N'', NULL, NULL, 58, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'65421aa4-89fe-412d-8d42-8e9ac05c1420', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'740aa1d7-7113-43ed-ab01-902eb8e9e603', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Precast Contractor', N'', NULL, NULL, 55, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'740aa1d7-7113-43ed-ab01-902eb8e9e603', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0115d846-31a3-405e-8c23-92906df2ef77', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Elevator Contractor', N'', NULL, NULL, 31, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'0115d846-31a3-405e-8c23-92906df2ef77', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'47269347-037d-41db-bf70-93d49c36a275', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Internal Railings Contractor', N'', NULL, NULL, 50, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'47269347-037d-41db-bf70-93d49c36a275', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'92883fec-dfdf-4bf3-a033-94f129c42272', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Countertop Material', N'Owner to Provide. Supplier Moda in San Clemente, Deniz- 949-244-4839', NULL, NULL, 21, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'92883fec-dfdf-4bf3-a033-94f129c42272', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'175a03b2-565e-4eb3-a5d7-9728befbe82e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Pool/Spa', N'', NULL, NULL, 69, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'175a03b2-565e-4eb3-a5d7-9728befbe82e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'265a03b2-565e-4eb3-a5d7-9728befbe82e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Retaining Wall', N'', NULL, NULL, 70, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'265a03b2-565e-4eb3-a5d7-9728befbe82e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'9b2bba0c-90bc-4892-a064-98d8d5838182', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Flooring Material', N'', NULL, NULL, 30, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'9b2bba0c-90bc-4892-a064-98d8d5838182', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'86c74224-0170-400c-bd74-99a4146229af', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Landscape', N'', NULL, NULL, 68, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'86c74224-0170-400c-bd74-99a4146229af', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'063d6c29-2430-40de-bb2b-9a83941120cd', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Material Delivery', N'', NULL, NULL, 52, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'063d6c29-2430-40de-bb2b-9a83941120cd', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a1b01fd2-872e-4a6b-a834-9f230342a1c1', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Insulation Contractor', N'', NULL, NULL, 47, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'a1b01fd2-872e-4a6b-a834-9f230342a1c1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'd2c47096-9af5-44d1-8cd2-a0ecee5a8c42', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Structural Steel', N'', NULL, NULL, 66, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'd2c47096-9af5-44d1-8cd2-a0ecee5a8c42', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'ae56314b-7ac7-49fe-a2e0-a53adbfc6332', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Finish Material', N'', NULL, NULL, 29, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'ae56314b-7ac7-49fe-a2e0-a53adbfc6332', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'0a5e9f89-2d1d-40b9-8244-a65f94cc02a3', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Temp Utilities', N'', NULL, NULL, 7, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'0a5e9f89-2d1d-40b9-8244-a65f94cc02a3', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c5ff414e-8f3d-498b-bfc3-a9fc0bb7bd4b', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Electrical Fixtures', N'Owner to Provide: Sconces, Pendants, Fans, Chandeliers', NULL, NULL, 23, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'c5ff414e-8f3d-498b-bfc3-a9fc0bb7bd4b', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)


INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'cee94ee2-f3d6-4f63-8b63-ae0ff14313eb', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Shower Doors Contractor', N'', NULL, NULL, 59, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'cee94ee2-f3d6-4f63-8b63-ae0ff14313eb', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a1c3e88f-e7ec-46d1-adf8-b38997770c31', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Foundation Contractor', N'', NULL, NULL, 40, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'a1c3e88f-e7ec-46d1-adf8-b38997770c31', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'62022341-ec48-4087-aa56-b4d120100ca6', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Demolition', N'', NULL, NULL, 6, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'62022341-ec48-4087-aa56-b4d120100ca6', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'01282e22-a193-48be-8508-b6e4055459a2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'General Contractor Contingency', N'', NULL, NULL, 77, N'a5da1985-1858-49e8-abb8-558efc382aac', N'01282e22-a193-48be-8508-b6e4055459a2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'746abd5e-956d-459c-9a49-b73ba7d596a8', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Flooring Contractor', N'', NULL, NULL, 39, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'746abd5e-956d-459c-9a49-b73ba7d596a8', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'25b467dd-97f0-4816-b0f6-ba193a7acb51', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Closets', N'', NULL, NULL, 26, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'25b467dd-97f0-4816-b0f6-ba193a7acb51', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'1bf7e623-8741-4bfb-a9f2-bd009d658ffc', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Electrical Contractor', N'', NULL, NULL, 30, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'1bf7e623-8741-4bfb-a9f2-bd009d658ffc', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'82599839-ac72-4e9e-82ca-bdb53085c253', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Drywall Contractor', N'', NULL, NULL, 29, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'82599839-ac72-4e9e-82ca-bdb53085c253', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'81fcb8c4-ce90-47e4-bb9b-c01643f4b80f', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Special Inspections', N'', NULL, NULL, 64, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'81fcb8c4-ce90-47e4-bb9b-c01643f4b80f', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7b180310-468e-4b59-9337-c2d84b23cb14', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Building permits', N'', NULL, NULL, 3, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'7b180310-468e-4b59-9337-c2d84b23cb14', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'7023d8e9-6716-4ee1-9c51-c4041baaf865', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Site Drainage Contractor', N'', NULL, NULL, 62, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'7023d8e9-6716-4ee1-9c51-c4041baaf865', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'edd0ee18-a505-4a74-8adf-c57517fae7f1', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Front Door', N'', NULL, NULL, 31, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'edd0ee18-a505-4a74-8adf-c57517fae7f1', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'bae8de79-c74a-4be5-b3eb-cca2d535c2bf', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'BBQ Grill/Firepit', N'', NULL, NULL, 64, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'bae8de79-c74a-4be5-b3eb-cca2d535c2bf', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'e3ad07e4-1f16-4d2d-b305-d04fd0b1cbe7', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Tile Contractor', N'', NULL, NULL, 67, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'e3ad07e4-1f16-4d2d-b305-d04fd0b1cbe7', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'271aa5bc-f5b2-4f56-94fb-d37d7305da08', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Appliances', N'Owner to Provide', NULL, NULL, 17, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'271aa5bc-f5b2-4f56-94fb-d37d7305da08', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'5b3a00a9-4fc6-42fc-9b77-d73fd784700a', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Property Tax', N'', NULL, NULL, 81, N'a5da1985-1858-49e8-abb8-558efc382aac', N'5b3a00a9-4fc6-42fc-9b77-d73fd784700a', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'a44dbb0d-fc97-43d1-b12c-d744428a4df8', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Grading', N'', NULL, NULL, 4, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'a44dbb0d-fc97-43d1-b12c-d744428a4df8', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'af245b53-809c-4531-9f8b-dc7114b0f4b4', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Garage Doors Contractor', N'', NULL, NULL, 42, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'af245b53-809c-4531-9f8b-dc7114b0f4b4', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'cfe6afff-4a8e-47ba-b24a-dff9e6b43e3e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior Doors/Windows', N'"Budget Freindly windows Andersen 100 or Milgard.  Doors- La Cantina higher end and Windor or Milgard for budget friendly."', NULL, NULL, 25, N'c00df57f-c09b-4967-bc93-e2330e2f303f', N'cfe6afff-4a8e-47ba-b24a-dff9e6b43e3e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)


INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'986cbdc3-be93-406a-babc-e2d14ba882db', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Fireplace  Contractor', N'', NULL, NULL, 38, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'986cbdc3-be93-406a-babc-e2d14ba882db', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'68b3fffd-d37f-481b-8303-e43a00a0799e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Roofing Contractor', N'', NULL, NULL, 56, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'68b3fffd-d37f-481b-8303-e43a00a0799e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'416fdc24-339a-499d-9bde-e4be5ef19ed9', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Heating & Air Contractor', N'', NULL, NULL, 45, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'416fdc24-339a-499d-9bde-e4be5ef19ed9', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'075f528a-c636-4b6e-9cc7-e729d0c0ee9e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Framing Contractor', N'', NULL, NULL, 41, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'075f528a-c636-4b6e-9cc7-e729d0c0ee9e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'c207c3cd-494c-462f-903c-e75f8c8a9707', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Finish Carpentry', N'Install: Finish Material, bath accessories, interior doors', NULL, NULL, 36, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'c207c3cd-494c-462f-903c-e75f8c8a9707', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'01a4dd1e-36bf-427e-8f28-e803351e48fe', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Exterior Finish Contractor', N'', NULL, NULL, 33, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'01a4dd1e-36bf-427e-8f28-e803351e48fe', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'2929c384-45bd-494e-9e98-f0e140c99aa0', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Temp Services', N'', NULL, NULL, 72, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'2929c384-45bd-494e-9e98-f0e140c99aa0', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'6ecf6dbd-171f-4978-857e-f8545d997164', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Shower Enclosure/Mirror Contractor', N'', NULL, NULL, 61, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'6ecf6dbd-171f-4978-857e-f8545d997164', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'6e525d26-883e-402d-94a4-f87d96b7cb52', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Insurance', N'', NULL, NULL, 5, N'3866ffa5-d95b-4fb3-b857-73fd386e1005', N'6e525d26-883e-402d-94a4-f87d96b7cb52', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'9a0da22f-afd4-4813-b7a5-f94a5d37416b', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Driveway', N'', NULL, NULL, 67, N'af1e1f78-c83d-480a-b244-1e28f3ac766d', N'9a0da22f-afd4-4813-b7a5-f94a5d37416b', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'21578f57-31ed-420f-bfc1-fd77098a2bb2', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Gutters', N'', NULL, NULL, 44, N'851efb5a-de4f-4380-a264-ab31bb3acecd', N'21578f57-31ed-420f-bfc1-fd77098a2bb2', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)

INSERT [dbo].[ProposalTemplatesLineItems] ([Id], [ProposalTemplateId], [Name], [Description], [Amount], [Percentage], [Sequence], [ParentId], [EstimateCategoryId], [IsDeleted], [CreatedBy], [UpdatedBy], [DateCreated], [DateUpdated]) VALUES (N'f3832da8-ea6f-436e-9c99-ff65cd5b061e', N'd1103b14-2196-48c9-bb93-dc6de65962c2', N'Land', N'', NULL, NULL, 80, N'a5da1985-1858-49e8-abb8-558efc382aac', N'f3832da8-ea6f-436e-9c99-ff65cd5b061e', 0, 1, NULL, CAST(N'2025-03-30T01:25:47.827' AS DateTime), NULL)


END

-- CONSTRUCTION TASKS
IF NOT EXISTS (SELECT 1 FROM ConstructionTasks)
BEGIN


INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'bb356b9f-deed-4b5a-8419-081c81f4326c', N'Finish - Tile', NULL, 41, N'49f8f6e4-9753-4360-b351-9ece11a9b7aa', 22, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'9523e1b2-0916-45e2-ab4c-0c4395ab2efc', N'Windows - Install', NULL, 22, N'725a8835-84bc-4e24-a9fc-f8ceb15f889d', 15, NULL, NULL, NULL, NULL, NULL, NULL, 30, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'bcc387b7-c2db-474d-b610-0ecd040bd5cf', N'Fireplaces', NULL, 14, N'6a8525f4-e2ea-4c79-b4ed-8944fb97977e', 3, NULL, NULL, NULL, NULL, NULL, NULL, 21, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'd1608fad-7918-40b5-8581-166a41080d4d', N'Countertop Fabricator', NULL, 46, N'4051d28a-0d5e-4ed2-8ac3-b77975c8026f', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'b0921fd2-29d9-4a4f-80b7-1d7244d87f60', N'Asbestos Removal', NULL, 2, N'4a5ab3c2-66a0-45ff-979c-2969f6dce12c', 3, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'2f52652b-ce6d-4b20-886b-1d7eb61feea0', N'Perimeter Wall', NULL, 10, N'69b1a0b6-eef5-4114-b702-91088a244705', 15, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'98aec203-4478-4f3c-bab2-1df3f1db8126', N'Install Flooring', NULL, 51, N'd1608fad-7918-40b5-8581-166a41080d4d', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'237c25fe-ac54-404f-bbec-22b66b9f5489', N'Order Appliances', NULL, 21, N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', 1, NULL, NULL, NULL, NULL, NULL, NULL, 28, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'8bb2ec0f-a952-4b8c-acd6-2918748da488', N'Roof Watertight', NULL, 24, N'd58a5a9c-1197-4f28-b71f-f3c22806b92c', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'4a5ab3c2-66a0-45ff-979c-2969f6dce12c', N'Temp Services', NULL, 1, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'8b207d2b-7bdd-4b52-99fe-2ec9460a63d6', N'Site Work', NULL, 40, N'8f50d294-ec1e-48be-abb2-d386f7ac8b3f', 10, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'9562f010-a549-42f9-a49f-35b46a37c600', N'Stucco - Scratch/Brown Coat', NULL, 37, N'1b348a79-8897-46d6-a7c0-c021f4ff077f', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'67690091-1ea7-418b-8768-360263cafd25', N'Rough Electrical', NULL, 18, N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', 21, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'ee3894d4-edfa-4787-9d00-3b015ca35487', N'Cabinets - Order', NULL, 20, N'6a8525f4-e2ea-4c79-b4ed-8944fb97977e', 1, NULL, NULL, NULL, NULL, NULL, NULL, 28, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'c64282b2-0b08-486c-b8a4-4f7df86acc90', N'Order Fixtures / Shower Doors', NULL, 36, N'700ac951-18e9-48c9-a77d-90c60fea73d6', 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'cea40d95-fd45-435a-90b8-560a4e9cc5a1', N'Lumber - Order', NULL, 5, N'4a5ab3c2-66a0-45ff-979c-2969f6dce12c', 2, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'0e7e9000-a23a-4793-85db-59ee7d1ec7b7', N'Order Doors, Trim, Moulding', NULL, 35, N'700ac951-18e9-48c9-a77d-90c60fea73d6', 1, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'74ed91a4-ade7-4bb1-b7e7-5b459686ba60', N'Cabinets - Install', NULL, 34, N'ff8e9887-af2d-43a0-bfd7-c84fc4cd8ba3', 7, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'2c9b294c-97f8-47b0-99ee-67fc5215df6e', N'Demo', NULL, 3, N'b0921fd2-29d9-4a4f-80b7-1d7244d87f60', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'78acd102-9279-414b-bc10-69c8905f15ce', N'Concrete', NULL, 7, N'e06cb604-251d-4fa1-9050-da72d2f8f745', 14, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'73ec88f1-f17b-4c81-b13f-784cccfde2a6', N'Solar - Install', NULL, 28, N'813f7542-9972-48be-a46d-dec7c404e9a9', 21, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'f6919ad2-ec4a-4d04-af6b-7c92a0b0c516', N'Concrete - Underground', NULL, 8, N'78acd102-9279-414b-bc10-69c8905f15ce', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'9f2b9cf3-fad2-42a0-b5ae-7f534e3bcbc0', N'Finish - Electrical', NULL, 47, N'bb356b9f-deed-4b5a-8419-081c81f4326c', 5, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'011d3e19-56e6-408c-bf5b-820a990f5d97', N'Insulation', NULL, 29, N'8f5aecb4-41f2-49bb-ba34-e61edcb286b0', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'af8d4b3a-c660-4fb6-be47-82114b4ac3d7', N'Certificate of Occupany', NULL, 54, N'7409e125-74a9-48b0-9ae6-fafc1857ee08', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'6a8525f4-e2ea-4c79-b4ed-8944fb97977e', N'Framing - Walls', NULL, 11, N'69b1a0b6-eef5-4114-b702-91088a244705', 15, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'd5d61de7-c299-4b95-91e4-8be4998c71c2', N'Gutters', NULL, 27, N'813f7542-9972-48be-a46d-dec7c404e9a9', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'700ac951-18e9-48c9-a77d-90c60fea73d6', N'Drywall Inside (Nailing)', NULL, 30, N'011d3e19-56e6-408c-bf5b-820a990f5d97', 10, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'69b1a0b6-eef5-4114-b702-91088a244705', N'Concrete - Slab', NULL, 9, N'f6919ad2-ec4a-4d04-af6b-7c92a0b0c516', 3, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'd8764acc-5d62-418b-99c5-97cd655f1f8d', N'Rough HVAC', NULL, 19, N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', 7, NULL, NULL, NULL, NULL, NULL, NULL, 10, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'd74fe261-4b89-4156-bdd2-9e5e54f2b0b1', N'Hot Mop', NULL, 32, N'700ac951-18e9-48c9-a77d-90c60fea73d6', 2, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'49f8f6e4-9753-4360-b351-9ece11a9b7aa', N'Drywall Mud', NULL, 33, N'700ac951-18e9-48c9-a77d-90c60fea73d6', 12, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'0aad6bfb-c6df-4b7d-b4df-a796bb3120b8', N'Sheet Metal', NULL, 17, N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', 3, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'0166f0b9-3c15-49ef-af2c-aadc2a34cda7', N'Finish - Painting/Stain', NULL, 42, N'49f8f6e4-9753-4360-b351-9ece11a9b7aa', 35, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'acd9d1b5-e5b3-4516-88ed-ad20ad895dda', N'Grading', NULL, 4, N'2c9b294c-97f8-47b0-99ee-67fc5215df6e', 10, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'4051d28a-0d5e-4ed2-8ac3-b77975c8026f', N'Granite', NULL, 45, N'bb356b9f-deed-4b5a-8419-081c81f4326c', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'68e5e799-51ad-43ef-8273-b7d5ea862f92', N'Finish - Carpentry/Railings', NULL, 44, N'49f8f6e4-9753-4360-b351-9ece11a9b7aa', 21, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'd96f1804-4fa7-426f-aa83-b82f49e27b3f', N'Finish Stairs', NULL, 43, N'49f8f6e4-9753-4360-b351-9ece11a9b7aa', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', N'Framing - Roof', NULL, 12, N'2f52652b-ce6d-4b20-886b-1d7eb61feea0', 15, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'486c740a-2fde-47ce-8102-baa8b9d2251e', N'Carpet Install', NULL, 52, N'98aec203-4478-4f3c-bab2-1df3f1db8126', 2, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'1b348a79-8897-46d6-a7c0-c021f4ff077f', N'Stucco - Outside (Paper/Wire)', NULL, 31, N'011d3e19-56e6-408c-bf5b-820a990f5d97', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'2899e884-5141-4df2-a53c-c74992afaf34', N'Rock Veneer', NULL, 38, N'9562f010-a549-42f9-a49f-35b46a37c600', 14, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'ff8e9887-af2d-43a0-bfd7-c84fc4cd8ba3', N'Cabinets - Build', NULL, 23, N'ee3894d4-edfa-4787-9d00-3b015ca35487', 15, NULL, NULL, NULL, NULL, NULL, NULL, 30, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'6e6cbaa4-d089-47cc-a883-cf74f607bf2b', N'Finish - HVAC', NULL, 48, N'bb356b9f-deed-4b5a-8419-081c81f4326c', 7, NULL, NULL, NULL, NULL, NULL, NULL, 5, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'8f50d294-ec1e-48be-abb2-d386f7ac8b3f', N'Stucco - Color', NULL, 39, N'9562f010-a549-42f9-a49f-35b46a37c600', 7, NULL, NULL, NULL, NULL, NULL, NULL, 28, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'e06cb604-251d-4fa1-9050-da72d2f8f745', N'Foundation Forming', NULL, 6, N'acd9d1b5-e5b3-4516-88ed-ad20ad895dda', 14, NULL, NULL, NULL, NULL, NULL, NULL, 9, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'e858b7f7-095c-4837-a4db-dcdfb28c47a0', N'Rough Plumbing', NULL, 16, N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', 14, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'813f7542-9972-48be-a46d-dec7c404e9a9', N'Roof Completion', NULL, 26, N'8bb2ec0f-a952-4b8c-acd6-2918748da488', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'b2a0a349-921c-4afe-85ef-e6118cb93e7d', N'Finish - Plumbing', NULL, 49, N'bb356b9f-deed-4b5a-8419-081c81f4326c', 7, NULL, NULL, NULL, NULL, NULL, NULL, 7, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'8f5aecb4-41f2-49bb-ba34-e61edcb286b0', N'Low Voltage', NULL, 25, N'67690091-1ea7-418b-8768-360263cafd25', 7, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'adb013bb-25a9-4bb2-a08c-ed6d61f2ae7e', N'Punch List / Clean-up', NULL, 55, N'af8d4b3a-c660-4fb6-be47-82114b4ac3d7', 10, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'd58a5a9c-1197-4f28-b71f-f3c22806b92c', N'Framing - Pickup', NULL, 15, N'1fd869a7-0fe0-4a87-9e96-b8641d01bb6e', 5, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'725a8835-84bc-4e24-a9fc-f8ceb15f889d', N'Windows - Ordering', NULL, 13, N'6a8525f4-e2ea-4c79-b4ed-8944fb97977e', 3, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'15bc5e4f-4fe0-4621-b072-faedf3afa60b', N'Install Garage Doors', NULL, 50, N'8f50d294-ec1e-48be-abb2-d386f7ac8b3f', 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

INSERT [dbo].[ConstructionTasks] ([ID], [Name], [Description], [Sequence], [ParentTaskID], [Duration], [Created], [CreatedBy], [Updated], [UpdatedBy], [QBClassID], [Pred1ID], [Pred1Lag], [Pred2ID], [Pred2Lag], [Pred3ID], [Pred3Lag]) VALUES (N'7409e125-74a9-48b0-9ae6-fafc1857ee08', N'Appliances - Install', NULL, 53, N'486c740a-2fde-47ce-8102-baa8b9d2251e', 2, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL)

END

-- COMPANY SETTINGS
IF NOT EXISTS (SELECT 1 FROM CompanySettings)
BEGIN
	INSERT INTO CompanySettings (CompanyName, CompanyEmail, GeneralContractorName, CreatedBy, DateCreated)
	VALUES ('CH Anderson Construction', 'chaconstruction@gmail.com', 'Craig Anderson',1, GETDATE())
END

-- SYS BACKGROUND JOBS
IF NOT EXISTS (SELECT 1 FROM SysBackgroundJobs)
BEGIN
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'QuickBooks Data Sync', CAST(N'2025-06-02T22:17:34.990' AS DateTime), N'Scheduled', N'/quickbooksdatasync', 1, 0, 1)
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'Identify Active Jobs ', CAST(N'2025-06-02T22:24:36.513' AS DateTime), N'Scheduled', N'/sync-active-jobs', 1, 0, 1)
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'Create Missing Proposals', CAST(N'2025-06-02T22:25:27.503' AS DateTime), N'Scheduled', N'/sync-proposals', 1, 0, 1)
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'Update Project Totals', CAST(N'2025-06-02T22:25:56.593' AS DateTime), N'Scheduled', N'/sync-project-totals', 1, 0, 1)
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'Import Data from Production', CAST(N'2025-06-02T22:38:09.127' AS DateTime), N'Standby', N'/sync-production-data', 1, 1, 0)
END
IF NOT EXISTS (SELECT 1 FROM SysBackgroundJobs where JobName = 'Sync QB Classes')
BEGIN
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'Sync QB Classes', CAST(N'2025-06-02T22:38:09.127' AS DateTime), N'Standby', N'/sync-qbclass', 1, 1, 0)
END
IF NOT EXISTS (SELECT 1 FROM SysBackgroundJobs where JobName = 'Sync QB Customers')
BEGIN
	INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) VALUES (N'Sync QB Customers', CAST(N'2025-06-02T22:38:09.127' AS DateTime), N'Standby', N'/sync-qbcustomers', 1, 1, 0)
END


IF NOT EXISTS (SELECT 1 FROM [dbo].[SysBackgroundJobs] where JobName = 'Weekly Run Task Action Item')
BEGIN
INSERT [dbo].[SysBackgroundJobs] ([JobName], [DateOfExecution], [LastStatus], [ApiEndpoint], [Development], [Staging], [Production]) 
VALUES (N'Weekly Run Task Action Item', CAST(N'2025-06-02T22:38:09.127' AS DateTime), N'Scheduled', N'/weekly-task-action-item', 1, 1, 1)
END

-- ACTION TYPES
-- Cost Change
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'Cost Change')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (1, N'Cost Change', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- Schedule Change
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'Schedule Change')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (2, N'Schedule Change', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- Client Contact
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'Client Contact')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (3, N'Client Contact', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- Note
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'Note')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (5, N'Note', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- Follow up, Sub-contractor, Client, City, etc.
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'Follow up, Sub-contractor, Client, City, etc.')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (6, N'Follow up, Sub-contractor, Client, City, etc.', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- Reminder, alarm, notification text/email
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'Reminder, alarm, notification text/email')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (7, N'Reminder, alarm, notification text/email', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- General Change Order
IF NOT EXISTS (SELECT 1 FROM ActionTypes WHERE Title = N'General Change Order')
BEGIN
	SET IDENTITY_INSERT [dbo].[ActionTypes] ON
	INSERT [dbo].[ActionTypes] ([Id], [Title], [DateCreated], [CreatedBy], [DateUpdated], [UpdatedBy], [IsDeleted])
	VALUES (8, N'General Change Order', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, 0)
	SET IDENTITY_INSERT [dbo].[ActionTypes] OFF
END

-- Update Txnnumber '4783' from QBTransactions status to voided
IF EXISTS (SELECT 1 FROM QBTransactions with(nolock) WHERE ClassID = '9AEDCA41-737F-4CC6-A274-08DD51948569' AND TxnNumber IN ('4783'))
BEGIN
	UPDATE QBTransactions
	SET [Status] = 'Voided'
	WHERE ClassID = '9AEDCA41-737F-4CC6-A274-08DD51948569'
		AND TxnNumber IN ('4783')
END

-- SubContractorCategories --
IF NOT EXISTS (SELECT 1 FROM SubContractorCategories)
BEGIN
    INSERT INTO [SubContractorCategories] (Id, Name, IsActive)
	SELECT NEWID() as Id,
	  ec.Name,
	  1 as IsActive
    FROM EstimateCategories ec
    INNER JOIN EstimateCategories parent
        ON ec.ParentEstimateCategoryId = parent.Id
    WHERE UPPER(parent.Name) = 'SUB CONTRACTORS'
END

-- EmailTemplate --
IF NOT EXISTS (SELECT 1 FROM EmailTemplate)
BEGIN
    INSERT INTO EmailTemplate(Id, [Name], EmailType, Body, OwnerId, IsDefault, DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Status Report Template', 'Status Report', '<h2><span class="text-big" style="background-color:transparent;color:#000000;"><strong>Weekly Update (Auto Fill Project Name - Don''t Remove) (Manual Enter Week Number Here)</strong></span></h2><p style="margin-left:36pt;">&nbsp;</p><h2><span style="background-color:transparent;color:#222222;"><strong>Past Weeks Work</strong></span></h2><p><span style="background-color:transparent;color:#222222;"><strong>What subs have been on site</strong></span></p><ul><li>(Auto Fill Subs - Don''t Remove)</li><li>&nbsp;</li></ul><p><span style="background-color:transparent;color:#222222;"><strong>What work has transpired</strong>&nbsp;</span></p><ul><li>&nbsp;</li></ul><p><span style="background-color:transparent;color:#222222;"><strong>What changes have occurred that have caused scheduling/budget to be missed</strong></span></p><ul><li>&nbsp;</li></ul><p>&nbsp;</p><h2><span style="background-color:transparent;color:#222222;"><strong>Future Weeks Work</strong></span></h2><p><span style="background-color:transparent;color:#222222;"><strong>What subs will be on-site, and on what days</strong></span></p><ul><li>&nbsp;</li></ul><p><span style="background-color:transparent;color:#222222;"><strong>Meetings planned</strong></span></p><ul><li>&nbsp;</li></ul><p><span style="background-color:transparent;color:#222222;"><strong>Questions for (Auto Fill Client Name - Don''t Remove)</strong></span></p><ul><li>&nbsp;</li></ul>', 1, 1, GETDATE(), 1, GETDATE(), 1)

	INSERT INTO EmailTemplate(Id, [Name], EmailType, Body, OwnerId, IsDefault, DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Email To Client Template', 'Email To Client', '', 1, 1, GETDATE(), 1, GETDATE(), 1)

	INSERT INTO EmailTemplate(Id, [Name], EmailType, Body, OwnerId, IsDefault, DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Schedule Report Template', 'Schedule Report', '<p>Attached is a copy of your Project Schedule Report.&nbsp;</p><p>Please review and let''s meet or call to discuss.&nbsp;</p>', 1, 1, GETDATE(), 1, GETDATE(), 1)

	INSERT INTO EmailTemplate(Id, [Name], EmailType, Body, OwnerId, IsDefault, DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Proposal Report Template', 'Proposal Report', '<p>Attached is a copy of your construction cost estimate.&nbsp;</p><p>Please review and let''s meet or call to discuss.&nbsp;</p>', 1, 1, GETDATE(), 1, GETDATE(), 1)

	INSERT INTO EmailTemplate(Id, [Name], EmailType, Body, OwnerId, IsDefault, DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Budget To Actual Report Template', 'Budget To Actual Report', '<p>Attached is a copy of your Estimate To Actual Report.&nbsp;</p><p>Please review and let''s meet or call to discuss.&nbsp;</p>', 1, 1, GETDATE(), 1, GETDATE(), 1)
END

-- Proposal Template --
IF NOT EXISTS (SELECT 1 FROM ProposalTemplates WHERE [Name] = 'Ground-Up (Default)')
BEGIN
	INSERT INTO [dbo].[ProposalTemplates]
           ([Id]
           ,[Name]
           ,[IsDefault]
           ,[IsActive]
           ,[CreatedBy]
           ,[DateCreated])
	VALUES
	('D1103B14-2196-48C9-BB93-DC6DE65962C2',
	'Ground-Up (Default)',
	1,
	1,
	1,
	GETDATE()
	)

INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('AF1E1F78-C83D-480A-B244-1E28F3AC766D','D1103B14-2196-48C9-BB93-DC6DE65962C2','Site Work','',NULL,NULL,4,NULL,'AF1E1F78-C83D-480A-B244-1E28F3AC766D',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('A5DA1985-1858-49E8-ABB8-558EFC382AAC','D1103B14-2196-48C9-BB93-DC6DE65962C2','Overhead','',NULL,NULL,5,NULL,'A5DA1985-1858-49E8-ABB8-558EFC382AAC',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('3866FFA5-D95B-4FB3-B857-73FD386E1005','D1103B14-2196-48C9-BB93-DC6DE65962C2','Preparation','',NULL,NULL,1,NULL,'3866FFA5-D95B-4FB3-B857-73FD386E1005',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('851EFB5A-DE4F-4380-A264-AB31BB3ACECD','D1103B14-2196-48C9-BB93-DC6DE65962C2','Sub Contractors','',NULL,NULL,3,NULL,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('C00DF57F-C09B-4967-BC93-E2330E2F303F','D1103B14-2196-48C9-BB93-DC6DE65962C2','Material','',NULL,NULL,2,NULL,'C00DF57F-C09B-4967-BC93-E2330E2F303F',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('BAE8DE79-C74A-4BE5-B3EB-CCA2D535C2BF','D1103B14-2196-48C9-BB93-DC6DE65962C2','BBQ Grill/Firepit','',0.00,NULL,64,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','BAE8DE79-C74A-4BE5-B3EB-CCA2D535C2BF',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('2929C384-45BD-494E-9E98-F0E140C99AA0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Temp Services','',0.00,NULL,72,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','2929C384-45BD-494E-9E98-F0E140C99AA0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('9A0DA22F-AFD4-4813-B7A5-F94A5D37416B','D1103B14-2196-48C9-BB93-DC6DE65962C2','Driveway','',0.00,NULL,67,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','9A0DA22F-AFD4-4813-B7A5-F94A5D37416B',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('A5533127-AEA0-48DE-82AF-8886827FC1C5','D1103B14-2196-48C9-BB93-DC6DE65962C2','Site Drainage','',0.00,NULL,71,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','A5533127-AEA0-48DE-82AF-8886827FC1C5',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('175A03B2-565E-4EB3-A5D7-9728BEFBE82E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Pool/Spa','',0.00,NULL,69,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','175A03B2-565E-4EB3-A5D7-9728BEFBE82E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('265A03B2-565E-4EB3-A5D7-9728BEFBE82E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Retaining Wall','',0.00,NULL,70,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','265A03B2-565E-4EB3-A5D7-9728BEFBE82E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('86C74224-0170-400C-BD74-99A4146229AF','D1103B14-2196-48C9-BB93-DC6DE65962C2','Landscape','',0.00,NULL,68,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','86C74224-0170-400C-BD74-99A4146229AF',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('980D37EC-9EDE-45FA-8A9D-451D218A4182','D1103B14-2196-48C9-BB93-DC6DE65962C2','Curbs & Gutter Contractor','',0.00,NULL,66,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','980D37EC-9EDE-45FA-8A9D-451D218A4182',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('C27BDE33-8F87-4A7B-5866-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Hardscape','',0.00,NULL,74,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','C27BDE33-8F87-4A7B-5866-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('A09F00BB-5E94-463B-37E0-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Backfill','',0.00,NULL,40,'AF1E1F78-C83D-480A-B244-1E28F3AC766D','A09F00BB-5E94-463B-37E0-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('86ABEF50-AD0B-4448-7441-08DCF1E6BCDD','D1103B14-2196-48C9-BB93-DC6DE65962C2','On Site Supervision','',0.00,NULL,12,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','86ABEF50-AD0B-4448-7441-08DCF1E6BCDD',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('B68851C1-9BD1-4234-9BDA-28490931A31F','D1103B14-2196-48C9-BB93-DC6DE65962C2','Other','',0.00,NULL,88,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','6504C427-BE01-44E4-AB3C-5EA2A6DBB7F1',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('7EEADBE1-D848-4E07-9AEC-2F4E7D3979BD','D1103B14-2196-48C9-BB93-DC6DE65962C2','Interest Expense','',0.00,NULL,82,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','7EEADBE1-D848-4E07-9AEC-2F4E7D3979BD',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('1594908E-CB37-428D-B3D8-640A683960B9','D1103B14-2196-48C9-BB93-DC6DE65962C2','General Contractor Fee','',0.00,NULL,78,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','1594908E-CB37-428D-B3D8-640A683960B9',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('01282E22-A193-48BE-8508-B6E4055459A2','D1103B14-2196-48C9-BB93-DC6DE65962C2','General Contractor Contingency','',0.00,NULL,77,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','01282E22-A193-48BE-8508-B6E4055459A2',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('5B3A00A9-4FC6-42FC-9B77-D73FD784700A','D1103B14-2196-48C9-BB93-DC6DE65962C2','Property Tax','',0.00,NULL,81,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','5B3A00A9-4FC6-42FC-9B77-D73FD784700A',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('F3832DA8-EA6F-436E-9C99-FF65CD5B061E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Land','',0.00,NULL,80,'A5DA1985-1858-49E8-ABB8-558EFC382AAC','F3832DA8-EA6F-436E-9C99-FF65CD5B061E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('6E525D26-883E-402D-94A4-F87D96B7CB52','D1103B14-2196-48C9-BB93-DC6DE65962C2','Insurance','',0.00,NULL,5,'3866FFA5-D95B-4FB3-B857-73FD386E1005','6E525D26-883E-402D-94A4-F87D96B7CB52',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('A44DBB0D-FC97-43D1-B12C-D744428A4DF8','D1103B14-2196-48C9-BB93-DC6DE65962C2','Grading','',0.00,NULL,4,'3866FFA5-D95B-4FB3-B857-73FD386E1005','A44DBB0D-FC97-43D1-B12C-D744428A4DF8',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('62022341-EC48-4087-AA56-B4D120100CA6','D1103B14-2196-48C9-BB93-DC6DE65962C2','Demolition','',0.00,NULL,6,'3866FFA5-D95B-4FB3-B857-73FD386E1005','62022341-EC48-4087-AA56-B4D120100CA6',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('7B180310-468E-4B59-9337-C2D84B23CB14','D1103B14-2196-48C9-BB93-DC6DE65962C2','Building permits','',0.00,NULL,3,'3866FFA5-D95B-4FB3-B857-73FD386E1005','7B180310-468E-4B59-9337-C2D84B23CB14',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('0A5E9F89-2D1D-40B9-8244-A65F94CC02A3','D1103B14-2196-48C9-BB93-DC6DE65962C2','Temp Utilities','',0.00,NULL,7,'3866FFA5-D95B-4FB3-B857-73FD386E1005','0A5E9F89-2D1D-40B9-8244-A65F94CC02A3',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('AB4C749C-4A10-4EB9-9779-575A60307F1E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Asbestos Removal','',0.00,NULL,8,'3866FFA5-D95B-4FB3-B857-73FD386E1005','AB4C749C-4A10-4EB9-9779-575A60307F1E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('DAAD3D7B-4D42-4F22-AE1A-5514D88B2600','D1103B14-2196-48C9-BB93-DC6DE65962C2','Plans/survey','',0.00,NULL,1,'3866FFA5-D95B-4FB3-B857-73FD386E1005','DAAD3D7B-4D42-4F22-AE1A-5514D88B2600',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('7CD6334B-653D-4537-37E2-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Excavation','',0.00,NULL,25,'3866FFA5-D95B-4FB3-B857-73FD386E1005','7CD6334B-653D-4537-37E2-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('585030D5-0300-421E-37E3-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Stain Contractor','',0.00,NULL,77,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','585030D5-0300-421E-37E3-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('11FB7A0E-D6AF-4952-37E4-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Electrical Panel Upgrade','',0.00,NULL,81,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','11FB7A0E-D6AF-4952-37E4-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('676D782F-201A-4A5F-A15F-1341599130D1','D1103B14-2196-48C9-BB93-DC6DE65962C2','Hot Mop Contractor','',0.00,NULL,46,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','676D782F-201A-4A5F-A15F-1341599130D1',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('C2682AA6-A285-413A-A081-13EB66499965','D1103B14-2196-48C9-BB93-DC6DE65962C2','Interior Design','',0.00,NULL,48,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','C2682AA6-A285-413A-A081-13EB66499965',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('1F86BC77-E947-44DC-A4ED-1591F87626EE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Countertop Fabricator','',0.00,NULL,27,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','1F86BC77-E947-44DC-A4ED-1591F87626EE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('107C1DD9-9C5E-4E82-5867-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Scaffolding','',0.00,NULL,57,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','107C1DD9-9C5E-4E82-5867-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('BA366BD1-6213-4034-5868-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Exterior and Interior Railing','Per Plans',0.00,NULL,34,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','BA366BD1-6213-4034-5868-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('467B86DD-A9EF-4965-5869-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Shower Pan','Shower Waterproofing',0.00,NULL,60,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','467B86DD-A9EF-4965-5869-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('E7F27724-E523-4C3B-586A-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Garage Floor','Level and Epoxy Floor',0.00,NULL,43,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','E7F27724-E523-4C3B-586A-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('0997663D-ED47-4EAB-586B-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Window and Door Install','Per Plans',0.00,NULL,70,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','0997663D-ED47-4EAB-586B-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('0CB8B9C0-7E57-4EA1-A8DA-429ACE1C6107','D1103B14-2196-48C9-BB93-DC6DE65962C2','Interior Doors/Hardware','',0.00,NULL,49,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','DF7ACF8B-9156-4CF8-A3D2-589A7953466E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('E1903443-4536-4D24-8507-32D8FBFC5903','D1103B14-2196-48C9-BB93-DC6DE65962C2','Fire Sprinklers Contractor','',0.00,NULL,37,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','E1903443-4536-4D24-8507-32D8FBFC5903',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('B24BACD8-EDC7-4878-8AA4-37C5724237F2','D1103B14-2196-48C9-BB93-DC6DE65962C2','Deck Contractor','',0.00,NULL,28,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','B24BACD8-EDC7-4878-8AA4-37C5724237F2',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('45A886D9-F473-41C2-9359-55F9B968BDDC','D1103B14-2196-48C9-BB93-DC6DE65962C2','Trash Hauling','',0.00,NULL,68,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','45A886D9-F473-41C2-9359-55F9B968BDDC',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('E8C89FAC-0014-438E-89BA-56C89026AEB4','D1103B14-2196-48C9-BB93-DC6DE65962C2','Stairs Contractor','',0.00,NULL,65,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','E8C89FAC-0014-438E-89BA-56C89026AEB4',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('117EBB33-6930-4E8C-8230-686C8DA61688','D1103B14-2196-48C9-BB93-DC6DE65962C2','Painting Contractor','',0.00,NULL,53,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','117EBB33-6930-4E8C-8230-686C8DA61688',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('536B2C08-B239-463C-8F46-69DFA835DDB2','D1103B14-2196-48C9-BB93-DC6DE65962C2','Low Voltage Contractor','Priced for Prewire. Automated shades?, Security? Cable and Internet, speakers. ',0.00,NULL,51,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','536B2C08-B239-463C-8F46-69DFA835DDB2',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('CEE94EE2-F3D6-4F63-8B63-AE0FF14313EB','D1103B14-2196-48C9-BB93-DC6DE65962C2','Shower Doors Contractor','',0.00,NULL,59,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','CEE94EE2-F3D6-4F63-8B63-AE0FF14313EB',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('A1C3E88F-E7EC-46D1-ADF8-B38997770C31','D1103B14-2196-48C9-BB93-DC6DE65962C2','Foundation Contractor','',0.00,NULL,40,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','A1C3E88F-E7EC-46D1-ADF8-B38997770C31',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('746ABD5E-956D-459C-9A49-B73BA7D596A8','D1103B14-2196-48C9-BB93-DC6DE65962C2','Flooring Contractor','',0.00,NULL,39,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','746ABD5E-956D-459C-9A49-B73BA7D596A8',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('25B467DD-97F0-4816-B0F6-BA193A7ACB51','D1103B14-2196-48C9-BB93-DC6DE65962C2','Closets','',0.00,NULL,26,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','25B467DD-97F0-4816-B0F6-BA193A7ACB51',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('1BF7E623-8741-4BFB-A9F2-BD009D658FFC','D1103B14-2196-48C9-BB93-DC6DE65962C2','Electrical Contractor','',0.00,NULL,30,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','1BF7E623-8741-4BFB-A9F2-BD009D658FFC',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('82599839-AC72-4E9E-82CA-BDB53085C253','D1103B14-2196-48C9-BB93-DC6DE65962C2','Drywall Contractor','',0.00,NULL,29,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','82599839-AC72-4E9E-82CA-BDB53085C253',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('81FCB8C4-CE90-47E4-BB9B-C01643F4B80F','D1103B14-2196-48C9-BB93-DC6DE65962C2','Special Inspections','',0.00,NULL,64,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','81FCB8C4-CE90-47E4-BB9B-C01643F4B80F',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('7023D8E9-6716-4EE1-9C51-C4041BAAF865','D1103B14-2196-48C9-BB93-DC6DE65962C2','Site Drainage Contractor','',0.00,NULL,62,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','7023D8E9-6716-4EE1-9C51-C4041BAAF865',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('825800A8-00CD-4AC2-5865-08DCE19EB8A0','D1103B14-2196-48C9-BB93-DC6DE65962C2','Waterproofing','',0.00,NULL,71,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','825800A8-00CD-4AC2-5865-08DCE19EB8A0',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('AF245B53-809C-4531-9F8B-DC7114B0F4B4','D1103B14-2196-48C9-BB93-DC6DE65962C2','Garage Doors Contractor','',0.00,NULL,42,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','AF245B53-809C-4531-9F8B-DC7114B0F4B4',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('063D6C29-2430-40DE-BB2B-9A83941120CD','D1103B14-2196-48C9-BB93-DC6DE65962C2','Material Delivery','',0.00,NULL,52,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','063D6C29-2430-40DE-BB2B-9A83941120CD',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('A1B01FD2-872E-4A6B-A834-9F230342A1C1','D1103B14-2196-48C9-BB93-DC6DE65962C2','Insulation Contractor','',0.00,NULL,47,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','A1B01FD2-872E-4A6B-A834-9F230342A1C1',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('D2C47096-9AF5-44D1-8CD2-A0ECEE5A8C42','D1103B14-2196-48C9-BB93-DC6DE65962C2','Structural Steel','',0.00,NULL,66,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','D2C47096-9AF5-44D1-8CD2-A0ECEE5A8C42',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('D3BD6589-552B-464B-AE4D-76AEB609DE11','D1103B14-2196-48C9-BB93-DC6DE65962C2','External Railings Contractor','',0.00,NULL,35,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','D3BD6589-552B-464B-AE4D-76AEB609DE11',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('7BACE765-F941-4C33-A797-79DE17303864','D1103B14-2196-48C9-BB93-DC6DE65962C2','Exterior Doors/Hardware','',0.00,NULL,32,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','7BACE765-F941-4C33-A797-79DE17303864',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('D51655E7-3060-454D-930A-87F47B064312','D1103B14-2196-48C9-BB93-DC6DE65962C2','Cabinet Contractor','',0.00,NULL,25,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','D51655E7-3060-454D-930A-87F47B064312',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('65421AA4-89FE-412D-8D42-8E9AC05C1420','D1103B14-2196-48C9-BB93-DC6DE65962C2','Sheet metal','',0.00,NULL,58,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','65421AA4-89FE-412D-8D42-8E9AC05C1420',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('740AA1D7-7113-43ED-AB01-902EB8E9E603','D1103B14-2196-48C9-BB93-DC6DE65962C2','Precast Contractor','',0.00,NULL,55,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','740AA1D7-7113-43ED-AB01-902EB8E9E603',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('0115D846-31A3-405E-8C23-92906DF2EF77','D1103B14-2196-48C9-BB93-DC6DE65962C2','Elevator Contractor','',0.00,NULL,31,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','0115D846-31A3-405E-8C23-92906DF2EF77',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('47269347-037D-41DB-BF70-93D49C36A275','D1103B14-2196-48C9-BB93-DC6DE65962C2','Internal Railings Contractor','',0.00,NULL,50,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','47269347-037D-41DB-BF70-93D49C36A275',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('28BE9922-A0D4-44CA-8393-5E603CD0610F','D1103B14-2196-48C9-BB93-DC6DE65962C2','Wine Storage Contractor','',0.00,NULL,69,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','28BE9922-A0D4-44CA-8393-5E603CD0610F',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('FF3C03B9-D3BF-4B56-B2BE-6326BC1D8459','D1103B14-2196-48C9-BB93-DC6DE65962C2','Solar Contractor','',0.00,NULL,63,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','FF3C03B9-D3BF-4B56-B2BE-6326BC1D8459',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('21578F57-31ED-420F-BFC1-FD77098A2BB2','D1103B14-2196-48C9-BB93-DC6DE65962C2','Gutters','',0.00,NULL,44,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','21578F57-31ED-420F-BFC1-FD77098A2BB2',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('6ECF6DBD-171F-4978-857E-F8545D997164','D1103B14-2196-48C9-BB93-DC6DE65962C2','Shower Enclosure/Mirror Contractor','',0.00,NULL,61,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','6ECF6DBD-171F-4978-857E-F8545D997164',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('E3AD07E4-1F16-4D2D-B305-D04FD0B1CBE7','D1103B14-2196-48C9-BB93-DC6DE65962C2','Tile Contractor','',0.00,NULL,67,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','E3AD07E4-1F16-4D2D-B305-D04FD0B1CBE7',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('986CBDC3-BE93-406A-BABC-E2D14BA882DB','D1103B14-2196-48C9-BB93-DC6DE65962C2','Fireplace  Contractor','',0.00,NULL,38,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','986CBDC3-BE93-406A-BABC-E2D14BA882DB',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('68B3FFFD-D37F-481B-8303-E43A00A0799E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Roofing Contractor','',0.00,NULL,56,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','68B3FFFD-D37F-481B-8303-E43A00A0799E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('416FDC24-339A-499D-9BDE-E4BE5EF19ED9','D1103B14-2196-48C9-BB93-DC6DE65962C2','Heating & Air Contractor','',0.00,NULL,45,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','416FDC24-339A-499D-9BDE-E4BE5EF19ED9',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('075F528A-C636-4B6E-9CC7-E729D0C0EE9E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Framing Contractor','',0.00,NULL,41,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','075F528A-C636-4B6E-9CC7-E729D0C0EE9E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('C207C3CD-494C-462F-903C-E75F8C8A9707','D1103B14-2196-48C9-BB93-DC6DE65962C2','Finish Carpentry','Install: Finish Material, bath accessories, interior doors',0.00,NULL,36,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','C207C3CD-494C-462F-903C-E75F8C8A9707',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('01A4DD1E-36BF-427E-8F28-E803351E48FE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Exterior Finish Contractor','',0.00,NULL,33,'851EFB5A-DE4F-4380-A264-AB31BB3ACECD','01A4DD1E-36BF-427E-8F28-E803351E48FE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('271AA5BC-F5B2-4F56-94FB-D37D7305DA08','D1103B14-2196-48C9-BB93-DC6DE65962C2','Appliances','Owner to Provide',0.00,NULL,17,'C00DF57F-C09B-4967-BC93-E2330E2F303F','271AA5BC-F5B2-4F56-94FB-D37D7305DA08',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('B325DD9D-D055-40A6-9139-75EB7B19C7DB','D1103B14-2196-48C9-BB93-DC6DE65962C2','Lumber/Hardware','',0.00,NULL,33,'C00DF57F-C09B-4967-BC93-E2330E2F303F','B325DD9D-D055-40A6-9139-75EB7B19C7DB',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('92883FEC-DFDF-4BF3-A033-94F129C42272','D1103B14-2196-48C9-BB93-DC6DE65962C2','Countertop Material','Owner to Provide. Supplier Moda in San Clemente, Deniz- 949-244-4839',0.00,NULL,21,'C00DF57F-C09B-4967-BC93-E2330E2F303F','92883FEC-DFDF-4BF3-A033-94F129C42272',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('9B2BBA0C-90BC-4892-A064-98D8D5838182','D1103B14-2196-48C9-BB93-DC6DE65962C2','Flooring Material','',0.00,NULL,30,'C00DF57F-C09B-4967-BC93-E2330E2F303F','9B2BBA0C-90BC-4892-A064-98D8D5838182',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('AE56314B-7AC7-49FE-A2E0-A53ADBFC6332','D1103B14-2196-48C9-BB93-DC6DE65962C2','Finish Material','',0.00,NULL,29,'C00DF57F-C09B-4967-BC93-E2330E2F303F','AE56314B-7AC7-49FE-A2E0-A53ADBFC6332',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('CFE6AFFF-4A8E-47BA-B24A-DFF9E6B43E3E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Exterior Doors/Windows','"Budget Freindly windows Andersen 100 or Milgard.  Doors- La Cantina higher end and Windor or Milgard for budget friendly."',0.00,NULL,25,'C00DF57F-C09B-4967-BC93-E2330E2F303F','CFE6AFFF-4A8E-47BA-B24A-DFF9E6B43E3E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('EDD0EE18-A505-4A74-8ADF-C57517FAE7F1','D1103B14-2196-48C9-BB93-DC6DE65962C2','Front Door','',0.00,NULL,31,'C00DF57F-C09B-4967-BC93-E2330E2F303F','EDD0EE18-A505-4A74-8ADF-C57517FAE7F1',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('C5FF414E-8F3D-498B-BFC3-A9FC0BB7BD4B','D1103B14-2196-48C9-BB93-DC6DE65962C2','Electrical Fixtures','Owner to Provide: Sconces, Pendants, Fans, Chandeliers',0.00,NULL,23,'C00DF57F-C09B-4967-BC93-E2330E2F303F','C5FF414E-8F3D-498B-BFC3-A9FC0BB7BD4B',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('BA5C7B3A-B441-4958-857D-6A0D940CF118','D1103B14-2196-48C9-BB93-DC6DE65962C2','Bath accessories','Owner to Provide: Towel Bars, Mirrors, Toilet paper holders, Hardware',0.00,NULL,19,'C00DF57F-C09B-4967-BC93-E2330E2F303F','BA5C7B3A-B441-4958-857D-6A0D940CF118',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('DF7ACF8B-9156-4CF8-A3D2-589A7953466E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Interior doors/Windows','',0.00,NULL,32,'C00DF57F-C09B-4967-BC93-E2330E2F303F','B0B192E0-0CC8-42AC-96CC-4A77ADCA09E5',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('3C32D1DE-A305-4F38-B22B-5B5FE0C3EA04','D1103B14-2196-48C9-BB93-DC6DE65962C2','Finish Hardware','Owner to Provide: Cabinet Hardware',0.00,NULL,27,'C00DF57F-C09B-4967-BC93-E2330E2F303F','3C32D1DE-A305-4F38-B22B-5B5FE0C3EA04',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('CDD4AFAB-20D3-40B2-B53F-3E31773F729E','D1103B14-2196-48C9-BB93-DC6DE65962C2','Tile Material','',0.00,NULL,35,'C00DF57F-C09B-4967-BC93-E2330E2F303F','CDD4AFAB-20D3-40B2-B53F-3E31773F729E',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('D31868A9-EB0A-4202-A7D3-4396F5E0B741','D1103B14-2196-48C9-BB93-DC6DE65962C2','Plumbing Fixtures','',0.00,NULL,34,'C00DF57F-C09B-4967-BC93-E2330E2F303F','D31868A9-EB0A-4202-A7D3-4396F5E0B741',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('1B99337E-A152-477D-37DD-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Water Heater','Upgrade',0.00,NULL,75,'C00DF57F-C09B-4967-BC93-E2330E2F303F','1B99337E-A152-477D-37DD-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('FD41B3FA-DF36-4DE1-37DE-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Sauna','',0.00,NULL,76,'C00DF57F-C09B-4967-BC93-E2330E2F303F','FD41B3FA-DF36-4DE1-37DE-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('43F6C83E-44FC-4D9F-37DF-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Epoxy Garage Floor','',0.00,NULL,77,'C00DF57F-C09B-4967-BC93-E2330E2F303F','43F6C83E-44FC-4D9F-37DF-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
INSERT INTO [dbo].[ProposalTemplatesLineItems] ([Id],[ProposalTemplateId],[Name],[Description],[Amount],[Percentage],[Sequence],[ParentId],[EstimateCategoryId],[IsDeleted],[CreatedBy],[DateCreated]) VALUES ('5671CD57-D2AD-40A6-37E1-08DCEFC578AE','D1103B14-2196-48C9-BB93-DC6DE65962C2','Panel upgrade','',0.00,NULL,99,'C00DF57F-C09B-4967-BC93-E2330E2F303F','5671CD57-D2AD-40A6-37E1-08DCEFC578AE',0,1,'2024-10-28 13:23:42');
END


-- Contract --
IF NOT EXISTS (SELECT 1 FROM [Contract] WHERE [Name] = 'Cost Plus Contract Template')
BEGIN
    INSERT INTO [Contract](Id, [Name], [BodyTemplate], [DefaultFolderName], DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Cost Plus Contract Template', '', 'CostPlusContract', GETDATE(), 1, GETDATE(), 1)
END

IF NOT EXISTS (SELECT 1 FROM [Contract] WHERE [Name] = 'Fixed Fee Contract Template')
BEGIN
	INSERT INTO [Contract](Id, [Name], [BodyTemplate], [DefaultFolderName], DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Fixed Fee Contract Template', '', 'FixedFeeContract', GETDATE(), 1, GETDATE(), 1)
END

IF NOT EXISTS (SELECT 1 FROM [Contract] WHERE [Name] = 'Fixed Cost Contract Template')
BEGIN
	INSERT INTO [Contract](Id, [Name], [BodyTemplate], [DefaultFolderName], DateCreated, CreatedById, DateModified, ModifiedById)
	VALUES (NEWID(), 'Fixed Cost Contract Template', '', 'FixedCostContract', GETDATE(), 1, GETDATE(), 1)
END
