
/****** Object:  UserDefinedFunction [dbo].[fnGetOrderLineItemPrice]    Script Date: 15/01/2015 1:17:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create FUNCTION [dbo].[fnGetOrderLineItemPrice]
(
	  @OrderId UNIQUEIDENTIFIER,
	  @PriceBookId UNIQUEIDENTIFIER, 
	  @ProductId  UNIQUEIDENTIFIER
)    
RETURNS DECIMAL(18,4)
AS
 BEGIN
 	DECLARE @Result DECIMAL(18,4) = 0

	Select @Result = LineItemPrice FROM OrderLineItem WHERE OrderId = @OrderId and PriceBookId = @PriceBookId and ProductId = @ProductId and Active=1 and Authorized=1;
 
	RETURN @Result
 END

