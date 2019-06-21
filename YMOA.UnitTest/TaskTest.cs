using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            int result = DALCore.GetInstance().TaskCore.BatchInsert(listTask,"zxy");
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
            paras["TaskId"] = "50e6d5bc11686281";
            paras["userName"] = "lyl";
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
            paras["userName"] = "admin";
            bool result = DALCore.GetInstance().TaskCore.TaskInsertJudge(paras);
            Assert.AreEqual(result, true);
        }

        /// <summary>
        ///  查询用户任务
        /// </summary>
        [TestMethod]
        public void TestTaskList()
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars.Add("qryTag", 0);
            pars.Add("userName", "admin");
            Pagination pagination = new Pagination();
            pagination.sidx = "CreateTime";
            pagination.sord = "ASC";
            pagination.rows = 20;
            pagination.page = 1;
            var tasks = DALCore.GetInstance().TaskCore.QryTask<TaskEntity>(pagination, pars).ToList();
            var data = new
            {
                rows = tasks,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            List<string> teams = new List<string>();
            foreach (TaskEntity task in tasks)
            {
                teams.Add(task.ID);
            }
            // 去重复
            teams = teams.Where((x, i) => teams.FindIndex(z => z == x) == i).ToList();
            string strTeams = String.Join("','", teams);
            strTeams = "'" + strTeams + "'";
            // 查询任务团员
            var teamList = DALCore.GetInstance().TeamCore.GetTeams<TeamEntity>(strTeams);
            Assert.AreNotEqual(tasks.Count, 0);
        }

        /// <summary>
        ///  实体转DataTable
        /// </summary>
        [TestMethod]
        public void TestToDatatable()
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
            List<TeamEntity> teams = new List<TeamEntity>();
            foreach (TaskEntity t in listTask)
            {
                foreach (TeamEntity e in t.listTeam)
                {
                    e.ProjectId = t.ProjectId;
                    e.TaskId = t.ID;
                    teams.Add(e);
                }
            }
            string[] strArr = new string[] { "ID","Name", "ProjectId", "ParentId", "EndTime",
                "Describe", "Remarks", "Estimate", "Consume", "Sort", "State", "Send","CreateBy", "CreateTime"};
            DataTable taskDT = ToDatatable.ListToDataTable(listTask, strArr);
            //DataTable teamDT = ToDatatable.ListToDataTable(teams);
        }

        /// <summary>
        ///  根据账号查询真实姓名
        /// </summary>
        [TestMethod]
        public void TestQryRealName()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            string[] strs = { "admin", "zxy" };
            paras["names"] = strs;
            var users = DALCore.GetInstance().UserCore.QryRealName<UserEntity>(paras).ToList();
            Assert.AreNotEqual(users.Count, 0);
        }

    }
}
