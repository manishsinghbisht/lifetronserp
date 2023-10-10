
/****** Object:  Table [dbo].[PriceBookLineItem]    Script Date: 4/23/2014 3:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PriceBookLineItem](
	[Id] [UniqueIdentifier] NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID(), 
	[PriceBookId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES PriceBook(Id),
	[ProductId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Product(Id),
	[ListPrice] [Decimal](18, 4) NOT NULL DEFAULT 0,	
	[ShrtDesc] [nvarchar](400) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[PriceBookLineItem]
  ADD CONSTRAINT PK_PriceBookLineItem_PriceBookId_ProductId PRIMARY KEY (PriceBookId, ProductId);

ALTER TABLE [dbo].[PriceBookLineItem] WITH CHECK 
	ADD  CONSTRAINT [CK_PriceBookLineItem_ModifiedBy] CHECK  (([dbo].[fncCheckUserProductOrg]([ModifiedBy],[ProductId],[OrgId])=(1)));