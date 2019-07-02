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
    public class HoursDAL : BaseDal, IHoursDAL
    {
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
        /// 获取所有成员工时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagination"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAllPerson<T>()
        {
            string sql = "SELECT RealName Person ,Person PersonName ,SUM(Consume) Hour FROM v_hour_statistics GROUP BY RealName,Person";
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
            string sql = "SELECT PName TaskId, RealName Person,Person PersonName,SUM(Consume) Hour,MAX(FinishTime) FinishTime FROM v_hour_statistics";
            if (paras != null)
            {
                WhereBuilder builder = new WhereBuilder();
                builder.AddWhereAndParameter(paras, "ProName", "PID");
                builder.AddWhereAndParameter(paras, "StartTime", "FinishTime", ">=");
                builder.AddWhereAndParameter(paras, "EndTime", "FinishTime", "<=");
                if (builder.Wheres.Count > 0)
                {
                    sql += " WHERE " + String.Join(" AND ", builder.Wheres);
                }
            }
            sql += " GROUP BY PName,RealName,Person";
            return QueryList<T>(sql, paras);
        }

        /// <summary>
        /// 获取成员在各项目中工时详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProName">项目名称</param>
        /// <returns></returns>
        public IEnumerable<T> GetProjectHoursByPerson<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT PID ProjectId, PName TaskId, RealName Person ,Person PersonName ,SUM(Consume) Hour ,MAX(FinishTime) FinishTime FROM v_hour_statistics";
            if (paras != null)
            {
                WhereBuilder builder = new WhereBuilder();
                builder.AddWhereAndParameter(paras, "PerName", "Person");
                builder.AddWhereAndParameter(paras, "StartTime", "FinishTime", ">=");
                builder.AddWhereAndParameter(paras, "EndTime", "FinishTime", "<=");
                if (builder.Wheres.Count > 0)
                {
                    sql += " WHERE " + String.Join(" AND ", builder.Wheres);
                }
            }
            sql += " GROUP BY PName, RealName,Person, PID";
            return QueryList<T>(sql, paras);
        }

        /// <summary>
        /// 获取对应项目中成员的任务工时详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> GetTaskByPerAndPro<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT PName ProjectId, TName TaskId, RealName Person,Person PersonName,Consume Hour,FinishTime FinishTime from v_hour_statistics ";
            if (paras != null)
            {
                WhereBuilder builder = new WhereBuilder();
                builder.AddWhereAndParameter(paras, "ProName", "PID");
                builder.AddWhereAndParameter(paras, "PerName", "Person");
                builder.AddWhereAndParameter(paras, "StartTime", "FinishTime", ">=");
                builder.AddWhereAndParameter(paras, "EndTime", "FinishTime", "<=");
                if (builder.Wheres.Count > 0)
                {
                    sql += " WHERE " + String.Join(" AND ", builder.Wheres);
                }
            }
            sql += " AND TID is not null";
            return QueryList<T>(sql, paras);
        }

    }
}
