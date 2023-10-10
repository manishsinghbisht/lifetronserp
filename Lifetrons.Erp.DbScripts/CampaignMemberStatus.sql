USE [EasySales]
GO

/****** Object:  Table [dbo].[CampaignMemberStatus]    Script Date: 5/15/2014 2:10:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CampaignMemberStatus](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[CampaignMemberStatus]
  ADD CONSTRAINT UQ_CampaignMemberStatus_Name UNIQUE(Name);

ALTER TABLE [dbo].[CampaignMemberStatus]
  ADD CONSTRAINT UQ_CampaignMemberStatus_Code UNIQUE(Code);