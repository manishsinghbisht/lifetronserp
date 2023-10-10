USE [EasySales]
GO

/****** Object:  Table [dbo].[CaseReason]    Script Date: 5/19/2014 5:30:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CaseReason](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[CaseReason]
  ADD CONSTRAINT UQ_CaseReason_Name UNIQUE(Name);

ALTER TABLE [dbo].[CaseReason]
  ADD CONSTRAINT UQ_CaseReason_Code UNIQUE(Code);