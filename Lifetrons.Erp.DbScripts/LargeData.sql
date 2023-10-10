CREATE TABLE [dbo].[LargeData](
	[Id] [UniqueIdentifier] PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[ParentId] [UniqueIdentifier] NOT NULL,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,
	[Desc] [nvarchar](max) NULL,
	[Notes] [ntext] NULL,
	[BinaryData] [VarBinary](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[TimeStamp] [Timestamp] NOT NUll) ON [Large]
GO

ALTER TABLE [dbo].[LargeData]
  ADD CONSTRAINT UQ_LargeData_Name_OrgId UNIQUE(Name, OrgId);

ALTER TABLE [dbo].[LargeData]
  ADD CONSTRAINT UQ_LargeData_Code_OrgId UNIQUE(Code, OrgId);

ALTER TABLE [dbo].[LargeData] WITH CHECK 
	ADD  CONSTRAINT [CK_LargeData_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
