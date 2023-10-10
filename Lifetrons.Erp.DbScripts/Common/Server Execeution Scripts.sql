
ALTER TABLE Campaign
ALTER COLUMN [ExpectedResponsePercent] [Decimal](18, 4) NULL;

ALTER TABLE Opportunity
ALTER COLUMN [ProbabilityPercent] [Decimal](18, 4) NULL;

ALTER TABLE Task
ADD [ProgressPercent] [Decimal](18, 4) NULL;

ALTER TABLE Task
ADD [ProgressDesc] [nvarchar](max) NULL;

ALTER TABLE Task
ADD [TimeStamp] [Timestamp] NOT NUll;

ALTER TABLE Lead
ALTER COLUMN [NumberOfEmployees] [Decimal](18, 4) NULL;

ALTER TABLE dbo.Product
ALTER COLUMN [Active][Bit] NOT NULL;

ALTER TABLE dbo.Product ADD CONSTRAINT C_Product_Active_Default DEFAULT 1 FOR [Active]; 

ALTER TABLE dbo.PriceBook
ALTER COLUMN [Authorized][Bit] NOT NULL;

ALTER TABLE dbo.PriceBook ADD CONSTRAINT C_PriceBook_Authorized_Default DEFAULT 0 FOR [Active]; 

ALTER TABLE dbo.PriceBook
ALTER COLUMN [Active][Bit] NOT NULL;

ALTER TABLE dbo.PriceBook ADD CONSTRAINT C_PriceBook_Active_Default DEFAULT 1 FOR [Active]; 