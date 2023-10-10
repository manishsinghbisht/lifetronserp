USE [EasySales]
GO

/****** Object:  Table [dbo].[WeightUnit]    Script Date: 5/19/2014 1:20:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WeightUnit](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[WeightUnit]
  ADD CONSTRAINT UQ_WeightUnit_Name UNIQUE(Name);

ALTER TABLE [dbo].[WeightUnit]
  ADD CONSTRAINT UQ_WeightUnit_Code UNIQUE(Code);