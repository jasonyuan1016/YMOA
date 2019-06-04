using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;
using System.Data;
using Dapper;

namespace YMOA.UnitTest
{
    [TestClass]
    public class PagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars["keyword"] = "Test_";
            Pagination pagination = new Pagination();
            pagination.sidx = "ID";
            pagination.sord = "DESC";
            pagination.rows = 10;
            pagination.page = 1;
            
            var data = new
            {
                rows = DALCore.GetInstance().UserCore.QryUsers<UserEntity>(pagination, pars),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            Assert.AreNotEqual(pagination.records, 0);
        }
        [TestMethod]
        public void TestMethod2()
        {
            DynamicParameters pars = new DynamicParameters();
            pars.Add("CreateBy", "admin");
            Pagination pagination = new Pagination();
            pagination.sidx = "ID";
            pagination.sord = "DESC";
            pagination.rows = 10;
            pagination.page = 1;
            
            var data = new
            {
                rows = DALCore.GetInstance().ProjectCore.QryProjects<ProjectEntity>(pars,pagination),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            Assert.AreNotEqual(pagination.records, 0);
        }
        [TestMethod]
        public void TestMethod3()
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            
            pars["ID"] = Guid.NewGuid().To16String();
            pars["Name"] = "resfsa";
            pars["StartTime"] = DateTime.Now.ToDate();
            pars["EndTime"] = DateTime.Now.ToDate();
            pars["Describe"] = "啊打发撒旦";
            pars["Victors"] = "1,2";
            pars["CreateBy"] = "jaydeny";
            pars["CreateTime"] = DateTime.Now.ToDate();
            pars["DutyPerson"] = "J·Y";
            pars["Remarks"] = "";
            pars["State"] = 1;
            ProjectEntity projectEntity = new ProjectEntity();
            List<TeamEntity> teams = new List<TeamEntity>();
            teams.Add(new TeamEntity() { ProjectId = pars["ID"].ToString(), TaskId = "0", Person = "user1" });
            teams.Add(new TeamEntity() { ProjectId = pars["ID"].ToString(), TaskId = "0", Person = "user12" });
            teams.Add(new TeamEntity() { ProjectId = pars["ID"].ToString(), TaskId = "0", Person = "user14" });
            projectEntity.Teams = teams;
            var dtCheckInfo = new DataTable();
            dtCheckInfo.Columns.Add("ProjectId", typeof(string));
            dtCheckInfo.Columns.Add("TaskId", typeof(string));
            dtCheckInfo.Columns.Add("Person", typeof(string));
            foreach (var item in projectEntity.Teams)
            {
                var row = dtCheckInfo.NewRow();
                row[0] = item.ProjectId;
                row[1] = item.TaskId;
                row[2] = item.Person;
                dtCheckInfo.Rows.Add(row);
            }
            pars["Team"] = dtCheckInfo;
            int rows = DALCore.GetInstance().ProjectCore.Save(pars);
            Assert.AreNotEqual(rows, 1);
        }
    }
}
