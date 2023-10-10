USE [EasySales]
GO

/****** Object:  Table [dbo].[Order]    Script Date: 5/19/2014 05:10:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Order](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,
	[OrderNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,	
	[ShrtDesc] [nvarchar](400) NULL,
	[SearchKey] AS [SearchKey] AS ([Name] + ' '  + [Code] + ' '  + [OrderNo] + ' ' + [ShrtDesc]),
	[Remark] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ContractId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contract(Id),
	[OpportunityId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Opportunity(Id),
	[QuoteId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Quote(Id),
	[AccountId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Account(Id),
	[SubAccountId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Account(Id),
	[RefNo] [nvarchar](400) NULL,
	[PriorityId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Priority(Id),
	[CustSignedById] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[CustSignedByDate] [DateTime] NULL,
	[CompanySignedById] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[StartDate] [DateTime] NULL,
	[ActivatedDate] [DateTime] NULL,
	[ActivatedById] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[DeliveryDate] [DateTime] NULL,
	[DeliveryStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES DeliveryStatus(Id),
	[ProgressPercent] [Decimal](18, 4) NULL,
	[ProgressDesc] [nvarchar](max) NULL,
	[ReportCompletionToId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[DeliveryTerms] [nvarchar](max) NULL,
	[PaymentTerms] [nvarchar](max) NULL,
	[SpecialTerms] [nvarchar](max) NULL,
	[LineItemsAmount] AS (dbo.fnOrderLineItemsAmount(Id)),
	[LineItemsQuantity] AS (dbo.fnOrderLineItemsQuantity(Id)),
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

ALTER TABLE [dbo].[Order]
  ADD CONSTRAINT UQ_Order_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Order]
  ADD CONSTRAINT UQ_Order_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Order] WITH CHECK 
	ADD  CONSTRAINT [CK_Order_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

