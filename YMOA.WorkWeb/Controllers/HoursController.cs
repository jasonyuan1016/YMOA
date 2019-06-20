using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 工时
    /// </summary>
    public class HoursController : BaseController
    {
        /// <summary>
        /// 返回工时记录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
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
            return null;
        }

    }
}