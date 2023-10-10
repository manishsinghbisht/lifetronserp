Use LtSysS1

Update Lead set CreatedDate = DateAdd(mm,1,CreatedDate) WHERE CreatedDate < '2017-04-01' 
Update Lead set ModifiedDate = DateAdd(mm,1,ModifiedDate) WHERE ModifiedDate < '2017-04-01' 
Select CreatedDate, ModifiedDate from Lead order by createddate


Update Opportunity set CreatedDate = DateAdd(mm,1,CreatedDate) WHERE CreatedDate < '2017-04-01' 
Update Opportunity set ModifiedDate = DateAdd(mm,1,ModifiedDate) WHERE ModifiedDate < '2017-04-01' 
Select CreatedDate, ModifiedDate from Opportunity order by createddate

Update [Order] set CreatedDate = DateAdd(mm,1,CreatedDate) WHERE CreatedDate < '2017-04-01' 
Update [Order] set ModifiedDate = DateAdd(mm,1,ModifiedDate) WHERE ModifiedDate < '2017-04-01'  
Update [Order] set StartDate = DateAdd(mm,1,StartDate) WHERE StartDate < '2017-04-01' 

Select CreatedDate, ModifiedDate from [Order] order by createddate
