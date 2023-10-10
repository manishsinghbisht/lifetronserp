using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Web.Models;

namespace Lifetrons.Erp.Web.Controllers
{
    public class SearchController : BaseController
    {
        // GET: Search
        public ActionResult ShowSearchForm(string action, string controller, string placeholder)
        {
            var model = new Search();
            model.Action = action;
            model.Controller = controller;
            model.Placeholder = placeholder;

            return PartialView("_SearchForm", model);
        }
    }
}