USE [EasySales]
GO

/****** Object:  Table [dbo].[Industry]    Script Date: 5/15/2014 9:00:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Industry](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1)

GO
ALTER TABLE [dbo].[Industry]
  ADD CONSTRAINT UQ_Industry_Name UNIQUE(Name);

ALTER TABLE [dbo].[Industry]
  ADD CONSTRAINT UQ_Industry_Code UNIQUE(Code);