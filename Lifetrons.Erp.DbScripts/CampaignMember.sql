USE [EasySales]
GO

/****** Object:  Table [dbo].[CampaignMember]    Script Date: 5/18/2014 02:19:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CampaignMember](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[CampaignId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Campaign(Id),
	[LeadId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Lead(Id),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[CampaignMemberStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES CampaignMemberStatus(Id),
	[ShrtDesc] [nVarChar] (400) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max))

GO

ALTER TABLE [dbo].[CampaignMember]
  ADD CONSTRAINT UQ_CampaignMember_CampaignId_LeadId_ContactId UNIQUE(CampaignId, LeadId, ContactId);

ALTER TABLE [dbo].[CampaignMember] WITH CHECK 
	ADD  CONSTRAINT [CK_CampaignMember_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));