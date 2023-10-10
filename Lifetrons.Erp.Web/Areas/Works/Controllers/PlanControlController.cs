using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Lifetrons.Erp.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using WebGrease.Css.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class PlanControlController : BaseController
    {
        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IProdPlanDetailService ProdPlanDetailService { get; set; }

        [Dependency]
        public IJobIssueHeadService JobIssueHeadService { get; set; }

        [Dependency]
        public IJobReceiptHeadService JobReceiptHeadService { get; set; }

        [Dependency]
        public IJobProductIssueService JobProductIssueService { get; set; }

        [Dependency]
        public IJobProductReceiptService JobProductReceiptService { get; set; }

        [Dependency]
        public IStockItemIssueService StockItemIssueService { get; set; }

        [Dependency]
        public IStockItemReceiptService StockItemReceiptService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IDispatchService DispatchService { get; set; }

        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

        [HttpGet]
        public async Task<ActionResult> JobStatus()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> JobStatus(string jobNo)
        {
            decimal jobNoDecimal = -1;
            try
            {
                jobNoDecimal = jobNo.ToJobNumber();
            }
            catch (Exception exception)
            {
                HandleException(exception);
                return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"].ToString() });
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var model = new JobStatusModel();

            model.JobNo = jobNo.ToJobNumber();
            model.JobProductIssues =
                await
                    JobProductIssueService.SelectAsyncLineItemsByJobNo(jobNo, applicationUser.Id, applicationUser.OrgId.ToString());
            model.JobProductReceipts =
                await
                    JobProductReceiptService.SelectAsyncLineItemsByJobNo(jobNo, applicationUser.Id, applicationUser.OrgId.ToString());



            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PendingJobReceipts()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.ProcessId = new SelectList(await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PendingJobReceipts(string startDate, string endDate, string processId)
        {
            Guid processIdGuid = processId.ToSysGuid();
            DateTime startDateTime = Convert.ToDateTime(startDate);
            DateTime endDateTime = Convert.ToDateTime(endDate);

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            List<PendingReceipts> viewModelList = new List<PendingReceipts>();

            var paramProcessName = ProcessService.Find(processId, applicationUser.Id, applicationUser.OrgId.ToString()).Name;
            var processes = await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString());
            processes.ForEach(p =>
            {
                var processwiseQtyIssues = JobProductIssueService.ProcesswiseQuantitiesGroupByJobs(startDateTime, endDateTime, p.Id.ToString(), processId, applicationUser.Id, applicationUser.OrgId.ToString());
                var processwiseQtyReceipts = JobProductReceiptService.ProcesswiseQuantitiesGroupByJobs(startDateTime, endDateTime, processId, p.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                var rcptJobsList = processwiseQtyReceipts.ToList();


                foreach (var issu in processwiseQtyIssues)
                {
                    //Initialze and set rcpts qty to zero if no rcpts found
                    var o = rcptJobsList.Find(r => r.JobNo == issu.JobNo) ??
                               new JobQuantityTotals() { JobNo = issu.JobNo, TotalQuantity = 0 };

                    if (issu.TotalQuantity - o.TotalQuantity > 0) //Add item only if yo have any pending quantity
                    {
                        var viewModel = new PendingReceipts();

                        viewModel.IssuedFromProcess = p.Name;
                        viewModel.IssuedToProcess = paramProcessName;
                        viewModel.JobNo = issu.JobNo;
                        viewModel.PendingQuantity = issu.TotalQuantity - o.TotalQuantity;
                        viewModelList.Add(viewModel);
                    }
                }
            });

            return View(viewModelList);
        }

        [HttpGet]
        // GET: /ProductionScheduleForecast/
        public async Task<ActionResult> ProductionScheduleForecast()
        {
            int? page = 1;

            DateTime startDateTime = Convert.ToDateTime(DateTime.Now.AddDays(-15));
            DateTime endDateTime = Convert.ToDateTime(DateTime.Now.AddDays(15));

            List<MasterProductionScheduleViewModel> newModel = await CalcProductionForecast(startDateTime);

            return View(newModel);
        }
        

        [HttpPost]
        public async Task<ActionResult> ProductionScheduleForecast(string startDate, string endDate, int? page)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(startDate))
            {
                return View();
            }

            DateTime startDateTime = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(startDate), User.TimeZone());
            DateTime endDateTime = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(endDate), User.TimeZone());
            if (startDateTime > endDateTime) endDateTime = startDateTime.AddDays(1);

            List<MasterProductionScheduleViewModel> newModel = await CalcProductionForecast(startDateTime);

            return View(newModel);
        }

        [HttpGet]
        public async Task<ActionResult> CurrentProductionStatus()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CurrentProductionStatus(string startDate)
        {
            DateTime startDateTime = Convert.ToDateTime(startDate);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var viewModelList = new List<ProductionLoad>();

            var processes = await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString());
            processes.ForEach(p =>
            {
                decimal loadIn = 0;
                decimal loadOut = 0;

                if (p.Id.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid()))
                {
                    loadIn = JobProductReceiptService.AssemblyLoadIn(startDateTime, applicationUser.OrgId.ToString());
                    loadOut = JobProductIssueService.AssemblyLoadOut(startDateTime, applicationUser.OrgId.ToString());
                }
                else
                {
                    loadIn = JobProductIssueService.ProcessLoadIn(startDateTime, p.Id.ToString(), applicationUser.OrgId.ToString());
                    loadOut = JobProductIssueService.ProcessLoadOut(startDateTime, p.Id.ToString(), applicationUser.OrgId.ToString());
                }

                var viewModel = new ProductionLoad { Process = p.Name, Quantity = loadIn - loadOut };
                viewModelList.Add(viewModel);
            });

            return View(viewModelList);
        }

        [HttpGet]
        public async Task<ActionResult> CurrentJobwiseProductionStatus()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CurrentJobwiseProductionStatus(string startDate)
        {
            DateTime startDateTime = Convert.ToDateTime(startDate);
            
            var viewModelList = new List<ProductionLoad>();

            await GetJobwiseProductionStatus(startDateTime, viewModelList);

            return View(viewModelList);
        }

        private async Task GetJobwiseProductionStatus(DateTime startDateTime, List<ProductionLoad> viewModelList)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var processes = await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString());
            processes.ForEach(p =>
            {
                IEnumerable<JobQuantityTotals> loadIn;
                IEnumerable<JobQuantityTotals> loadOut;
                if (p.Id.Equals(Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid()))
                {
                    loadIn = JobProductReceiptService.AssemblyLoadInJobwise(startDateTime, applicationUser.OrgId.ToString());
                    loadOut = JobProductIssueService.AssemblyLoadOutJobwise(startDateTime, applicationUser.OrgId.ToString());
                }
                else
                {
                    loadIn = JobProductIssueService.ProcessLoadInJobwise(startDateTime, p.Id.ToString(),
                        applicationUser.OrgId.ToString());
                    loadOut = JobProductIssueService.ProcessLoadOutJobwise(startDateTime, p.Id.ToString(),
                        applicationUser.OrgId.ToString());
                }

                var loadOutList = loadOut.ToList();
                foreach (var ins in loadIn)
                {
                    //Initialze and set rcpts qty to zero if no rcpts found
                    var outs = loadOutList.Find(r => r.JobNo == ins.JobNo) ??
                               new JobQuantityTotals() {JobNo = ins.JobNo, TotalQuantity = 0};

                    var viewModel = new ProductionLoad();
                    viewModel.Process = p.Name;
                    viewModel.JobNo = ins.JobNo;
                    viewModel.Quantity = ins.TotalQuantity - outs.TotalQuantity;
                    viewModelList.Add(viewModel);
                }
            });
        }
        private async Task<List<MasterProductionScheduleViewModel>> CalcProductionForecast(DateTime startDateTime)
        {
            decimal compositeCycleTimeInHour = 0;
            decimal minimumCycleCapacity = 0;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = await ProdPlanDetailService.SelectAsyncLineItems(startDateTime, applicationUser.Id, applicationUser.OrgId.ToString());
            var newModel = new List<MasterProductionScheduleViewModel>();

            if (model.Any())
            {
                model.ForEach(p =>
                {
                    //Get the balance quantity available for planning
                    decimal quantityInPlanning = ProdPlanDetailService.QuantityInPlanning(p.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    decimal quantityInProduction = ProdPlanDetailService.QuantityInProduction(p.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var orderLineItem = OrderLineItemService.SelectSingle(p.JobNo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    decimal orderProductionQuantity = orderLineItem.ProductionQuantity ?? 0;
                    decimal quantityUnPlanned = orderProductionQuantity - quantityInPlanning;

                    //Get the minimum capacity in the operation list coz that will be the maximum capacity produced the complete manufacturing cycle
                    var firstOrDefault = orderLineItem.Product.Operations.FirstOrDefault();
                    if (firstOrDefault != null)
                        minimumCycleCapacity = firstOrDefault.CycleCapacity;

                    //Fill the process for entity for each operation. This lloping is required here to order by Process serial in main loop
                    foreach (var operation in orderLineItem.Product.Operations)
                    {
                        if (minimumCycleCapacity > operation.CycleCapacity)
                            minimumCycleCapacity = operation.CycleCapacity;
                        operation.Process = ProcessService.Find(operation.ProcessId);
                    }


                    DateTime operationStartDateTime = ControllerHelper.ConvertDateTimeFromUtc(p.StartDateTime, User.TimeZone());
                    DateTime operationEndDateTime = operationStartDateTime;
                    int operationSequence = 1; //This is required in case o operation sequence is null or not given for any operation

                    //Order by operation sequence then by process sequence (this will be valuable in case operation sequence is NULL)
                    foreach (var operation in orderLineItem.Product.Operations.OrderBy(o => o.Serial).ThenBy(o => o.Process.Serial))
                    {
                        var viewModel = new MasterProductionScheduleViewModel();
                        viewModel.OperationSequence = operation.Serial ?? operationSequence;
                        viewModel.OperationName = operation.Process.Name;
                        viewModel.OperationTimeInHour = operation.CycleTimeInHour;
                        viewModel.OperationCapacity = operation.CycleCapacity;
                        viewModel.Id = p.Id;
                        viewModel.Serial = p.Serial;
                        viewModel.JobNo = p.JobNo;
                        viewModel.Quantity = p.Quantity; // this should be less than minimumCycleCapacity calculated above.
                        viewModel.Weight = p.Weight;
                        viewModel.WeightUnitId = p.WeightUnitId;
                        viewModel.WeightUnit = p.WeightUnit;
                        viewModel.IsRawBookingDone = p.IsRawBookingDone;
                        viewModel.IsIssuedForProduction = p.IsIssuedForProduction;
                        viewModel.QuantityUnplanned = quantityUnPlanned;
                        viewModel.QuantityInProduction = quantityInProduction;

                        //Add the operation time to the last operatin time to get the current operation start and end time.
                        //Check the Process work timings from ProcessTimeConfig and adjust job start and end time accordingly.

                        operationEndDateTime = CalculateEndDateTime(operation.Process, operationStartDateTime, Convert.ToDouble(operation.CycleTimeInHour));

                        viewModel.StartDateTime = operationStartDateTime;
                        viewModel.EndDateTime = operationEndDateTime;

                        newModel.Add(viewModel);

                        //
                        var viewModelList = new List<ProductionLoad>();

                        GetJobwiseProductionStatus(startDateTime, viewModelList);

                        operationSequence++;
                        operationStartDateTime = operationEndDateTime;
                    }
                });
            }

            return newModel;
        }
        private DateTime CalculateEndDateTime(Process process, DateTime startDateTime, double cycleTimeInHours)
        {
            DateTime endDateTime = startDateTime.AddMinutes(1);
            TimeSpan workDayStartTime = TimeSpan.FromHours(10);
            TimeSpan workDayEndTime = TimeSpan.FromHours(18);

            ProcessTimeConfiguration(process, startDateTime, out workDayStartTime, out workDayEndTime);
            double dailyWorkHours = (workDayEndTime - workDayStartTime).TotalHours;

            // Work hours left in start day
            double workHrsCoveredOnStartDay = (workDayEndTime - startDateTime.TimeOfDay).TotalHours;
            //Update balance cycle time and move end date to next day 
            double balanceCycleTimeHours = cycleTimeInHours - workHrsCoveredOnStartDay;
            if (balanceCycleTimeHours > 0)
            {
                endDateTime = new DateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day,
                    workDayEndTime.Hours, workDayEndTime.Minutes, workDayEndTime.Seconds); // Move to End of current work day
            }
            else
            {
                endDateTime = endDateTime.AddHours(cycleTimeInHours);
                return endDateTime;
            }

            //After handling start day hours, Now loop
            while (balanceCycleTimeHours > 0)
            {
                if (balanceCycleTimeHours > dailyWorkHours)
                {
                    balanceCycleTimeHours = cycleTimeInHours - dailyWorkHours;
                    //Move to end of next work day
                    endDateTime = AddDay(endDateTime);
                }
                else
                {
                    //First Set Time to start time of day
                    endDateTime = new DateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day,
                                                workDayStartTime.Hours, workDayStartTime.Minutes, workDayStartTime.Seconds);
                    //Then Add day
                    endDateTime = AddDay(endDateTime);
                    //Then Add hours
                    endDateTime = endDateTime.AddHours(balanceCycleTimeHours);
                    //Come out of loop
                    balanceCycleTimeHours = 0;
                }
            }

            return endDateTime;
        }
        private DateTime AddDay(DateTime date, bool isSundayOff = true)
        {
            DateTime newDate = date;
            newDate = newDate.AddDays(1);

            if (newDate.DayOfWeek == DayOfWeek.Sunday)
            {
                if (isSundayOff)
                {
                    newDate = newDate.AddDays(1);
                }
            }

            //Also check for other holidays here

            return newDate;
        }
        private void ProcessTimeConfiguration(Process process, DateTime date, out TimeSpan workDayStartTime, out TimeSpan workDayEndTime)
        {
            Guid processIdGuid = process.Id;
            // Check n set current process start datetime. If ProcessTime Config records not found, taske StartTime=10:00:00.0000000 and EndTime=18:00:00.0000000
            workDayStartTime = process.ProcessTimeConfigs.Where(ptc =>
                ptc.FromDate >= date.Date & ptc.ToDate <= date.Date & ptc.ProcessId == processIdGuid)
                .Select(x => x.StartTime).LastOrDefault();
            
            if (workDayStartTime == System.TimeSpan.Zero)
            {
                //If no records found for given date n process pick last record for process
                workDayStartTime = process.ProcessTimeConfigs.Where(ptc => ptc.ProcessId == processIdGuid).Select(x => x.StartTime).LastOrDefault();
            }

            if (workDayStartTime == System.TimeSpan.Zero)
            {
                //If still no records found for given process, pick an last record for process
                workDayStartTime = process.ProcessTimeConfigs.Select(x => x.StartTime).LastOrDefault();
                //Still, If there are no record found at all in ProcessTimeConfig, HardCode time.
                workDayStartTime = workDayStartTime == System.TimeSpan.Zero ? TimeSpan.FromHours(10) : workDayStartTime;
            }

            workDayEndTime = process.ProcessTimeConfigs.Where(ptc =>
                ptc.FromDate >= date.Date & ptc.ToDate <= date.Date)
                .Select(x => x.EndTime).LastOrDefault();

            if (workDayEndTime == System.TimeSpan.Zero)
            {
                //If no records find for given date pick last record for process
                workDayEndTime = process.ProcessTimeConfigs.Select(x => x.EndTime).LastOrDefault();
                //If there is no record for proces at all in ProcessTimeConfig, HardCode time.
                workDayEndTime = workDayEndTime == System.TimeSpan.Zero ? TimeSpan.FromHours(18) : workDayEndTime;
            }

        }

        #region Handle Exception
        private void HandleDbUpdateException(DbUpdateException dbUpdateException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbUpdateException));
            string innerExceptionMessage = dbUpdateException.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = dbUpdateException.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ItemId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Item. Item already exists.";
                    AddErrors("Duplicate Item. Item already exists.");
                }
                else
                {
                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
                    AddErrors("Duplicate Name or Code. Key record already exists.");
                }
            }
            else if (innerExceptionMessage.Contains("_CheckQuantity"))
            {
                TempData["CustomErrorMessage"] = "Invalid quantity.";
                AddErrors("Invalid quantity.");
            }
            else if (innerExceptionMessage.Contains("JobNo"))
            {
                AddErrors("Invalid Job No.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";

            }
            else if (innerExceptionMessage.Contains("CK_ProdPlan_CheckProdPlanProcess"))
            {
                AddErrors("Process cannot be changed. Not matching with master planning.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";
            }
            else
            {
                AddErrors(dbUpdateException.Message.ToString() + " ___" + DateTime.UtcNow);
                TempData["CustomErrorMessage"] = dbUpdateException.Message.ToString() + " - " + dbUpdateException.InnerException.Message.ToString();

            }
        }

        private string HandleDbEntityValidationException(DbEntityValidationException dbEntityValidationException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbEntityValidationException));
            // Retrieve the error messages as a list of strings.
            var errorMessages = dbEntityValidationException.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(dbEntityValidationException.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = dbEntityValidationException.InnerException.Message;
            AddErrors(exceptionMessage + " ___" + DateTime.UtcNow);

            return exceptionMessage;
        }

        private void HandleException(Exception exception)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            TempData["CustomErrorMessage"] = exception.Message;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;
            AddErrors(exception.Message.ToString() + " ___" + DateTime.UtcNow);
        }

        private void AddErrors(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        #endregion Handle Exception
    }
}