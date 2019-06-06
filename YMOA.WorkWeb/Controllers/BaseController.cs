using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;

namespace YMOA.WorkWeb.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 数据交互接口
        /// </summary>
        internal DALCore DALUtility => DALCore.GetInstance();
        protected ContentResult PagerData(int totalCount, object rows)
        {
            return Content(JsonConvert.SerializeObject(new { total = totalCount.ToString(), rows = rows }));
        }

        protected ContentResult OperationReturn(bool _success, string _msg = "")
        {
            return Content(JsonConvert.SerializeObject(new { msg = _msg != "" ? _msg : (_success ? "操作成功" : "操作失败"), success = _success }));
        }

        protected ContentResult AjaxReturn(ResultType resultType, string _msg)
        {
            return Content(new AjaxResult { state = resultType.ToString(), message = _msg }.ToJson());
        }

        // <summary>
        /// 当前登入者账号
        /// </summary>
        protected string UserId
        {
            get
            {
                if (Session["UserId"] != null)
                {
                    return Session["UserId"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 当前登入者名称
        /// </summary>
        protected string RealName
        {
            get
            {
                if (Session["RealName"] != null)
                {
                    return Session["RealName"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 当前语系
        /// </summary>
        public string ClientCulture { get; set; }

        /// <summary>
        /// 当前登入者权限ID
        /// </summary>
        protected int RoleId
        {
            get
            {
                if (Session["RoleId"] != null)
                {
                    return Convert.ToInt32(Session["RoleId"]);
                }
                else
                {
                    return 0;
                }
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = string.Empty;
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
            {
                cultureName = cultureCookie.Value;
            }
            else
            {
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                    Request.UserLanguages[0] : null;
            }
            cultureName = CultureHelper.GetImplementedCulture(cultureName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            return base.BeginExecuteCore(callback, state);
        }
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class result_base
    {
        public string errorCode { get; set; } = "";
        public string errorMsg { get; set; } = "";
        public object result { get; set; }
    }
}
