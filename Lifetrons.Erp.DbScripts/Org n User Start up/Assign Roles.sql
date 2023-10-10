USE [EasySales]
GO


--Assign 'canEdit' role
INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('',
           'c00d1c98-ef4d-4ca1-aef2-9af07c6acf1d')
GO

--Assign 'canAuthorize' role
INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('',
           'a0a4bf84-20e6-4bee-8b45-53739a7a5ef1')
GO

--Assign 'PriceBookManager' role
INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('',
           '7839e78f-7652-453f-b518-ac8b47daa39f')
		   

		   --Assign 'TeamLevel' role
INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('',
           'ac108b43-e1cb-4818-98b6-2ff046353b6d')
GO
		   	   --Assign 'DepartmentLevel' role
INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('',
           '99db0bb1-fa46-4d81-bc1a-4e0849871452')

GO
		   	   --Assign 'OrganizationLevel' role
INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
     VALUES
           ('',
           'e682acbe-d7d5-4b62-bd1e-3c8ad3dbb388')
GO
		   	   