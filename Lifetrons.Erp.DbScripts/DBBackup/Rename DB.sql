
--Open Microsoft SQL Server Management Studio.
--Connect to the server wherein the DB you want to rename is located.
--Modify the following script and run it –
-- Collapse | Copy Code
-- Replace all MyDBs with the name of the DB you want to change its name
USE [EasySales];
-- Changing Physical names and paths
-- Replace all NewMyDB with the new name you want to set for the DB
-- Replace 'C:\...\NewMyDB.mdf' with full path of new DB file to be used
ALTER DATABASE EasySales MODIFY FILE (NAME = 'EasySales_Data', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\LtSysDb1.mdf');
ALTER DATABASE EasySales MODIFY FILE (NAME = 'EasySales_log', FILENAME =  'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\LtSysDb1_log.ldf');
ALTER DATABASE EasySales MODIFY FILE (NAME = 'Large_Data', FILENAME =     'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\LtSysDb1Large.ndf');
ALTER DATABASE EasySales MODIFY FILE (NAME = 'Large_log', FILENAME =      'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\LtSysDb1Large_log.ldf');

-- Changing logical names
ALTER DATABASE EasySales MODIFY FILE (NAME = EasySales_Data, NEWNAME = LtSysDb1);
ALTER DATABASE EasySales MODIFY FILE (NAME = EasySales_log, NEWNAME = LtSysDb1_log);
ALTER DATABASE EasySales MODIFY FILE (NAME = Large_Data, NEWNAME = LtSysDb1_Large);
ALTER DATABASE EasySales MODIFY FILE (NAME = Large_log, NEWNAME = LtSysDb1_Large_log);

--Right click on the DB and select Tasks>Take Offline
--Go to the location that MDF and LDF files are located and rename them exactly as you specified in first two alter commands. If you changed the folder path, then you need to move them there.
--Go back to Microsoft SQL Server Management Studio and right click on the DB and select Tasks>Bring Online.
--Now is the time to rename you DB to the new name.