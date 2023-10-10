
CREATE PROCEDURE [dbo].[spUserOpenWork]
    @OwnerId nVarChar(128),
	@OrgId UniqueIdentifier
AS 

    SET NOCOUNT ON;
	--WHERE DATEDIFF(day, GETDATE(), CreatedDate) <= 7
	-- select dateadd(wk, datediff(wk, 0, getdate()), 0) as StartDate ,
    -- (select dateadd(wk, datediff(wk, 0, getdate()), 0) + 5) as EndDate

	Select a.Id, a.Name, a.CreatedDate as [Date], b.Name as [Status] from Lead a
	inner join LeadStatus b on a.LeadStatusId = b.Id
	where b.Name IN ('Open - Not Contacted', 'Working - Contacted')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.CreatedDate as [Date], b.Name as [Status] from [Opportunity] a 
	inner join Stage b on a.StageId = b.Id
	where b.Name NOT IN ('Closed Won', 'Closed Lost')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.StartDate as [Date], b.Name as [Status] from Task a 
	inner join TaskStatus b on a.TaskStatusId = b.Id
	where b.Name IN ('Not Started', 'In Progress', 'Waiting on someone else')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.StartDate; 

	Select a.Id, a.Name, a.CreatedDate as [Date], b.Name as [Status] from [Case] a
	inner join CaseStatus b on a.CaseStatusId = b.Id
	where b.Name IN ('New', 'Working', 'Escalated')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.CreatedDate as [Date], b.Name as [Status] from [Quote] a 
	inner join QuoteStatus b on a.QuoteStatusId = b.Id
	where b.Name IN ('In Review', 'Draft', 'Presented', 'Approved', 'Needs Review')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.CreatedDate as [Date], b.Name as [Status] from [Order] a 
	inner join DeliveryStatus b on a.DeliveryStatusId = b.Id
	where b.Name IN ('Yet to begin' , 'In progress')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.CreatedDate; 

	Select a.Id, a.Name, a.CreatedDate as [Date], b.Name as [Status] from [Invoice] a 
	inner join InvoiceStatus b on a.InvoiceStatusId = b.Id
	where b.Name NOT IN ('Closed')
	AND a.OwnerId = @OwnerId
	AND a.OrgId = @OrgId
	AND a.Active = 1
	order by a.CreatedDate; 

GO