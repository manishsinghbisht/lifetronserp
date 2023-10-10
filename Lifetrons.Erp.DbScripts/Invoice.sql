USE [EasySales]
GO

/****** Object:  Table [dbo].[Invoice]    Script Date: 6/22/2014 11:02:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Invoice](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[InvoiceNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,
	[Remark] [nvarchar](max) NULL,
	[RefNo] [nvarchar](50) NULL,
	[OwnerId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[OpportunityId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Opportunity(Id),
	[QuoteId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Quote(Id),
	[OrderId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES [Order](Id),
	[InvoiceStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES InvoiceStatus(Id),
	[InvoiceDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[PaymentReceiptDetails] [nvarchar](max) NULL,
	[LineItemsAmount] AS (dbo.fnInvoiceLineItemsAmount(Id)),
	[LineItemsQuantity] AS (dbo.fnInvoiceLineItemsQuantity(Id)),
	[TaxPercent] [decimal](18,4) NULL,
	[PostTaxAmount] AS (dbo.fnInvoiceLineItemsAmount(Id) * ((100+ISNULL([TaxPercent],0))/100)),
	[OtherCharges] [decimal](18,4) NULL,
	[InvoiceAmount] AS (dbo.fnInvoiceLineItemsAmount(Id) * ((100+ISNULL([TaxPercent],0))/100) + ISNULL([OtherCharges],0)),
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
	[PaymentTerms] [nvarchar](max) NULL,
	[OtherTerms] [nvarchar](max) NULL,
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

ALTER TABLE [dbo].[Invoice]
  ADD CONSTRAINT UQ_Invoice_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Invoice]
  ADD CONSTRAINT UQ_Invoice_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Invoice] WITH CHECK 
	ADD  CONSTRAINT [CK_Invoice_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));