
CREATE FUNCTION dbo.fncCheckUserOrg
(
      @UserId NVARCHAR(128),
	  @OrgId UNIQUEIDENTIFIER
)    
RETURNS BIT

AS
 BEGIN
 DECLARE @ResultBit BIT = 0

	IF EXISTS  (Select  Id from AspNetUsers where OrgId=@OrgId and Id=@UserId and Active=1)
	BEGIN
		SELECT @ResultBit = 1
	END
 
	RETURN @ResultBit

 END