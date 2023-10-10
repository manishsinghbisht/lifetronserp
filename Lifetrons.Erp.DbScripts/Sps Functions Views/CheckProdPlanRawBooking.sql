
CREATE FUNCTION dbo.CheckProdPlanRawBooking
(
      @ProdPlanDetailId UNIQUEIDENTIFIER
)    

RETURNS BIT

AS
 BEGIN
	DECLARE @ResultBit BIT = 0

  		IF EXISTS  (Select Id from ProdPlanRawBooking where ProdPlanDetailId=@ProdPlanDetailId)
			BEGIN
				SELECT @ResultBit = 0
			END
		ELSE
			BEGIN
				SELECT @ResultBit = 1
			END
 
		RETURN @ResultBit
 END