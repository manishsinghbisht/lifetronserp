using Lifetrons.Erp.Controllers;
using Repository.Pattern.UnitOfWork;
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Sales.Controllers
{
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Helpers;
    using Lifetrons.Erp.Models;
    using Lifetrons.Erp.Service;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    [EsAuthorize]
    public partial class DashboardController : BaseController
    {
        private readonly IHierarchyService _hierarchyService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IDepartmentService _departmentService;
        private readonly ITeamService _teamService;
        private readonly INoticeBoardService _noticeBoardService;
        private readonly ITargetService _targetService;
        private readonly BuildChart _buildChart;

        public DashboardController(IHierarchyService hierarchyService, ITeamService teamService, IUnitOfWorkAsync unitOfWork,
            IAspNetUserService aspNetUserService, IDepartmentService departmentService, ITargetService targetService, INoticeBoardService noticeBoardService)
        {
            _hierarchyService = hierarchyService;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _departmentService = departmentService;
            _teamService = teamService;
            _targetService = targetService;
            _noticeBoardService = noticeBoardService;
            _buildChart = new BuildChart();
        }
        
        /// <summary>
        /// Creates dashboard data tables and chart
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index(string userId = "", string orgId = "")
        {
            ApplicationUser user = ManageUserForDashboard(userId, orgId);

            //Start DB operation of excuting SPs
            var db = new Entities();
            var resultSet = db.spUserOpenWork(user.Id, user.OrgId);
            
            //Create data sets for dash
            TempData["OpenLeads"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet as IEnumerable<spUserOpenWork_Result>);
            TempData["OpenOpportunities"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserOpenWork_Result>() as IEnumerable<spUserOpenWork_Result>);
            TempData["OpenTasks"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserOpenWork_Result>() as IEnumerable<spUserOpenWork_Result>);
            TempData["OpenCases"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserOpenWork_Result>() as IEnumerable<spUserOpenWork_Result>);
            TempData["OpenQuotes"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserOpenWork_Result>() as IEnumerable<spUserOpenWork_Result>);
            TempData["OpenOrders"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserOpenWork_Result>() as IEnumerable<spUserOpenWork_Result>);

            //Create grapph and keep it in temp data
            UserPerformanceComparisonMonthlyGraph(user.Id);

            //return dashboard view
            return View();
        }

        private ApplicationUser ManageUserForDashboard(string userId, string orgId)
        {
            ApplicationUser user;
            // Get and validate the user and hierarchy
            if (userId == "" && orgId == "")
            {
                //Logged in User's dashboard
                user = new AccountController().GetUserById(User.Identity.GetUserId());
            }
            else
            {
                //Downline User's dashboard. Not logged in user but may be of down line or in team
                user = new AccountController().GetUserById(userId);
                //Get application user for verification
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                //Validate
                if (user.OrgId != applicationUser.OrgId)
                {
                    var exception = new ArgumentNullException("Organization of logged in user not matched.");
                    TempData["CustomErrorMessage"] =
                        "Organization of logged in user not matched.";
                    TempData["CustomErrorDetail"] = exception.Message.GetType();
                    throw exception;
                }
            }

            //Get and Validate User Hierarchy (User's department, Team, ReportsTo). Load temp data for page
            var userHierarchy = _hierarchyService.SelectUserHierarchy(user.Id, user.OrgId.ToString());
            var firstOrDefault = userHierarchy.FirstOrDefault();
            if (firstOrDefault != null)
            {
                var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                var userRoles = userManager.GetRoles(user.Id);
                //Load data for downline and teams. These TempData variable are also used to show hide various links on dashboard.
                if (userRoles.Contains("TeamLevel"))
                {
                    TempData["TeamId"] = firstOrDefault.TeamId;
                    TempData["TeamName"] = firstOrDefault.Team.Name;
                    TempData["DepartmentId"] = null;
                    TempData["DepartmentName"] = null;
                }
                if (userRoles.Contains("DepartmentLevel"))
                {
                    TempData["DepartmentId"] = firstOrDefault.DepartmentId;
                    TempData["DepartmentName"] = firstOrDefault.Department.Name;
                }
            }
            //Load basic user info
            TempData["Username"] = user.UserName;
            TempData["UserId"] = user.Id;

            //Check for new Notices accordian exapands according to OpeningDate
            TempData["NewNoticeExists"] = false;
            var noticeBoard = _noticeBoardService.Get(user.Id, user.OrgId.ToString());
            var firstordefaultNotice = noticeBoard.FirstOrDefault();
            if (firstordefaultNotice != null)
            {
                noticeBoard.ForEach(p =>
                {
                    if (p.OpeningDate.Date == DateTime.Today.Date && p.OpeningDate.Date <= DateTime.Today.Date.AddDays(2))
                    {
                        TempData["NewNoticeExists"] = true;
                    }
                });
            }
            TempData["noticeBoard"] = noticeBoard;

            //Check if Hierarchy accordian on Dashboard should be collapsed or expanded
            TempData["HierarchyExists"] = false;
            var downLineHierarchy = _hierarchyService.SelectDownlineHierarchy(user.Id, user.OrgId.ToString());
            var firstOrDefaultdownLineHierarchy = downLineHierarchy.FirstOrDefault();
            if (firstOrDefaultdownLineHierarchy != null)
            {
                TempData["HierarchyExists"] = true;
            }

            return user;
        }

        [Authorize]
        public ActionResult LeadComparisonChart(string userId)
        {
            if (null != TempData["LeadComparisonGraph"])
            {
                return TempData["LeadComparisonGraph"] as ContentResult;
            }
            var db = new Entities();
            var user = new AccountController().GetUserById(userId);
            //var resultSet = db.sViewDashboardMonthlyLeadOppComaprison(applicationUser.Id, applicationUser.OrgId);
            var resultSet = db.spUserPerformanceComaprisonMonthly(userId, user.OrgId);

            //Reach Lead data set. First two datasets are of lead.
            var currentMonthLeads = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthLeads = currentMonthLeads;
            //Read second data set
            var lastMonthLeads = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthLeads = lastMonthLeads;

            var result = new StringBuilder();

            var leadData = new List<IEnumerable<spUserPerformanceComaprisonMonthly_Result>>
            {
                currentMonthLeads,
                lastMonthLeads
            };

            var leadSeries = new List<decimal>
            {
                currentMonthLeads.Count,
                lastMonthLeads.Count
            };


            var chart = _buildChart.BuildNewChart("Lead Counts Comparison", leadSeries);
            result.Append(_buildChart.getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));

            return Content(result.ToString());
        }

        [Authorize]
        public ActionResult OpportunityComparisonChart(string userId)
        {
            if (null != TempData["OpportunityComparisonGraph"])
            {
                return TempData["OpportunityComparisonGraph"] as ContentResult;
            }
            var db = new Entities();
            var user = new AccountController().GetUserById(userId);
            //var resultSet = db.sViewDashboardMonthlyLeadOppComaprison(applicationUser.Id, applicationUser.OrgId);
            var resultSet = db.spUserPerformanceComaprisonMonthly(userId, user.OrgId);

            //Reach opportunity dataset by moving twice. Third and Fourth datasets are of opportunity
            //Place counter on 2nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();
            //Reach third dataset.
            var currentMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOpportunities = currentMonthOpportunities;
            //Reach fourth dataset.
            var lastMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOpportunities = lastMonthOpportunities;

            var result = new StringBuilder();

            var oppData = new List<IEnumerable<spUserPerformanceComaprisonMonthly_Result>>
            {
                currentMonthOpportunities,
                lastMonthOpportunities
            };

            var oppSeries = new List<decimal>
            {
                currentMonthOpportunities.Count,
                lastMonthOpportunities.Count
            };

            var chart = _buildChart.BuildNewChart("Opportunity Counts Comparison", oppSeries);
            result.Append(_buildChart.getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));

            return Content(result.ToString());
        }

        [Authorize]
        public ActionResult OrderComparisonChart(string userId)
        {
            if (null != TempData["OrderComparisonGraph"])
            {
                return TempData["OrderComparisonGraph"] as ContentResult;
            }

            var db = new Entities();
            var user = new AccountController().GetUserById(userId);
            //var resultSet = db.sViewDashboardMonthlyLeadOppComaprison(applicationUser.Id, applicationUser.OrgId);
            var resultSet = db.spUserPerformanceComaprisonMonthly(userId, user.OrgId);

            //Reach order dataset. fifth and sixth datasets are of opportunity 
            //Place counter on 2nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();
            //Place counter on 3nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();
            //Place counter on 4nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();

            ////Reach order set. 5th dataset
            var currentMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOrders = currentMonthOrders;
            //6th dataset
            var lastMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOrders = lastMonthOrders;

            var result = new StringBuilder();

            var orderData = new List<IEnumerable<spUserPerformanceComaprisonMonthly_Result>>
            {
                currentMonthOrders,
                lastMonthOrders
            };

           var orderSeries = new List<decimal>
            {
                currentMonthOrders.Count,
                lastMonthOrders.Count
            };

            var chart = _buildChart.BuildNewChart("Order Counts Comparison", orderSeries);
            result.Append(_buildChart.getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));

            return Content(result.ToString());
        }

        [Authorize]
        public ActionResult OpportunityAmountComparisonChart(string userId)
        {
            if (null != TempData["OpportunityAmountComparisonGraph"])
            {
                return TempData["OpportunityAmountComparisonGraph"] as ContentResult;
            }
            var db = new Entities();
            var user = new AccountController().GetUserById(userId);
            //var resultSet = db.sViewDashboardMonthlyLeadOppComaprison(applicationUser.Id, applicationUser.OrgId);
            var resultSet = db.spUserPerformanceComaprisonMonthly(userId, user.OrgId);

            //Reach opportunity dataset by moving twice. Third and Fourth datasets are of opportunity
            //Place counter on 2nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();
            //Reach third dataset.
            var currentMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOpportunities = currentMonthOpportunities;
            //Reach fourth dataset.
            var lastMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOpportunities = lastMonthOpportunities;

            var result = new StringBuilder();

            var oppData = new List<IEnumerable<spUserPerformanceComaprisonMonthly_Result>>
            {
                currentMonthOpportunities,
                lastMonthOpportunities
            };

            var oppSeries = new List<decimal>
            {
                // var sum = list.Aggregate((acc, cur) => acc + cur);
                // var result = propertyDetailsList.Where(d => d.Sequence < 4).Sum(d => d.Length);
                Convert.ToDecimal(currentMonthOpportunities.Sum( p => p.LineItemsAmount)),
                Convert.ToDecimal(lastMonthOpportunities.Sum( p => p.LineItemsAmount)),
            };

            var chart = _buildChart.BuildNewChart("Opportunity Amount Comparison", oppSeries);
            result.Append(_buildChart.getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));

            return Content(result.ToString());
        }

        [Authorize]
        public ActionResult OrderAmountComparisonChart(string userId)
        {
            if (null != TempData["OrderAmountComparisonGraph"])
            {
                return TempData["OrderAmountComparisonGraph"] as ContentResult;
            }

            var db = new Entities();
            var user = new AccountController().GetUserById(userId);
            //var resultSet = db.sViewDashboardMonthlyLeadOppComaprison(applicationUser.Id, applicationUser.OrgId);
            var resultSet = db.spUserPerformanceComaprisonMonthly(userId, user.OrgId);

            //Reach order dataset. fifth and sixth datasets are of opportunity 
            //Place counter on 2nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();
            //Place counter on 3nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();
            //Place counter on 4nd resultset
            resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>();

            ////Reach order set. 5th dataset
            var currentMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOrders = currentMonthOrders;
            //6th dataset
            var lastMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOrders = lastMonthOrders;

            var result = new StringBuilder();

            var orderData = new List<IEnumerable<spUserPerformanceComaprisonMonthly_Result>>
            {
                currentMonthOrders,
                lastMonthOrders
            };

           var orderSeries = new List<decimal>
            {
                // var sum = list.Aggregate((acc, cur) => acc + cur);
                // var result = propertyDetailsList.Where(d => d.Sequence < 4).Sum(d => d.Length);
                Convert.ToDecimal(currentMonthOrders.Sum( p => p.LineItemsAmount)),
                Convert.ToDecimal(lastMonthOrders.Sum( p => p.LineItemsAmount)),
            };

            var chart = _buildChart.BuildNewChart("Order Amount Comparison", orderSeries);
            result.Append(_buildChart.getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));

            return Content(result.ToString());
        }

        /// <summary>
        /// This method draws chart in temp data. So this should be used after main dashboard Index call.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        public bool UserPerformanceComparisonMonthlyGraph(string userId)
        {
            var db = new Entities();
            var user = new AccountController().GetUserById(userId);

            var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            var targetDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastDayOfMonth);
            var targetResultSet = _targetService.SelectTarget(userId, user.OrgId.ToString(), targetDate, userId);
            decimal targetFigure = targetResultSet == null ? 0 : targetResultSet.TargetFigure;

            var resultSet = db.spUserPerformanceComaprisonMonthly(user.Id, user.OrgId);

            //Leads
            var currentMonthLeads = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthLeads = currentMonthLeads;

            var lastMonthLeads = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthLeads = lastMonthLeads;

            //Opportunities
            var currentMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOpportunities = currentMonthOpportunities;

            var lastMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOpportunities = lastMonthOpportunities;

            //Orders
            ////Reach order set. 5th dataset
            var currentMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOrders = currentMonthOrders;

            //6th dataset
            var lastMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spUserPerformanceComaprisonMonthly_Result>() as IEnumerable<spUserPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOrders = lastMonthOrders;

            var leadSeries = new List<decimal>
            {
                currentMonthLeads.Count,
                lastMonthLeads.Count
            };

            var oppSeries = new List<decimal>
            {
                currentMonthOpportunities.Count,
                lastMonthOpportunities.Count
            };

            var orderSeries = new List<decimal>
            {
                currentMonthOrders.Count,
                lastMonthOrders.Count,
            };

            var oppAmountSeries = new List<decimal>
            {
                // var sum = list.Aggregate((acc, cur) => acc + cur);
                // var result = propertyDetailsList.Where(d => d.Sequence < 4).Sum(d => d.Length);
                Convert.ToDecimal(currentMonthOpportunities.Sum( p => p.LineItemsAmount)),
                Convert.ToDecimal(lastMonthOpportunities.Sum( p => p.LineItemsAmount)),
            };

            var orderAmountSeries = new List<decimal>
            {
                // var sum = list.Aggregate((acc, cur) => acc + cur);
                // var result = propertyDetailsList.Where(d => d.Sequence < 4).Sum(d => d.Length);
                targetFigure,
                Convert.ToDecimal(currentMonthOrders.Sum( p => p.LineItemsAmount)),
                Convert.ToDecimal(lastMonthOrders.Sum( p => p.LineItemsAmount)),
            };

            var chart = _buildChart.BuildNewChart("Lead Counts Comparison", leadSeries);
            var resultLead = new StringBuilder();
            resultLead.Append(_buildChart.getChartImage(chart));
            resultLead.Append(chart.GetHtmlImageMap("ImageMap"));
            TempData["LeadComparisonGraph"] = Content(resultLead.ToString());

            chart = _buildChart.BuildNewChart("Opportunity Counts Comparison", oppSeries);
            var resultOpportunity = new StringBuilder();
            resultOpportunity.Append(_buildChart.getChartImage(chart));
            resultOpportunity.Append(chart.GetHtmlImageMap("ImageMap"));
            TempData["OpportunityComparisonGraph"] = Content(resultOpportunity.ToString());

            chart = _buildChart.BuildNewChart("Order Counts Comparison", orderSeries);
            var resultOrder = new StringBuilder();
            resultOrder.Append(_buildChart.getChartImage(chart));
            resultOrder.Append(chart.GetHtmlImageMap("ImageMap"));
            TempData["OrderComparisonGraph"] = Content(resultOrder.ToString());

            chart = _buildChart.BuildNewChart("Opportunity Amount Comparison", oppAmountSeries);
            var resultOpportunityAmount = new StringBuilder();
            resultOpportunityAmount.Append(_buildChart.getChartImage(chart));
            resultOpportunityAmount.Append(chart.GetHtmlImageMap("ImageMap"));
            TempData["OpportunityAmountComparisonGraph"] = Content(resultOpportunityAmount.ToString());

            chart = _buildChart.BuildNewChart("Order Amount Comparison", orderAmountSeries);
            var resultOrderAmount = new StringBuilder();
            resultOrderAmount.Append(_buildChart.getChartImage(chart));
            resultOrderAmount.Append(chart.GetHtmlImageMap("ImageMap"));
            TempData["OrderAmountComparisonGraph"] = Content(resultOrderAmount.ToString());

            return true;
        }

        /// <summary>
        /// Return downline hierarchy members
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult DownlineHierarchy(string userId)
        {
            var user = new AccountController().GetUserById(userId);
            var downLineHierarchy = _hierarchyService.SelectDownlineHierarchy(user.Id, user.OrgId.ToString());
            return PartialView("_downlineHierarchy", downLineHierarchy);
        }

    }
}