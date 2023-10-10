USE [EasySales]
GO

/****** Object:  Table [dbo].[Opportunity]    Script Date: 5/16/2014 10:45:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Opportunity](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[OpportunityNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,
	[Remark] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[Private] [Bit] NOT NULL DEFAULT 1,
	[CampaignId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Campaign(Id),
	[LeadId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Lead(Id), 
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[OpportunityTypeId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES OpportunityType(Id),
	[LeadSourceId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES LeadSource(Id),
	[LineItemsAmount] AS (dbo.fnOpportunityLineItemsAmount(Id)),
	[LineItemsQuantity] AS (dbo.fnOpportunityLineItemsQuantity(Id)),
	[RefNo] [nvarchar](50) NULL,
	[OrderNo] [nvarchar](50) NULL,
	[NumberOfEmployees] [Decimal](10) NULL,
	[ExpectedRevenue] [Decimal](18,4) NULL,
	[CloseDate] [DateTime] NULL,
	[NextStep] [nvarchar](max) NULL,
	[StageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Stage(Id),
	[ProbabilityPercent] [Decimal](18, 4) NULL,
	[DeliveryStatusId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES DeliveryStatus(Id),
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
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Opportunity]
  ADD CONSTRAINT UQ_Opportunity_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Opportunity]
  ADD CONSTRAINT UQ_Opportunity_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Opportunity] WITH CHECK 
	ADD  CONSTRAINT [CK_Opportunity_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));