
/****** Object:  UserDefinedFunction [dbo].[fnDispatchLineItemsAmount]    Script Date: 15/01/2015 1:30:12 PM ******/

Create FUNCTION [dbo].[fnDispatchLineItemsAmount]
(
	  @DispatchId UNIQUEIDENTIFIER
)    
RETURNS DECIMAL(18,4)
AS
 BEGIN
 	DECLARE @Result DECIMAL(18,4) = 0

	Select @Result = SUM(LineItemAmount) FROM DispatchLineItem WHERE DispatchId = @DispatchId and Active=1 and Authorized=1
 
	RETURN @Result
 END

