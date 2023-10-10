using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Web.Areas.Sales.Models;
using System.IO;
using System.Net;
using Lifetrons.Erp.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using Newtonsoft.Json;

namespace Lifetrons.Erp.Sales.Controllers
{
    [Authorize]
    public class GeoLocationController : BaseController
    {
        // GET: Sales/GeoAPI
        public ActionResult Index()
        {
            return View("Index");
        }

        //// GET api/Csv/GetCSV
        //public HttpResponseMessage GetCSV()
        //{
        //    MemoryStream stream = new MemoryStream();
        //    StreamWriter writer = new StreamWriter(stream);
        //    writer.Write("Hello, World!");
        //    writer.Flush();
        //    stream.Position = 0;

        //    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //    result.Content = new StreamContent(stream);
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "GeoData.csv" };
        //    return result;
        //}


        public JsonResult GetNearbyLocations(string city, string state, string zipCode)
        {
            city = string.IsNullOrEmpty(city) ? "" : city.Trim().ToUpper();
            state = string.IsNullOrEmpty(state) ? "" : state.Trim().ToUpper();
            zipCode = string.IsNullOrEmpty(zipCode) ? "" : zipCode.Trim().ToUpper();

            if (Lifetrons.Erp.Web.Areas.Sales.Models.Data.RawData == null)
            {
                Lifetrons.Erp.Web.Areas.Sales.Models.Data.RawData = new List<RegionModel>();

                using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/GeoAddress.csv")))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        RegionModel newLine = new RegionModel();
                        newLine.LocationName = string.IsNullOrEmpty(values[0]) ? "" : values[0].Trim().ToUpper();
                        newLine.City = string.IsNullOrEmpty(values[3]) ? "" : values[3].Trim().ToUpper();
                        newLine.State = string.IsNullOrEmpty(values[4]) ? "" : values[4].Trim().ToUpper();
                        newLine.ZipCode = string.IsNullOrEmpty(values[5]) ? "" : values[5].Trim().ToUpper();
                        newLine.Latitude = string.IsNullOrEmpty(values[6]) ? "" : values[6].Trim().ToUpper();
                        newLine.Longitude = string.IsNullOrEmpty(values[7]) ? "" : values[7].Trim().ToUpper();
                        newLine.Country = "USA";

                        Lifetrons.Erp.Web.Areas.Sales.Models.Data.RawData.Add(newLine);
                    }
                }
            }

            //Added data which fits the criteria
            List<RegionModel> returnAddresses = new List<RegionModel>();
            foreach (var item in Lifetrons.Erp.Web.Areas.Sales.Models.Data.RawData)
            {
                bool isValidRecord = false;

                if (city != "" && item.City == city && state != "" && item.State == state)
                {
                    isValidRecord = true;
                }
                else if (zipCode != "")
                {
                    double ZipCodeDouble = double.Parse(zipCode);

                    if (item.City == city
                        && (item.ZipCode.Contains(zipCode)
                        || item.ZipCode.Contains((ZipCodeDouble + 1).ToString())
                        || item.ZipCode.Contains((ZipCodeDouble + 2).ToString())
                        || item.ZipCode.Contains((ZipCodeDouble - 1).ToString())
                        || item.ZipCode.Contains((ZipCodeDouble - 2).ToString())))

                        isValidRecord = true;
                }

                if (isValidRecord) returnAddresses.Add(item);
            }

            var sortedList = returnAddresses.OrderBy(a => a.ZipCode).ToList();
            //var jsonString = JsonConvert.SerializeObject(sortedList);

            var response = new JsonResult();
            response = Json(sortedList, JsonRequestBehavior.AllowGet);
            
            return response;
        }

    }
}