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
            List<ProjectEntity> products = DALCore.GetInstance().TaskCore.QryInsertTask<ProjectEntity>(paras).ToList();
            List<TeamEntity> teams = null;
            if (products != null && products.Count > 0)
            {
                paras = new Dictionary<string, object>();
                paras["projectId"] = products[0].ID;
                paras["taskId"] = "0";
                teams = DALCore.GetInstance().TaskCore.GetTeams<TeamEntity>(paras).ToList();
            }
            return Content(JsonConvert.SerializeObject(new { total = products.Count, products, teams }));
        }

        #endregion

        #region 任务相关
        
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
            List<TaskEntity> tasks = DALUtility.TaskCore.QryTask<TaskEntity>(pagination, pars).ToList();
            tasks = DALCore.GetInstance().TaskCore.GetTeams(tasks);
            List<TaskEntityDTO> taskDTOList = SetPermissions(tasks);
            List<ProjectEntity> projects = DALUtility.TaskCore.GetProject<ProjectEntity>().ToList();
            Dictionary<string, string> tasksDictionary = projects.ToDictionary(key => key.ID, value => value.Name);
            foreach (TaskEntityDTO task in taskDTOList)
            {
                task.pName = tasksDictionary[task.ProjectId];
            }
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
        ///  任务树列图
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        private List<TaskEntityDTO> TreeGrid(List<TaskEntityDTO> tasks)
        {
            List<TaskEntityDTO> taskList = tasks.FindAll(t => t.ParentId == "0");
            List<TaskEntityDTO> childNodeList = tasks.FindAll(t => t.ParentId != "0");
            foreach(TaskEntityDTO task in childNodeList)
            {
                int index = taskList.FindIndex(x => x.ID == task.ParentId);
                if (index >= 0)
                {
                    task.subclass = true;
                    taskList.Insert(index+1, task);
                }
                else
                {
                    index = taskList.FindLastIndex(x => x.ProjectId == task.ProjectId);
                    if (index >= 0)
                    {
                        taskList.Insert(index+1, task);
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
        ///  添加，修改任务弹框
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Edit(string ID = "")
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            List<ProjectEntity> products = DALCore.GetInstance().TaskCore.QryInsertTask<ProjectEntity>(paras).ToList();
            ViewData["products"] = products;
            TaskEntity taskEntity = new TaskEntity();
            paras = new Dictionary<string, object>();
            if (ID != "")
            {
                paras = new Dictionary<string, object>();
                paras["ID"] = ID;
                taskEntity = DALCore.GetInstance().TaskCore.QryTask<TaskEntity>(paras);
                var accessorys = DALCore.GetInstance().TaskCore.GetAccessory<AccessoryEntity>(paras);
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
            bool boo = DALCore.GetInstance().TaskCore.TaskInsertJudge(paras);
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
                paras["Consume"] = task.Consume;
                paras["Sort"] = task.Sort;
                paras["State"] = task.State;
                paras["Send"] = null;
                paras["CreateBy"] = UserId;
                paras["CreateTime"] = DateTime.Now;
                boo = DALCore.GetInstance().TaskCore.TaskInsert(paras);
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
            List<ProjectEntity> projects = DALCore.GetInstance().TaskCore.QryInsertTask<ProjectEntity>(paras).ToList();
            // 赛选用户可添加任务
            List<TaskEntity> listTask = tasks.Where(a => projects.Exists(t => a.ProjectId.Contains(t.ID))).ToList();
            if (listTask.Count == 0)
            {
                return OperationReturn(false);
            }
            int count = DALCore.GetInstance().TaskCore.BatchInsert(listTask, user);
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
            bool boo = DALCore.GetInstance().TaskCore.TaskInsertJudge(paras);
            if (boo)
            {
                int count = DALCore.GetInstance().TaskCore.BatchInsert(tasks, user);
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
            paras["ProductId"] = task.ProjectId;
            paras["UpdateBy"] = UserId;
            paras["TaskId"] = task.ID;
            bool boo = DALCore.GetInstance().TaskCore.TaskUpdateJudge(paras);
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
                paras["Consume"] = task.Consume;
                paras["Sort"] = task.Sort;
                paras["State"] = task.State;
                paras["Send"] = null;
                paras["UpdateBy"] = UserId;
                paras["UpdateTime"] = DateTime.Now;
                boo = DALCore.GetInstance().TaskCore.TaskUpdate(paras);
                return AddFailure(boo, task);
            }
            return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskAdd"));
        }

        /// <summary>
        ///  任务删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(string ID, string pId)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProductId"] = pId;
            paras["UpdateBy"] = UserId;
            paras["TaskId"] = ID;
            bool boo = DALCore.GetInstance().TaskCore.TaskUpdateJudge(paras);
            if (boo)
            {
                boo = DALCore.GetInstance().TaskCore.ExistSubtask(ID);
                if (!boo)
                {
                    return OperationReturn(DALCore.GetInstance().TaskCore.TaskDelete(ID));
                }
                else
                {
                    return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_taskExistSubtask"));
                }
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
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = id;
            paras["State"] = state;
            paras["UpdateBy"] = UserId;
            paras["UpdateTime"] = DateTime.Now;
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
            return OperationReturn(DALCore.GetInstance().TaskCore.TaskUpdate(paras));
        }

        /// <summary>
        ///  设定可修改任务
        /// </summary>
        /// <param name="listTask"></param>
        /// <returns></returns>
        private List<TaskEntityDTO> SetPermissions(List<TaskEntity> listTask)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["userName"] = UserId;
            List<TaskEntity> tasks = DALCore.GetInstance().TaskCore.QryUpdateTask<TaskEntity>(paras).ToList();
            List<TaskEntityDTO> taskDTOList = new List<TaskEntityDTO>();
            TaskEntityDTO dTO = null;
            foreach (TaskEntity task in listTask)
            {
                dTO = new TaskEntityDTO(task);
                if (tasks.Exists( t => t.ID == task.ID))
                {
                    dTO.update = 1;
                }
                taskDTOList.Add(dTO);
            }
            return taskDTOList;
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
            // 判断用户是否可添加任务
            string folder = "~/file";
            string path = string.Format("{0}\\{1}", Server.MapPath(folder), url);
            // 文件是否存在
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return OperationReturn(DALCore.GetInstance().TaskCore.DeleteAccessory(id));
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
            return OperationReturn(DALCore.GetInstance().TaskCore.UpdateAccessory(paras));
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
            var teams = DALCore.GetInstance().TaskCore.GetTeams<TeamEntity>(para).ToList();
            return PagerData(teams.Count, teams);
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
                DALCore.GetInstance().TaskCore.SaveTeamAndAccessory(task.ProjectId, task.ID, task.listTeam, list);
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