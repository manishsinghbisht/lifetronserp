
CREATE PROCEDURE [dbo].[sViewQuote]
    @QuoteId UniqueIdentifier
AS 

    SET NOCOUNT ON;
	
	SELECT q.*, ql.*, o.Name, p.Name as Product from Quote q 
	Left Outer Join QuoteLineItem ql ON ql.QuoteId = q.Id
	inner Join Organization o ON o.Id = q.OrgId
	inner join Product p ON p.Id = ql.ProductId
	WHERE QuoteId = @QuoteId;
GO