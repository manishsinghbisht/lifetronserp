USE [EasySales]
GO

/****** Object:  Table [dbo].[Quote]    Script Date: 5/19/2014 11:02:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Quote](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[QuoteNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,
	[ShrtDesc] [nvarchar](400) NULL,
	[Remark] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[RefNo] [nvarchar](50) NULL,
	[OpportunityId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Opportunity(Id),
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[QuoteStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES QuoteStatus(Id),
	[FollowUpDate] [DateTime] NULL DEFAULT GETDATE(),
	[ExpirationDate] [DateTime] NULL DEFAULT GETDATE(),
	[PaymentTerms] [nvarchar](max) NULL,
	[DeliveryTerms] [nvarchar](max) NULL,
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
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Quote]
  ADD CONSTRAINT UQ_Quote_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Quote]
  ADD CONSTRAINT UQ_Quote_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Quote] WITH CHECK 
	ADD  CONSTRAINT [CK_Quote_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));