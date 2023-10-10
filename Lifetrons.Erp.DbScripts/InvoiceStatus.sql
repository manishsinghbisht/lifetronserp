USE [EasySales]
GO

/****** Object:  Table [dbo].[InvoiceStatus]    Script Date: 5/20/2014 8:50:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceStatus](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1, 
	[Serial] [int] NULL)

GO
ALTER TABLE [dbo].[InvoiceStatus]
  ADD CONSTRAINT UQ_InvoiceStatus_Name UNIQUE(Name);

ALTER TABLE [dbo].[InvoiceStatus]
  ADD CONSTRAINT UQ_InvoiceStatus_Code UNIQUE(Code);