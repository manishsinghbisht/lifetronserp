USE [EasySales]
GO

/****** Object:  Table [dbo].[CampaignType]    Script Date: 5/15/2014 6:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CampaignType](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1)

GO
ALTER TABLE [dbo].[CampaignType]
  ADD CONSTRAINT UQ_CampaignType_Name UNIQUE(Name);

ALTER TABLE [dbo].[CampaignType]
  ADD CONSTRAINT UQ_CampaignType_Code UNIQUE(Code);