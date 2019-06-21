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
    ///  工时数据访问层
    /// </summary>
    public class HoursDAL : BaseDal,IHoursDAL
    {
        /// <summary>
        ///  标准添加/修改
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public bool Standard(Dictionary<string, object> paras)
        {
            return StandardInsertOrUpdate("tbHours", paras) > 0;
        }

        /// <summary>
        ///  根据任务编号删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool DeleteTaskHours(string taskId)
        {
            string sql = "DELETE FROM tbHours WHERE TaskId = @TaskId";
            return Execute(sql, new { taskId }) > 0;
        }
        /// <summary>
        /// 获取所有项目工时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagination"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAllProject<T>()
        {
            string sql = "select tbProduct.ID TaskId, tbProduct.Name as ProjectId ,sum(Consume) as Hour from tbTask join tbTeam on tbTask.ID = tbTeam.TaskId join tbProduct on tbTask.ProjectId = tbProduct.ID group by tbProduct.Name ,tbProduct.ID";
            return QueryList<T>(sql);
        }
        /// <summary>
        /// 获取项目中子任务工时详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProName">项目名称</param>
        /// <returns></returns>
        public IEnumerable<T> GetProjectByPerson<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT PID ProjectId , PName TaskId  ,RealName Person ,COUNT(TID) TaskCount,SUM(Consume) Hour,MAX(FinishTime) FinishTime FROM v_hour_statistics WHERE PID = @ProName AND FinishTime > @StartTime AND FinishTime < @EndTime GROUP BY PName,RealName,PID ORDER BY PName,TaskCount,Hour";
            return QueryList<T>(sql,paras);
        }
        //public bool BatchInsert(List<HoursEntity> hours)
        //{

        //}

        /// <summary>
        ///  批量添加
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool BatchInsert(List<HoursEntity> hours, string taskId)
        {
            string[] arr = new string[] { "ProjectId", "TaskId", "Hour", "Person", "Date" };
            DataTable dt = ToDatatable.ListToDataTable(hours, arr);
            int result = Execute("P_Hours_BatchInsert", new { TaskId = taskId, hours = dt }, CommandType.StoredProcedure);
            return result > 0;
        }



    }
}
