using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace YMOA.WorkWeb.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult Default()
        {
            return View();
        }
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}