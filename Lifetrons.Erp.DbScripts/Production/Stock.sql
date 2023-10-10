Create Table StockReceiptHead(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[RefNo] [nvarchar](50), 
	[JobType] [nvarchar](1), 
	[Date] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ReceiptByEnterpriseStageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES EnterpriseStage(Id),
	[ReceiptByProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[ReceiptFromEnterpriseStageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES EnterpriseStage(Id),
	[ReceiptFromProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[EmployeeId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Employee(Id),
	[Remark] [nvarchar](max),
	[ProcurementOrderId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES ProcurementOrder(Id),
	[OrderId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES [Order](Id),
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
	ALTER TABLE [dbo].[StockReceiptHead] WITH CHECK ADD  CONSTRAINT [CK_StockReceiptHead_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

Create Table StockItemReceipt(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[StockReceiptId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES StockReceiptHead(Id),
	[Serial] [int] NULL,
	[ItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Item(Id),
	[JobNo] [decimal] (18) NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[CaseNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Case](CaseNo),
	[TaskNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Task](TaskNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ExtraQuantity] [decimal](18, 4) Default 0, 
	[Expenses] [decimal](18, 4) Default 0, 
	[Remark] [nVarChar](max),
	[CustomColumn1] [nvarchar](max),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];
	
	ALTER TABLE [dbo].[StockItemReceipt] ADD CONSTRAINT UQ_StockItemReceipt_StockReceiptId_ItemId UNIQUE(StockReceiptId, ItemId);
	ALTER TABLE [dbo].[StockItemReceipt] WITH CHECK ADD  CONSTRAINT [CK_StockItemReceipt_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	
	Create Table StockProductReceipt(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[StockReceiptId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES StockReceiptHead(Id),
	[Serial] [int] NULL,
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[JobNo] [decimal] (18) NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[CaseNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Case](CaseNo),
	[TaskNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Task](TaskNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ExtraQuantity] [decimal](18, 4) Default 0, 
	[Expenses] [decimal](18, 4) Default 0, 
	[Remark] [nVarChar](max),
	[CustomColumn1] [nvarchar](max),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];
	
	ALTER TABLE [dbo].[StockProductReceipt] ADD CONSTRAINT UQ_StockProductReceipt_StockReceiptId_ProductId UNIQUE(StockReceiptId, ProductId);
	ALTER TABLE [dbo].[StockProductReceipt] WITH CHECK ADD  CONSTRAINT [CK_StockProductReceipt_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

Create Table StockIssueHead(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[RefNo] [nvarchar](50), 
	[JobType] [nvarchar](1), 
	[Date] [DateTime] NOT NULL DEFAULT GETDATE(),
	[IssuedByEnterpriseStageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES EnterpriseStage(Id),
	[IssuedByProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[IssuedToEnterpriseStageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES EnterpriseStage(Id),
	[IssuedToProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[EmployeeId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Employee(Id),
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
	ALTER TABLE [dbo].[StockIssueHead] WITH CHECK ADD  CONSTRAINT [CK_StockIssueHead_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

	
Create Table StockItemIssue(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[StockIssueId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES StockIssueHead(Id),
	[Serial] [int] NULL,
	[ItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Item(Id),
	[JobNo] [decimal] (18) NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[CaseNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Case](CaseNo),
	[TaskNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Task](TaskNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ExtraQuantity] [decimal](18, 4) Default 0, 
	[DeliveryDate] [DateTime] NULL DEFAULT GETDATE(),
	[Remark] [nVarChar](max),
	[CustomColumn1] [nvarchar](max),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];
	
	ALTER TABLE [dbo].[StockItemIssue] ADD CONSTRAINT UQ_StockItemIssue_StockIssueId_ItemId UNIQUE(StockIssueId, ItemId);
	ALTER TABLE [dbo].[StockItemIssue] WITH CHECK ADD  CONSTRAINT [CK_StockItemIssue_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	ALTER TABLE [dbo].[StockItemIssue] WITH CHECK ADD  CONSTRAINT [CK_StockItemIssue_CheckQuantity] CHECK  ([dbo].[CheckStockIssue]([Id],[StockIssueId],NULL,[ItemId],[Quantity],[Weight])=(1));

	Create Table StockProductIssue(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[StockIssueId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES StockIssueHead(Id),
	[Serial] [int] NULL,
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[JobNo] [decimal] (18) NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[CaseNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Case](CaseNo),
	[TaskNo] [decimal] (18) NULL FOREIGN KEY REFERENCES [Task](TaskNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ExtraQuantity] [decimal](18, 4) Default 0, 
	[DeliveryDate] [DateTime] NULL DEFAULT GETDATE(),
	[Remark] [nVarChar](max),
	[CustomColumn1] [nvarchar](max),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];
	
	ALTER TABLE [dbo].[StockProductIssue] ADD CONSTRAINT UQ_StockProductIssue_StockIssueId_ProductId UNIQUE(StockIssueId, ProductId);
	ALTER TABLE [dbo].[StockProductIssue] WITH CHECK ADD  CONSTRAINT [CK_StockProductIssue_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	ALTER TABLE [dbo].[StockProductIssue] WITH CHECK ADD  CONSTRAINT [CK_StockProductIssue_CheckQuantity] CHECK  ([dbo].[CheckStockIssue]([Id],[StockIssueId],[ProductId],NULL,[Quantity],[Weight])=(1));

