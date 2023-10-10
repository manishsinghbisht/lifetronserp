
--Leads can be converted to accounts, contact, opportunities, and followup tasks.
--You should only convert a lead once you have identified it as qualified.
--After this lead has been converted, it can no longer be viewed or edited as a lead, but can be viewed in lead reports.



USE [EasySales]
GO

/****** Object:  Table [dbo].[Lead]    Script Date: 5/15/2014 3:10:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Lead](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[PrimaryPhone] [nvarchar](400) NULL,
	[PrimaryEMail] [nvarchar](400) NULL,
	[CompanyName] [nvarchar](400) NULL,
	[Department] [nvarchar](400) NULL,
	[Title] [nvarchar](400) NULL,
	[AnnualRevenue] [Decimal](18, 4) NULL,
	[NumberOfEmployees] [Decimal](18, 4) NULL,
	[CampaignId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Campaign(Id),
	[LeadSourceId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES LeadSource(Id),
	[LeadStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES LeadStatus(Id),
	[RatingId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Rating(Id),
	[IsConverted][Bit] NOT NULL DEFAULT 0,
	[AddressId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Address(Id),
	[AddressToName] [nvarchar](400) NULL,
	[AddressLine1] [nvarchar](400) NULL,
	[AddressLine2] [nvarchar](400) NULL,
	[AddressLine3] [nvarchar](400) NULL,
	[AddressCity] [nvarchar](400) NULL,
	[AddressPin] [nvarchar](400) NULL,
	[AddressState] [nvarchar](400) NULL,
	[AddressCountry] [nvarchar](400) NULL,
	[AddressPhone] [nvarchar](400) NULL,
	[AddressEMail] [nvarchar](400) NULL,
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Lead]
  ADD CONSTRAINT UQ_Lead_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Lead]
  ADD CONSTRAINT UQ_Lead_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Lead] WITH CHECK 
	ADD  CONSTRAINT [CK_Lead_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

