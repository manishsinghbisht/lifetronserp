USE [EasySales]
GO

/****** Object:  Table [dbo].[Contact]    Script Date: 5/15/2014 5:45:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contact](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[NamePrefix] [nvarchar](25) NULL,
	[FirstName] [nvarchar](400) NULL,
	[MiddleName] [nvarchar](400) NULL,
	[LastName] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Remark] [nvarchar](max) NULL,
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[PreferredOwnerId] [nvarchar](128) FOREIGN KEY REFERENCES AspNetUsers(Id),
	[Title] [nvarchar](400) NULL,
	[Department] [nvarchar](400) NULL,
	[ReportsTo] [UniqueIdentifier] NULL,
	[PrimaryPhone] [nvarchar](400) NULL,
	[PrimaryEMail] [nvarchar](400) NULL,
	[CompanyName] [nvarchar](400) NULL,
	[LeadSourceId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES LeadSource(Id),
	[IsProspect] [Bit] NOT NULL DEFAULT 0,
	[IsEndorsement] [Bit] NOT NULL DEFAULT 0,
	[IsEmployee] [Bit] NOT NULL DEFAULT 0,
	[MailingAddressId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Address(Id),
	[MailingAddressToName] [nvarchar](400) NULL,
	[MailingAddressLine1] [nvarchar](400) NULL,
	[MailingAddressLine2] [nvarchar](400) NULL,
	[MailingAddressLine3] [nvarchar](400) NULL,
	[MailingAddressCity] [nvarchar](400) NULL,
	[MailingAddressPin] [nvarchar](400) NULL,
	[MailingAddressState] [nvarchar](400) NULL,
	[MailingAddressCountry] [nvarchar](400) NULL,
	[MailingAddressPhone] [nvarchar](400) NULL,
	[MailingAddressEMail] [nvarchar](400) NULL,
	[OtherAddressId] [UniqueIdentifier]  NULL FOREIGN KEY REFERENCES Address(Id),
	[OtherAddressToName] [nvarchar](400) NULL,
	[OtherAddressLine1] [nvarchar](400) NULL,
	[OtherAddressLine2] [nvarchar](400) NULL,
	[OtherAddressLine3] [nvarchar](400) NULL,
	[OtherAddressCity] [nvarchar](400) NULL,
	[OtherAddressPin] [nvarchar](400) NULL,
	[OtherAddressState] [nvarchar](400) NULL,
	[OtherAddressCountry] [nvarchar](400) NULL,
	[OtherAddressPhone] [nvarchar](400) NULL,
	[OtherAddressEMail] [nvarchar](400) NULL,
	[LevelId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Level(Id),
	[Birthdate] [DateTime] NULL,
	[AnniversaryDate] [DateTime] NULL,
	[LeadId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Lead(Id),
	[CampaignId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Campaign(Id),
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
	[CustomColumn3] [nvarchar](max),
	[CustomColumn4] [nvarchar](max),
	[CustomColumn5] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL)

GO

ALTER TABLE [dbo].[Contact]
  ADD CONSTRAINT UQ_Contact_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Contact]
  ADD CONSTRAINT UQ_Contact_Code_OrgId UNIQUE(Code, OrgId);

  ALTER TABLE [dbo].[Contact] 
	ADD  CONSTRAINT [FK_Contact_ReportsTo] FOREIGN KEY (ReportsTo) REFERENCES Contact(Id);

ALTER TABLE [dbo].[Contact] WITH CHECK 
	ADD  CONSTRAINT [CK_Contact_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));