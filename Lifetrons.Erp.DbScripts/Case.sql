USE [EasySales]
GO

/****** Object:  Table [dbo].[Case]    Script Date: 5/20/2014 08:30:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Case](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[CaseNo] [decimal] (18) NOT NULL UNIQUE IDENTITY,
	[ShrtDesc] [nvarchar](400) NULL,
	[Subject] [nvarchar](400) NULL,
	[RefNo] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[InternalComments] [nvarchar](max) NULL,
	[PriorityId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Priority(Id),
	[CaseReasonId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES CaseReason(Id),
	[OpeningDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[CaseStatusId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES CaseStatus(Id),
	[OwnerId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[AssignedToId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[ProductId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Product(Id),
	[CampaignId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Campaign(Id),
	[OpportunityId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Opportunity(Id),
	[QuoteId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Quote(Id),
	[OrderId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES [Order](Id),
	[InvoiceId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Invoice(Id),
	[ReportCompletionToId] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
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
	[CustomColumn1] [nvarchar](max),
	[ColExtensionId] [UniqueIdentifier] NULL,
	[TimeStamp] [Timestamp] NOT NUll)

GO

ALTER TABLE [dbo].[Case]
  ADD CONSTRAINT UQ_Case_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Case]
  ADD CONSTRAINT UQ_Case_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Case] WITH CHECK 
	ADD  CONSTRAINT [CK_Case_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));