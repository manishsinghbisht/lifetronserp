USE [LtSysDb1]
GO

/****** Object:  Table [dbo].[Process]    Script Date: 5/15/2014 6:300:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Process](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[EnterpriseStageId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES EnterpriseStage(Id),
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
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[Serial] [int] NULL)

GO

ALTER TABLE [dbo].[Process]
  ADD CONSTRAINT UQ_Process_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Process] WITH CHECK 
	ADD  CONSTRAINT [CK_Process_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));