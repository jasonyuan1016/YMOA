using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    public class TaskController : BaseController
    {
        // GET: Task
        public ActionResult Index()
        {
            List<ProjectEntity> products = DALCore.GetInstance().TaskCore.GetProject<ProjectEntity>().ToList();
            ViewData["products"] = products;
            return View();
        }

        public ActionResult GetGridJson(Pagination pagination)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["qryTag"] = Request["qryTag"] == "" ? 0 : Convert.ToInt32(Request["qryTag"]);
            paras["userName"] = Session["RealName"].ToString();
            paras["page"] = pagination.page;
            paras["rows"] = pagination.rows;
            int iCount = 0;
            var tasks = DALCore.GetInstance().TaskCore.UserTaskList<TaskEntity>(paras, pagination.sidx, pagination.sord, out iCount);
            pagination.records = iCount;
            var data = new
            {
                rows = tasks,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        ///  添加，修改弹框
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Edit(string ID = "")
        {
            List<ProjectEntity> products = DALCore.GetInstance().TaskCore.GetProject<ProjectEntity>().ToList();
            ViewData["products"] = products;
            TaskEntity taskEntity = new TaskEntity();
            Dictionary<string, object> para = null;
            if (ID != "" )
            {
                para = new Dictionary<string, object>();
                para["ID"] = ID;
                taskEntity = DALCore.GetInstance().TaskCore.QryTask<TaskEntity>(para);
            }
            para = new Dictionary<string, object>();
            if (ID == "")
            {
                para["projectId"] = ID == "" ? products[0].ID : ID;
                para["TaskId"] = "0";
            }
            else
            {
                para["TaskId"] = ID;
            }
            ViewData["teams"] = DALCore.GetInstance().TaskCore.GetTeams<TeamEntity>(para);
            return View(taskEntity);
        }

        /// <summary>
        ///  批量添加
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchAdd()
        {
            return View();
        }

    }
}