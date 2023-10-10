﻿using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Routing;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class OpportunityLineItemController : BaseController
    {
        private readonly IOpportunityLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IOpportunityService _opportunityservice;
        //private readonly IPriceBookService _priceBookService;
        private readonly IPriceBookLineItemService _priceBookLineItemService;
        private readonly IWeightUnitService _weightUnitService;

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        public OpportunityLineItemController(IOpportunityLineItemService service, IUnitOfWorkAsync unitOfWork,
            IOpportunityService opportunityservice, IPriceBookService priceBookService, IPriceBookLineItemService priceBookLineItemService, IWeightUnitService weightUnitService)
        {
            _service = service;
            _opportunityservice = opportunityservice;
            _unitOfWork = unitOfWork;

           // _priceBookService = priceBookService;
            _priceBookLineItemService = priceBookLineItemService;
            _weightUnitService = weightUnitService;
        }

        // GET: /OpportunityLineItem/
        public ActionResult Index(string opportunityId)
        {
            ViewBag.OpportunityId = opportunityId;
            ViewBag.PriceBookId = null;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(opportunityId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (model.Any())
            {
                ViewBag.PriceBookId = model.FirstOrDefault().PriceBookId;
            }
            return PartialView(model);
        }

        // GET: /OpportunityLineItem/Details/5
        public async Task<ActionResult> Details(string opportunityId, string priceBookId, string productId)
        {
            if (priceBookId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            OpportunityLineItem instance = await _service.FindAsync(opportunityId, priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.Opportunity = await _opportunityservice.FindAsync(instance.OpportunityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.AspNetUser = await AspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await AspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.WeightUnit = await _weightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBookLineItem = await _priceBookLineItemService.FindAsync(instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBook = instance.PriceBookLineItem.PriceBook;

            return View(instance);
        }

        // GET: /OpportunityLineItem/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create(string opportunityId, string priceBookId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OpportunityId = opportunityId;
            ViewBag.OpportunityName = (await _opportunityservice.FindAsync(opportunityId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.PriceBookId = null;

            if (!string.IsNullOrEmpty(priceBookId))
            {
                ViewBag.PriceBookId = priceBookId;
            }

            var model = InitializeCreateModel(priceBookId, applicationUser);

            return View(model);
        }

        private OpportunityLineItem InitializeCreateModel(string priceBookId, ApplicationUser applicationUser)
        {
            //Load product from PricBookLineItemSelector
            var model = new OpportunityLineItem();

            if (TempData["PriceBookLineItemSelectorSelectedId"] != null)
            {
                try
                {
                    var selectedProductId = TempData["PriceBookLineItemSelectorSelectedId"].ToString().ToSysGuid();
                    PriceBookLineItem priceBookLineItem = _priceBookLineItemService.Find(priceBookId, selectedProductId.ToString(),
                                applicationUser.Id, applicationUser.OrgId.ToString());
                    model = new OpportunityLineItem()
                    {
                        ProductId = priceBookLineItem.ProductId,
                        PriceBookId = priceBookLineItem.PriceBookId,
                        LineItemPrice = priceBookLineItem.ListPrice,
                        SalesPrice = priceBookLineItem.ListPrice,
                        ShrtDesc = priceBookLineItem.ShrtDesc,
                        Quantity = 1,
                        DiscountPercent = 0,
                    };
                }
                catch (Exception)
                {
                }
                finally
                {
                    TempData["PriceBookLineItemSelectorSelectedId"] = null;
                }
            }
            return model;
        }


        // POST: /OpportunityLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "ShrtDesc,Desc,OpportunityId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,Serial,SpecialInstructions,Authorized")] OpportunityLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                instance.Id = Guid.NewGuid();
                instance.OrgId = applicationUser.OrgId.ToSysGuid();
                instance.CreatedBy = applicationUser.Id;
                instance.CreatedDate = DateTime.UtcNow;
                instance.ModifiedBy = applicationUser.Id;
                instance.ModifiedDate = DateTime.UtcNow;
                instance.Active = true;
                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var exceptionMessage = HandleDbEntityValidationException(ex);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    HandleDbUpdateException(exception);
                    throw;
                }
            }

            return RedirectToAction("Details", new { opportunityId = instance.OpportunityId.ToString(), priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });

        }

        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string opportunityId, string priceBookId, string productId)
        {
            if (string.IsNullOrEmpty(opportunityId) || string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            OpportunityLineItem instance = await _service.FindAsync(opportunityId, priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "SalesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("SalesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);
            ViewBag.OpportunityId = opportunityId;
            ViewBag.OpportunityName = (await _opportunityservice.FindAsync(opportunityId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // POST: /OpportunityLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ShrtDesc,Desc,OpportunityId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,LineItemPrice,Serial,Authorized,SpecialInstructions,TimeStamp")] OpportunityLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                OpportunityLineItem model = await _service.FindAsync(instance.OpportunityId.ToString(), instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("SalesAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of SalesAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Authorized = instance.Authorized;
                model.ShrtDesc = instance.ShrtDesc;
                model.Desc = instance.Desc;

                model.Serial = instance.Serial;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.DiscountPercent = instance.DiscountPercent;
                model.SalesPrice = instance.SalesPrice;
                model.SpecialInstructions = instance.SpecialInstructions;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "ShrtDesc", "Desc", "OpportunityId", "PriceBookId", "ProductId", "SalesPrice", "Quantity", "Weight", "WeightUnitId", "DiscountPercent", "DiscountAmount", "LineItemPrice", "Serial", "Authorized", "SpecialInstructions" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { opportunityId = instance.OpportunityId.ToString(), priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var exceptionMessage = HandleDbEntityValidationException(ex);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    HandleDbUpdateException(exception);
                    throw;
                }

                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: /OpportunityLineItem/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid opportunityId, Guid priceBookId, Guid productId)
        {
            if (opportunityId.ToString().IsNullOrWhiteSpace() || priceBookId.ToString().IsNullOrWhiteSpace() || productId.ToString().IsNullOrWhiteSpace())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            OpportunityLineItem model = await _service.FindAsync(opportunityId.ToString(), priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            model.Opportunity = await _opportunityservice.FindAsync(model.OpportunityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            model.AspNetUser = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(model.CreatedBy);
            model.AspNetUser1 = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(model.ModifiedBy);
            model.WeightUnit = await _weightUnitService.FindAsync(model.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.PriceBookLineItem = await _priceBookLineItemService.FindAsync(model.PriceBookId.ToString(), model.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.PriceBook = model.PriceBookLineItem.PriceBook;

            return View(model);
        }

        // POST: /OpportunityLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid opportunityId, Guid priceBookId, Guid productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    OpportunityLineItem model = await _service.FindAsync(opportunityId.ToString(), priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: OpportunityId=" + opportunityId + " PriceBookId=" + priceBookId + " ProductId=" + productId);
                }
                return RedirectToAction("Details", "Opportunity", new { id = opportunityId });
            }

            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Handle Exception
        private void HandleDbUpdateException(DbUpdateException exception)
        {
            string innerExceptionMessage = exception.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ProductId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Product. Product already exists.";
                }
                else
                {
                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
                }
            }
        }

        private string HandleDbEntityValidationException(DbEntityValidationException ex)
        {
            // Retrieve the error messages as a list of strings.
            var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = ex.GetType();
            return exceptionMessage;
        }

        #endregion Handle Exception
    }
}
