
CREATE FUNCTION dbo.CheckAttendance
(
      @EmployeeId UNIQUEIDENTIFIER,
	  @InDate DateTime
)    
RETURNS DECIMAL

AS
 BEGIN
 	DECLARE @RecordCount DECIMAL = 0
  	Select @RecordCount = Count(*)  from Attendance where EmployeeId=@EmployeeId and CONVERT(date, InDateTime)=CONVERT(date, @InDate)
	RETURN @RecordCount


 END