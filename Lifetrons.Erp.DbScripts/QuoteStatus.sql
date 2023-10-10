USE [EasySales]
GO

/****** Object:  Table [dbo].[QuoteStatus]    Script Date: 5/19/2014 10:50:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuoteStatus](
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[QuoteStatus]
  ADD CONSTRAINT UQ_QuoteStatus_Name UNIQUE(Name);

ALTER TABLE [dbo].[QuoteStatus]
  ADD CONSTRAINT UQ_QuoteStatus_Code UNIQUE(Code);