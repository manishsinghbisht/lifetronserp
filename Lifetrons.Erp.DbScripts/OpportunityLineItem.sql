USE [EasySales]
GO

/****** Object:  Table [dbo].[OpportunityLineItem]    Script Date: 5/19/2014 5:02:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OpportunityLineItem](
	[Id] [UniqueIdentifier] NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID(),
	[OpportunityId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Opportunity(Id),
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
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[OpportunityLineItem]
  ADD CONSTRAINT PK_OpportunityLineItem_OpportunityId_PriceBookId_ProductId PRIMARY KEY (OpportunityId,	PriceBookId, ProductId);

ALTER TABLE [dbo].[OpportunityLineItem]
	ADD CONSTRAINT FK_OpportunityLineItem_PriceBookLineItem FOREIGN KEY (PriceBookId, ProductId)  
      REFERENCES PriceBookLineItem (PriceBookId, ProductId);

ALTER TABLE [dbo].[OpportunityLineItem] WITH CHECK 
	ADD  CONSTRAINT [CK_OpportunityLineItem_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));