using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.DALFactory;

namespace YMOA.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 数据交互接口
        /// </summary>
        internal DALCore DALUtility
        {
            get
            {
                return DALCore.GetInstance();
            }
        }

        protected ContentResult PagerData(int totalCount, object rows)
        {
            return Content(JsonConvert.SerializeObject(new { total = totalCount.ToString(), rows = rows }));
        }
    }
}