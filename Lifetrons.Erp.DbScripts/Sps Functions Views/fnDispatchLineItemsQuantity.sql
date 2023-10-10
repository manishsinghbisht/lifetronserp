
/****** Object:  UserDefinedFunction [dbo].[fnDispatchLineItemsQuantity]    Script Date: 25/06/2014 1:17:12 PM ******/

Create FUNCTION [dbo].[fnDispatchLineItemsQuantity]
(
	  @DispatchId UNIQUEIDENTIFIER
)    
RETURNS DECIMAL(18,4)
AS
 BEGIN
 	DECLARE @Result DECIMAL(18,4) = 0

	Select @Result = SUM(Quantity) FROM DispatchLineItem WHERE DispatchId = @DispatchId and Active=1 and Authorized=1
 
	RETURN @Result
 END

