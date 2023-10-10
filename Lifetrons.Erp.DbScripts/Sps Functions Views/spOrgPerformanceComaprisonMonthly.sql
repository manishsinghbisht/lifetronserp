
CREATE PROCEDURE [dbo].[spOrgPerformanceComaprisonMonthly]
  
	@OrgId UniqueIdentifier

AS 

    SET NOCOUNT ON;

		--WHERE DATEDIFF(day, GETDATE(), CreatedDate) <= 7
	-- select dateadd(wk, datediff(wk, 0, getdate()), 0) as StartDate ,
    -- (select dateadd(wk, datediff(wk, 0, getdate()), 0) + 5) as EndDate

	Select a.Id, a.Name, 0.0 as [LineItemsAmount], 0.0 as [LineItemsQuantity], a.CreatedDate as [Date], b.Name as [Status] from Lead a
	inner join LeadStatus b on a.LeadStatusId = b.Id
	where 
	--b.Name NOT IN ('Open - Not Contacted', 'Working - Contacted') AND 
	DATEDIFF(MONTH, a.CreatedDate, GETDATE()) = 0 AND
	a.OrgId = @OrgId AND 
	a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, 0.0 as [LineItemsAmount], 0.0 as [LineItemsQuantity], a.CreatedDate as [Date], b.Name as [Status] from Lead a
	inner join LeadStatus b on a.LeadStatusId = b.Id
	where 
	--b.Name NOT IN ('Open - Not Contacted', 'Working - Contacted') AND 
	DATEDIFF(MONTH, a.CreatedDate, GETDATE()) = 1 AND
	a.OrgId = @OrgId AND 
	a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.[LineItemsAmount], a.[LineItemsQuantity], a.CreatedDate as [Date], b.Name as [Status] from [Opportunity] a 
	inner join Stage b on a.StageId = b.Id
	where 
	--b.Name IN ('Closed Won', 'Closed Lost') AND 
	DATEDIFF(MONTH, a.CreatedDate, GETDATE()) = 0 AND
	a.OrgId = @OrgId AND 
	a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.[LineItemsAmount], a.[LineItemsQuantity], a.CreatedDate as [Date], b.Name as [Status] from [Opportunity] a 
	inner join Stage b on a.StageId = b.Id
	where 
	--b.Name IN ('Closed Won', 'Closed Lost') AND 
	DATEDIFF(MONTH, a.CreatedDate, GETDATE()) = 1 AND
	a.OrgId = @OrgId AND 
	a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.[LineItemsAmount], a.[LineItemsQuantity], a.CreatedDate as [Date], b.Name as [Status] from [Order] a 
	inner join DeliveryStatus b on a.DeliveryStatusId = b.Id
	where 
	DATEDIFF(MONTH, a.CreatedDate, GETDATE()) = 0 AND
	a.OrgId = @OrgId AND 
	a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.[LineItemsAmount], a.[LineItemsQuantity], a.CreatedDate as [Date], b.Name as [Status] from [Order] a 
	inner join DeliveryStatus b on a.DeliveryStatusId = b.Id
	where 
	DATEDIFF(MONTH, a.CreatedDate, GETDATE()) = 1 AND
	a.OrgId = @OrgId AND 
	a.Active = 1
	order by a.CreatedDate; 

GO