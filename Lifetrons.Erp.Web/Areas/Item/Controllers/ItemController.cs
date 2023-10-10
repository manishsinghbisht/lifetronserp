using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Controllers
{
    [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
    public class ItemController : BaseController
    {
        private readonly IItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMediaService _mediaService;

        [Dependency]
        public IWeightUnitService WeightUnitService { get; set; }

        [Dependency]
        public IItemClassificationService ItemClassificationService { get; set; }

        [Dependency]
        public IItemCategoryService ItemCategoryService { get; set; }

        [Dependency]
        public IItemTypeService ItemTypeService { get; set; }

        [Dependency]
        public IItemGroupService ItemGroupService { get; set; }

        [Dependency]
        public IItemSubGroupService ItemSubGroupService { get; set; }

        [Dependency]
        public ICostingGroupService CostingGroupService { get; set; }

        [Dependency]
        public ICostingSubGroupService CostingSubGroupService { get; set; }

        [Dependency]
        public INatureService NatureService { get; set; }

        [Dependency]
        public IShapeService ShapeService { get; set; }

        [Dependency]
        public IColourService ColourService { get; set; }

        [Dependency]
        public IStyleService StyleService { get; set; }

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        public ItemController(IItemService service, IUnitOfWorkAsync unitOfWork, IMediaService mediaService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
        }

        // GET: Item
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: Item/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Lifetrons.Erp.Data.Item instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId);  //Owner
            instance.WeightUnit = await WeightUnitService.FindAsync(instance.LotWeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.ItemClassification = await ItemClassificationService.FindAsync(instance.ClassificationId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.ItemCategory = await ItemCategoryService.FindAsync(instance.CategoryId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.ItemType = await ItemTypeService.FindAsync(instance.TypeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()); 
            instance.ItemGroup = await ItemGroupService.FindAsync(instance.GroupId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.ItemSubGroup = await ItemSubGroupService.FindAsync(instance.SubGroupId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()); 
            instance.CostingGroup = await CostingGroupService.FindAsync(instance.CostingGroupId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.CostingSubGroup = await CostingSubGroupService.FindAsync(instance.CostingSubGroupId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()); 
            instance.Nature = await NatureService.FindAsync(instance.NatureId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()); 
            instance.Shape = await ShapeService.FindAsync(instance.ShapeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Colour = await ColourService.FindAsync(instance.ColourId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Style = await StyleService.FindAsync(instance.StyleId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            //Media code starts
            var Media = _mediaService.FindByParent("Item", instance.Id.ToString());
            ViewBag.Media = Media;
            //Media Code ends

            return View(instance);
        }

        // GET: Item/Create
        [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.OwnerId = new SelectList(await AspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.LotWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ClassificationId = new SelectList(await ItemClassificationService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CategoryId = new SelectList(await ItemCategoryService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.TypeId = new SelectList(await ItemTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.GroupId = new SelectList(await ItemGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.SubGroupId = new SelectList(await ItemSubGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CostingGroupId = new SelectList(await CostingGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CostingSubGroupId = new SelectList(await CostingSubGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.NatureId = new SelectList(await NatureService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ShapeId = new SelectList(await ShapeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ColourId = new SelectList(await ColourService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.StyleId = new SelectList(await StyleService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            
            return View();
        }

        // POST: Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
        public async Task<ActionResult> Create([Bind(Include = "Name,Code,ShrtDesc,Desc,OwnerId,OrgId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Authorized,Active,Size,LotWeight,QuantityPerLot,LotWeightUnitId,ABCClass,VEDClass,LeadTime,MinStkLvl,SafetyQty,ReOrdrQty,OpeningQty,OpeningLotCost,Wastage,ScrapRate,Gauge,Level,AlternativeId,ClassificationId,CategoryId,TypeId,GroupId,SubGroupId,CostingGroupId,CostingSubGroupId,NatureId,ShapeId,ColourId,StyleId")] Lifetrons.Erp.Data.Item instance)
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
                instance.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;

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

            return RedirectToAction("Details", new { id = instance.Id });
        }

        // GET: Item/Edit/5
        [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lifetrons.Erp.Data.Item instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            ViewBag.OwnerId = new SelectList(await AspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            ViewBag.LotWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LotWeightUnitId);
            ViewBag.ClassificationId = new SelectList(await ItemClassificationService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ClassificationId);
            ViewBag.CategoryId = new SelectList(await ItemCategoryService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CategoryId);
            ViewBag.TypeId = new SelectList(await ItemTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.TypeId);
            ViewBag.GroupId = new SelectList(await ItemGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.GroupId);
            ViewBag.SubGroupId = new SelectList(await ItemSubGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.SubGroupId);
            ViewBag.CostingGroupId = new SelectList(await CostingGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CostingGroupId);
            ViewBag.CostingSubGroupId = new SelectList(await CostingSubGroupService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CostingSubGroupId);
            ViewBag.NatureId = new SelectList(await NatureService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.NatureId);
            ViewBag.ShapeId = new SelectList(await ShapeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ShapeId);
            ViewBag.ColourId = new SelectList(await ColourService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ColourId);
            ViewBag.StyleId = new SelectList(await StyleService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.StyleId);
            
            return View(instance);
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Desc,OwnerId,Authorized,Active,Size,LotWeight,QuantityPerLot,LotWeightUnitId,ABCClass,VEDClass,LeadTime,MinStkLvl,SafetyQty,ReOrdrQty,OpeningQty,OpeningLotCost,Wastage,ScrapRate,Gauge,Level,AlternativeId,ClassificationId,CategoryId,TypeId,GroupId,SubGroupId,CostingGroupId,CostingSubGroupId,NatureId,ShapeId,ColourId,StyleId,TimeStamp")] Lifetrons.Erp.Data.Item instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Lifetrons.Erp.Data.Item model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  StockAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("StockAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of StockAuthorize and record is Authorized, stop editng and show appropriate message.
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
                model.OwnerId = instance.OwnerId;
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.ShrtDesc = instance.ShrtDesc;
                model.Desc = instance.Desc;
                model.Size = instance.Size;
                model.QuantityPerLot = instance.QuantityPerLot;
                model.LotWeight = instance.LotWeight;
                model.LotWeightUnitId = instance.LotWeightUnitId;
                
                model.ABCClass = instance.ABCClass;
                model.VEDClass = instance.VEDClass;
                model.LeadTime = instance.LeadTime;
                model.MinStkLvl = instance.MinStkLvl;
                model.SafetyQty = instance.SafetyQty;
                model.ReOrdrQty = instance.ReOrdrQty;
                model.OpeningQty = instance.OpeningQty;
                model.OpeningLotCost = instance.OpeningLotCost;
                model.Wastage = instance.Wastage;
                model.ScrapRate = instance.ScrapRate;
                model.Gauge = instance.Gauge;
                model.Level = instance.Level;
                model.AlternativeId = instance.AlternativeId;
                model.ClassificationId = instance.ClassificationId;
                model.CategoryId = instance.CategoryId;
                model.TypeId = instance.TypeId;
                model.GroupId = instance.GroupId;
                model.SubGroupId = instance.SubGroupId;
                model.CostingGroupId = instance.CostingGroupId;
                model.CostingSubGroupId = instance.CostingSubGroupId;
                model.NatureId = instance.NatureId;
                model.ShapeId = instance.ShapeId;
                model.ColourId = instance.ColourId;
                model.StyleId = instance.StyleId;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Id", "Name", "Code", "ShrtDesc", "Desc", "OwnerId", "ModifiedBy", "ModifiedDate", "Authorized", "Active", "Size", "AvgWt_Weight", "AvgWt_PerLotPcs", "AvgWt_WeightUnitId", "ABCClass", "VEDClass", "LeadTime", "MinStkLvl", "SafetyQty", "ReOrdrQty", "OpeningQty", "OpeningLotCost", "Wastage", "ScrapRate", "Gauge", "Level", "AlternativeId", "ClassificationId", "CategoryId", "TypeId", "GroupId", "SubGroupId", "CostingGroupId", "CostingSubGroupId", "NatureId", "ShapeId", "ColourId", "StyleId" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.Id });
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

        // GET: Item/Delete/5
        [EsAuthorize(Roles = "StockAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lifetrons.Erp.Data.Item model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Lifetrons.Erp.Data.Item model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: ItemId=" + id + " Name=" + model.Name + " Code=" + model.Code);

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

        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.Code.ToLower().Contains(searchParam.ToLower()))
                    .OrderBy(x => x.Name)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
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
