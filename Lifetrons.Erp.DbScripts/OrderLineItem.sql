USE [EasySales]
GO

/****** Object:  Table [dbo].[OrderLineItem]    Script Date: 5/21/2014 07:30:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OrderLineItem](
	[Id] [UniqueIdentifier] UNIQUE DEFAULT NEWSEQUENTIALID(),
	[OrderId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [Order](Id),
	[PriceBookId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES PriceBook(Id),
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[SalesPrice] [decimal] (18, 4) NOT NULL,
	[Quantity] [decimal] (18, 4) NOT NULL,
	[Weight] [decimal] (18, 4) NULL,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[DiscountPercent] [decimal] (18, 4),
	[DiscountAmount] AS ([SalesPrice]*(ISNULL([DiscountPercent],0)/(100))),
	[LineItemPrice] AS ([SalesPrice]-[SalesPrice]*(ISNULL([DiscountPercent],0)/(100))),
	[LineItemAmount] AS ([SalesPrice]-[SalesPrice]*(ISNULL([DiscountPercent],0)/(100))) * Quantity,
	[Serial] [int] NULL,
	[SpecialInstructions] [nvarchar](max),
	[ProductionQuantity] [decimal] (18, 4) NULL Default 0,
	[ProductionWeight] [decimal] (18, 4) NULL,
	[ProductionWeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[JobNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,
	[JobStatus] [decimal] (1) NOT NULL Default 0, -- [Not Started = 0, InProcess = 1, Completed = 2]
	[ProductionInstructions] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[OrderLineItem]
  ADD CONSTRAINT PK_OrderLineItem_OrderId_PriceBookId_ProductId PRIMARY KEY (OrderId, PriceBookId, ProductId);

ALTER TABLE [dbo].[OrderLineItem]
	ADD CONSTRAINT FK_OrderLineItem_PriceBookLineItem FOREIGN KEY (PriceBookId, ProductId)  
      REFERENCES PriceBookLineItem (PriceBookId, ProductId);

ALTER TABLE [dbo].[OrderLineItem] WITH CHECK 
	ADD  CONSTRAINT [CK_OrderLineItem_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));