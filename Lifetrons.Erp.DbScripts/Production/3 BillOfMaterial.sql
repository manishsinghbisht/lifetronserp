Create Table BOM
	(
	[ProductId] [UniqueIdentifier] FOREIGN KEY REFERENCES Product(Id) PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[OtherCharges] [decimal](18,4) NULL,
	[Amount] AS (dbo.fnBOMLineItemsAmount(ProductId) + ISNULL([OtherCharges],0)),
	[OwnerId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];

	ALTER TABLE [dbo].[BOM] ADD CONSTRAINT UQ_BOM_Name_OrgId UNIQUE(Name, OrgId);
	ALTER TABLE [dbo].[BOM] ADD CONSTRAINT UQ_BOM_Code_OrgId UNIQUE(Code, OrgId);
	ALTER TABLE [dbo].[BOM] WITH CHECK ADD  CONSTRAINT [CK_BOM_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));


Create Table BOMLineItem
(
	[Id] [UniqueIdentifier] NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID(), 
	[ProductId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES BOM(ProductId),
	[ItemId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Item(Id),
	[Serial] [int] NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[Rate] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Amount] AS Rate * Quantity,
	[AllowedLossQuantity] [Decimal](18,4) NOT NULL DEFAULT 0,
	[AllowedLossWeight] [decimal] (18, 4) NULL DEFAULT 0,
	[AllowedLossWeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];

	ALTER TABLE [dbo].[BOMLineItem]  ADD CONSTRAINT PK_BOMLineItem_ProductId_ItemId PRIMARY KEY (ProductId, ItemId);
	ALTER TABLE [dbo].[BOMLineItem] WITH CHECK ADD  CONSTRAINT [CK_BOMLineItem_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
