USE [EasySales]
GO

/****** Object:  Table [dbo].[Product]    Script Date: 4/21/2014 3:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Product](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[Weight] [decimal] (18, 4) NULL,
	[WeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ProductFamilyId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES ProductFamily(Id),
	[ProductTypeId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES ProductType(Id),
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[CustomColumn2] [nvarchar](max),
	[CustomColumn3] [nvarchar](max),
	[CustomColumn4] [nvarchar](max),
	[CustomColumn5] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL)
GO
ALTER TABLE [dbo].[Product]
  ADD CONSTRAINT UQ_Product_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Product]
  ADD CONSTRAINT UQ_Product_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Product]  WITH CHECK 
	ADD  CONSTRAINT [CK_Product_ModifiedBy] CHECK (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
	
GO