USE [EasySales]
GO

/****** Object:  Table [dbo].[Stage]    Script Date: 5/15/2014 6:300:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Stage](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[Stage]
  ADD CONSTRAINT UQ_Stage_Name UNIQUE(Name);

ALTER TABLE [dbo].[Stage]
  ADD CONSTRAINT UQ_Stage_Code UNIQUE(Code);