
USE [EasySales]
GO

/****** Object:  Table [dbo].[EmailConfig]    Script Date: 5/15/2014 2:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailConfig](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[UserId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[Name] [nvarchar](25) NOT NULL, --Name to be displayed in Email
	[SmtpUsername] [nvarchar](400) NOT NULL, --Email
	[SmtpPassword] [nvarchar](400) NULL, 
	[SmtpHost] [nvarchar](400) NULL,
	[SmtpPort] [int] NULL,
	[Pop3Username] [nvarchar](400) NOT NULL, --Email
	[Pop3Password] [nvarchar](400) NULL,
	[Pop3Host] [nVarchar](400) NULL,
	[Pop3Port] [int] NULL,
	[Ssl] [Bit] NOT NULL DEFAULT 0,
	[Tls] [Bit] NOT NULL DEFAULT 0,
	[IsPrimary][Bit] NOT NULL DEFAULT 1,
	[HeskSettings] [nVarChar](max) NULL,
	[OtherSettings] [nVarChar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1)

GO
ALTER TABLE [dbo].[EmailConfig]
  ADD CONSTRAINT UQ_EmailConfig_UserId UNIQUE(UserId);

