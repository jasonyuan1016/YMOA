using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.UnitTest
{
    /// <summary>
    ///  任务测试类
    /// </summary>
    [TestClass]
    public class TaskTest
    {

        /// <summary>
        ///  任务添加
        /// </summary>
        [TestMethod]
        public void TestTaskInsert()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = Guid.NewGuid().To16String();
            paras["Name"] = "测试任务1";
            paras["ProjectId"] = "4adf5dfe42afa";
            paras["ParentId"] = "0";
            paras["EndTime"] = "2019-11-22";
            paras["Describe"] = "测试数据1"; //可以为空
            paras["Remarks"] = "测试数据1"; //可以为空
            paras["Estimate"] = decimal.Parse("200");
            paras["Consume"] = decimal.Parse("0.0");
            paras["Sort"] = 1;
            paras["State"] = 1;
            paras["Send"] = null; //可以为空
            paras["CreateBy"] = "admin";
            paras["CreateTime"] = DateTime.Now;
            // 可以为空
            List<TeamEntity> listTeam = new List<TeamEntity>();
            TeamEntity team = new TeamEntity();
            team.ProjectId = "4adf5dfe42afa";
            team.Person = "admin";
            listTeam.Add(team);
            team = new TeamEntity();
            team.ProjectId = "4adf5dfe42afa";
            team.Person = "J·Y";
            listTeam.Add(team);
            team = new TeamEntity();
            team.ProjectId = "4adf5dfe42afa";
            team.Person = "zxy";
            listTeam.Add(team);
            // 可以为空
            List<AccessoryEntity> listAccessory = new List<AccessoryEntity>();
            AccessoryEntity accessory = new AccessoryEntity();
            accessory.ID = Guid.NewGuid().To16String();
            accessory.Name = "测试附件1";
            accessory.AccessoryUrl = "测试路径1";
            listAccessory.Add(accessory);
            accessory = new AccessoryEntity();
            accessory.ID = Guid.NewGuid().To16String();
            accessory.Name = "测试附件2";
            accessory.AccessoryUrl = "测试路径2";
            listAccessory.Add(accessory);
            bool result = DALCore.GetInstance().TaskCore.TaskInsert(paras, listTeam, listAccessory);
            Assert.AreEqual(result, true);
        }

        /// <summary>
        ///  任务修改
        /// </summary>
        [TestMethod]
        public void TestTaskUpdate()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = "375c5d125f1c586e";
            paras["Name"] = "测试任务1-1";
            paras["ProjectId"] = "1";
            paras["ParentId"] = "0";
            paras["EndTime"] = "2019-11-22";
            paras["Describe"] = "测试数据1-1"; //可以为空
            paras["Remarks"] = "测试数据1-1"; //可以为空
            paras["Estimate"] = decimal.Parse("200");
            paras["Consume"] = decimal.Parse("0.0");
            paras["Sort"] = 1;
            paras["State"] = 1;
            paras["Send"] = null; //可以为空
            // 可以为空
            List<TeamEntity> listTeam = new List<TeamEntity>();
            TeamEntity team = new TeamEntity();
            team.ProjectId = "1";
            team.Person = "user2";
            listTeam.Add(team);
            team = new TeamEntity();
            team.ProjectId = "1";
            team.Person = "user3";
            listTeam.Add(team);
            // 可以为空
            List<AccessoryEntity> listAccessory = new List<AccessoryEntity>();
            AccessoryEntity accessory = new AccessoryEntity();
            accessory.ID = Guid.NewGuid().To16String();
            accessory.Name = "测试附件1-1";
            accessory.AccessoryUrl = "测试路径1-1";
            listAccessory.Add(accessory);
            accessory = new AccessoryEntity();
            accessory.ID = Guid.NewGuid().To16String();
            accessory.Name = "测试附件2-2";
            accessory.AccessoryUrl = "测试路径2-2";
            listAccessory.Add(accessory);
            bool result = DALCore.GetInstance().TaskCore.TaskUpdate(paras, listTeam, listAccessory);
            Assert.AreEqual(result, true);
        }

        /// <summary>
        /// 任务批量添加
        /// </summary>
        [TestMethod]
        public void TestBatchInsert()
        {
            List<TaskEntity> listTask = new List<TaskEntity>();
            TaskEntity task = new TaskEntity();
            task.Name = "测试任务1";
            task.ProjectId = "1";
            task.ParentId = "0";
            task.EndTime = DateTime.ParseExact("2019-11-11", "yyyy-MM-dd", CultureInfo.CurrentCulture);
            task.Describe = "测试数据1";
            task.Remarks = "测试数据1";
            task.Estimate = decimal.Parse("200");
            task.Consume = decimal.Parse("0.0");
            task.Sort = 1;
            task.State = 1;
            task.Send = null;
            task.CreateBy = "zxy";
            task.CreateTime = DateTime.Now;
            List<TeamEntity> listTeam = new List<TeamEntity>();
            TeamEntity team = new TeamEntity();
            team.ProjectId = "1";
            team.Person = "user22";
            listTeam.Add(team);
            task.listTeam = listTeam;
            listTask.Add(task);
            task = new TaskEntity();
            task.Name = "测试任务2";
            task.ProjectId = "1";
            task.ParentId = "0"; // 不可为空
            task.EndTime = DateTime.ParseExact("2019-11-11", "yyyy-MM-dd", CultureInfo.CurrentCulture);
            task.Describe = null;
            task.Remarks = null;
            task.Estimate = decimal.Parse("200");
            task.Consume = decimal.Parse("0.0");
            task.Sort = 1;
            task.State = 1;
            task.Send = "";
            task.CreateBy = "zxy";
            task.CreateTime = DateTime.Now;
            listTeam = new List<TeamEntity>();
            team = new TeamEntity();
            team.ProjectId = "1";
            team.Person = "user33";
            listTeam.Add(team);
            task.listTeam = listTeam;
            listTask.Add(task);
            int result = DALCore.GetInstance().TaskCore.BatchInsert(listTask);
            Assert.AreNotEqual(result, 0);
        }

        /// <summary>
        ///  任务删除
        /// </summary>
        [TestMethod]
        public void TestTaskDelete()
        {
            bool result = DALCore.GetInstance().TaskCore.TaskDelete("45de68f5db7bdba5");
            Assert.AreEqual(result, true);
        }

        /// <summary>
        ///  判断用户修改权限
        /// </summary>
        [TestMethod]
        public void TestTaskUpdateJudge()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProductId"] = "4adf5dfe42afa";
            paras["TaskId"] = "17845c65344d4f3e";
            paras["UpdateBy"] = "zxy";
            bool result = DALCore.GetInstance().TaskCore.TaskUpdateJudge(paras);
            Assert.AreEqual(result, true);
        }

        /// <summary>
        ///  判断用户修改权限
        /// </summary>
        [TestMethod]
        public void TestTaskInsertJudge()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProductId"] = "4adf5dfe42afa";
            paras["CreateBy"] = "admin";
            bool result = DALCore.GetInstance().TaskCore.TaskInsertJudge(paras);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestTaskList()
        {
            List<TaskEntity> tasks = new List<TaskEntity>();
            int total = 0;
            DALCore.GetInstance().TaskCore.TaskList<TaskEntity,int>(0, "J·Y",1,2, "", "ASC", ref tasks, ref total);
            Assert.AreNotEqual(tasks.Count, 0);
        }

    }
}
