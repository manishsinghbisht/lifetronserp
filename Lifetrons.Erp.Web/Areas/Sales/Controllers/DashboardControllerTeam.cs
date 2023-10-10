using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize]
    public partial class DashboardController
    {
        /// <summary>
        /// Creates dashboard data tables and chart
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel, TeamLevel")]
        public ActionResult TeamDashboard(string teamId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var teamHierarchy = _hierarchyService.SelectTeamHierarchy(applicationUser.Id, applicationUser.OrgId.ToString(), teamId);

            if (!ValidateTeamHierarchy(teamId, teamHierarchy, applicationUser)) return null;
            TempData["TeamId"] = teamHierarchy.FirstOrDefault().TeamId;
            TempData["TeamName"] = teamHierarchy.FirstOrDefault().Team.Name;

            var db = new Entities();
            var resultSet = db.spTeamOpenWork(teamId.ToSysGuid(), applicationUser.OrgId);

            TempData["OpenLeads"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet as IEnumerable<spTeamOpenWork_Result>);
            TempData["OpenOpportunities"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamOpenWork_Result>() as IEnumerable<spTeamOpenWork_Result>);
            TempData["OpenTasks"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamOpenWork_Result>() as IEnumerable<spTeamOpenWork_Result>);
            TempData["OpenCases"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamOpenWork_Result>() as IEnumerable<spTeamOpenWork_Result>);
            TempData["OpenQuotes"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamOpenWork_Result>() as IEnumerable<spTeamOpenWork_Result>);
            TempData["OpenOrders"] = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamOpenWork_Result>() as IEnumerable<spTeamOpenWork_Result>);

            TeamPerformanceComparisonMonthlyGraph(teamId);

            return View();
        }

        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel, TeamLevel")]
        public ActionResult TeamHierarchy(string teamId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var teamHierarchy = _hierarchyService.SelectTeamHierarchy(applicationUser.Id, applicationUser.OrgId.ToString(), teamId);

            return PartialView("_downlineHierarchy", teamHierarchy);
        }

        /// <summary>
        /// This method draws chart in temp data. So this should be used after TeamDashboard method call.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [EsAuthorize(Roles = "OrganizationLevel, DepartmentLevel, TeamLevel")]
        public ActionResult TeamPerformanceComparisonMonthlyGraph(string teamId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            var targetDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastDayOfMonth);
            var targetResultSet = _targetService.SelectTarget(applicationUser.Id, applicationUser.OrgId.ToString(), targetDate, teamId.ToString());
            decimal targetFigure = targetResultSet == null ? 0 : targetResultSet.TargetFigure;

            var db = new Entities();
            var resultSet = db.spTeamPerformanceComaprisonMonthly(teamId.ToSysGuid(), applicationUser.OrgId.ToSysGuid());

            //Leads
            var currentMonthLeads = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet as IEnumerable<spTeamPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthLeads = currentMonthLeads;

            var lastMonthLeads = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamPerformanceComaprisonMonthly_Result>() as IEnumerable<spTeamPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthLeads = lastMonthLeads;

            //Opportunities
            var currentMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamPerformanceComaprisonMonthly_Result>() as IEnumerable<spTeamPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOpportunities = currentMonthOpportunities;

            var lastMonthOpportunities = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamPerformanceComaprisonMonthly_Result>() as IEnumerable<spTeamPerformanceComaprisonMonthly_Result>);
            //ViewBag.LastMonthOpportunities = lastMonthOpportunities;

            //Orders
            ////Reach order set. 5th dataset
            var currentMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamPerformanceComaprisonMonthly_Result>() as IEnumerable<spTeamPerformanceComaprisonMonthly_Result>);
            //ViewBag.CurrentMonthOrders = currentMonthOrders;

            //6th dataset
            var lastMonthOrders = Lifetrons.Erp.Data.ExtensionMethods.DeepCopy(resultSet.GetNextResult<spTeamPerformanceComaprisonMonthly_Result>() as IEnumerable<spTeamPerformanceComaprisonMonthly_Result>);
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
                lastMonthOrders.Count
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

            var combinedChart = new StringBuilder();
            combinedChart.Append(resultLead);
            combinedChart.Append(resultOpportunity);
            combinedChart.Append(resultOrder);
            combinedChart.Append(resultOpportunityAmount);
            combinedChart.Append(resultOrderAmount);

            return Content(combinedChart.ToString());
        }

        private bool ValidateTeamHierarchy(string teamId, IEnumerable<Hierarchy> teamHierarchy, ApplicationUser applicationUser)
        {
            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            var userRoles = userManager.GetRoles(applicationUser.Id);

            var firstOrDefault = teamHierarchy.FirstOrDefault();
            if (firstOrDefault == null)
            {
                return false;

            }
            else if (firstOrDefault.TeamId != teamId.ToGuid())
            {
                // Unequal teamId is acceptable only if LoggedIn user is in role of "DepartmentLevel" or "OrganizationLevel"
                if (userRoles.Contains("OrganizationLevel") || userRoles.Contains("DepartmentLevel"))
                {
                    return true;
                }

                var exception =
                    new ArgumentNullException("TeamId mentioned in Organization hierarchy is not matching with TeamId passed.");
                TempData["CustomErrorMessage"] =
                    "TeamId mentioned in Organization hierarchy is not matching with TeamId passed.";
                TempData["CustomErrorDetail"] = exception.Message.GetType();
                throw exception;
            }
            return true;
        }

    }
}