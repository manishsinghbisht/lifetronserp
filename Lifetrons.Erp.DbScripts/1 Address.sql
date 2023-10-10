USE [EasySales]
GO

/****** Object:  Table [dbo].[Address]    Script Date: 5/14/2014 9:15:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Address](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[AddressType] [nvarchar](100) NOT NULL,
	[AddressToName] [nvarchar](400) NULL,
	[AddressToTitle] [nvarchar](400) NULL,
	[AddressToEMail] [nvarchar](400) NULL,
	[AddressLine1] [nvarchar](400) NOT NULL,
	[AddressLine2] [nvarchar](400) NOT NULL,
	[AddressLine3] [nvarchar](400) NOT NULL,
	[Milestone] [nvarchar](max) NULL,
	[City] [nvarchar](100) NULL,
	[State] [nvarchar](100) NULL,
	[PostalCode] [nvarchar](100) NULL,
	[Country] [nvarchar](100) NOT NULL,
	[Website] [nvarchar](400) NULL,
	[ServiceURI] [nvarchar](max) NULL,
	[EMail] [nvarchar](400) NULL,
	[Mobile] [nvarchar](400) NULL,
	[Phone1] [nvarchar](400) NULL,
	[Phone2] [nvarchar](400) NULL,
	[FAX] [nvarchar](400) NULL,
	[FallBack] [nvarchar](max) NULL,
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit]  NOT NULL DEFAULT 1,
	[ColExtensionId] [UniqueIdentifier] NULL)

GO

ALTER TABLE [dbo].[Address]
  ADD CONSTRAINT UQ_Address_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Address]
  ADD CONSTRAINT UQ_Address_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Address] WITH CHECK 
	ADD  CONSTRAINT [CK_Address_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));