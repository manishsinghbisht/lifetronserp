
/****** Object:  Table [dbo].[Media]    Script Date: 5/11/2015 08:30:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Media](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[ParentType] [nvarchar](400) NOT NULL, -- Testimonial Id, Cricket Record Id, Profile Id etc
	[ParentId] [UniqueIdentifier] NOT NULL,

	[MediaType] [nvarchar](max) NOT NULL, -- Image, Video
	[MediaPath] [nvarchar](max) NULL,
	[MediaName] [nvarchar](400) NOT NULL,
	[Tags] [nvarchar](max) NULL,
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[IsMarkedAbuse] [Bit] NOT NULL DEFAULT 0,
	[MarkedAbuseByUserId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),

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

ALTER TABLE [dbo].[Media]
  ADD CONSTRAINT UQ_Media_MediaName_OrgId UNIQUE(MediaName, OrgId);

ALTER TABLE [dbo].[Media] WITH CHECK 
	ADD  CONSTRAINT [CK_Media_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));