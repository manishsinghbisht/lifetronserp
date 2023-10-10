
CREATE FUNCTION dbo.fncCheckUserProductOrg
(
      @UserId NVARCHAR(128),
	  @ProductId UNIQUEIDENTIFIER,
	  @OrgId UNIQUEIDENTIFIER
)    
RETURNS BIT

AS
 BEGIN
 DECLARE @ResultBit BIT = 0

	IF EXISTS  (Select  Id from AspNetUsers where OrgId=@OrgId and Id=@UserId and Active=1)
	BEGIN
		IF EXISTS  (Select Id from Product where OrgId=@OrgId and Id=@ProductId)
		BEGIN
			SELECT @ResultBit = 1
		END
	END
 
RETURN @ResultBit

 END