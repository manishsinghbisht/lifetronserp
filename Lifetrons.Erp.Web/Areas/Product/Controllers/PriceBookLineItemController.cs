using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Product.Controllers
{
    [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit, ProductsView")]
    public class PriceBookLineItemController : BaseController
    {
        private readonly IPriceBookLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPriceBookService _priceBookService;
        private readonly IProductService _productService;

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        // GET: /PriceBookLineItem/
        public PriceBookLineItemController(IPriceBookLineItemService service, IUnitOfWorkAsync unitOfWork, IProductService productService, IPriceBookService priceBookService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _priceBookService = priceBookService;
            _productService = productService;
        }

        [EsAuthorize(Roles = "PriceBookManager")]
        public ActionResult Index(string id, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.PriceBookId = id;


            //var records = await _service.GetAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            //var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            //var enumerablePage = records.OrderBy(x => x.Product.Name).ToPagedList(pageNumber, 1); // will only contain 25 products max because of the pageSize

            //return PartialView(enumerablePage);

            return PartialView(_service.SelectLineItems(id, applicationUser.Id, applicationUser.OrgId.ToString()));
        }

        // GET: /PriceBookLineItem/Details/5
        [EsAuthorize(Roles = "PriceBookManager")]
        public async Task<ActionResult> Details(string priceBookId, string productId)
        {
            if (priceBookId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            PriceBookLineItem instance = await _service.FindAsync(priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = await AspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await AspNetUserService.FindAsync(instance.ModifiedBy); // Modified

            ViewBag.PriceBookId = instance.PriceBookId.ToString();
            ViewBag.PriceBookName = instance.PriceBook.Name;
            //ViewBag.ProductId = instance.ProductId.ToString();
            //ViewBag.ProductName = instance.Product.Name;

            return View(instance);
        }

        // GET: /PriceBookLineItem/Create
        [EsAuthorize(Roles = "PriceBookManager")]
        public async Task<ActionResult> Create(string id)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //Get Master Id and Name for ViewBag
            var instance = await _priceBookService.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            ViewBag.PriceBookId = instance.Id.ToString();
            ViewBag.PriceBookName = instance.Name;

            //Here you need product service coz user can add any product to price book. Otherwise always use "IPriceBookLineItemService"
            ViewBag.ProductId = new SelectList(await _productService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /PriceBookLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PriceBookManager")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Create([Bind(Include = "PriceBookId,ProductId,ListPrice,ShrtDesc,Authorized")] PriceBookLineItem instance)
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

            return RedirectToAction("Details", new { priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
        }

        //GET: /PriceBookLineItem/Edit/5
        [EsAuthorize(Roles = "PriceBookManager")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit(string priceBookId, string productId)
        {
            if (priceBookId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            PriceBookLineItem instance = await _service.FindAsync(priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "ProductsAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("ProductsAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.PriceBookId = instance.PriceBookId.ToString();
            ViewBag.PriceBookName = instance.PriceBook.Name;
            //ViewBag.ProductId = instance.ProductId.ToString();
            //ViewBag.ProductName = instance.Product.Name;

            return View(instance);
        }

        // POST: /PriceBookLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PriceBookManager")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,PriceBookId,ProductId,ListPrice,ShrtDesc,Authorized")] PriceBookLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                PriceBookLineItem model = await _service.FindAsync(instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                // Object Comparison and logging in audit
                var membersToCompare = new List<string>() { "PriceBookId", "ProductId", "ListPrice", "ShrtDesc", "Authorized" };
                var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                //If user is in role of  ProductsAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("ProductsAuthorize"))
                {
                    model.Authorized = instance.Authorized;
                }
                //If user is not in role of ProductsAuthorize and record is Authorized, stop editng and show appropriate message.
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
                model.ListPrice = instance.ListPrice;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        return RedirectToAction("Details", new { priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
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

        // GET: /PriceBookLineItem/Delete/5
        [EsAuthorize(Roles = "PriceBookManager")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid priceBookId, Guid productId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            PriceBookLineItem model = await _service.FindAsync(priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /PriceBookLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PriceBookManager")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid priceBookId, Guid productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    PriceBookLineItem model = await _service.FindAsync(priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    // Object Comparison and logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: PriceBookId=" + priceBookId + " ProductId=" + productId);

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();
                }
                return RedirectToAction("Index");
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

        //////// PATCH: odata/Customers(5)
        ////[AcceptVerbs("PATCH", "MERGE")]
        ////public async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<PriceBookLineItem> patch)
        ////{
        ////    if (!ModelState.IsValid)
        ////    {
        ////        return BadRequest(ModelState);
        ////    }

        ////    PriceBookLineItem instance = await _service.FindAsync(key);

        ////    if (instance == null)
        ////    {
        ////        return HttpNotFound();
        ////    }

        ////    patch.Patch(instance);
        ////    instance.ObjectState = ObjectState.Modified;

        ////    try
        ////    {
        ////        //await _unitOfWorkAsync.SaveChangesAsync();
        ////        await _unitOfWork.SaveChangesAsync();
        ////    }
        ////    catch (DbUpdateConcurrencyException)
        ////    {
        ////        if (!instanceExists(key))
        ////        {
        ////            return NotFound();
        ////        }
        ////        throw;
        ////    }

        ////    return Updated(instance);
        ////}


    }
}
