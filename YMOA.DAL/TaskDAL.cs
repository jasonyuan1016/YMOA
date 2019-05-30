using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;
using YMOA.IDAL;
using YMOA.Model;

namespace YMOA.DAL
{
    /// <summary>
    ///  任务数据访层
    ///  创建者: zxy
    ///  创建时间: 2019年5月29日
    /// </summary>
    public class TaskDAL : BaseDal, ITaskDAL
    {

        /// <summary>
        ///  添加任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        public bool TaskInsert(Dictionary<string, object> tasks, List<TeamEntity> teams, List<AccessoryEntity> accessories)
        {
            string id = Guid.NewGuid().To16String();
            tasks["ID"] = id;
            int result = StandardInsert("tbTask", tasks);
            if (result > 0)
            {
                string projectId = tasks["ProjectId"].ToString();
                string taskId = id;
                SaveTeamAndAccessory(projectId,id,teams,accessories);
            }
            return result > 0;
        }

        /// <summary>
        ///  修改任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        public bool TaskUpdate(Dictionary<string, object> tasks, List<TeamEntity> teams, List<AccessoryEntity> accessories)
        {
            string id = tasks["ID"].ToString();
            int result = StandardUpdate("tbTask", tasks);
            if (result > 0)
            {
                string projectId = tasks["ProjectId"].ToString();
                string taskId = id;
                SaveTeamAndAccessory(projectId, id, teams, accessories);
            }
            return result > 0;
        }
        
        /// <summary>
        ///  批量添加团员与附件
        /// </summary>
        /// <param name="projectId">项目编号</param>
        /// <param name="taskId">任务编号</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        private void SaveTeamAndAccessory(string projectId, string taskId, List<TeamEntity> teams, List<AccessoryEntity> accessories)
        {
            var dtTeam = new DataTable();
            dtTeam.Columns.Add("ProjectId", typeof(string));
            dtTeam.Columns.Add("TaskId", typeof(string));
            dtTeam.Columns.Add("Person", typeof(string));
            foreach (var item in teams)
            {
                var row = dtTeam.NewRow();
                row[0] = projectId;
                row[1] = taskId;
                row[2] = item.Person;
                dtTeam.Rows.Add(row);
            }
            var dtAccessory = new DataTable();
            dtAccessory.Columns.Add("ID", typeof(string));
            dtAccessory.Columns.Add("Name", typeof(string));
            dtAccessory.Columns.Add("TaskId", typeof(string));
            dtAccessory.Columns.Add("AccessoryUrl", typeof(string));
            foreach (var item in accessories)
            {
                var row = dtAccessory.NewRow();
                row[0] = item.ID;
                row[1] = item.Name;
                row[2] = taskId;
                row[3] = item.AccessoryUrl;
                dtAccessory.Rows.Add(row);
            }
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ProjectId"] = projectId;
            paras["TaskId"] = taskId;
            paras["team"] = dtTeam.AsTableValuedParameter();
            paras["accessory"] = dtAccessory.AsTableValuedParameter();
            QuerySingle<int>("P_TeamAndAccessory_SaveBatch", paras, CommandType.StoredProcedure);
        }

        /// <summary>
        ///  批量添加任务
        /// </summary>
        /// <param name="listTask"></param>
        /// <returns></returns>
        public int BatchInsert(List<TaskEntity> listTask)
        {
            var dtTask = new DataTable();
            dtTask.Columns.Add("ID", typeof(string));
            dtTask.Columns.Add("Name", typeof(string));
            dtTask.Columns.Add("ProjectId", typeof(string));
            dtTask.Columns.Add("ParentId", typeof(string));
            dtTask.Columns.Add("EndTime", typeof(DateTime));
            dtTask.Columns.Add("Describe", typeof(string));
            dtTask.Columns.Add("Remarks", typeof(string));
            dtTask.Columns.Add("Estimate", typeof(Decimal));
            dtTask.Columns.Add("Consume", typeof(Decimal));
            dtTask.Columns.Add("Sort", typeof(int));
            dtTask.Columns.Add("State", typeof(int));
            dtTask.Columns.Add("Send", typeof(string));
            dtTask.Columns.Add("CreateBy", typeof(string));
            dtTask.Columns.Add("CreateTime", typeof(DateTime));
            var dtTeam = new DataTable();
            dtTeam.Columns.Add("ProjectId", typeof(string));
            dtTeam.Columns.Add("TaskId", typeof(string));
            dtTeam.Columns.Add("Person", typeof(string));
            string id;
            foreach (var item in listTask)
            {
                var row = dtTask.NewRow();
                id = Guid.NewGuid().To16String();
                row[0] = id;
                row[1] = item.Name;
                row[2] = item.ProjectId;
                row[3] = item.ParentId;
                row[4] = item.EndTime;
                row[5] = item.Describe;
                row[6] = item.Remarks;
                row[7] = item.Estimate;
                row[8] = item.Consume;
                row[9] = item.Sort;
                row[10] = item.State;
                row[11] = item.Send;
                row[12] = item.CreateBy;
                row[13] = item.CreateTime;
                foreach (var team in item.listTeam)
                {
                    var rowTeam = dtTeam.NewRow();
                    rowTeam[0] = item.ProjectId;
                    rowTeam[1] = id;
                    rowTeam[2] = team.Person;
                    dtTeam.Rows.Add(rowTeam);
                }
                dtTask.Rows.Add(row);
            }
            int result = Execute("P_TeamAndAccessory_SaveBatch", new { task = dtTask, team = dtTeam }, CommandType.StoredProcedure);
            return 1;
        }

        /// <summary>
        ///  删除任务
        /// </summary>
        /// <param name="taskId">任务编号</param>
        /// <returns></returns>
        public bool TaskDelete(string taskId)
        {
            return Execute("P_Team_Delete", new { taskId }, CommandType.StoredProcedure) > 0;
        }

    }
}
