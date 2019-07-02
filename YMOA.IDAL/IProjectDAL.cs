using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;

namespace YMOA.IDAL
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/05/29
    /// 项目接口
    /// </summary>
    public interface IProjectDAL
    {
        /// <summary>
        /// 查询项目基本信息
        /// </summary>
        /// <typeparam name="T">传入类型，返回类型</typeparam>
        /// <param name="dp">查询条件</param>
        /// <param name="pagination">分页条件</param>
        /// <returns></returns>
        IEnumerable<T> QryProjects<T>(DynamicParameters dp, Pagination pagination);

        /// <summary>
        /// 修改/保存项目基本信息
        /// </summary>
        /// <param name="paras">修改/保存内容</param>
        /// <returns></returns>
        int Save(Dictionary<string, object> paras);

        /// <summary>
        /// 删除项目，任务及团队
        /// </summary>
        /// <param name="paras">删除内容</param>
        /// <returns></returns>
        bool DeleteProject(Dictionary<string,object>paras);

        /// <summary>
        /// 查询单个项目详细信息
        /// </summary>
        /// <typeparam name="T">传入类型，返回类型</typeparam>
        /// <param name="dp">查询条件</param>
        /// <returns></returns>
        T QryProjectInfo<T>(DynamicParameters dp);
        
        /// <summary>
        ///  查询用户可添加项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> QryInsertTask<T>(Dictionary<string, object> paras);
    }
}
