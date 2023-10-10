USE [EasySales]
GO

/****** Object:  Table [dbo].[CampaignStatus]    Script Date: 5/15/2014 6:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CampaignStatus](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Serial] [int] NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1)

GO
ALTER TABLE [dbo].[CampaignStatus]
  ADD CONSTRAINT UQ_CampaignStatus_Name UNIQUE(Name);

ALTER TABLE [dbo].[CampaignStatus]
  ADD CONSTRAINT UQ_CampaignStatus_Code UNIQUE(Code);