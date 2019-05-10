using System;
using System.Data;
using System.Collections.Generic;
using YMOA.Model;

namespace YMOA.IDAL
{
    /// <summary>
    /// 角色接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IRoleDAL
    {

        /// <summary>
        /// 获得对应角色的选单权限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> GetMenuPermissionByGroupID<T>(Dictionary<string, object> paras);

        /// <summary>
        /// 获得角色
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetRoles<T>(Dictionary<string, object> paras);

        /// <summary>
        /// 获得选单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> GetMenus<T>(Dictionary<string, object> paras);

        int Save(Dictionary<string, object> paras);
    }
}
