USE [EasySales]
GO

/****** Object:  Table [dbo].[Task]    Script Date: 8/8/2014 5:15:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Task](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,
	[TaskNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[LeadId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Lead(Id),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[RelatedToObjectName] [nvarchar] (100) NULL,
	[RelatedToId] [UniqueIdentifier] NULL,
	[StartDate] [DateTime] NULL DEFAULT GETDATE(),
	[EndDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[IsAllDay][Bit] NOT NULL DEFAULT 0,
	[TaskStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES TaskStatus(Id),
	[PriorityId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES [Priority](Id),
	[Reminder] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ProgressPercent] [Decimal](18, 4) NULL,
	[ProgressDesc] [nvarchar](max) NULL,
	[ReportCompletionToId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[SharedWith] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Task]
  ADD CONSTRAINT UQ_Task_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Task]
  ADD CONSTRAINT UQ_Task_Code_OrgId UNIQUE(Code, OrgId);


ALTER TABLE [dbo].[Task] WITH CHECK 
	ADD  CONSTRAINT [CK_Task_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));