using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;
using YMOA.WorkWeb.Resources;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    public class TaskController : BaseController
    {

        #region 项目相关
        
        /// <summary>
        ///  查询项目
        /// </summary>
        /// <returns></returns>
        public ActionResult QryProducts()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            // 获取用户可添加项目
            List<ProjectEntity> products = DALUtility.ProjectCore.QryInsertTask<ProjectEntity>(paras).ToList();
            List<UserEntity> users = null;
            if (products != null && products.Count > 0)
            {
                paras = new Dictionary<string, object>();
                paras["projectId"] = products[0].ID;
                paras["taskId"] = "0";
                var teams = DALUtility.TeamCore.GetTeams<TeamEntity>(paras).ToList();
                users = ToUsers(teams);
            }
            return Content(JsonConvert.SerializeObject(new { total = products.Count, products, users }));
        }

        #endregion

        #region 任务相关
        
        /// <summary>
        ///  任务管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  任务批量添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchAdd()
        {
            return View();
        }

        /// <summary>
        ///  子添加
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchChildToAdd()
        {
            return View();
        }

        /// <summary>
        ///  任务查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetGridJson(Pagination pagination)
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars.Add("qryTag", Request["qryTag"] == "" ? 0 : Convert.ToInt32(Request["qryTag"]));
            if (Request["ProjectId"] != null && Request["ProjectId"] != "")
            {
                pars.Add("ProjectId", Request["ProjectId"]);
            }
            pars.Add("userName", UserId);
            // 查询任务
            List<TaskEntityDTO> tasks = DALUtility.TaskCore.QryTask<TaskEntityDTO>(pagination, pars).ToList();
            // 写入成员
            tasks = JoinTeams(tasks);
            // 设定用户可修改任务
            List<TaskEntityDTO> taskDTOList = SetPermissions(tasks);
            // 分布树列图
            taskDTOList = TreeGrid(taskDTOList);
            var data = new
            {
                rows = taskDTOList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        ///  添加，修改任务弹框
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Edit(string ID = "")
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            List<ProjectEntity> products = DALUtility.ProjectCore.QryInsertTask<ProjectEntity>(paras).ToList();
            ViewData["products"] = products;
            TaskEntity taskEntity = new TaskEntity();
            paras = new Dictionary<string, object>();
            if (ID != "")
            {
                ViewData["Update"] = true;
                paras = new Dictionary<string, object>();
                paras["ID"] = ID;
                taskEntity = DALUtility.TaskCore.QryTask<TaskEntity>(paras);
                var accessorys = DALUtility.TaskCore.GetAccessory<AccessoryEntity>(paras);
                ViewData["accessorys"] = accessorys;
            }
            return View(taskEntity);
        }

        /// <summary>
        ///  任务添加
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public ActionResult Add()
        {
            TaskEntity task = Request.Form["task"].ToObject<TaskEntity>();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProductId"] = task.ProjectId;
            paras["userName"] = UserId;
            // 判断用户是否可添加任务
            bool boo = DALUtility.TaskCore.TaskInsertJudge(paras);
            if (boo)
            {
                paras = new Dictionary<string, object>();
                paras["ID"] = task.ID;
                paras["Name"] = task.Name;
                paras["ProjectId"] = task.ProjectId;
                paras["ParentId"] = task.ParentId;
                paras["EndTime"] = task.EndTime;
                paras["Describe"] = task.Describe;
                paras["Remarks"] = task.Remarks;
                paras["Estimate"] = Math.Round(task.Estimate, 1);
                paras["Consume"] = Math.Round(task.Consume, 1);
                paras["Sort"] = task.Sort;
                paras["State"] = task.State;
                paras["Send"] = null;
                paras["CreateBy"] = UserId;
                paras["CreateTime"] = DateTime.Now;
                boo = DALUtility.TaskCore.TaskInsert(paras);
                return AddFailure(boo, task);
            }
            return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskAdd"));
        }

        /// <summary>
        ///  任务批量添加
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public ActionResult BatchAddTask(List<TaskEntity> tasks)
        {
            if (tasks == null)
            {
                return OperationReturn(true);
            }
            string user = UserId;
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = user;
            // 获取用户可添加项目
            List<ProjectEntity> projects = DALUtility.ProjectCore.QryInsertTask<ProjectEntity>(paras).ToList();
            // 赛选用户可添加任务
            List<TaskEntity> listTask = tasks.Where(a => projects.Exists(t => a.ProjectId.Contains(t.ID))).ToList();
            if (listTask.Count == 0)
            {
                return OperationReturn(false);
            }
            int count = DALUtility.TaskCore.BatchInsert(listTask, user);
            return OperationReturn(count > 0);
        }

        /// <summary>
        ///  批量添加子任务
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public ActionResult BatchChildToAddTask(List<TaskEntity> tasks, string pId)
        {
            if (tasks == null)
            {
                return OperationReturn(true);
            }
            string user = UserId;
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProductId"] = pId;
            paras["userName"] = user;
            // 判断用户是否可添加任务
            bool boo = DALUtility.TaskCore.TaskInsertJudge(paras);
            if (boo)
            {
                int count = DALUtility.TaskCore.BatchInsert(tasks, user);
                return OperationReturn(count > 0);
            }
            return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskAdd"));
        }
        
        /// <summary>
        ///  任务修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Update()
        {
            TaskEntity task = Request.Form["task"].ToObject<TaskEntity>();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            paras["TaskId"] = task.ID;
            bool boo = DALUtility.TaskCore.TaskUpdateJudge(paras);
            if (boo)
            {
                paras = new Dictionary<string, object>();
                paras["ID"] = task.ID;
                paras["Name"] = task.Name;
                paras["ProjectId"] = task.ProjectId;
                paras["ParentId"] = task.ParentId;
                paras["EndTime"] = task.EndTime;
                paras["Describe"] = task.Describe;
                paras["Remarks"] = task.Remarks;
                paras["Estimate"] = Math.Round(task.Estimate, 1);
                paras["Consume"] = Math.Round(task.Consume, 1);
                paras["Sort"] = task.Sort;
                paras["Send"] = null;
                paras["UpdateBy"] = UserId;
                paras["UpdateTime"] = DateTime.Now;
                boo = DALUtility.TaskCore.TaskUpdate(paras);
                return AddFailure(boo, task);
            }
            return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskAdd"));
        }

        /// <summary>
        ///  任务修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult UpdateTaskState(string id, int state)
        {
            DateTime date = DateTime.Now;
            Dictionary<string, object>  paras = new Dictionary<string, object>();
            paras["ID"] = id;
            paras["State"] = state;
            paras["UpdateBy"] = UserId;
            paras["UpdateTime"] = date;
            string action = "";
            if (state == 2)
            {
                action = "Start";
            }
            else if (state == 3)
            {
                action = "Finish";
            }
            else if (state == 4)
            {
                action = "Cancel";
            }
            else if (state == 5)
            {
                action = "Close";
            }
            if (action != "")
            {
                paras[action + "By"] = UserId;
                paras[action + "Time"] = DateTime.Now;
            }
            bool boo = DALUtility.TaskCore.TaskUpdate(paras);
            if (boo)
            {
                paras = new Dictionary<string, object>();
                paras["ID"] = id;
            }
            return OperationReturn(boo);
        }

        /// <summary>
        ///  任务删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(string ID, string pId)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            paras["TaskId"] = ID;
            bool boo = DALUtility.TaskCore.TaskUpdateJudge(paras);
            if (boo)
            {
                boo = DALUtility.TaskCore.ExistSubtask(ID);
                if (!boo)
                {
                    return OperationReturn(DALUtility.TaskCore.TaskDelete(ID));
                }
                else
                {
                    return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskExistSubtask"));
                }
            }
            return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskAdd"));
        }

        /// <summary>
        ///  任务树列图
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        private List<TaskEntityDTO> TreeGrid(List<TaskEntityDTO> tasks)
        {
            List<TaskEntityDTO> taskList = tasks.FindAll(t => t.ParentId == "0");
            List<TaskEntityDTO> childNodeList = tasks.FindAll(t => t.ParentId != "0");
            foreach (TaskEntityDTO task in childNodeList)
            {
                int index = taskList.FindIndex(x => x.ID == task.ParentId);
                if (index >= 0)
                {
                    task.subclass = true;
                    taskList.Insert(index + 1, task);
                }
                else
                {
                    index = taskList.FindLastIndex(x => x.ProjectId == task.ProjectId);
                    if (index >= 0)
                    {
                        taskList.Insert(index + 1, task);
                    }
                    else
                    {
                        taskList.Add(task);
                    }
                }
            }
            return taskList;
        }

        /// <summary>
        ///  任务加入成员
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<TaskEntityDTO> JoinTeams(List<TaskEntityDTO> list)
        {
            List<string> teams = new List<string>();
            foreach (TaskEntityDTO task in list)
            {
                teams.Add(task.ID);
            }
            string strTeams = String.Join("','", teams);
            strTeams = "'" + strTeams + "'";
            // 查询任务团员
            List<TeamEntity> teamList = DALUtility.TeamCore.GetTeams<TeamEntity>(strTeams).ToList();
            // 查询成员姓名
            var users = ToUsers(teamList);
            // List -> Dictionary
            Dictionary<string, string> ListToDictionary = users.ToDictionary(key => key.AccountName, value => value.RealName);
            foreach (TaskEntityDTO task in list)
            {
                task.listTeam = new List<TeamEntity>();
                foreach (TeamEntity team in teamList)
                {
                    if (task.ID == team.TaskId)
                    {
                        team.Person = ListToDictionary[team.Person];
                        task.listTeam.Add(team);
                    }
                }
            }
            return list;
        }

        /// <summary>
        ///  设定可修改任务
        /// </summary>
        /// <param name="listTask"></param>
        /// <returns></returns>
        private List<TaskEntityDTO> SetPermissions(List<TaskEntityDTO> listTask)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            List<TaskEntity> tasks = DALUtility.TaskCore.QryUpdateTask<TaskEntity>(paras).ToList();
            foreach (TaskEntityDTO task in listTask)
            {
                if (tasks.Exists( t => t.ID == task.ID))
                {
                    task.update = 1;
                }
            }
            return listTask;
        }
        
        #endregion

        #region 附件相关

        /// <summary>
        ///  上传文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="accessories"></param>
        /// <param name="failure">上传失败任务</param>
        /// <returns></returns>
        private List<AccessoryEntity> FileUpload(HttpFileCollectionBase files, List<AccessoryEntity> accessories, out List<string> failure)
        {
            failure = new List<string>();
            int count = files.Count;
            string fileName = "";
            List<AccessoryEntity> list = new List<AccessoryEntity>();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    // 获取上传的文件
                    HttpPostedFileBase file = Request.Files[i];
                    fileName = file.FileName;
                    // 获取文件后缀名
                    int nlndex = fileName.LastIndexOf(".");
                    if (nlndex >= 0)
                    {
                        string strExtension = fileName.Substring(nlndex);
                        // 判断文件后缀
                        string fileSuffix = ConfigurationManager.AppSettings["FileSuffix"].ToString();
                        if (fileSuffix.IndexOf(strExtension + ".", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            failure.Add(fileName);
                            // 跳到下一个循环
                            continue;
                        }
                        if (accessories[i].Name == "" || accessories[i].Name == null)
                        {
                            accessories[i].Name = fileName.Substring(0, fileName.LastIndexOf("."));
                        }
                        fileName = accessories[i].ID + strExtension;
                    }
                    string folder = "~/file";
                    // 判断文件夹是否存在
                    if (Directory.Exists(Server.MapPath(folder)) == false)
                    {
                        // 创建文件夹
                        Directory.CreateDirectory(Server.MapPath(folder));
                    }
                    string path = string.Format("{0}\\{1}", Server.MapPath(folder), fileName);
                    // 文件是否存在
                    if (System.IO.File.Exists(path))
                    {
                        failure.Add(accessories[i].Name);
                        // 跳到下一个循环
                        continue;
                    }
                    accessories[i].AccessoryUrl = fileName;
                    list.Add(accessories[i]);
                    // 新建文件,写入用
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                    file.SaveAs(path);
                }
                catch (Exception ex)
                {
                    failure.Add(fileName);
                    // 跳到下一个循环
                    continue;
                }
            }
            return list;
        }

        /// <summary>
        ///  删除附件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult DeleteFile(string id, string url)
        {
            string folder = "~/file";
            string path = string.Format("{0}\\{1}", Server.MapPath(folder), url);
            // 文件是否存在
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return OperationReturn(DALUtility.TaskCore.DeleteAccessory(id));
        }

        /// <summary>
        ///  修改附件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult UpdateFile(string id, string name)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = id;
            paras["Name"] = name;
            return OperationReturn(DALUtility.TaskCore.UpdateAccessory(paras));
        }

        #endregion

        #region 成员相关

        /// <summary>
        ///  查询成员
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public ActionResult GetTeams(string projectId = "", string taskId = "")
        {
            Dictionary<string, object> para = new Dictionary<string, object>();
            if (taskId != "")
            {
                para["taskId"] = taskId;
            }
            else
            {
                para["projectId"] = projectId;
                para["taskId"] = "0";
            }
            var teams = DALUtility.TeamCore.GetTeams<TeamEntity>(para).ToList();
            return PagerData(teams.Count, ToUsers(teams));
        }

        /// <summary>
        ///  根据账号获取姓名
        /// </summary>
        /// <param name="teams"></param>
        /// <returns></returns>
        private List<UserEntity> ToUsers(List<TeamEntity> teams)
        {
            string[] arr = teams.Select(x => x.Person).ToArray();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["names"] = arr;
            var users = DALUtility.UserCore.QryRealName<UserEntity>(paras).ToList();
            return users;
        }

        #endregion

        #region 其他相关

        /// <summary>
        ///  更新成员与附件
        /// </summary>
        /// <param name="boo"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private ActionResult AddFailure(bool boo, TaskEntity task)
        {
            List<string> failure = new List<string>();
            if (boo)
            {
                List<AccessoryEntity> list = FileUpload(Request.Files, task.listAccessory, out failure);
                DALUtility.TaskCore.SaveTeamAndAccessory(task.ProjectId, task.ID, task.listTeam, list);
            }
            if (failure.Count > 0)
            {
                return OperationReturn(boo, Resource.ResourceManager.GetString("ormsg_accessoryAdd") + String.Join(",", failure));
            }
            return OperationReturn(boo);
        }

        #endregion

    }
}