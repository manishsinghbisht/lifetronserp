USE [EasySales]
GO

/****** Object:  Table [dbo].[Organization]    Script Date: 4/15/2014 2:58:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Organization](
	[Id] [UniqueIdentifier] NOT NULL,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](4) NOT NULL UNIQUE,
	[ShortName] [nvarchar](400) UNIQUE,
	[Slogan] [nvarchar](400) UNIQUE,
	[Phone1] [nvarchar](max) NULL,
	[Phone2] [nvarchar](max) NULL,
	[Email1] [nvarchar](max) NULL,
	[Email2] [nvarchar](max) NULL,
	[AddressLine1] [nvarchar](max) NULL,
	[AddressLine2] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[PostalCode] [nvarchar](max) NULL,
	[ApproverEMail] [nvarchar](max) NULL,
	[ApproverPhone] [nvarchar](max) NULL,
	[TIN] [nvarchar](400) NULL UNIQUE,
	[RegistrationNumber] [nvarchar](400) NULL UNIQUE,
	[Logo][binary] NULL,
	[Trademark][binary] NULL,
	[ImagePath] [nvarchar](max) NULL,
	[ExportLicense] [nvarchar](max) NULL,
	[ImportLicense] [nvarchar](max) NULL,
	[Website] [nvarchar](400) NULL UNIQUE,
	[ServiceURI] [nvarchar](max) NULL,
	[FAX] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] DEFAULT 0,
	[Active][Bit] DEFAULT 1,
	[CustomColumn1] [nvarchar](max),
	[CustomColumn2] [nvarchar](max)
	
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Organization] FOREIGN KEY([OrgId])
REFERENCES [dbo].[Organization] ([Id])
GO
