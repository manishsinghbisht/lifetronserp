CREATE FUNCTION dbo.CheckStockIssue
(
		@Id UNIQUEIDENTIFIER,
		@StockIssueId UNIQUEIDENTIFIER,
		@ProductId UNIQUEIDENTIFIER,
		@ItemId UNIQUEIDENTIFIER,
		@Quantity decimal(18,4),
		@Weight decimal(18,4)
	 
)    
RETURNS BIT

AS
 BEGIN
	 DECLARE @IssuedByProcessId UNIQUEIDENTIFIER
	 DECLARE @ProcessReceiptQuantity DECIMAL(18,4) = 0
	 DECLARE @ProcessIssuesQuantity DECIMAL(18,4) = 0
	 DECLARE @ProcessReceiptWeight DECIMAL(18,4) = 0
	 DECLARE @ProcessIssuesWeight DECIMAL(18,4) = 0
	 DECLARE @CurrentQuantity DECIMAL(18,4) = 0
	 DECLARE @CurrentWeight DECIMAL(18,4) = 0
	 DECLARE @InProcessQuantity DECIMAL(18,4) = 0
	 DECLARE @InProcessWeight DECIMAL(18,4) = 0
	 DECLARE @ResultBit BIT = 0

	 --InProcessQuantity = Receipts - Issues
	 If (@ItemId IS NULL)
	 BEGIN
		  If EXISTS(Select * from StockIssueHead where Id = @StockIssueId)
		 BEGIN
				 Select @IssuedByProcessId = IssuedByProcessId from StockIssueHead where Id = @StockIssueId;

				 Select 
				 @ProcessReceiptQuantity =  IIF(Sum(d.Quantity) IS NULL, 0, Sum(d.Quantity)), 
				 @ProcessReceiptWeight = IIF(Sum(d.Weight) IS NULL, 0, Sum(d.Weight))   
				 from StockProductReceipt d LEFT OUTER JOIN StockReceiptHead m ON d.StockReceiptId = m.id 
					where m.ReceiptByProcessId = @IssuedByProcessId
					AND d.ProductId = @ProductId;
	
				Select 
				@ProcessIssuesQuantity = IIF(Sum(d.Quantity) IS NULL, 0, Sum(d.Quantity)),  
				@ProcessIssuesWeight = IIF(Sum(d.Weight) IS NULL, 0, Sum(d.Weight)) 
				from StockProductIssue d LEFT OUTER JOIN StockIssueHead m ON d.StockIssueId = m.id 
					where m.IssuedByProcessId = @IssuedByProcessId 
					AND d.ProductId = @ProductId;
	
				--In case of edit, get the current quantity to Subtract from Totals
				Select 
				@CurrentQuantity = Quantity, 
				@CurrentWeight = Weight  
				from StockProductIssue where Id = @Id AND ProductId = @ProductId;

				SET @ProcessIssuesQuantity = @ProcessIssuesQuantity - @CurrentQuantity

				SET @InProcessQuantity = @ProcessReceiptQuantity - @ProcessIssuesQuantity;

					If(@Quantity <= @InProcessQuantity)
						BEGIN
							SET @ResultBit = 1
						--If(@Weight > @InProcessWeight)
						--	SELECT @ResultBit = 0
						END
		 END
	 END 
	ELSE If(@ProductId IS NULL)
	 BEGIN
	  If EXISTS(Select * from StockIssueHead where Id = @StockIssueId)
		 BEGIN
				 Select @IssuedByProcessId = IssuedByProcessId from StockIssueHead where Id = @StockIssueId;

				 Select 
				 @ProcessReceiptQuantity = IIF(Sum(d.Quantity) IS NULL, 0, Sum(d.Quantity)), 
				 @ProcessReceiptWeight = IIF(Sum(d.Weight) IS NULL, 0, Sum(d.Weight))  
				 from StockItemReceipt d LEFT OUTER JOIN StockReceiptHead m ON d.StockReceiptId = m.id 
					where m.ReceiptByProcessId = @IssuedByProcessId
					AND d.ItemId = @ItemId;
	
				Select 
				@ProcessIssuesQuantity = IIF(Sum(d.Quantity) IS NULL, 0, Sum(d.Quantity)), 
				@ProcessIssuesWeight = IIF(Sum(d.Weight) IS NULL, 0, Sum(d.Weight)) 
				from StockItemIssue d LEFT OUTER JOIN StockIssueHead m ON d.StockIssueId = m.id 
					where m.IssuedByProcessId = @IssuedByProcessId 
					AND d.ItemId = @ItemId;
	
				--In case of edit, get the current quantity to Subtract from Totals
				Select 
				@CurrentQuantity = Quantity, 
				@CurrentWeight = Weight  
				from StockItemIssue where Id = @Id AND ItemId = @ItemId;

				SET @ProcessIssuesQuantity = @ProcessIssuesQuantity - @CurrentQuantity

				SET @InProcessQuantity = @ProcessReceiptQuantity - @ProcessIssuesQuantity;

					If(@Quantity <= @InProcessQuantity)
						BEGIN
							SET @ResultBit = 1
						--If(@Weight > @InProcessWeight)
						--	SELECT @ResultBit = 0
						END
		 END
	 END

	RETURN @ResultBit

 END

