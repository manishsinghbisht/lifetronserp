/****** Object:  UserDefinedFunction [dbo].[fnProcurementOrderDetailAmount]    Script Date: 25/06/2014 1:17:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create FUNCTION [dbo].[fnProcurementOrderDetailAmount]
(
	  @Id UNIQUEIDENTIFIER
)    
RETURNS DECIMAL(18,4)
AS
 BEGIN
 	DECLARE @Result DECIMAL(18,4) = 0

	Select @Result = SUM(LineItemAmount) FROM ProcurementOrderDetail WHERE ProcurementOrderId = @Id and Active=1 and Authorized=1
 
	RETURN @Result
 END

