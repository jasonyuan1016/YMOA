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
        ///  添加任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        bool TaskInsert(Dictionary<string, object> tasks, List<TeamEntity> teams, List<AccessoryEntity> accessories);

        /// <summary>
        ///  修改任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        bool TaskUpdate(Dictionary<string, object> tasks, List<TeamEntity> teams, List<AccessoryEntity> accessories);


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


        /// <summary>
        ///  判断用户修改权限
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        bool TaskUpdateJudge(Dictionary<string, object> paras);

        /// <summary>
        ///  判断用户添加权限
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        bool TaskInsertJudge(Dictionary<string, object> paras);

        void TaskList<T1, T2>(int qryTag, string userName, int page, int rows, string sidx, string sord, ref List<T1> taskList, ref int total);
    }
}
