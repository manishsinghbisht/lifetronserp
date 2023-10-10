using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;


namespace Lifetrons.Erp
{
    public class WebImageResult : ActionResult
    {
        private readonly WebImage _image;
        private readonly string _format;

        public WebImageResult(WebImage image) : this(image, null)
        { }

        public WebImageResult(WebImage image, string format)
        {
            _image = image;
            _format = format;
        }

        public WebImage WebImage
        {
            get { return _image; }
        }

        public string Format
        {
            get { return _format; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (_format == null)
                _image.Write(_format);
            else
                _image.Write(_format);
        }
    }
}