USE [EasySales]
GO

/****** Object:  Table [dbo].[LeadSource]    Script Date: 5/15/2014 2:10:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LeadSource](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1)

GO
ALTER TABLE [dbo].[LeadSource]
  ADD CONSTRAINT UQ_LeadSource_Name UNIQUE(Name);

ALTER TABLE [dbo].[LeadSource]
  ADD CONSTRAINT UQ_LeadSource_Code UNIQUE(Code);