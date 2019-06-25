using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/05/30
    /// 项目控制器
    /// </summary>
    public class ItemController : BaseController
    {
        // GET: Item
        #region  获取项目基本信息
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取项目的基本信息
        /// </summary>
        /// <param name="pagination">页面参数</param>
        /// <param name="keywords">查询条件</param>
        /// <returns></returns>
        public ActionResult GetItem(Pagination pagination, string keywords)
        {
            string person = Request["Person"];
            switch (person)
            {
                case null:
                    person = "";
                    break;
                case "":
                    person = "";
                    break;
                case "1":
                    person = UserId;
                    break;
            }
            int state = Request["State"] == null ? 1 : Convert.ToInt32(Request["State"]);
            Dictionary<string, object> paras = new Dictionary<string, object>();
            DynamicParameters dp = new DynamicParameters();

            dp.Add("keywords", keywords == null ? "" : keywords);
            dp.Add("State", state);
            dp.Add("Person", person);
            dp.Add("ID", Request["ID"] == null ? "" : Request["ID"]);
            var data = new
            {
                rows = DALUtility.ProjectCore.QryProjects<ProjectEntity>(dp, pagination),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        #endregion

        #region 项目的新增和修改

        /// <summary>
        /// 新增/修改页面
        /// </summary>
        /// <param name="ID">项目ID</param>
        /// <returns></returns>
        
        public ActionResult ProjectEdit(string ID = "")
        {
            var projectInfo = new ProjectEntity();
            if (!ID.IsEmpty())
            {
                ViewData["boo"] = true;
                DynamicParameters dp = new DynamicParameters();
                dp.Add("keywords", "");
                dp.Add("State", 1);
                dp.Add("Person", "");
                dp.Add("ID",ID);
                projectInfo = DALUtility.ProjectCore.QryProjectInfo<ProjectEntity>(dp);
            }
            return View(projectInfo);
        }
        public ActionResult Add(ProjectEntity project)
        {
            return SubmitForm(project);
        }
        public ActionResult Edit(ProjectEntity project)
        {
            return SubmitForm(project);
        }
        /// <summary>
        /// 提交方法
        /// </summary>
        /// <param name="projectEntity"></param>
        /// <returns></returns>
        public ActionResult SubmitForm(ProjectEntity projectEntity)
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            var dtCheckInfo = new DataTable();
            if (projectEntity.ID.IsEmpty())
            {
                pars["ID"] = Guid.NewGuid().To16String();
                dtCheckInfo.Columns.Add("ProjectId", typeof(string));
                dtCheckInfo.Columns.Add("TaskId", typeof(string));
                dtCheckInfo.Columns.Add("Person", typeof(string));
                foreach (var item in projectEntity.Teams)
                {
                    var row = dtCheckInfo.NewRow();
                    row[0] = pars["ID"];
                    row[1] = "0";
                    row[2] = item.Person;
                    dtCheckInfo.Rows.Add(row);
                    
                }
                pars["Team"] = dtCheckInfo;
            }
            else
            {
                pars["ID"] = projectEntity.ID;
            }

            pars["Name"] = projectEntity.Name;
            pars["StartTime"] = projectEntity.StartTime;
            pars["EndTime"] = projectEntity.EndTime;
            pars["Describe"] = projectEntity.Describe;
            pars["Victors"] = projectEntity.Victors;
            pars["CreateBy"] = UserId;
            pars["CreateTime"] = DateTime.Now.ToDate();
            pars["DutyPerson"] = projectEntity.DutyPerson;
            pars["Remarks"] = projectEntity.Remarks;
            if (projectEntity.State <= 0)
            {
                projectEntity.State = 1;
                pars["State"] = projectEntity.State;
            }
            else
            {
                pars["State"] = projectEntity.State;
            }
            
            int rows = DALUtility.ProjectCore.Save(pars);
            return OperationReturn(rows == 0);
        }
        #endregion

        #region 团队的查看和批量添加
        public ActionResult Team()
        {
            return View();
        }
        /// <summary>
        /// 获取当前项目下团队人员
        /// </summary>
        /// <param name="Id">项目ID</param>
        /// <returns></returns>
        public ActionResult GetTeam(string Id = "")
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("ProjectId", Id);
            paras.Add("TaskId", "0");
            var team = DALUtility.TeamCore.QryTeam<TeamEntity>(paras);
            return Content(JsonConvert.SerializeObject(team));
        }
        /// <summary>
        /// 添加团队人员
        /// </summary>
        /// <param name="teams">团队人员</param>
        /// <returns></returns>
        public ActionResult AddTeam(List<TeamEntity> teams)
        {
            return OperationReturn(DALUtility.TeamCore.Save(teams) > 0);
        }
        #endregion

        #region 删除项目、任务及团队
        public ActionResult Delete()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["ID"] = Request["ID"];
            return OperationReturn(DALUtility.ProjectCore.DeleteProject(param));
        }
        #endregion

        public ActionResult Task()
        {
            return View();
        }
    }
}