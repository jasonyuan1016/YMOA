using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 工时控制层
    /// </summary>
    public class HoursController : BaseController
    {
        /// <summary>
        /// 返回项目工时记录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 返回任务完成工时记录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProHours()
        {
            return View();
        }
        /// <summary>
        /// 获取所有项目工时
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetAllProject()
        {

            var hoursList = DALUtility.HoursCore.GetAllProject<HoursEntity>();
            return Content(hoursList.ToJson());
        }
        /// <summary>
        /// 获取项目中子任务工时详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProjectByPerson()
        {
            string proName = Request["ProName"];
            string startTime = Request["startTime"];
            string endTime = Request["endTime"];

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProName"] = proName;
            paras["StartTime"] = startTime == null ? DateTime.Now.AddDays(-7):DateTime.Parse(startTime);
            paras["EndTime"] = endTime == null ? DateTime.Now.ToDate() :DateTime.Parse(endTime);
            var hoursList = DALUtility.HoursCore.GetProjectByPerson<HoursEntity>(paras);
            return Content(hoursList.ToJson());
        }

    }
}