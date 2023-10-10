CREATE FUNCTION dbo.CheckJobProductIssue
(
		@Id UNIQUEIDENTIFIER,
		@JobIssueId UNIQUEIDENTIFIER,
		@JobNo decimal(18),
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
	 DECLARE @ResultBitString nVARCHAR(max)

	 --InProcessQuantity = Receipts - Issues
	 If EXISTS(Select * from JobProductReceipt where JobNo = @JobNo )
	 BEGIN
			
			 Select @IssuedByProcessId = IssuedByProcessId from JobIssueHead where Id=@JobIssueId;

			 Select 
			 @ProcessReceiptQuantity = IIF(Sum(d.Quantity) IS NULL, 0, Sum(d.Quantity)), 
			 @ProcessReceiptWeight = IIF(Sum(d.Weight) IS NULL, 0, Sum(d.Weight))  
			 from JobProductReceipt d LEFT OUTER JOIN JobReceiptHead m ON d.JobReceiptId = m.id 
				where m.ReceiptByProcessId = @IssuedByProcessId
				AND d.JobNo = @JobNo;
	
			Select 
			@ProcessIssuesQuantity = IIF(Sum(d.Quantity) IS NULL, 0, Sum(d.Quantity)), 
			@ProcessIssuesWeight = IIF(Sum(d.Weight) IS NULL, 0, Sum(d.Weight))  
			from JobProductIssue d LEFT OUTER JOIN JobIssueHead m ON d.JobIssueId = m.id 
				where m.IssuedByProcessId = @IssuedByProcessId 
				AND d.JobNo = @JobNo;

			--In case of edit, get the current quantity to Subtract from Totals
			Select 
			@CurrentQuantity = Quantity, 
			@CurrentWeight = Weight  
			from JobProductIssue where JobNo = @JobNo AND Id = @Id;

			SET @ProcessIssuesQuantity = @ProcessIssuesQuantity - @CurrentQuantity

			SET @InProcessQuantity = @ProcessReceiptQuantity - @ProcessIssuesQuantity;

			If(@Quantity <= @InProcessQuantity)
				BEGIN
					SET @ResultBit = 1
				--If(@Weight > @InProcessWeight)
				--	SELECT @ResultBit = 0
				END
	 END
 
	RETURN @ResultBit

 END


