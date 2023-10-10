USE [EasySales]
GO

INSERT INTO [dbo].[Department]
           ([Id]
           ,[Name]
           ,[Code]
           ,[ShrtDesc]
           ,[OrgId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[Authorized]
           ,[Active])
     VALUES
           (NEWSEQUENTIALID()
           ,'Department 1'
           ,'Department 1'
           ,'Department 1'
           ,@OrgId
           ,@UserId
           ,GETDATE()
           ,@UserId
           ,GETDATE()
           ,1
           ,1)
GO






