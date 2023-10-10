Use LtSysDb1
Create Table Dispatch(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL UNIQUE,
	[Dispatcher] [nvarchar](max) NULL,
	[RefNo] [nvarchar](50), 
	[Date] [DateTime] NOT NULL DEFAULT GETDATE(),
	[MarketType] [nvarchar](400) NOT NULL,
	[AccountId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Account(Id),
	[SubAccountId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Account(Id),
	[ContactId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Contact(Id),
	[OrderId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES [Order](Id),
	[Notify] [nvarchar](max) NULL,
	[Consignee] [nvarchar](max) NULL,
	[DeliveryDate] [DateTime] NULL,
	[DeliveryStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES DeliveryStatus(Id),
	[DeliveryTerms] [nvarchar](max) NULL,
	[PaymentTerms] [nvarchar](max) NULL,
	[SpecialTerms] [nvarchar](max) NULL,
	[Remark] [nVarChar](max) NULL,
	[LineItemsAmount] AS (dbo.fnDispatchLineItemsAmount(Id)),
	[LineItemsQuantity] AS (dbo.fnDispatchLineItemsQuantity(Id)),
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
	[TimeStamp] [Timestamp] NOT NUll)on [Primary];

	ALTER TABLE [dbo].[Dispatch] ADD CONSTRAINT UQ_Dispatch_Code_OrgId UNIQUE(Code, OrgId);
	ALTER TABLE [dbo].[Dispatch] WITH CHECK ADD  CONSTRAINT [CK_Dispatch_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

Use LtSysDb1
Create Table DispatchLineItem(
	[Id] [UniqueIdentifier] Primary Key DEFAULT NEWSEQUENTIALID(),
	[DisptachId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [Dispatch](Id),
	[OrderLineItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [OrderLineItem](Id),
	[OrderId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [Order](Id),
	[PriceBookId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES PriceBook(Id),
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[Serial] [int] NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Quantity] [decimal] (18, 4) NOT NULL,
	[Weight] [decimal] (18, 4) NULL,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[LineItemAmount] AS (dbo.fnGetOrderLineItemPrice(OrderId, PriceBookId, ProductId)) * Quantity,
	[Remark] [nvarchar](max),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];

	
ALTER TABLE [dbo].[DispatchLineItem]
	ADD CONSTRAINT FK_DispatchLineItem_OrderLineItem FOREIGN KEY (OrderId, PriceBookId, ProductId)  
      REFERENCES OrderLineItem (OrderId, PriceBookId, ProductId);


ALTER TABLE [dbo].[DispatchLineItem] WITH CHECK ADD  CONSTRAINT [CK_DispatchLineItem_ModifiedBy] CHECK (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

ALTER TABLE [dbo].[DispatchLineItem] ADD CONSTRAINT UQ_OrderLineItemId_DispatchId UNIQUE(OrderLineItemId, DispatchId);