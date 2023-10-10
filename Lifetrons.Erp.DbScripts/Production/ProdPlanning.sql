
Create Table ProcessTimeConfig(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[ProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[FromDate] [Date] NOT NULL DEFAULT GETDATE(),
	[ToDate] [Date] NOT NULL DEFAULT GETDATE(),
	[StartTime] [Time] NOT NULL DEFAULT GETDATE(),
	[EndTime] [Time] NOT NULL DEFAULT GETDATE(),
	[WorkHours] AS (DATEDIFF(hour, StartTime, EndTime)),
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
	
	ALTER TABLE [dbo].[ProcessTimeConfig] WITH CHECK ADD  CONSTRAINT [CK_ProcessTimeConfig_ModifiedBy] CHECK  ([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1));
	ALTER TABLE [dbo].[ProcessTimeConfig] WITH CHECK ADD  CONSTRAINT [CK_ProcessTimeConfig_FromDate] CHECK  ([dbo].[CheckProcessTimeConfig]([Id],[ProcessId],[FromDate])=(1));

	
	
Create Table ProdPlanDetail(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	--[ProdPlanId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES ProdPlan(Id),
	[ProcessId] [UniqueIdentifier]  NOT NULL FOREIGN KEY REFERENCES Process(Id),
	[Serial] [int] NULL,
	[OrderLineItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [OrderLineItem](Id),
	[OrderId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [Order](Id),
	[PriceBookId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES PriceBook(Id),
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[JobNo] [decimal] (18) NOT NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[JobType] [nvarchar](1) DEFAULT 'I', 
	[CycleTimeInHour] [decimal] (18, 4) NOT NULL DEFAULT 1,
	[CycleCapacity] [decimal] (18, 4) NOT NULL DEFAULT 1,
	[QuantityPerHour] AS ([CycleCapacity] / [CycleTimeInHour]),
	[SetupTimeInHrs] [decimal](18, 2) NOT NULL DEFAULT 0,
	[AddOnTimeInHrs] [decimal](18, 2) NOT NULL DEFAULT 0,
	[StartDateTime] [DateTime] NOT NULL DEFAULT GETDATE(),
	[EndDateTime] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ActualStartDateTime] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ActualEndDateTime] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Remark] [nvarchar](max),
	[IsRawBookingDone][Bit] NOT NULL DEFAULT 0,
	[IsIssuedForProduction][Bit] NOT NULL DEFAULT 0,
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

			
ALTER TABLE [dbo].[ProdPlanDetail]
	ADD CONSTRAINT FK_ProdPlanDetail_OrderLineItem FOREIGN KEY (OrderId, PriceBookId, ProductId)  
      REFERENCES OrderLineItem (OrderId, PriceBookId, ProductId);

ALTER TABLE [dbo].[ProdPlanDetail] WITH CHECK ADD  CONSTRAINT [CK_ProdPlanDetail_ProdPlanRawBooking] CHECK  (([dbo].[CheckProdPlanRawBooking]([Id])=(1)));
ALTER TABLE [dbo].[ProdPlanDetail] WITH CHECK ADD  CONSTRAINT [CK_ProdPlanDetail_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

	
	--Execute constraints checks after creation of both mastre detail tables
--ALTER TABLE [dbo].[ProdPlan] WITH CHECK ADD  CONSTRAINT [CK_ProdPlan_CheckProdPlanProcess] CHECK  (([dbo].[CheckProdPlanProcess](0,[Id],[ProcessId])=(1)));
--ALTER TABLE [dbo].[ProdPlanDetail] WITH CHECK ADD  CONSTRAINT [CK_ProdPlanDetail_CheckProdPlanProcess] CHECK  (([dbo].[CheckProdPlanProcess](1,[ProdPlanId],[ProcessId])=(1)));


	
Create Table ProdPlanRawBooking(
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ProdPlanDetailId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES ProdPlanDetail(Id),
	[JobNo] [decimal] (18) NOT NULL FOREIGN KEY REFERENCES OrderLineItem(JobNo),
	[ItemId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Item(Id),
	[Quantity] [Decimal](18,4) NOT NULL DEFAULT 1,
	[Weight] [decimal] (18, 4) NULL DEFAULT 0,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
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

ALTER TABLE [dbo].[ProdPlanRawBooking] WITH CHECK ADD  CONSTRAINT [CK_ProdPlanRawBooking_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
