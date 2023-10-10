USE [EasySales]
GO

/****** Object:  Table [dbo].[AccountType]    Script Date: 5/15/2014 9:00:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AccountType](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1)

GO
ALTER TABLE [dbo].[AccountType]
  ADD CONSTRAINT UQ_AccountType_Name UNIQUE(Name);

ALTER TABLE [dbo].[AccountType]
  ADD CONSTRAINT UQ_AccountType_Code UNIQUE(Code);