/* After Insert trigger on organization table */

USE [EasySales]
GO

IF OBJECT_ID('T_AfterInsertOrganization') IS NOT NULL
DROP TRIGGER [dbo].T_AfterInsertOrganization
GO

CREATE TRIGGER [dbo].T_AfterInsertOrganization ON [dbo].Organization
AFTER INSERT AS

BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
  SET NOCOUNT ON;

  -- get the last id value of the record inserted or updated
  DECLARE @OrgId UniqueIdentifier
  DECLARE @UserId nVarChar(128)
  
  SELECT @OrgId = [Id] FROM INSERTED
  SELECT @UserId = [ModifiedBy] FROM INSERTED

	--INSERT INTO ORGANIZATION_BACKUP SELECT * FROM INSERTED
	--Roles are inserted in Organization controller itseld. no need to do it here

	DECLARE @DepartmentId UniqueIdentifier
	SET @DepartmentId = NEWID()
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
			   (@DepartmentId
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
	
	INSERT INTO [dbo].[Team]
			   ([Id]
			   ,[Name]
			   ,[Code]
			   ,[ShrtDesc]
			   ,[DepartmentId]
			   ,[OrgId]
			   ,[CreatedBy]
			   ,[CreatedDate]
			   ,[ModifiedBy]
			   ,[ModifiedDate]
			   ,[Authorized]
			   ,[Active])
		 VALUES
			   (NEWID()
			   ,'Team 1'
			   ,'Team 1'
			   ,'Team 1 of Department 1'
			   ,@DepartmentId
			   ,@OrgId
			   ,@UserId
			   ,GETDATE()
			   ,@UserId
			   ,GETDATE()
			   ,1
			   ,1)

	DECLARE @ProductFamilyId UniqueIdentifier
  	SET @ProductFamilyId = NEWID()
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
			   (@ProductFamilyId
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

	DECLARE @ProductTypeId UniqueIdentifier
	SET @ProductTypeId = NEWID()
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
			   (@ProductTypeId
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

	INSERT INTO [dbo].[Product]
           ([Id]
           ,[Name]
           ,[Code]
           ,[ShrtDesc]
           ,[Desc]
           ,[ProductFamilyId]
           ,[ProductTypeId]
           ,[OrgId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[Authorized]
           ,[Active])
     VALUES
           (NEWID()
           ,'Sample Product 1'
           ,'Sample Product 1'
           ,'Sample Product 1'
           ,null
           ,@ProductFamilyId
           ,@ProductTypeId
           ,@OrgId
           ,@UserId
           ,GETDATE()
           ,@UserId
           ,GETDATE()
           ,1
           ,1)

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
           ,[ColExtensionId]
           ,[SharedWith])
     VALUES
           (NEWID()
           ,'Regular'
           ,'Regular'
           ,'Regular'
           ,null
           ,@UserId
           ,null
           ,null
           ,'EF94666F-14AA-4F74-8E49-C1B0ACCCB75A'
           ,'9229DEFB-9913-4F08-A438-DCBD6B551FD9'
           ,GETDATE()
           ,GETDATE()
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,@OrgId
           ,@UserId
           ,GETDATE()
           ,@UserId
           ,GETDATE()
           ,1
           ,1
           ,null
           ,null
           ,null
           ,null)

END

GO