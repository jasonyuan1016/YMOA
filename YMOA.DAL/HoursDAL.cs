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
        public IEnumerable<T> GetProjectByPerson<T>(string ProName)
        {
            string sql = "SELECT TName TaskId , Consume Hour , RealName Person FROM v_hour_statistics where TID is not null and PID =  @ProName";
            return QueryList<T>(sql,new { ProName });
        }



    }
}
