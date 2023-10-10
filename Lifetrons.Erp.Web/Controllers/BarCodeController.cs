using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Web.Models;
using System.Web.Helpers;

namespace Lifetrons.Erp.Web.Controllers
{
    public class BarCodeController : BaseController
    {
        // GET: Search
        public ActionResult BarCodeImage(string code, string name)
        {
            var BarCodeImage = Lifetrons.Erp.Web.Models.BarCode.GetBarcodeImage(code, name);
            return PartialView("_BarCodeImage", BarCodeImage);
        }

        // GET: Search
        public void GetBarCodeImage(string code, string name)
        {
            byte[] BarCodeImage = Lifetrons.Erp.Web.Models.BarCode.GetBarcodeImage(code, name);
            new WebImage(BarCodeImage).Write();
        }
    }
}