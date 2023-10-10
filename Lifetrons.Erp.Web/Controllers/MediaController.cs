using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Web.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Security.Policy;
using Microsoft.Practices.Unity;
using System.Web;
using Microsoft.Owin.Security;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Lifetrons.Erp.Controllers
{
    public class MediaController : BaseController
    {
        public class FacebookProfileImageResponse
        {
            public class Data
            {
                public bool is_silhouette { get; set; }
                public string url { get; set; }
            }
        }
        private readonly IMediaService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        [Dependency]
        public IProductService ProductService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

        private string _imagePath;

        public string ImagePath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_imagePath))
                    return _imagePath;

                if (ControllerContext == null)
                    return null;

                var context = ControllerContext.HttpContext;
                if (context == null)
                    return null;

                var server = context.Server;
                if (server == null)
                    return string.Empty;

                _imagePath = server.MapPath("~/images/bunny-peanuts.jpg");
                return _imagePath;
            }
        }

        public static readonly List<string> ParentTypes = new List<string>
        {
            "User",
            "Product",
            "Item",
            "Event",
            "Misc",
        };

        public MediaController(IMediaService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
        
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Gallery()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //Get user image list from file system. 
            //This searches for userId in imageNames in file system. ImageName format is ParentId_UserId_UTCDateTime. Not using this method coz it may be expensive.
            //var imageFiles = Directory.GetFiles(Server.MapPath("~/Images/Cricket/"));
            //var imagesModel = new ImageGallery();
            //foreach (var item in imageFiles)
            //{
            //    if (item.Contains(userId))
            //    {
            //        imagesModel.ImageList.Add(Path.GetFileName(item));
            //    }
            //}

            //Get user image list from database
            var galleryModel = await _service.FindAsyncAllByUserId(applicationUser.Id);

            return View(galleryModel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UploadVideo(string ParentType, string ParentId, string SystemTags, string returnURL)
        {
            ViewData["ParentType"] = ParentType;
            ViewData["ParentId"] = ParentId;
            ViewData["SystemTags"] = SystemTags;
            ViewData["returnURL"] = returnURL;
            return View("UploadImage");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UploadImage(string ParentType, string ParentId, string SystemTags, string returnURL)
        {
            if (Request.IsAuthenticated)
            {
                if (ParentTypes.Contains(ParentType))
                {
                    ViewData["ParentType"] = ParentType;
                    ViewData["ParentId"] = ParentId;
                    ViewData["SystemTags"] = SystemTags;
                    ViewData["returnURL"] = returnURL;
                    return View("UploadImage");
                }
            }

            return HttpNotFound();
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UploadImage([Bind(Include = "ParentType,ParentId,MediaType,MediaPath,MediaName,Tags,ShrtDesc,Desc")] Lifetrons.Erp.Data.Media instance, Stream imageStream, string returnURL)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            if (!ParentTypes.Contains(instance.ParentType) || applicationUser == null)
            {
                return HttpNotFound();
            }

            //Custom Image Folder name (Product, Item etc) under images folder according to image type
            var imageSavePath = @"~\images\";
            var imageUrlPath = "/images/";
            bool boolCreateThumbnailforIndex = false; // This will be true when first image of Item/Product is saved, it will create an additional image with Item/Product Guid and ".jpeg". So that its thumbnail can be shown at Index page.

            //Profile image checks
            if (instance.ParentType == "User")
            {
                // Check if current application user connects/owns the parent record
                var user = _aspNetUserService.Find(applicationUser.Id);
                if (user.Id != applicationUser.Id)
                {
                    AddErrors("You cannot upload image for others.");
                    return View("UploadImage");
                }
                else
                {
                    //Check if number of quota images for ParenType is already full
                    var images = await _service.FindAsyncByParentId(instance.ParentId.ToString());
                    if (images.Count() >= 10)
                    {
                        AddErrors("Max image count (10) reached for this profile. Please delete some profile images from gallery.");
                        return View("UploadImage");
                    }
                }
            }


            //PlayerRecord image checks
            if (instance.ParentType == "Product")
            {
                // Check if current application user connects/owns the parent record
                var product = ProductService.Find(instance.ParentId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                if (!(User.IsInRole("Support") || User.IsInRole("SuperAdmin")))
                {
                    AddErrors("UnAuthorized user.");
                    return View("UploadImage");
                }
                else
                {
                    //Check if number of quota images for ParenType is already full
                    var images = await _service.FindAsyncByParentId(instance.ParentId.ToString());
                    if (images.Count() >= 5)
                    {
                        AddErrors("Max image count (5) reached for this record.");
                        return View("UploadImage");
                    }

                    if (images.Count() == 0)
                    {
                        boolCreateThumbnailforIndex = true;
                    }

                    imageSavePath = @"~\images\Product\";
                    imageUrlPath = "/images/Product/";
                }
            }

            //Match image checks
            if (instance.ParentType == "Item")
            {
                // Check if current application user connects/owns the parent record
                var item = ItemService.Find(instance.ParentId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                if (!(User.IsInRole("Support") || User.IsInRole("SuperAdmin")))
                {
                    AddErrors("UnAuthorized user.");
                    return View("UploadImage");
                }
                else
                {
                    //Check if number of quota images for ParenType is already full
                    var images = await _service.FindAsyncByParentId(instance.ParentId.ToString());
                    if (images.Count() >= 5)
                    {
                        AddErrors("Max image count (5) reached for this record. ");
                        return View("UploadImage");
                    }

                    if (images.Count() == 0)
                    {
                        boolCreateThumbnailforIndex = true;
                    }

                    imageSavePath = @"~\images\Item\";
                    imageUrlPath = "/images/Item/";
                }
            }

            //Match image checks
            if (instance.ParentType == "Event")
            {                
                if (!(User.IsInRole("Support") || User.IsInRole("SuperAdmin")))
                {
                    AddErrors("UnAuthorized user.");
                    return View("UploadImage");
                }

                imageSavePath = @"~\images\Event\";
                imageUrlPath = "/images/Event/";
            }

            //Match image checks
            if (instance.ParentType == "Other")
            {
                if (!(User.IsInRole("Support") || User.IsInRole("SuperAdmin")))
                {
                    AddErrors("UnAuthorized user.");
                    return View("UploadImage");
                }
            }

            //Image upload code starts
            WebImage image = WebImage.GetImageFromRequest();
            long sizeInKBytes = image.GetBytes().Length / 1024;

            if (image != null)
            {
                while (sizeInKBytes > 200)
                {
                    image.Resize(image.Width / 2, image.Height / 2, true, true);
                    sizeInKBytes = image.GetBytes().Length / 1024;
                }

                instance.MediaType = "Image";
                //System.IO.Path.GetFileName(image.FileName);
                instance.MediaName = applicationUser.Id + "_" + instance.ParentId.ToString() + "_" + DateTime.UtcNow + System.IO.Path.GetExtension(image.FileName);
                instance.MediaName = instance.MediaName.Replace(":", "-");
                
                //Save image
                image.Save(imageSavePath + instance.MediaName);

                //if saving first image of Item / product, create a thumbnail for index pages
                if (boolCreateThumbnailforIndex)
                {
                    image.Save(imageSavePath + instance.ParentId.ToString() + System.IO.Path.GetExtension(image.FileName));
                }

                //Do not include "~" here in "instance.MediaPath" and as we do not want to include website name with path
                instance.MediaPath = @Url.Content(imageUrlPath + instance.MediaName);

                if (User.Identity.IsAuthenticated)
                {
                    instance.Id = Guid.NewGuid();
                    instance.Tags = instance.Tags + Helper.SysSeparator + ViewData["SystemTags"];
                    instance.OrgId = applicationUser.OrgId.ToSysGuid();
                    instance.CreatedBy = applicationUser.Id;
                    instance.CreatedDate = DateTime.UtcNow;
                    instance.ModifiedBy = applicationUser.Id;
                    instance.ModifiedDate = DateTime.UtcNow;
                    instance.Authorized = true;
                    instance.Active = true;

                    try
                    {
                        //Now create new record for newly uploaded profile picture
                        _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        ViewData["ParentType"] = instance.ParentType;
                        ViewData["ParentId"] = instance.ParentId;
                        ViewData["SystemTags"] = instance.Tags;
                        ViewData["returnURL"] = returnURL;

                        ////Update parent table for database efficiency in Media table
                        //if (instance.ParentType == "Profile")
                        //{
                        //    Profile profile = await _serviceProfile.FindAsync(instance.ParentId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                        //    profile.ProfilePictureId = instance.Id.ToString();
                        //    _serviceProfile.Update(profile, applicationUser.Id, applicationUser.OrgId.ToString());
                        //    await _unitOfWork.SaveChangesAsync();
                        //}

                        Response.Redirect(returnURL);
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                    }
                }
            }

            return View("UploadImage");
        }

        [AllowAnonymous]
        [HttpGet, ActionName("GetAsyncProfileImageThumbnail")]
        public async void GetAsyncProfileImageThumbnail(string userId)
        {
            string profilePicPath = "~" + await _service.GetAsyncProfilePicPath(userId);

            if (!string.IsNullOrEmpty(profilePicPath))
            {
                new WebImage(Server.MapPath(profilePicPath))
                .Resize(100, 100, true, true) // Resizing the image to 100x100 px on the fly... 
                    .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                    .Write();
            }
        }

        [AllowAnonymous]
        [HttpGet, ActionName("GetProfileImageThumbnail")]
        public void GetProfileImageThumbnail(string userId)
        {
            var profilePicPath = _service.GetProfilePicPath(userId);

            if (!string.IsNullOrEmpty(profilePicPath))
            {
                profilePicPath = "~" + profilePicPath;
                new WebImage(Server.MapPath(profilePicPath))
                            .Resize(100, 100, true, true) // Resizing the image to 100x100 px on the fly... 
                                .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                                .Write();
            }
        }

        public void GetImage()
        {
            new WebImage(ImagePath)
                .Write();
        }

        public void GetImage(string mediaPath)
        {
            new WebImage(mediaPath)
                .Write();
        }

        public void GetImage(string mediaPath, int width, int height)
        {
            new WebImage(mediaPath)
                .Resize(width, height, true, true) // Resizing the image on the fly... 
                    .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                    .Write();
        }

        public void GetImageThumbnail(string mediaPath)
        {
            new WebImage(Server.MapPath(mediaPath))
            .Resize(100, 100, true, true) // Resizing the image to 100x100 px on the fly... 
                .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                .Write();
        }

        public void GetProductImageThumbnail(string productId)
        {
            if (!string.IsNullOrEmpty(productId))
            {                
                var relativePath = "~/Images/Product/" + productId + ".jpg";
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (!System.IO.File.Exists(absolutePath))
                {
                    relativePath = "~/Images/Product/" + productId + ".png";
                }

                new WebImage(Server.MapPath(relativePath))
                            .Resize(100, 100, true, true) // Resizing the image to 100x100 px on the fly... 
                                .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                                .Write();
            }
        }

        public void GetProductImageThumbnailByJobNo(string jobNo)
        {
            if (!string.IsNullOrEmpty(jobNo))
            {
                var orderItem = OrderLineItemService.SelectSingle(jobNo);
                var relativePath = "~/Images/Product/" + orderItem.ProductId + ".jpg";
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (!System.IO.File.Exists(absolutePath))
                {
                    relativePath = "~/Images/Product/" + orderItem.ProductId + ".png";
                }
                new WebImage(Server.MapPath(relativePath))
                            .Resize(100, 100, true, true) // Resizing the image to 100x100 px on the fly... 
                                .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                                .Write();
            }
        }

        public void GetItemImageThumbnail(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                var relativePath = "~/Images/Item/" + itemId + ".jpg";
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (!System.IO.File.Exists(absolutePath))
                {
                    relativePath = "~/Images/Item/" + itemId + ".png";
                }

                new WebImage(Server.MapPath(relativePath))
                            .Resize(100, 100, true, true) // Resizing the image to 100x100 px on the fly... 
                                .Crop(1, 1)                    // Cropping it to remove 1px border at top and left sides (bug in WebImage) 
                                .Write();
            }
        }

        [AllowAnonymous]
        [HttpGet, ActionName("FindByParent")]
        public IEnumerable<Media> FindByParent(string parentType, string parentId)
        {
            return _service.FindByParent(parentType, parentId);
        }


        // GET: /Contact/Delete/5
        [EsAuthorize]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Media model = await _service.FindAsync(id.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = await _service.FindAsync(id.ToString());

            if (User.Identity.IsAuthenticated)
            {
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    _service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: MediaId=" + id + " ParentType=" + model.ParentType + " ParentId=" + model.ParentId + " ShrtDesc=" + model.ShrtDesc + " Tags=" + model.Tags);

                    //Delete physical file
                    string fullPath = Request.MapPath(model.MediaPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                return RedirectToAction("Gallery", "Media");
            }

            return HttpNotFound();
        }

        
        public ActionResult Cropped()
        {
            return View();
        }

        public Lifetrons.Erp.WebImageResult GetCropped()
        {
            var result = new Lifetrons.Erp.WebImageResult(
                new WebImage(ImagePath)
                    .Crop(50, 50, 50, 50)
            );
            return result;
        }

        public ActionResult HorizontalFlip()
        {
            return View();
        }

        public void GetHorizontalFlip()
        {
            new WebImage(ImagePath)
                .FlipHorizontal()
                .Write();
        }

        public ActionResult VerticalFlip()
        {
            return View();
        }

        public void GetVerticalFlip()
        {
            new WebImage(ImagePath)
                .FlipVertical()
                .Write();
        }

        public ActionResult Resized()
        {
            return View();
        }

        public void GetResized()
        {
            new WebImage(ImagePath)
                .Resize(200, 200) // resize image to 200x200 px
                .Write();
        }

        public ActionResult RotateLeft()
        {
            return View();
        }

        public void GetRotateLeft()
        {
            new WebImage(ImagePath)
                .RotateLeft()
                .Write();
        }

        public ActionResult RotateRight()
        {
            return View();
        }

        public void GetRotateRight()
        {
            new WebImage(ImagePath)
                .RotateRight()
                .Write();
        }

        public ActionResult TextWatermark()
        {
            return View();
        }

        public void GetTextWatermark()
        {
            new WebImage(ImagePath)
                .AddTextWatermark("Watermark", "white", 14, "Bold")
                .Write();
        }

        public ActionResult ImageWatermark()
        {
            return View();
        }

        public void GetImageWatermark()
        {
            var watermarkPath = HttpContext.Server.MapPath("~/images/watermark.png");
            var watermark = new WebImage(watermarkPath);

            new WebImage(ImagePath)
                .AddImageWatermark(watermark)
                .Write();
        }
    }
}