USE [EasySales]
GO

/****** Object:  Table [dbo].[Target]    Script Date: 08/14/2014 08:50:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Target](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[ObjectName] [nvarchar](400) NOT NULL DEFAULT 'USER', --ORGANIZATION, DEPARTMENT, TEAM, USER 
	[ObjectId] [UniqueIdentifier] NOT NULL,
	[TargetDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[TargetFigure] [decimal] (18, 4) NOT NULL DEFAULT 0,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[ClosingComments] [nvarchar](max) NULL,
	[SharedWith] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Target]
  ADD CONSTRAINT UQ_Target_ObjectId_TargetDate UNIQUE(ObjectId, TargetDate);

ALTER TABLE [dbo].[Target] WITH CHECK 
	ADD  CONSTRAINT [CK_Target_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

