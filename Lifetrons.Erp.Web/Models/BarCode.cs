using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using Lifetrons.Erp.Web.Models;

//namespace BarCodeGenerators.Views.BarCode
namespace Lifetrons.Erp.Web.Models
{
    public class IDAutomationBarCode
    {
        public static string BarcodeImageGenerator(string Code)
        {
            byte[] BarCode;
            string BarCodeImage;
            Bitmap objBitmap = new Bitmap(Code.Length * 28, 100);
            using (Graphics graphic = Graphics.FromImage(objBitmap))
            {
                Font newFont = new Font("IDAutomationHC39M Free Version", 18, FontStyle.Regular);
                PointF point = new PointF(2f, 2f);
                SolidBrush balck = new SolidBrush(Color.Black);
                SolidBrush white = new SolidBrush(Color.White);
                graphic.FillRectangle(white, 0, 0, objBitmap.Width, objBitmap.Height);
                graphic.DrawString("*" + Code + "*", newFont, balck, point);
            }
            using (MemoryStream Mmst = new MemoryStream())
            {
                objBitmap.Save(Mmst, ImageFormat.Png);
                BarCode = Mmst.GetBuffer();
                BarCodeImage = BarCode != null ? "data:image/jpg;base64," + Convert.ToBase64String((byte[])BarCode) : "";
                return BarCodeImage;
            }
        }
    }

    public class BarCode
    {
        public static Byte[] GetBarcodeImage(string Code, string file)
        {
            try
            {
                BarCode39 _barcode = new BarCode39();
                int barSize = 32;
                string fontFile = HttpContext.Current.Server.MapPath("~/fonts/FREE3OF9.TTF");
              
                return (_barcode.Code39("*" + Code + "*", barSize, true, file, fontFile));
            }
            catch (Exception ex)
            {
                //ErrorLog.WriteErrorLog("Barcode", ex.ToString(), ex.Message);
            }
            return null;
        }
    }
}