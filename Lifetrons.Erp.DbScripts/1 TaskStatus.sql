USE [EasySales]
GO

/****** Object:  Table [dbo].[TaskStatus]    Script Date: 6/9/2014 12:30:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TaskStatus](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[TaskStatus]
  ADD CONSTRAINT UQ_TaskStatus_Name UNIQUE(Name);

ALTER TABLE [dbo].[TaskStatus]
  ADD CONSTRAINT UQ_TaskStatus_Code UNIQUE(Code);