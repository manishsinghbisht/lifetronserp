USE [LtSysDb1]
GO
/****** Object:  UserDefinedFunction [dbo].[fnBOMLineItemsAmount]    Script Date: 18/12/2014 03:24:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create FUNCTION [dbo].[fnBOMLineItemsAmount]
(
	  @ProductId UNIQUEIDENTIFIER
)    
RETURNS DECIMAL(18,4)
AS
 BEGIN
 	DECLARE @Result DECIMAL(18,4) = 0

	Select @Result = SUM(Amount) FROM BOMLineItem WHERE ProductId = @ProductId and Active=1 and Authorized=1
 
	RETURN @Result
 END

