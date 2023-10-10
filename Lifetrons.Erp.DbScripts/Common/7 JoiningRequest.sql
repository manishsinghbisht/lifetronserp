USE [EasySales]
GO

/****** Object:  Table [dbo].[JoiningRequest]    Script Date: 4/29/2014 11:40:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JoiningRequest](
	[Id] [uniqueidentifier] NOT NULL,
	[RequestType] [nvarchar](50) NOT NULL,
	[RequestBy] [nvarchar](128) NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[RequestComments] [nvarchar](400) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[SubmittedTo] [nvarchar](128) NULL,
	[ApprovedBy] [nvarchar](128) NULL,
	[ApprovedDate] [datetime] NULL,
	[ResponseComments] [nvarchar](400) NULL,
 CONSTRAINT [PK_JoiningRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[JoiningRequest] ADD  CONSTRAINT [DF_JoiningRequest_Status]  DEFAULT (N'Pending') FOR [Status]
GO

ALTER TABLE [dbo].[JoiningRequest]  WITH CHECK ADD  CONSTRAINT [FK_JoiningRequest_AspNetUsers] FOREIGN KEY([RequestBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[JoiningRequest] CHECK CONSTRAINT [FK_JoiningRequest_AspNetUsers]
GO

ALTER TABLE [dbo].[JoiningRequest]  WITH CHECK ADD  CONSTRAINT [FK_JoiningRequest_AspNetUsers1] FOREIGN KEY([SubmittedTo])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[JoiningRequest] CHECK CONSTRAINT [FK_JoiningRequest_AspNetUsers1]
GO

ALTER TABLE [dbo].[JoiningRequest]  WITH CHECK ADD  CONSTRAINT [FK_JoiningRequest_AspNetUsers2] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[JoiningRequest] CHECK CONSTRAINT [FK_JoiningRequest_AspNetUsers2]
GO


