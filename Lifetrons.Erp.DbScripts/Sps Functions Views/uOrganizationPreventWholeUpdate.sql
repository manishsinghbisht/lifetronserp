
CREATE TRIGGER [dbo].[uOrganizationPreventWholeUpdate] 
ON [dbo].[Organization] 
FOR UPDATE AS 
BEGIN
     DECLARE @Count int
     SET @Count = @@ROWCOUNT;
         
     IF @Count >= (SELECT SUM(row_count)
         FROM sys.dm_db_partition_stats 
         WHERE OBJECT_ID = OBJECT_ID('dbo.Organization' ) 
         AND index_id = 1)
     BEGIN
         RAISERROR('Cannot update all rows',16,1) 
         ROLLBACK TRANSACTION
         RETURN;
     END
END
GO