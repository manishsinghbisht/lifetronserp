 
CREATE TRIGGER [dbo].[dPriceBookLineItemPreventWholeDelete] 
ON [dbo].[PriceBookLineItem] 
FOR DELETE AS 
BEGIN
     DECLARE @Count int
     SET @Count = @@ROWCOUNT;
         
     IF @Count >= (SELECT SUM(row_count)
         FROM sys.dm_db_partition_stats 
         WHERE OBJECT_ID = OBJECT_ID('dbo.PriceBookLineItem' ) 
         AND index_id = 1)
     BEGIN
         RAISERROR('Cannot delete all rows',16,1) 
         ROLLBACK TRANSACTION
         RETURN;
     END
END
GO  