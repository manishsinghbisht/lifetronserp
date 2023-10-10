USE [EasySales]
GO

/****** Object:  Table [dbo].[LeadStatus]    Script Date: 5/15/2014 2:10:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LeadStatus](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[LeadStatus]
  ADD CONSTRAINT UQ_LeadStatus_Name UNIQUE(Name);

ALTER TABLE [dbo].[LeadStatus]
  ADD CONSTRAINT UQ_LeadStatus_Code UNIQUE(Code);