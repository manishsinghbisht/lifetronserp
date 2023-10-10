USE [EasySales]
GO

INSERT INTO [dbo].[ProductFamily]
           ([Id]
           ,[Name]
           ,[Code]
           ,[ShrtDesc]
           ,[Desc]
           ,[OrgId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[Authorized]
           ,[Active])
     VALUES
           (NEWSEQUENTIALID()
           ,'Family 1'
           ,'Family 1'
           ,'Product Family 1'
           ,'Product Family 1'
           ,@OrgId
           ,@UserId
           ,GETDATE()
           ,@UserId
           ,GETDATE()
           ,1
           ,1)
GO


USE [EasySales]
GO

INSERT INTO [dbo].[ProductType]
           ([Id]
           ,[Name]
           ,[Code]
           ,[ShrtDesc]
           ,[Desc]
           ,[OrgId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[Authorized]
           ,[Active])
     VALUES
           (NEWSEQUENTIALID()
           ,'Type 1'
           ,'Type 1'
           ,'Product Type 1'
           ,'Product Type 1'
           ,@OrgId
           ,@UserId
           ,GETDATE()
           ,@UserId
           ,GETDATE()
           ,1
           ,1)
GO




