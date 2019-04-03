using YMOA.Comm;
using YMOA.DALFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YMOA.Web.Controllers
{
    public class ValidatorController : Controller
    {
        //
        // GET: /Validator/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetValidatorGraphics()
        {
            ValidatorCodeTools obj = new ValidatorCodeTools();

            string code = obj.CreateValidateCode(5);
            //采用cookie
            CookiesHelper.SetCookie("ValidatorCode", code);
            byte[] graphic = obj.CreateValidateGraphic(code);
            return File(graphic, @"image/jpeg");
        }
    }
}
