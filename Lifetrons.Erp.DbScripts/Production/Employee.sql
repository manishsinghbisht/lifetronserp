Create FUNCTION [dbo].[fnGetEmployeeName]
(
	  @ContactId UNIQUEIDENTIFIER
)    
RETURNS nvarchar(max)
AS
 BEGIN
 	DECLARE @Result nvarchar(max)

	Select @Result = Name  FROM Contact WHERE Contact.Id = @ContactId
 
	RETURN @Result
 END

CREATE TABLE Employee
	([Id] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Contact(Id),
	[Name] AS (dbo.fnGetEmployeeName(Id)),
	[ShrtDesc] [nvarchar](400) NULL,
	[Designation] [nvarchar](400) NULL,
	[DepartmentId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Department(Id),
	[ManagerId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Contact(Id), 
	[Grade] [decimal](2), 
	[DOJ] [DateTime] NOT NULL DEFAULT GETDATE(), 
	[DOL] [DateTime] NULL, 
	[PFFlag] [Bit] NOT NULL DEFAULT 0,
	[ESIFlag] [Bit] NOT NULL DEFAULT 0, 
	[FoodFlag] [Bit] NOT NULL DEFAULT 0,
	[FoodRate] [decimal](5,2), 
	[HourlyRate] [decimal](18, 4), 
	[Employed][Bit] NOT NULL DEFAULT 1, 
	[HasLeft] [Bit] NOT NULL DEFAULT 0, 
	[Basic] [decimal](18, 4), 
	[HRA] [decimal](18, 4), 
	[ExecA] [decimal](18, 4), 
	[ConvA] [decimal](18, 4), 
	[TeaA] [decimal](18, 4), 
	[WashA] [decimal](18, 4), 
	[CEA] [decimal](18, 4), 
	[OthA] [decimal](18, 4),
	[PFFix] [decimal](18, 4), 
	[CarLInst] [decimal](18, 4), 
	[IncomeTAXInst] [decimal](18, 4),
	[ProfTAXInst] [decimal](18, 4), 
	[EduTAXInst] [decimal](18, 4), 
	[ServiceTAXInst] [decimal](18, 4), 
	[OtherTAXInst] [decimal](18, 4), 
	[LifeInsuInst] [decimal](18, 4), 
	[HealthInsuInst] [decimal](18, 4),
	[LoanWInst] [decimal](18, 4), 
	[LoanWOInst] [decimal](18, 4), 
	[PFNo] [nvarchar](400), 
	[ESINo] [nvarchar](400), 
	[Bank] [nvarchar](400), 
	[BankAcNo] [nvarchar](400),
	[LifeInsuNo] [nvarchar](400), 
	[HealthInsuNo] [nvarchar](400), 
	[GratuityNo] [nvarchar](400), 
	[PAN] [nvarchar](400), 
	[EmpVend] [nChar](1) Default 'E',
	[DISPNo] [nvarchar](400), 
	[LastPerformanceReviewDate] [DateTime] NULL, 
	[NextPerformanceReviewDate] [DateTime] NULL, 
	[LastCompensationReviewDate] [DateTime] NULL, 
	[NextCompensationReviewDate] [DateTime] NULL, 
	[Remarks] [nvarchar](max) NULL,
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 1,
	[Active][Bit] NOT NULL DEFAULT 1);

	ALTER TABLE [dbo].[Employee]  ADD CONSTRAINT PK_Employee PRIMARY KEY (Id);
	ALTER TABLE [dbo].[Employee] WITH CHECK ADD  CONSTRAINT [CK_Employee_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));


 ALTER TABLE dbo.Employee
   ADD Name AS dbo.fnGetEmployeeName(Id)


CREATE TABLE Attendance_Old
	(	[Id] [UniqueIdentifier] Primary Key DEFAULT NEWSEQUENTIALID(),
		[EmployeeId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Employee(Id),
		AtdMonth [decimal](2) NOT NULL, 
		AtdYear [decimal](4) NOT NULL, 
		Day01 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day02 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day03 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day04 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day05 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day06 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day07 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day08 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day09 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day10 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day11 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day12 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day13 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day14 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day15 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day16 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day17 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day18 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day19 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day20 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day21 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day22 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day23 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day24 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day25 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day26 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day27 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day28 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day29 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day30 [nvarchar](2) NOT NULL DEFAULT 'H',
		Day31 [nvarchar](2) NOT NULL DEFAULT 'H',
		TotalMonthDays [decimal](2) NOT NULL DEFAULT 0, 
		WeeklyOffDays [decimal](2) NOT NULL DEFAULT 0,
		TotalOfficialWorkingDays [decimal](2) NOT NULL DEFAULT 0,
		Holidays [decimal](2) NOT NULL DEFAULT 0,
		Present [decimal](2) NOT NULL DEFAULT 0,
		HalfDays [decimal](2) NOT NULL DEFAULT 0,
		Leave [decimal](2) NOT NULL DEFAULT 0,
		LWP [decimal](2) NOT NULL DEFAULT 0,
		[Absent] [decimal](2) NOT NULL DEFAULT 0,
		[OTHours] [decimal](2) NOT NULL DEFAULT 0,
		[Remark] [nvarchar](max),
		[CustomColumn1] [nvarchar](max),
		[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
		[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
		[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
		[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
		[Authorized][Bit] NOT NULL DEFAULT 1,
		[Active][Bit] NOT NULL DEFAULT 1);
ALTER TABLE [dbo].[Attendance_Old]  ADD CONSTRAINT UQ_Attendance_Old UNIQUE (EmployeeId,AtdMonth,AtdYear);

CREATE TABLE WorkMonth
	(	[Id] [UniqueIdentifier] Primary Key DEFAULT NEWSEQUENTIALID(),
		[WorkUnitId] [UniqueIdentifier] NULL,
		[WorkDay] [DateTime] NOT NULL DEFAULT GETDATE(), 
		[DayStatus] [nvarchar](2), -- Working, Holiday
		[WorkHours] decimal(2),
		[Remark] [nvarchar](max),
		[CustomColumn1] [nvarchar](max),
		[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
		[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
		[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
		[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
		[Authorized][Bit] NOT NULL DEFAULT 1,
		[Active][Bit] NOT NULL DEFAULT 1);
		ALTER TABLE [dbo].[WorkMonth]  ADD CONSTRAINT UQ_WorkMonth UNIQUE (WorkUnitId, WorkDay);

CREATE TABLE Attendance
	(	[Id] [UniqueIdentifier] Primary Key DEFAULT NEWSEQUENTIALID(),
		[EmployeeId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Employee(Id),
		[InDateTime] [DateTime] NOT NULL DEFAULT GETDATE(), 
		[OutDateTime] [DateTime] NOT NULL DEFAULT GETDATE(), 
		[WorkHours] As DATEDIFF(hh, [InDateTime], [OutDateTime]),
		[AtendanceStatus] [nvarchar](2),
		[Remark] [nvarchar](max),
		[CustomColumn1] [nvarchar](max),
		[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
		[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
		[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
		[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
		[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
		[Authorized][Bit] NOT NULL DEFAULT 1,
		[Active][Bit] NOT NULL DEFAULT 1);

ALTER TABLE [dbo].[Attendance]  ADD CONSTRAINT UQ_Attendance UNIQUE (EmployeeId,InDateTime);

Alter table Attendance  add constraint outDateCheck_date CHECK (CONVERT(date, OutDateTime) = CONVERT(date, InDateTime))
Alter table Attendance  add constraint outDateCheck_time  CHECK( DATEDIFF(hh, InDateTime, OutDateTime) > 0);

ALTER TABLE [dbo].[Attendance] WITH CHECK ADD  CONSTRAINT [CK_Attendance_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
ALTER TABLE [dbo].[Attendance]  WITH CHECK ADD  CONSTRAINT [CK_Attendance] CHECK  (([dbo].[CheckAttendance]([EmployeeId],[InDateTime])=(1)));

