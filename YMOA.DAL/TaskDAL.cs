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
            List<TeamEntity> teams = new List<TeamEntity>();
            foreach (TaskEntity t in listTask)
            {
                t.CreateBy = user;
                t.CreateTime = DateTime.Now;
                if (t.listTeam != null)
                {
                    t.Estimate = Math.Round(t.Estimate, 1);
                    foreach (TeamEntity e in t.listTeam)
                    {
                        e.ProjectId = t.ProjectId;
                        e.TaskId = t.ID;
                        teams.Add(e);
                    }
                }
            }
            string[] arrTask = new string[] { "ID","Name", "ProjectId", "ParentId", "EndTime",
                "Describe", "Remarks", "Estimate", "Consume", "Sort", "State", "Send","CreateBy", "CreateTime"};
            DataTable dtTask = ToDatatable.ListToDataTable(listTask, arrTask);
            DataTable dtTeam = ToDatatable.ListToDataTable(teams);
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
        /// <param name="pagination"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> QryTask<T>(Pagination pagination, Dictionary<string, object> paras)
        {
            if (pagination.sidx == null || pagination.sidx == "")
            {
                pagination.sidx = "ProjectId";
            }
            bool boo = pagination.sidx.IndexOf("ProjectId", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!boo)
            {
                pagination.sidx = "ProjectId," + pagination.sidx;
            }
            WhereBuilder builder = new WhereBuilder();
            builder.FromSql = "fun_userTask(@userName)";
            builder.AddParameter("userName", paras["userName"]);
            builder.AddWhereAndParameter(paras, "ProjectId");
            int tag = int.Parse(paras["qryTag"].ToString());
            if (tag == 1)
            {
                builder.AddWhere(" CreateBy = @userName");
            }
            if (tag == 2)
            {
                builder.AddWhere(" FinishBy = @userName");
            }
            return SortAndPage<T>(builder, pagination);
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
        ///  判断是否存在子任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistSubtask(string id)
        {
            string sql = "SELECT COUNT(0) FROM tbTask WHERE ParentId = @ID";
            return QuerySingle<int>(sql, new { ID = id }) > 0;
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
            WhereBuilder builder = new WhereBuilder();
            if (paras != null)
            {
                builder.AddWhereAndParameter(paras, "projectId");
                builder.AddWhereAndParameter(paras, "taskId");
            }
            return QueryList<T>(sql, builder);
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
            DataTable dtTeam = ToDatatable.ListToDataTable(teams);
            DataTable dtAccessory = ToDatatable.ListToDataTable(accessories);
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
