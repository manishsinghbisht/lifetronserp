USE [EasySales]
GO

/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 4/15/2014 5:35:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[UserName] [nvarchar](400) NULL UNIQUE,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[AuthenticatedEmail] [nvarchar](400) NULL UNIQUE,
	[Email] [nvarchar](400) NULL UNIQUE,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[BirthDate] [datetime] NULL,
	[Mobile] [nvarchar](400) NULL UNIQUE,
	[AddressLine1] [nvarchar](max) NULL,
	[AddressLine2] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[PostalCode] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
	[Culture] [nVarChar](50) NULL,
	[TimeZone] [nVarChar](400) NOT NULL DEFAULT 'UTC',
	[OrgId] [UniqueIdentifier] NULL,
	[Active] [BIT] NOT NULL DEFAULT 0,
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE()
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO




