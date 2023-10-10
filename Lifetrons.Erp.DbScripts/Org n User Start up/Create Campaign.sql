USE [EasySales]
GO
 @UserId nVarChar;
@OrgId UniqueIdentifier;

INSERT INTO [dbo].[Campaign]
           ([Id]
           ,[Name]
           ,[Code]
           ,[ShrtDesc]
           ,[Desc]
           ,[OwnerId]
           ,[NumberOfEmployees]
           ,[EmployeeDetails]
           ,[CampaignTypeId]
           ,[CampaignStatusId]
           ,[StartDate]
           ,[EndDate]
           ,[ExpectedRevenue]
           ,[BudgetCost]
           ,[ActualCost]
           ,[ExpectedResponsePercent]
           ,[NumSent]
           ,[ParentCampaignId]
           ,[Delivery]
           ,[Competitors]
           ,[OrgId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[Authorized]
           ,[Active]
           ,[CustomColumn1]
           ,[CustomColumn2]
           ,[ColExtensionId])
     VALUES
           (NEWSEQUENTIALID()
           ,'Regular'
           ,'Regular'
           ,'Regular'
           ,'Regular'
           ,@UserId
           ,NULL
           ,NULL
           ,'EF94666F-14AA-4F74-8E49-C1B0ACCCB75A'
           ,'9229DEFB-9913-4F08-A438-DCBD6B551FD9'
           ,GETDATE()
           ,GETDATE()
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,@OrgId
           ,@UserId
           ,GETDATE()
           ,@UserId
           ,GETDATE()
           ,1
           ,1
           ,NULL
           ,NULL
           ,NULL)
GO


