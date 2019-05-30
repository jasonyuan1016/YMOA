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

        [TestMethod]
        public void TestTaskSave()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            string id = Guid.NewGuid().To16String();
            // 有ID 就修改
            //paras["ID"] = "ab42e70cbcd43354";

            // 添加
            paras["Name"] = "测试任务";
            paras["ProjectId"] = "1";
            paras["ParentId"] = "";
            paras["EndTime"] = "2019-11-22";
            paras["Describe"] = "测试数据2";
            paras["Remarks"] = "测试数据2";
            paras["Estimate"] = decimal.Parse("200");
            paras["Consume"] = decimal.Parse("0.0");
            paras["Sort"] = 1;
            paras["State"] = 1;
            paras["Send"] = "";
            paras["CreateBy"] = "zxy";
            paras["CreateTime"] = DateTime.Now;

            List<TeamEntity> listTeam = new List<TeamEntity>();
            TeamEntity team = new TeamEntity();
            team.ProjectId = "1";
            team.TaskId = id;
            team.Person = "user2";
            listTeam.Add(team);
            team = new TeamEntity();
            team.ProjectId = "1";
            team.TaskId = id;
            team.Person = "zxy";
            listTeam.Add(team);
            team = new TeamEntity();
            team.ProjectId = "1";
            team.TaskId = id;
            team.Person = "user3";
            listTeam.Add(team);

            List<AccessoryEntity> listAccessory = new List<AccessoryEntity>();
            AccessoryEntity accessory = new AccessoryEntity();
            accessory.ID = Guid.NewGuid().To16String();
            accessory.Name = "测试附件2";
            accessory.TaskId = id;
            accessory.AccessoryUrl = "测试路径2";
            listAccessory.Add(accessory);
            accessory = new AccessoryEntity();
            accessory.ID = Guid.NewGuid().To16String();
            accessory.Name = "测试附件3";
            accessory.TaskId = id;
            accessory.AccessoryUrl = "测试路径3";
            listAccessory.Add(accessory);
            int result = DALCore.GetInstance().TaskCore.TaskSave(paras, listTeam, listAccessory);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void TestBatchInsert()
        {
            List<TaskEntity> listTask = new List<TaskEntity>();
            TaskEntity task = new TaskEntity();
            task.Name = "测试任务1";
            task.ProjectId = "1";
            task.ParentId = "";
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
            task.ParentId = "";
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
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestTaskDelete()
        {
            bool result = DALCore.GetInstance().TaskCore.TaskDelete("9e5e1af053673690");
            Assert.AreEqual(result, true);
        }

    }
}
