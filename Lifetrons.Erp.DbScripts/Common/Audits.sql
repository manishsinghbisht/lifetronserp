USE [EasySales]
GO

/****** Object:  Table [dbo].[Audits]    Script Date: 5/15/2014 2:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


Create TABLE [dbo].[Audits](
	[AuditId] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[SessionId] [nVarchar](400) NULL,
	[IpAddress] [nVarchar](400) NULL,
	[UserName] [nVarchar](400) NULL, 
	[UrlAccessed] [nVarChar](400) NULL,
	[TimeAccessed] [DateTime] NULL,
	[Data] [nVarChar](max)) ON [LARGE]

GO