CREATE TABLE [dbo].[Subscribers]
	(
	Id int PRIMARY KEY NOT NULL IDENTITY (1, 1),
	PhoneNumber nvarchar(50) NOT NULL,
	Subscribed bit NOT NULL,
	CreatedAt datetime NOT NULL,
	UpdatedAt datetime NOT NULL
	)  