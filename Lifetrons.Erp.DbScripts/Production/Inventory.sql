Select JobNo, Process.Name, JIH.Id, Sum([Weight]) as ProductWt, Sum(Quantity) ProductPcs, JIH.[Date] as TranDate, 
	'I' as TranType, 'J' as ObjectType, 0 as ItemId,  '0' as Code, JIH.EmployeeId, Employee.Name 
	from 
	JobIssueHead JIH, JobProductIssue JPI, Process, Employee
	where 
	JIH.Id = JPI.JobIssueId And
	JIH.IssuedByProcessId = Process.Id And
	JIH.EmployeeId = Employee.Id
	group by JobNo, Process.Name, JIH.Id, JIH.[Date], JIH.EmployeeId, Employee.Name
Union All
Select JobNo, Process.Name, JIH.Id, Sum([Weight]) as ProductWt, Sum(Quantity) ProductPcs, JIH.[Date] as TranDate, 
	'R' as TranType, 'J' as ObjectType, 0 as ItemId,  '0' as Code, JIH.EmployeeId, Employee.Name 
	from 
	JobReceiptHead JIH, JobProductReceipt JPI, Process, Employee
	where 
	JIH.Id = JPI.JobReceiptId And
	JIH.ReceiptByProcessId = Process.Id And
	JIH.EmployeeId = Employee.Id
	group by JobNo, Process.Name, JIH.Id, JIH.[Date], JIH.EmployeeId, Employee.Name

	-----------Items ------------------------------------------------------------
Select JobNo, Process.Name, JIH.Id, Sum([Weight]) as ProductWt, Sum(Quantity) ProductPcs, JIH.[Date] as TranDate, 
	'I' as TranType, 'J' as ObjectType, 0 as ItemId,  '0' as Code, JIH.EmployeeId, Employee.Name 
	from 
	JobIssueHead JIH, JobItemIssue JPI, Process, Employee
	where 
	JIH.Id = JPI.JobIssueId And
	JIH.IssuedByProcessId = Process.Id And
	JIH.EmployeeId = Employee.Id
	group by JobNo, Process.Name, JIH.Id, JIH.[Date], JIH.EmployeeId, Employee.Name
UNION ALL
Select JobNo, Process.Name, JIH.Id, Sum([Weight]) as ProductWt, Sum(Quantity) ProductPcs, JIH.[Date] as TranDate, 
	'R' as TranType, 'J' as ObjectType, 0 as ItemId,  '0' as Code, JIH.EmployeeId, Employee.Name 
	from 
	JobReceiptHead JIH, JobItemReceipt JPI, Process, Employee
	where 
	JIH.Id = JPI.JobReceiptId And
	JIH.ReceiptByProcessId = Process.Id And
	JIH.EmployeeId = Employee.Id
	group by JobNo, Process.Name, JIH.Id, JIH.[Date], JIH.EmployeeId, Employee.Name
