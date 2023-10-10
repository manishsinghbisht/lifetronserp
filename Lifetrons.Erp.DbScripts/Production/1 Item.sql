Create Table ItemClassification
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table ItemCategory
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table ItemType
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table ItemGroup
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table ItemSubGroup
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL,
	[GroupId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES ItemGroup(Id)
) on [Primary];

Create Table CostingGroup
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table CostingSubGroup
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL,
	[CostingGroupId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES CostingGroup(Id)
) on [Primary];

Create Table Nature
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table Shape
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table Colour
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table Style
	(	
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL
) on [Primary];

Create Table Item
	(
	[Id] [UniqueIdentifier] PRIMARY KEY,
	[Name] [nvarchar](400) NOT NULL UNIQUE,
	[Code] [nvarchar](400) NOT NULL UNIQUE,	
	[ShrtDesc] [nvarchar](400) NULL,
	[Desc] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](128) NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[OrgId] [UniqueIdentifier] NOT NULL FOREIGN KEY REFERENCES Organization(Id),
	[CreatedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[CreatedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [nvarchar](128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id),
	[ModifiedDate] [DateTime] NOT NULL DEFAULT GETDATE(),
	[Authorized][Bit] NOT NULL DEFAULT 0,
	[Active][Bit] NOT NULL DEFAULT 1,
	[Size] [nVarChar](100), 
	[QuantityPerLot] [Decimal](8,3),
	[LotWeight] [Decimal](8,3), 
	[LotWeightUnitId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES WeightUnit(Id),
	[ABCClass] [nvarchar](1), 
	[VEDClass] [nvarchar](1),
	[LeadTime] [Decimal] (4,2), 
	[MinStkLvl] [Decimal](18, 4) NULL, 
	[SafetyQty] [Decimal](18, 4) NULL,
	[ReOrdrQty] [Decimal](18, 4) NULL,
	[OpeningQty] [Decimal](18, 4) NULL,
	[OpeningLotCost] [Decimal](18, 4) NULL,
	[Wastage] [Decimal](4,2), 
	[ScrapRate] [Decimal](8,2), 
	[Gauge] [Decimal](18,4), 
	[Level] [Decimal](4,2), 
	[AlternativeId] [UniqueIdentifier], 
	[ClassificationId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES ItemClassification(Id),
	[CategoryId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES ItemCategory(Id),
	[TypeId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES ItemType(Id),
	[StyleId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Style(Id),
	[GroupId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES ItemGroup(Id),
	[SubGroupId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES ItemSubGroup(Id),
	[CostingGroupId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES CostingGroup(Id),
	[CostingSubGroupId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES CostingSubGroup(Id),
	[NatureId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Nature(Id),
	[ShapeId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Shape(Id),
	[ColourId] [UniqueIdentifier] NULL FOREIGN KEY REFERENCES Colour(Id),
	[TimeStamp] [Timestamp] NOT NUll) on [Primary];

	GO

	ALTER TABLE [dbo].[Item] ADD CONSTRAINT UQ_Item_Name_OrgId UNIQUE(Name, OrgId);
	ALTER TABLE [dbo].[Item] ADD CONSTRAINT UQ_Item_Code_OrgId UNIQUE(Code, OrgId);
	ALTER TABLE [dbo].[Item] WITH CHECK ADD  CONSTRAINT [CK_Item_ModifiedBy] CHECK  (([dbo].[fncCheckUserOrg]([ModifiedBy],[OrgId])=(1)));
