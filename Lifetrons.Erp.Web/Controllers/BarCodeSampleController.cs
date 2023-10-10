using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lifetrons.Erp.Controllers
{
    public class BarCodeSampleController : BaseController
    {

        //// GET: /BarCode/  
        //public ActionResult BarCode(string Code, string Name)
        //{
        //    var BarCodeImage = Lifetrons.Erp.Web.Models.BarCode.GetBarcodeImage(Code, Name);
        //    return PartialView("_BarCodeImage", BarCodeImage);
        //}

        public ActionResult Index()
        {
            return PartialView("_BarCodeImage");
        }

        // GET: /BarCode/  
        public ActionResult IDAutomationBar()
        {
            return PartialView("_BarCodeImage");
        }

    }
}