USE [EasySales]
GO

/****** Object:  Table [dbo].[Contract]    Script Date: 5/19/2014 11:02:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contract](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ContractNo] [nvarchar](25) NULL,
	[RefNo] [nvarchar](400) NULL,
	[OpportunityId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Opportunity(Id),
	[QuoteId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Quote(Id),
	[AccountId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Account(Id),
	[PriceBookId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES PriceBook(Id),
	[CustSignedById] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[CustSignedByTitle] [nvarchar](400) NULL,
	[CustSignedByDate] [DateTime] NULL,
	[ContractStartDate] [DateTime] NULL,
	[ContractTenure] [Decimal] (18,4),
	[ContractExpirationDate] [DateTime] NULL DEFAULT GETDATE(),
	[ExpirationNotice] [Decimal] (18,4),
	[CompanySignedById] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CompanySignedDate] [DateTime] NULL,
	[DeliveryTerms] [nvarchar](max) NULL,
	[PaymentTerms] [nvarchar](max) NULL,
	[SpecialTerms] [nvarchar](max) NULL,
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
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Contract]
  ADD CONSTRAINT UQ_Contract_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Contract]
  ADD CONSTRAINT UQ_Contract_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Contract] WITH CHECK 
	ADD  CONSTRAINT [CK_Contract_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));