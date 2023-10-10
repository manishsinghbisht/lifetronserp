CREATE FUNCTION dbo.CheckProcessTimeConfig
(
		@Id UNIQUEIDENTIFIER,
		@ProcessId UNIQUEIDENTIFIER,
		@FromDate Date
)    
RETURNS BIT

AS

BEGIN
	DECLARE @ResultBit BIT = 0
	DECLARE @RecordsCount DECIMAL = 0
			
  		Select @RecordsCount = Count(*)  from ProcessTimeConfig WHERE ToDate >= @FromDate and ProcessId = @ProcessId and Id <> @Id
		IF(@RecordsCount > 0)
			BEGIN
				SELECT @ResultBit = 0
			END
		ELSE
			BEGIN
				SELECT @ResultBit = 1
			END
 
	RETURN @ResultBit
END