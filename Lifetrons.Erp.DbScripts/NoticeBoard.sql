USE [EasySales]
GO

/****** Object:  Table [dbo].[NoticeBoard]    Script Date: 12/11/2014 08:00:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NoticeBoard](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[RefNo] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OpeningDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[CloseDate] [DateTime] NULL DEFAULT GETDATE(),
	[ClosingComments] [nvarchar](max) NULL,
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[NoticeBoard]
  ADD CONSTRAINT UQ_NoticeBoard_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[NoticeBoard]
  ADD CONSTRAINT UQ_NoticeBoard_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[NoticeBoard] WITH CHECK 
	ADD  CONSTRAINT [CK_NoticeBoard_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));