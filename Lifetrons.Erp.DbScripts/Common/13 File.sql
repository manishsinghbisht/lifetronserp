
/****** Object:  Table [dbo].[File]    Script Date: 11/04/2017 11:30:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[File](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

	[FileType] [nvarchar](100) NOT NULL, -- CV, Image, Video, Transcription
	[TemplateName] [nvarchar](400),
	[TemplateUrl] [nvarchar](max),
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[SubAccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[RateType] [nvarchar](100) NULL DEFAULT 'STANDARD', -- STANDARD, SPECIAL 
	[Rate] [decimal] (18,2) NULL DEFAULT 0,

	[FilePath] [nvarchar](max) NULL,
	[FileName] [nvarchar](400) NOT NULL,
	[FileExtension][nvarchar](100),
	[FileSentDate] [DateTime] NULL DEFAULT GETDATE(),
	[RcvdFileEmailSubject][nvarchar](400),
	[RcvdFileEmailBody] [nvarchar](max) NULL, 
	[FileCode][nvarchar](100), 
	[FileRefNo][nvarchar](100), 

	[Tags] [nvarchar](max) NULL,
	[NumberOfPagesRecieved][decimal](3),
	[SubmittedFilePath] [nvarchar](max) NULL,
	[SubmittedFileName] [nvarchar](400) NULL,
	[NumberOfPagesSubmitted][decimal](3),
	[SubmittedDate] [DateTime] NULL DEFAULT GETDATE(),
	[SubmittedFileEmailBody] [nvarchar](max) NULL, 

	[DeliveredFileEmailSubject][nvarchar](400), 
	[DeliveredDate] [DateTime] NULL DEFAULT GETDATE(),

	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[Status] [nvarchar](200) NULL DEFAULT 'Queued', -- Queued, Assigned, Review, Approved, Submitted, Delivered, Rejected
	[IsVerified] [Bit] NULL DEFAULT 0,	

	[Origin][nvarchar](max),
	[OriginCode][nvarchar](max), 
	[OriginSenderEmail] [nvarchar](max), 
	[OriginCCEmails] [nvarchar](max), 
	[ProcessorCCEmails] [nvarchar](max), 
	[OriginApproverEmail] [nvarchar](max), 
	[ProcessorEmail] [nvarchar](max), 
	[ProcessorId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ApproverEmail] [nvarchar](max), 
	[ApproverId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[IsApproved] [Bit] NULL DEFAULT 0,	
	[BackupEmail] [nvarchar](max), 
	
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

ALTER TABLE [dbo].[File] WITH CHECK 
	ADD  CONSTRAINT [CK_File_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));


GO
	CREATE TABLE [dbo].[FileRateTable](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[FileType] [nvarchar](100) NOT NULL, -- CV, Image, Video, Transcription
	[TemplateName] [nvarchar](400), 
	[TemplateUrl] [nvarchar](max), 
	[TemplateUpdateDate] [DateTime] DEFAULT GETDATE(),
	[ContactId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Contact(Id),
	[AccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[SubAccountId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Account(Id),
	[RateType] [nvarchar](100) NULL DEFAULT 'STANDARD', -- STANDARD, SPECIAL 
	[Rate] [decimal] (18,2) NULL DEFAULT 0,
	[StartDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[EndDate] [DateTime],
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
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
ALTER TABLE [dbo].[FileRateTable]
  ADD CONSTRAINT UQ_FileRateTable_ContactId_TemplateName UNIQUE(ContactId, TemplateName);

ALTER TABLE [dbo].[FileRateTable] WITH CHECK 
	ADD  CONSTRAINT [CK_FileRateTable_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

	

	 
