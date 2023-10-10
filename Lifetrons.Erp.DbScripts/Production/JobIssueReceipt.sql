

Create Table JobIssueHead(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[RefNo] [nvarchar](50), 
	[JobType] [nvarchar](1), 
	[Date] [DateTime] NOT NULL DEFAULT GETDATE(),
	[IssuedByProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id), --IssuedBy Process
	[IssuedToProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id), --IssuedTo Process
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
	ALTER TABLE [dbo].[JobIssueHead] WITH CHECK ADD  CONSTRAINT [CK_JobIssueHead_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	
Create Table JobProductIssue(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[JobIssueId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES JobIssueHead(Id),
	[Serial] [int] NULL,
	[JobNo] [decimal] (18) NOT NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[DeliveryDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ExtraQuantity] [decimal](18, 4) Default 0, 
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
	ALTER TABLE [dbo].[JobProductIssue] ADD CONSTRAINT UQ_JobProductIssue_JobNo_JobIssueId UNIQUE(JobNo, JobIssueId);
	ALTER TABLE [dbo].[JobProductIssue] WITH CHECK ADD  CONSTRAINT [CK_JobProductIssue_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	ALTER TABLE [dbo].[JobProductIssue] WITH CHECK ADD  CONSTRAINT [CK_JobProductIssue_CheckQuantity] CHECK  ([dbo].[CheckJobProductIssue]([Id],[JobIssueId],[JobNo],[Quantity],[Weight])=(1));

		
Create Table JobItemIssue(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[JobIssueId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES JobIssueHead(Id),
	[Serial] [int] NULL,
	[JobNo] [decimal] (18) NOT NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[ItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Item(Id),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[Rate] [Decimal](18,4) NOT NULL DEFAULT 1,
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
	ALTER TABLE [dbo].[JobItemIssue] ADD CONSTRAINT UQ_JobItemIssue_JobNo_JobIssueId_ItemId UNIQUE(JobNo, JobIssueId, ItemId);
	ALTER TABLE [dbo].[JobItemIssue] WITH CHECK ADD  CONSTRAINT [CK_JobItemIssue_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	ALTER TABLE [dbo].[JobItemIssue] WITH CHECK ADD  CONSTRAINT [CK_JobItemIssue_CheckQuantity] CHECK  ([dbo].[CheckJobItemIssue]([Id],[JobIssueId],[JobNo],[ItemId],[Quantity],[Weight])=(1));

	Create Table JobReceiptHead(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[RefNo] [nvarchar](50), 
	[JobType] [nvarchar](1), 
	[Date] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ReceiptByProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id), --ReceiptBy Process
	[ReceiptFromProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id), --ReceiptBy Process
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
	ALTER TABLE [dbo].[JobReceiptHead] WITH CHECK ADD  CONSTRAINT [CK_JobReceiptHead_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

Create Table JobProductReceipt(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[JobReceiptId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES JobReceiptHead(Id),
	[Serial] [int] NULL,
	[JobNo] [decimal] (18) NOT NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ExtraQuantity] [decimal](18, 4) Default 0, 
	[LabourCharge] [decimal](18, 4) Default 0, 
	[OtherCharge] [decimal](18, 4) Default 0, 
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
	ALTER TABLE [dbo].[JobProductReceipt] ADD CONSTRAINT UQ_JobProductReceipt_JobNo_JobReceiptId UNIQUE(JobNo, JobReceiptId);
	ALTER TABLE [dbo].[JobProductReceipt] WITH CHECK ADD  CONSTRAINT [CK_JobProductReceipt_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
		
Create Table JobItemReceipt(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
	[JobReceiptId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES JobReceiptHead(Id),
	[Serial] [int] NULL,
	[JobNo] [decimal] (18) NOT NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[ItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Item(Id),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[Rate] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Rejection][Bit] NOT NULL DEFAULT 0,
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
	ALTER TABLE [dbo].[JobItemReceipt] ADD CONSTRAINT UQ_JobItemReceipt_JobNo_JobReceiptId_ItemId UNIQUE(JobNo, JobReceiptId, ItemId);
	ALTER TABLE [dbo].[JobItemReceipt] WITH CHECK ADD  CONSTRAINT [CK_JobItemReceipt_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

