USE [EasySales]
GO

/****** Object:  Table [dbo].[OpportunityType]    Script Date: 5/15/2014 6:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OpportunityType](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1)

GO
ALTER TABLE [dbo].[OpportunityType]
  ADD CONSTRAINT UQ_OpportunityType_Name UNIQUE(Name);

ALTER TABLE [dbo].[OpportunityType]
  ADD CONSTRAINT UQ_OpportunityType_Code UNIQUE(Code);