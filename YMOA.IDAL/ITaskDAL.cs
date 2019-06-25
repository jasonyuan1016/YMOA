using Dapper;
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
    ///  任务数据访层接口
    ///  创建者: zxy
    ///  创建时间: 2019年5月29日
    /// </summary>
    public interface ITaskDAL
    {

        #region 任务相关

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

        /// <summary>
        ///  添加任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        bool TaskInsert(Dictionary<string, object> tasks);

        /// <summary>
        ///  修改任务
        /// </summary>
        /// <param name="tasks">任务参数</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        /// <returns></returns>
        bool TaskUpdate(Dictionary<string, object> tasks);

        /// <summary>
        ///  批量添加任务
        /// </summary>
        /// <param name="listTask"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        int BatchInsert(List<TaskEntity> listTask, string user);

        /// <summary>
        ///  删除任务
        /// </summary>
        /// <param name="taskId">任务编号</param>
        /// <returns></returns>
        bool TaskDelete(string taskId);

        /// <summary>
        ///  查询用户可修改任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> QryUpdateTask<T>(Dictionary<string, object> paras);

        /// <summary>
        ///  根据用户查询任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagination"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> QryTask<T>(Pagination pagination, Dictionary<string, object> paras);
        
        /// <summary>
        ///  根据编号查询任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        T QryTask<T>(Dictionary<string, object> paras);

        /// <summary>
        ///  判断是否存在子任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ExistSubtask(string id);

        #endregion

        #region 附件相关

        /// <summary>
        ///  根据任务编号查询附件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> GetAccessory<T>(Dictionary<string, object> paras);

        /// <summary>
        ///  删除附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteAccessory(string id);

        /// <summary>
        ///  附件修改
        /// </summary>
        /// <param name="accessory"></param>
        /// <returns></returns>
        bool UpdateAccessory(Dictionary<string, object> accessory);

        #endregion

        #region 附件与成员相关

        /// <summary>
        ///  批量添加团员与附件
        /// </summary>
        /// <param name="projectId">项目编号</param>
        /// <param name="taskId">任务编号</param>
        /// <param name="teams">团员</param>
        /// <param name="accessories">附件</param>
        void SaveTeamAndAccessory(string projectId, string taskId, List<TeamEntity> teams, List<AccessoryEntity> accessories);

        #endregion
    }
}
