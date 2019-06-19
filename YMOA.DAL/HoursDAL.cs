using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        //public bool BatchInsert(List<HoursEntity> hours)
        //{
            
        //}



    }
}
