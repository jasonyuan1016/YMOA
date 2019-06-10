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

        #region 项目相关

        /// <summary>
        ///  查询用户可添加项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> QryInsertTask<T>(Dictionary<string, object> paras)
        {
            return QueryList<T>("P_Product_UserAdd", paras, CommandType.StoredProcedure);
        }

        /// <summary>
        ///  查询项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetProject<T>()
        {
            string sql = "SELECT ID,Name FROM tbProduct";
            return QueryList<T>(sql);
        }

        #endregion

        #region 任务相关

        /// <summary>
        ///  判断用户修改权限
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public bool TaskUpdateJudge(Dictionary<string, object> paras)
        {
            int result = QuerySingle<int>("P_Task_UpdateJudge", paras, CommandType.StoredProcedure);
            return result > 0;
        }

        /// <summary>
        ///  判断用户添加权限
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public bool TaskInsertJudge(Dictionary<string, object> paras)
        {
            int result = QuerySingle<int>("P_Product_UserJudgeAdd", paras, CommandType.StoredProcedure);
            return result > 0;
        }

        /// <summary>
        ///  添加任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        public bool TaskInsert(Dictionary<string, object> tasks)
        {
            int result = StandardInsert("tbTask", tasks, "id");
            return result > 0;
        }

        /// <summary>
        ///  修改任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        public bool TaskUpdate(Dictionary<string, object> tasks)
        {
            int result = StandardUpdate("tbTask", tasks);
            return result > 0;
        }

        /// <summary>
        ///  批量添加任务
        /// </summary>
        /// <param name="listTask"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public int BatchInsert(List<TaskEntity> listTask, string user)
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
            foreach (var item in listTask)
            {
                var row = dtTask.NewRow();
                row[0] = item.ID;
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
                row[12] = user;
                row[13] = DateTime.Now;
                foreach (var team in item.listTeam)
                {
                    var rowTeam = dtTeam.NewRow();
                    rowTeam[0] = item.ProjectId;
                    rowTeam[1] = item.ID;
                    rowTeam[2] = team.Person;
                    dtTeam.Rows.Add(rowTeam);
                }
                dtTask.Rows.Add(row);
            }
            int result = Execute("P_Task_BatchInsert", new { task = dtTask, team = dtTeam }, CommandType.StoredProcedure);
            return result;
        }
        
        /// <summary>
        ///  删除任务
        /// </summary>
        /// <param name="taskId">任务编号</param>
        /// <returns></returns>
        public bool TaskDelete(string taskId)
        {
            int result = Execute("P_Task_Delete", new { taskId }, CommandType.StoredProcedure);
            return result > 0;
        }
        
        /// <summary>
        ///  查询用户可修改任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> QryUpdateTask<T>(Dictionary<string, object> paras)
        {
            return QueryList<T>("P_Task_UserUpdateTask", paras, CommandType.StoredProcedure);
        }

        /// <summary>
        ///  根据用户查询任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dp"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public IEnumerable<T> QryTaskList<T>(DynamicParameters dp, Pagination pagination)
        {
            using (IDbConnection conn = GetConnection())
            {
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.Query<T>("P_Task_GetTask", dp, null, true, null, CommandType.StoredProcedure);
                pagination.records = dp.Get<int>("Count");
                return objRet;
            }
        }
        
        /// <summary>
        ///  根据编号查询任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public T QryTask<T>(Dictionary<string, object> paras)
        {
            return QuerySingle<T>("SELECT * FROM tbTask WHERE ID=@ID", paras, CommandType.Text);
        }
        
        /// <summary>
        ///  多任务查询成员并写入相应任务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<TaskEntity> GetTeams(List<TaskEntity> list)
        {
            List<string> teams = new List<string>();
            foreach (TaskEntity task in list)
            {
                teams.Add(task.ID);
            }
            string strTeams = String.Join("','", teams);
            strTeams = "'" + strTeams + "'";
            // 查询任务团员
            List<TeamEntity> teamList = GetTeams<TeamEntity>(strTeams).ToList();
            foreach (TaskEntity task in list)
            {
                task.listTeam = new List<TeamEntity>();
                foreach (TeamEntity team in teamList)
                {
                    if (task.ID == team.TaskId)
                    {
                        task.listTeam.Add(team);
                    }
                }
            }
            return list;
        }

        #endregion

        #region 成员相关

        /// <summary>
        ///  查询成员
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetTeams<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT * FROM tbTeam";
            if (paras != null)
            {
                WhereBuilder builder = new WhereBuilder();
                builder.AddWhereAndParameter(paras, "projectId");
                builder.AddWhereAndParameter(paras, "taskId");
                if (builder.Wheres.Count > 0)
                {
                    sql += " WHERE " + String.Join(" and ", builder.Wheres);
                }
            }
            return QueryList<T>(sql, paras);
        }

        /// <summary>
        ///  多任务查询成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTeams<T>(string tasks)
        {
            string sql = "SELECT * FROM tbTeam WHERE TaskId IN (" + tasks + ")";
            return QueryList<T>(sql);
        }

        #endregion

        #region 附件相关

        /// <summary>
        ///  根据任务编号查询附件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAccessory<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT * FROM tbAccessory WHERE TaskId = @ID";
            return QueryList<T>(sql, paras);
        }

        /// <summary>
        ///  删除附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteAccessory(string id)
        {
            string sql = "DELETE FROM tbAccessory WHERE ID = @id";
            return Execute(sql, new { id }) > 0;
        }

        /// <summary>
        ///  附件修改
        /// </summary>
        /// <param name="accessory"></param>
        /// <returns></returns>
        public bool UpdateAccessory(Dictionary<string, object> accessory)
        {

            string id = accessory["ID"].ToString();
            int result = StandardUpdate("tbAccessory", accessory);
            return result > 0;
        }

        #endregion

        #region 附件与成员相关

        /// <summary>
        ///  批量添加团员与附件
        /// </summary>
        /// <param name="projectId">项目编号</param>
        /// <param name="taskId">任务编号</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        public void SaveTeamAndAccessory(string projectId, string taskId, List<TeamEntity> teams, List<AccessoryEntity> accessories)
        {
            if (teams == null)
            {
                teams = new List<TeamEntity>();
            }
            if (accessories == null)
            {
                accessories = new List<AccessoryEntity>();
            }
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

        #endregion


    }
}
