using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;
using YMOA.Model;

namespace YMOA.IDAL
{
    /// <summary>
    ///  工时数据访问接口
    /// </summary>
    public interface IHoursDAL
    {
        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagination"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> GetAllProject<T>();
        /// <summary>
        ///  批量添加
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        bool BatchInsert(List<HoursEntity> hours, string taskId);


        /// <summary>
        ///  根据任务编号删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        bool DeleteTaskHours(string taskId);

        /// <summary>
        /// 获取项目中子任务工时详情
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProName">项目名称</param>
        /// <returns></returns>
        IEnumerable<T> GetProjectByPerson<T>(string ProName);
    }
}
