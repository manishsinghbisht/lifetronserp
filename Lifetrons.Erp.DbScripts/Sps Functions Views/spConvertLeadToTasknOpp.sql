
CREATE PROCEDURE dbo.spConvertLeadToTasknOpp
   @LeadId UniqueIdentifier,
   @returnValue int output

 AS
DECLARE @err_message nvarchar(255)
SET @returnValue = 0;  

	IF EXISTS  (Select IsConverted from Lead where Id = @LeadId and Active=1 and IsConverted=0)
		BEGIN

			INSERT INTO Task (Name, Code, ShrtDesc, OwnerId, ReportCompletionToId, LeadId, TaskStatusId, PriorityId, Reminder, OrgId, CreatedBy, ModifiedBy, Authorized, Active) 
			SELECT 'L-' + Name + '-' + CONVERT(nVarChar, SYSUTCDATETIME()), 'L-' + Name + '-' + CONVERT(nVarChar, SYSUTCDATETIME()), ShrtDesc, OwnerId, OwnerId, Id, 'CE73B734-A4EF-E311-846B-F0921C571FF8', '0392B1F1-E302-4279-88EB-B1BA0620278D', DATEADD(day,4,GETUTCDATE()) , OrgId, CreatedBy, ModifiedBy, 0, 1
			FROM Lead
			WHERE Id = @LeadId;
		
			INSERT INTO Opportunity (Name, Code, ShrtDesc, OwnerId, LeadId, OpportunityTypeId, LeadSourceId, StageId, CampaignId, OrgId, CreatedBy, ModifiedBy, Authorized, Active) 
			SELECT 'L-' + Name + '-' + CONVERT(nVarChar, SYSUTCDATETIME()), 'L-' + Name + '-' + CONVERT(nVarChar, SYSUTCDATETIME()), ShrtDesc, OwnerId, Id, 'D55448B0-BD7B-460B-A3C7-B000CB375488', LeadSourceId, '8B169AB0-8CDA-4ED2-9972-82832E855BE2', CampaignId, OrgId, CreatedBy, ModifiedBy, 0, 1
			FROM Lead
			WHERE Id = @LeadId;

			UPDATE Lead SET IsConverted=1, LeadStatusId='068B908B-D967-4F55-B0FA-17CFB37A8477' WHERE Id=@LeadId;

			SET @returnValue = 1;
					
		END
	ELSE
		BEGIN
			SET @err_message = 'Lead not found raise sev 10. Conversion Failed.'
    		RAISERROR (@err_message,10, 1) 
		END

		return @returnValue;
GO


