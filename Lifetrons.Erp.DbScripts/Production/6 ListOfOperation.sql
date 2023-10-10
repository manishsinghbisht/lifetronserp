Create Table Operation
	(
	[Id] [UniqueIdentifier] NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID(), --Keep this [Id] as unique key to use in  OperationBOMLineItem as Foreign key
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[EnterpriseStageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES EnterpriseStage(Id),
	[ProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[Serial] [int] NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[LabourRatePerHour] [decimal] (18, 4) NULL,
	[DepreciationRatePerHour] [decimal] (18, 4) NULL,
	[EnergyRatePerHour] [decimal] (18, 4) NULL,
	[OverheadRatePerHour] [decimal] (18, 4) NULL,
	[OtherDirectExpensesRatePerHour] [decimal] (18, 4) NULL,
	[OtherInDirectExpensesRatePerHour] [decimal] (18, 4) NULL,
	[RatePerHour] AS (ISNULL([LabourRatePerHour],0) + ISNULL([DepreciationRatePerHour],0) + ISNULL([EnergyRatePerHour],0) + ISNULL([OverheadRatePerHour],0) + ISNULL([OtherDirectExpensesRatePerHour],0) + ISNULL([OtherInDirectExpensesRatePerHour],0)),
	[CycleTimeInHour] [decimal] (18, 4) NOT NULL DEFAULT 1,
	[CycleCapacity] [decimal] (18, 4) NOT NULL DEFAULT 1,
	[QuantityPerHour] AS ([CycleCapacity] / [CycleTimeInHour]),
	[PerUnitCost] AS (((ISNULL([LabourRatePerHour],0) + ISNULL([DepreciationRatePerHour],0) + ISNULL([EnergyRatePerHour],0) + ISNULL([OverheadRatePerHour],0) + ISNULL([OtherDirectExpensesRatePerHour],0) + ISNULL([OtherInDirectExpensesRatePerHour],0)) * [CycleTimeInHour])/[CycleCapacity]),
	[Type] [nvarchar](400) NULL,
	[Remark] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];

	ALTER TABLE [dbo].[Operation]  ADD CONSTRAINT PK_Operation_ProductId_EnterpriseStageId_ProcessId PRIMARY KEY (ProductId, EnterpriseStageId, ProcessId);
	ALTER TABLE [dbo].[Operation] WITH CHECK ADD  CONSTRAINT [CK_Operation_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));


Create Table OperationBOMLineItem
(
	[Id] [UniqueIdentifier] NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID(), 
	[OperationId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Operation(Id),
	[ProductId] [UniqueIdentifier] NOT NULL,
	[EnterpriseStageId] [UniqueIdentifier] NOT NULL,
	[ProcessId] [UniqueIdentifier]  NOT NULL,
	[ItemId] [UniqueIdentifier]  NOT NULL,
	[Serial] [int] NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
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

	ALTER TABLE [dbo].[OperationBOMLineItem]  ADD CONSTRAINT PK_OperationBOMLineItem PRIMARY KEY (ProductId, EnterpriseStageId, ProcessId, ItemId);
	ALTER TABLE [dbo].[OperationBOMLineItem]  ADD CONSTRAINT FK_OperationBOMLineItem_Operation FOREIGN KEY(ProductId, EnterpriseStageId, ProcessId) REFERENCES Operation(ProductId, EnterpriseStageId, ProcessId);
	ALTER TABLE [dbo].[OperationBOMLineItem]  ADD CONSTRAINT FK_OperationBOMLineItem_BOMLineItem FOREIGN KEY(ProductId, ItemId) REFERENCES BOMLineItem(ProductId, ItemId);
	ALTER TABLE [dbo].[OperationBOMLineItem]  ADD CONSTRAINT UQ_ProductId_EnterpriseStageId_ProcessId_ItemId UNIQUE(ProductId, EnterpriseStageId, ProcessId, ItemId);
	ALTER TABLE [dbo].[OperationBOMLineItem] WITH CHECK ADD  CONSTRAINT [CK_OperationBOMLineItem_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
