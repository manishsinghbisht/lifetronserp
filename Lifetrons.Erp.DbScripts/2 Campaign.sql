USE [EasySales]
GO

/****** Object:  Table [dbo].[Campaign]    Script Date: 5/16/2014 10:45:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Campaign](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[NumberOfEmployees] [Decimal](10) NULL,
	[EmployeeDetails] [nvarchar](max) NULL,
	[CampaignTypeId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES CampaignType(Id),
	[CampaignStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES CampaignStatus(Id),
	[StartDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[EndDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ExpectedRevenue] [Decimal](18, 4) NULL,
	[BudgetCost] [Decimal](18, 4) NULL,
	[ActualCost] [Decimal](18, 4) NULL,
	[ExpectedResponsePercent] [Decimal](18, 4) NULL,
	[NumSent] [Decimal](18, 4) NULL,
	[ParentCampaignId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Campaign(Id),
	[Delivery] [nvarchar](max) NULL,
	[Competitors] [nvarchar](max) NULL,
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[CustomColumn2] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL)

GO

ALTER TABLE [dbo].[Campaign]
  ADD CONSTRAINT UQ_Campaign_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Campaign]
  ADD CONSTRAINT UQ_Campaign_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Campaign] WITH CHECK 
	ADD  CONSTRAINT [CK_Campaign_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));