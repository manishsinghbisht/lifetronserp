USE [EasySales]
GO

/****** Object:  Table [dbo].[ProductFamily]    Script Date: 4/21/2014 2:58:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductFamily](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit]  NOT NULL DEFAULT 0,
	[Active][Bit]  NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL)
GO

ALTER TABLE [dbo].[ProductFamily]
  ADD CONSTRAINT UQ_ProductFamily_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[ProductFamily]
  ADD CONSTRAINT UQ_ProductFamily_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[ProductFamily] WITH CHECK 
	ADD  CONSTRAINT [CK_ProductFamily_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));