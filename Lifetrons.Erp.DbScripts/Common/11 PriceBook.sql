USE [EasySales]
GO

/****** Object:  Table [dbo].[PriceBook]    Script Date: 4/22/2014 09:00:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PriceBook](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit]  NOT NULL DEFAULT 0,
	[Active][Bit]  NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[CustomColumn2] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL)
GO
ALTER TABLE [dbo].[PriceBook]
  ADD CONSTRAINT UQ_PriceBook_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[PriceBook]
  ADD CONSTRAINT UQ_PriceBook_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[PriceBook] WITH CHECK 
	ADD  CONSTRAINT [CK_PriceBook_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	