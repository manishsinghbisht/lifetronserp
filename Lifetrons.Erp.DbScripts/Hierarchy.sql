USE [EasySales]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Table [dbo].[Department]    Script Date: 7/15/2014 2:00:00 PM ******/

CREATE TABLE [dbo].[Department](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1);

ALTER TABLE [dbo].[Department] ADD CONSTRAINT UQ_Department_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Department] ADD CONSTRAINT UQ_Department_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Department] WITH CHECK ADD  CONSTRAINT [CK_Department_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

	GO

/****** Object:  Table [dbo].[Team]    Script Date: 7/15/2014 2:00:00 PM ******/

CREATE TABLE [dbo].[Team](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[Name] [nvarchar](400) NOT NULL,
	[Code] [nvarchar](400) NOT NULL,	
	[ShrtDesc] [nvarchar](400) NULL,
	[DepartmentId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Department(Id),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1);

ALTER TABLE [dbo].[Team] ADD CONSTRAINT UQ_Team_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[Team] ADD CONSTRAINT UQ_Team_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[Team] ADD CONSTRAINT UQ_Team_Id_DepartmentId_OrgId UNIQUE(Id, DepartmentId);

ALTER TABLE [dbo].[Team] WITH CHECK ADD  CONSTRAINT [CK_Team_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));

	GO

/****** Object:  Table [dbo].[Hierarchy]    Script Date: 7/15/2014 2:00:00 PM ******/

CREATE TABLE [dbo].[Hierarchy](
	[UserId] [nvarchar](128) PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id),
	[DepartmentId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Department(Id),
	[TeamId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Team(Id),
	[ReportsTo] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[Designation] [nvarchar](400) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id))

GO

