USE [LtSysDb1]
GO

/****** Object:  Table [dbo].[EnterpriseStage]    Script Date: 5/15/2014 6:300:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EnterpriseStage](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[DepartmentId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Department(Id),
	[Type] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[EnterpriseStage]
  ADD CONSTRAINT UQ_EnterpriseStage_Name UNIQUE(Name);

ALTER TABLE [dbo].[EnterpriseStage]
  ADD CONSTRAINT UQ_EnterpriseStage_Code UNIQUE(Code);