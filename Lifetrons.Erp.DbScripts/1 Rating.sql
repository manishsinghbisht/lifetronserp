USE [EasySales]
GO

/****** Object:  Table [dbo].[Rating]    Script Date: 5/15/2014 2:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Rating](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](25) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] DEFAULT 1,
	[Active][Bit] DEFAULT 1)

GO
ALTER TABLE [dbo].[Rating]
  ADD CONSTRAINT UQ_Rating_Name_OrgId UNIQUE(Name);

ALTER TABLE [dbo].[Rating]
  ADD CONSTRAINT UQ_Rating_Code_OrgId UNIQUE(Code);