using Dapper;
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
    public class ItemController : BaseController
    {
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }
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
        public ActionResult ProjectEdit(string ID = "")
        {
            var projectInfo = new ProjectEntity();
            if (!ID.IsEmpty())
            {
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
                //pars["Team"] = null;
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
                if(projectEntity.StartTime< DateTime.Now.ToDate())
                {
                    projectEntity.State = 1;
                }
                pars["State"] = projectEntity.State;
            }
            else
            {
                pars["State"] = projectEntity.State;
            }
            
            int rows = DALCore.GetInstance().ProjectCore.Save(pars);
            return OperationReturn(rows == 0);
        }

        public ActionResult Delete()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["ID"] = Request["ID"];
            return OperationReturn(DALUtility.ProjectCore.DeleteProject(param));
        }
    }
}