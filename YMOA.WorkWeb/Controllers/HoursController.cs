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
    /// 工时控制器
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
        /// 返回对应项目中成员的任务工时详情页
        /// </summary>
        /// <returns></returns>
        public ActionResult TaskHours() {
            return View();
        }

        /// <summary>
        /// 返回成员工时页
        /// </summary>
        /// <returns></returns>
        public ActionResult PerHours()
        {
            return View();
        }

        /// <summary>
        /// 返回成员在各个项目工时页
        /// </summary>
        /// <returns></returns>
        public ActionResult PerProHours()
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
        /// 获取所有成员工时
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetAllPerson()
        {
            var hoursList = DALUtility.HoursCore.GetAllPerson<HoursEntity>();
            return Content(hoursList.ToJson());
        }

        /// <summary>
        /// 获取项目中子成员工时详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProjectByPerson(DateTime? startTime = null, DateTime? endTime = null)
        {
            string proName = Request["ProName"];

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProName"] = proName;
            string key = startTime == null ? "" : "StartTime";
            paras[key] = startTime;
            key = startTime == null ? "" : "EndTime";
            paras[key] = endTime;
            var hoursList = DALUtility.HoursCore.GetProjectByPerson<HoursEntity>(paras);
            return Content(hoursList.ToJson());
        }

        /// <summary>
        /// 获取成员在项目中工时详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProjectHoursByPerson(DateTime? startTime = null, DateTime? endTime = null)
        {
            string perName = Request["PerName"];

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["PerName"] = perName;
            string key = startTime == null ? "" : "StartTime";
            paras[key] = startTime;
            key = startTime == null ? "" : "EndTime";
            paras[key] = endTime;
            var hoursList = DALUtility.HoursCore.GetProjectHoursByPerson<HoursEntity>(paras);
            return Content(hoursList.ToJson());
        }

        /// <summary>
        /// 获取对应项目中成员的任务工时详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTaskByPerAndPro(DateTime? startTime = null, DateTime? endTime = null)
        {
            string proName = Request["ProName"];
            string perName = Request["PerName"];

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProName"] = proName;
            paras["PerName"] = perName;
            string key = startTime == null ? "" : "StartTime";
            paras[key] = startTime;
            key = startTime == null ? "" : "EndTime";
            paras[key] = endTime;
            var hoursList = DALUtility.HoursCore.GetTaskByPerAndPro<HoursEntity>(paras);
            return Content(hoursList.ToJson());
        }

    }
}