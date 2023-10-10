using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using ZXing;
using ZXing.Common;

//Erp.Web.Helpers
namespace Lifetrons.Erp.Web
{
    /// <summary>
    /// Converts the "u" querystring in a ".qr" image to a qr code
    /// Example use: img src="test.qr?u=http://www.google.com"
    /// </summary>
    
    public class QRHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //begin the 8 lines o' magic
            string urlToQRify = context.Server.UrlDecode(context.Request.QueryString["u"]);
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions() { Height = 500, Width = 500, Margin = 0 }; //We can resize them with CSS later

            using (var bitmap = qrWriter.Write(urlToQRify))
            {
                using (var stream = new MemoryStream())
                {
                    context.Response.ContentType = "image/png";
                    bitmap.Save(context.Response.OutputStream, ImageFormat.Png);
                }
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        //Code to READ QR Code
        //static void Main(string[] args)
        //{
        //    // create a barcode reader instance
        //    var barcodeReader = new BarcodeReader();

        //    // create an in memory bitmap
        //    var barcodeBitmap = (Bitmap)Bitmap.FromFile(@"C:\Users\jeremy\Desktop\qrimage.bmp");

        //    // decode the barcode from the in memory bitmap
        //    var barcodeResult = barcodeReader.Decode(barcodeBitmap);

        //    // output results to console
        //    Console.WriteLine($"Decoded barcode text: {barcodeResult?.Text}");
        //    Console.WriteLine($"Barcode format: {barcodeResult?.BarcodeFormat}");
        //}

        //Code to WRITE the QR Code
        //static void Main(string[] args)
        //{
        //    // instantiate a writer object
        //    var barcodeWriter = new BarcodeWriter();

        //    // set the barcode format
        //    barcodeWriter.Format = BarcodeFormat.QR_CODE;

        //    // write text and generate a 2-D barcode as a bitmap
        //    barcodeWriter
        //        .Write("https://jeremylindsayni.wordpress.com/")
        //        .Save(@"C:\Users\jeremy\Desktop\generated.bmp");
        //}
    }
}