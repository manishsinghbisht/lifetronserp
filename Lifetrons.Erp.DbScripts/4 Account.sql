USE [EasySales]
GO

/****** Object:  Table [dbo].[Account]    Script Date: 5/15/2014 9:00:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Account](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Remark] [nvarchar](max) NULL,
	[IsSupplier][Bit] NOT NULL DEFAULT 0,
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[Priority] [nvarchar](25) NULL DEFAULT 'MEDIUM',
	[AccountTypeId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES AccountType(Id),
	[IndustryId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Industry(Id),
	[OwnershipId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES [Ownership](Id),
	[AnnualRevenue] [Decimal](18, 4) NULL,
	[NumberOfEmployees] [Decimal](10) NULL,
	[NumberOfLocations] [Decimal](10) NULL,
	[RatingId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Rating(Id),
	[AgreementSerialNumber] [nvarchar](400) NULL,
	[AgreementExpDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[LeadId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Lead(Id),
	[CampaignId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Campaign(Id),
	[BillingAddressId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Address(Id),
	[BillingAddressToName] [nvarchar](400) NULL,
	[BillingAddressLine1] [nvarchar](400) NULL,
	[BillingAddressLine2] [nvarchar](400) NULL,
	[BillingAddressLine3] [nvarchar](400) NULL,
	[BillingAddressCity] [nvarchar](400) NULL,
	[BillingAddressPin] [nvarchar](400) NULL,
	[BillingAddressState] [nvarchar](400) NULL,
	[BillingAddressCountry] [nvarchar](400) NULL,
	[BillingAddressPhone] [nvarchar](400) NULL,
	[BillingAddressEMail] [nvarchar](400) NULL,
	[ShippingAddressId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Address(Id),
	[ShippingAddressToName] [nvarchar](400) NULL,
	[ShippingAddressLine1] [nvarchar](400) NULL,
	[ShippingAddressLine2] [nvarchar](400) NULL,
	[ShippingAddressLine3] [nvarchar](400) NULL,
	[ShippingAddressCity] [nvarchar](400) NULL,
	[ShippingAddressPin] [nvarchar](400) NULL,
	[ShippingAddressState] [nvarchar](400) NULL,
	[ShippingAddressCountry] [nvarchar](400) NULL,
	[ShippingAddressPhone] [nvarchar](400) NULL,
	[ShippingAddressEMail] [nvarchar](400) NULL,
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
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Account]
  ADD CONSTRAINT UQ_Account_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Account]
  ADD CONSTRAINT UQ_Account_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Account] WITH CHECK 
	ADD  CONSTRAINT [CK_Account_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));