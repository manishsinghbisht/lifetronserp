
USE [LtSysDb1]
GO
--DROP PROCEDURE [dbo].[spSetupOrg]

CREATE PROCEDURE [dbo].[spSetupOrg]
	
	@OrgId UniqueIdentifier,
	@UserId nVarChar(128)
AS 

BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
  SET NOCOUNT ON;

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

		   
	INSERT INTO [dbo].[PriceBook]
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
           (NEWID()
           ,'Default Price Book'
           ,'Default Price Book'
           ,'Default Price Book'
           ,null
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

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Planning'
        ,'Planning'
        ,'Planning'
        ,'846dee0e-b400-4ba8-8bc4-ea9b3651dfb8'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Planning'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,1)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('RM Stock'
        ,'RM Stock'
        ,'RM Stock'
        ,'6c146128-2b84-426e-a373-fa1a63758a05'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'RM Stock'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,1)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('FG Stock'
        ,'FG Stock'
        ,'FG Stock'
        ,'6c146128-2b84-426e-a373-fa1a63758a05'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'FG Stock'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,2)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Scrap Stock'
        ,'Scrap Stock'
        ,'Scrap Stock'
        ,'6c146128-2b84-426e-a373-fa1a63758a05'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Scrap Stock'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,3)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Procurement'
        ,'Procurement'
        ,'Procurement'
        ,'dd82da9b-ce4a-4ceb-a93f-df41873bcc09'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Procurement'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,1)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Assembly'
        ,'Assembly'
        ,'Assembly'
        ,'442e4ca7-1d3a-4589-ba3e-738b233e9b95'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Assembly'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,1)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Quality'
        ,'Quality'
        ,'Quality'
        ,'442e4ca7-1d3a-4589-ba3e-738b233e9b95'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Quality'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,2)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Packing'
        ,'Packing'
        ,'Packing'
        ,'442e4ca7-1d3a-4589-ba3e-738b233e9b95'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Packing'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,3)

		INSERT INTO [dbo].[Process]
        ([Name]
        ,[Code]
        ,[ShrtDesc]
        ,[EnterpriseStageId]
        ,[LabourRatePerHour]
        ,[DepreciationRatePerHour]
        ,[EnergyRatePerHour]
        ,[OverheadRatePerHour]
        ,[OtherDirectExpensesRatePerHour]
        ,[OtherInDirectExpensesRatePerHour]
        ,[QuantityPerHour]
        ,[CycleTimeInHour]
        ,[Desc]
        ,[OrgId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate]
        ,[Authorized]
        ,[Active]
        ,[Serial])
    VALUES
        ('Dispatch'
        ,'Dispatch'
        ,'Dispatch'
        ,'7b3b9c4e-6d41-407b-9821-f2ce8de77e28'
        ,5
        ,1
        ,2
        ,2
        ,1
        ,1
        ,5
        ,4
        ,'Dispatch'
        ,@OrgId
        ,@UserId
        ,GETDATE()
        ,@UserId
        ,GETDATE()
        ,1
        ,1
        ,1)

END
GO