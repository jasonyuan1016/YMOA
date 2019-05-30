using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Model;

namespace YMOA.IDAL
{
    /// <summary>
    ///  任务数据访层接口
    ///  创建者: zxy
    ///  创建时间: 2019年5月29日
    /// </summary>
    public interface ITaskDAL
    {

        /// <summary>
        ///  任务添加/修改
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="dtTeam">团员</param>
        /// <param name="dtAccessory">附件</param>
        /// <returns></returns>
        int TaskSave(Dictionary<string, object> tasks, List<TeamEntity> listTeam, List<AccessoryEntity> listAccessory);


        /// <summary>
        ///  任务批量添加
        /// </summary>
        /// <param name="listTask"></param>
        /// <returns></returns>
        int BatchInsert(List<TaskEntity> listTask);

        /// <summary>
        ///  删除任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool TaskDelete(string id);

    }
}
