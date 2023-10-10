Use EasySales

CREATE TABLE [dbo].[ProductImage](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[Name] [nvarchar](400) UNIQUE NOT NULL,
	[Tag] [nvarchar](400) NULL,
	[Desc] [nvarchar](400) NULL,
	[BinaryData] [VarBinary](max) NOT NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[TimeStamp] [Timestamp] NOT NUll) ON [Large]
GO

ALTER TABLE [dbo].[ProductImage]
  ADD CONSTRAINT UQ_ProductImage_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[ProductImage] WITH CHECK 
	ADD  CONSTRAINT [CK_ProductImage_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
